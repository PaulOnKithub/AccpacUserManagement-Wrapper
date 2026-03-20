using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using ACCPAC.Advantage;
using AccpacUserManagement_Wrappe.Models;

namespace AccpacUserManagement_Wrapper.Services
{
    public sealed class Sage300SessionManager : IDisposable
    {
        private static readonly Lazy<Sage300SessionManager> _instance =
            new Lazy<Sage300SessionManager>(() => new Sage300SessionManager());

        private Session _session;
        private DBLink _dbLink;
        private DBLink _systemDbLink;
        private bool _disposed = false;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Checks if the session has been initialized.
        /// </summary>
        public bool IsInitialized => _session != null;

        /// <summary>
        /// Gets the singleton instance of the Sage300SessionManager.
        /// </summary>
        public static Sage300SessionManager Instance => _instance.Value;

        /// <summary>
        /// Private constructor to prevent external instantiation.
        /// </summary>
        private Sage300SessionManager()
        {
            // Constructor is private for singleton pattern
            Debug.WriteLine("Sage300SessionManager instance created");
        }

        /// <summary>
        /// Initializes and opens the Sage 300 session using configuration from Web.config.
        /// This method should be called once during application startup.
        /// </summary>
        public void Initialize()
        {
            lock (_lockObject)
            {
                if (_session != null)
                {
                    Debug.WriteLine("Sage300SessionManager already initialized");
                    return;
                }

                try
                {
                    // Read configuration from Web.config
                    string appId = ConfigurationManager.AppSettings["Sage300:AppId"] ?? "SAMINC";
                    string programName = ConfigurationManager.AppSettings["Sage300:ProgramName"] ?? "WebAPIWrapper";
                    string version = ConfigurationManager.AppSettings["Sage300:Version"] ?? "68A";
                    string orgId = ConfigurationManager.AppSettings["Sage300:OrgId"] ?? "SAMLTD";
                    string userId = ConfigurationManager.AppSettings["Sage300:UserId"] ?? "ADMIN";
                    string password = ConfigurationManager.AppSettings["Sage300:Password"] ?? "";
                    string companyId = ConfigurationManager.AppSettings["Sage300:CompanyId"] ?? "SAMINC";

                    Debug.WriteLine($"Initializing Sage 300 session for user: {userId}, org: {orgId}, company: {companyId}");

                    // Create and initialize the session
                    _session = new Session();
                    _session.Init("", "XY", "XY1000", "71A");

                    // Sign on to Sage 300
                    _session.Open(userId, password, companyId, DateTime.Today, 0);

                    // Open database link
                    _dbLink = _session.OpenDBLink(DBLinkType.Company, DBLinkFlags.ReadWrite);
                    _systemDbLink=_session.OpenDBLink(DBLinkType.System, DBLinkFlags.ReadWrite);

                    Debug.WriteLine("Sage300 Session initialized successfully");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception during Sage300SessionManager initialization : {accpacSessionErrorsHandler(ex).ToString()}");

                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");


                    // Clean up partial initialization
                    CleanupResources();


                    throw new InvalidOperationException("Failed to initialize Sage 300 session. Check configuration and Sage 300 availability.", ex);
                }
            }
        }

        /// <summary>
        /// Gets the active Sage 300 session.
        /// </summary>
        /// <returns>The active Sage 300 session object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the session has not been initialized.</exception>
        public Session GetSession()
        {
            if (_session == null)
            {
                throw new InvalidOperationException("Sage 300 session not initialized. Call Initialize() first.");
            }

            return _session;
        }

        /// <summary>
        /// Gets the active database link.
        /// </summary>
        /// <returns>The active database link object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the session has not been initialized.</exception>
        public DBLink GetDBLink()
        {
            if (_dbLink == null)
            {
                throw new InvalidOperationException("Sage 300 database link not initialized. Call Initialize() first.");
            }

            return _dbLink;
        }

        public DBLink GetSystemDBLink()
        {
            if (_systemDbLink == null)
            {
                throw new InvalidOperationException("Sage 300 system database link not initialized. Call Initialize() first.");
            }
            return _systemDbLink;
        }

        public AccpacErrorDto accpacSessionErrorsHandler(Exception ex)
        {

            if (_session.Errors == null || _session.Errors.Count == 0)
            {

                return new AccpacErrorDto { Message = ex.Message, Source = "Non-Accpac Error" };

            }
            else
            {
                long count = _session.Errors.Count;
                int i = 0;
                AccpacErrorDto returnValue = null;
                while (count > 0)
                {
                    switch (_session.Errors[i].Priority)
                    {
                        case ErrorPriority.Error:
                            returnValue=new AccpacErrorDto {Priority="ERROR", Message = _session.Errors[i].Message, Source = _session.Errors[i].Message };
                            break;
                         
                        case ErrorPriority.Message:
                            returnValue = new AccpacErrorDto {Priority="MESSAGE" ,Message = _session.Errors[i].Message, Source = _session.Errors[i].Message };
                            break;

                        case ErrorPriority.Security:
                            returnValue = new AccpacErrorDto {Priority="SECURITY" ,Message = _session.Errors[i].Message, Source = _session.Errors[i].Message };
                            break;

                        case ErrorPriority.SevereError:
                            returnValue = new AccpacErrorDto {Priority="SEVERE ERROR" ,Message = _session.Errors[i].Message, Source = _session.Errors[i].Message };
                            break;

                        case ErrorPriority.Warning:
                            returnValue = new AccpacErrorDto {Priority="WARNING" ,Message = _session.Errors[i].Message, Source = _session.Errors[i].Message };
                            break;

                        default:
                            returnValue = new AccpacErrorDto {Priority="UNSPECIFIED PRIORITY" ,Message = _session.Errors[i].Message, Source = _session.Errors[i].Message };
                            break;

                    }

                    count--;
                    i++;
                }

                _session.Errors.Clear();
                return returnValue;
            }
        }


        /// <summary>
        /// Cleans up Sage 300 resources.
        /// </summary>
        private void CleanupResources()
        {
            try
            {
                if (_dbLink != null)
                {
                    _dbLink.Dispose();
                    _dbLink = null;
                }

                if (_session != null)
                {
                    if (_session.IsOpened)
                    {
                        _session.Dispose();
                    }
                    _session = null;
                }

                Debug.WriteLine("Sage300SessionManager resources cleaned up");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during Sage300SessionManager cleanup: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes the Sage 300 session and releases resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                lock (_lockObject)
                {
                    CleanupResources();
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Finalizer to ensure resources are released.
        /// </summary>
        ~Sage300SessionManager()
        {
            Dispose(false);
        }
    }
}
