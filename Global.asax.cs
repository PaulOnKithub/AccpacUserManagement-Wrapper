using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AccpacUserManagement_Wrapper.Services;

namespace AccpacUserManagement_Wrapper
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            try
            {
                Debug.WriteLine("Initializing Sage 300 Services...");
                // Start the Sage 300 task executor thread
                Sage300TaskExecutor.Instance.Start();
                Debug.WriteLine("Sage300TaskExecutor started successfully");

                // Initialize the Sage 300 session manager
                Sage300SessionManager.Instance.Initialize();
                Debug.WriteLine("Sage300SessionManager initialized successfully");

                Debug.WriteLine("Sage 300 Services initialization complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CRITICAL: Failed to initialize Sage 300 Services: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                // Log to event log as well for production environments
                EventLog.WriteEntry("WebAppDotNet48",
                    $"Failed to initialize Sage 300 Services: {ex.Message}",
                    EventLogEntryType.Error);

                // Rethrow to prevent application from starting in a bad state
                throw;
            }
        }


        protected void Application_End()
        {
            try
            {
                Debug.WriteLine("Shutting down Sage 300 Services...");

                // Dispose the session manager to close the session
                if (Sage300SessionManager.Instance.IsInitialized)
                {
                    Sage300SessionManager.Instance.Dispose();
                    Debug.WriteLine("Sage300SessionManager disposed successfully");
                }

                // Stop the task executor thread
                if (Sage300TaskExecutor.Instance.IsRunning)
                {
                    Sage300TaskExecutor.Instance.Stop();
                    Debug.WriteLine("Sage300TaskExecutor stopped successfully");
                }

                Debug.WriteLine("Sage 300 Services shutdown complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during Sage 300 Services shutdown: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
