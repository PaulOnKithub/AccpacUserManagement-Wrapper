using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using AccpacUserManagement_Wrapper.Models;

namespace AccpacUserManagement_Wrapper.Services.Sage300Services
{
    public class UserService
    {
        /// <summary>
        /// Retrieves all users from Sage 300.
        /// </summary>
        /// <returns>A list of user DTOs.</returns>
        public async Task<List<UserAttributesDto>> GetUsersAsync()
        {
            return await Sage300TaskExecutor.Instance.ExecuteAsync(() =>
            {
                Debug.WriteLine("GetUsersAsync executing on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);

                var users = new List<UserAttributesDto>();

                try
                {
                    
                    ACCPAC.Advantage.DBLink dbLink = Sage300SessionManager.Instance.GetDBLink();

                    // Open the users view
                    var view = dbLink.OpenView("AS0003");
                    

                    view.Browse("", true);

                    while (view.Fetch(false))
                    {
                        var user = new UserAttributesDto
                        {
                            UserId = view.Fields.FieldByName("USERID").Value?.ToString(),
                            UserName = view.Fields.FieldByName("USERNAME").Value?.ToString(),

                            /*Password = Encoding.UTF8.GetBytes(
                                view.Fields.FieldByName("PASSWORD").Value?.ToString() ?? string.Empty
                            ),*/
                            Password = view.Fields.FieldByName("PASSWORD").Value?.ToString(),

                            AccountType = (AccountType)Convert.ToInt32(
                                view.Fields.FieldByName("ACCTTYPE").Value
                            ),

                            AccountStatus = (AccountStatus)Convert.ToInt32(
                                view.Fields.FieldByName("ACCTSTATUS").Value
                            ),

                            LanguageCode = view.Fields.FieldByName("LANGUAGE").Value?.ToString(),

                            Language = (Language)Convert.ToInt32(
                                view.Fields.FieldByName("LANGLIST").Value
                            ),

                            MustChangePassword = Convert.ToInt32(view.Fields.FieldByName("CHNGEPASS").Value) == 1,
                            CannotChangePassword = Convert.ToInt32(view.Fields.FieldByName("NOCHNGPASS").Value) == 1,
                            PasswordNeverExpires = Convert.ToInt32(view.Fields.FieldByName("PASSNOEXP").Value) == 1,

                            Disabled = Convert.ToInt32(view.Fields.FieldByName("DISABLED").Value) == 1,
                            LockedOut = Convert.ToInt32(view.Fields.FieldByName("LOCKEDOUT").Value) == 1,
                            Restricted = Convert.ToInt32(view.Fields.FieldByName("RESTRICTED").Value) == 1,

                            OkMonday = Convert.ToInt32(view.Fields.FieldByName("OKMONDAY").Value) == 1,
                            OkTuesday = Convert.ToInt32(view.Fields.FieldByName("OKTUESDAY").Value) == 1,
                            OkWednesday = Convert.ToInt32(view.Fields.FieldByName("OKWEDNESDY").Value) == 1,
                            OkThursday = Convert.ToInt32(view.Fields.FieldByName("OKTHURSDAY").Value) == 1,
                            OkFriday = Convert.ToInt32(view.Fields.FieldByName("OKFRIDAY").Value) == 1,
                            OkSaturday = Convert.ToInt32(view.Fields.FieldByName("OKSATURDAY").Value) == 1,
                            OkSunday = Convert.ToInt32(view.Fields.FieldByName("OKSUNDAY").Value) == 1,

                            /*TimeBegin = TimeSpan.Parse(view.Fields.FieldByName("TIMEBEGIN").Value.ToString()),
                            TimeEnd = TimeSpan.Parse(view.Fields.FieldByName("TIMEEND").Value.ToString()),*/
                            TimeBegin = view.Fields.FieldByName("TIMEBEGIN").Value.ToString(),
                            TimeEnd=view.Fields.FieldByName("TIMEEND").Value.ToString(),

                            AuthenticationMethod = (AuthenticationMethod)Convert.ToInt32(
                                view.Fields.FieldByName("AUTHMETH").Value.ToString()
                            ),

                            WindowsDomain = view.Fields.FieldByName("WINDOMAIN").Value?.ToString(),
                            WindowsUser = view.Fields.FieldByName("WINUSER").Value?.ToString(),

                            Phone = view.Fields.FieldByName("PHONE").Value?.ToString(),
                            Email1 = view.Fields.FieldByName("EMAIL1").Value?.ToString(),
                            Email2 = view.Fields.FieldByName("EMAIL2").Value?.ToString(),

                            Role = (JobRole)Convert.ToInt32(
                                view.Fields.FieldByName("ROLE").Value
                            ),

                            OptInEmail = Convert.ToInt32(view.Fields.FieldByName("OPTIN").Value) == 1,

                            LocaleId = view.Fields.FieldByName("LOCALEID").Value?.ToString(),

                            Locale = (LocaleList)Convert.ToInt32(
                                view.Fields.FieldByName("LOCALELIST").Value
                            ),

                            Office365UserName = view.Fields.FieldByName("O365USRNAM").Value?.ToString()
                        };

                        users.Add(user);
                    }

                    view.Dispose();
                    Debug.WriteLine($"Retrieved {users.Count} customers");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error retrieving userss: {ex.Message}");
                    var accpacError = Sage300SessionManager.Instance.accpacSessionErrorsHandler(ex);
                    throw new InvalidOperationException($"Failed to retrive users from Sage 300 \n Accpac Error {accpacError.ToString()}", ex);
                }

                return users;
            });
        }

        public async Task<UserAttributesDto> GetUserByIdAsync(string id)
        {
            return await Sage300TaskExecutor.Instance.ExecuteAsync(() =>
            {
                Debug.WriteLine("GetUserByIdAsync executing on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
                UserAttributesDto user = null;
                try
                {
                    var dbLink = Sage300SessionManager.Instance.GetDBLink();
                    var view = dbLink.OpenView("AS0003");

                    view.Fields.FieldByName("USERID").SetValue(id, false);

                    bool found = view.Read(false);

                    if (found)
                    {
                        user = new UserAttributesDto
                        {
                            UserId = view.Fields.FieldByName("USERID").Value.ToString(),
                            UserName = view.Fields.FieldByName("USERNAME").Value.ToString(),

                            Password = view.Fields.FieldByName("PASSWORD").Value?.ToString(),

                            AccountType = (AccountType)Convert.ToInt32(
                                view.Fields.FieldByName("ACCTTYPE").Value
                            ),

                            AccountStatus = (AccountStatus)Convert.ToInt32(
                                view.Fields.FieldByName("ACCTSTATUS").Value
                            ),

                            LanguageCode = view.Fields.FieldByName("LANGUAGE").Value?.ToString(),

                            Language = (Language)Convert.ToInt32(
                                view.Fields.FieldByName("LANGLIST").Value
                            ),

                            MustChangePassword = Convert.ToInt32(view.Fields.FieldByName("CHNGEPASS").Value) == 1,
                            CannotChangePassword = Convert.ToInt32(view.Fields.FieldByName("NOCHNGPASS").Value) == 1,
                            PasswordNeverExpires = Convert.ToInt32(view.Fields.FieldByName("PASSNOEXP").Value) == 1,

                            Disabled = Convert.ToInt32(view.Fields.FieldByName("DISABLED").Value) == 1,
                            LockedOut = Convert.ToInt32(view.Fields.FieldByName("LOCKEDOUT").Value) == 1,
                            Restricted = Convert.ToInt32(view.Fields.FieldByName("RESTRICTED").Value) == 1,

                            OkMonday = Convert.ToInt32(view.Fields.FieldByName("OKMONDAY").Value) == 1,
                            OkTuesday = Convert.ToInt32(view.Fields.FieldByName("OKTUESDAY").Value) == 1,
                            OkWednesday = Convert.ToInt32(view.Fields.FieldByName("OKWEDNESDY").Value) == 1,
                            OkThursday = Convert.ToInt32(view.Fields.FieldByName("OKTHURSDAY").Value) == 1,
                            OkFriday = Convert.ToInt32(view.Fields.FieldByName("OKFRIDAY").Value) == 1,
                            OkSaturday = Convert.ToInt32(view.Fields.FieldByName("OKSATURDAY").Value) == 1,
                            OkSunday = Convert.ToInt32(view.Fields.FieldByName("OKSUNDAY").Value) == 1,

                            /*TimeBegin = TimeSpan.Parse(view.Fields.FieldByName("TIMEBEGIN").Value.ToString()),
                            TimeEnd = TimeSpan.Parse(view.Fields.FieldByName("TIMEEND").Value.ToString()),*/
                            TimeBegin = view.Fields.FieldByName("TIMEBEGIN").Value.ToString(),
                            TimeEnd = view.Fields.FieldByName("TIMEEND").Value.ToString(),

                            AuthenticationMethod = (AuthenticationMethod)Convert.ToInt32(
                                view.Fields.FieldByName("AUTHMETH").Value.ToString()
                            ),

                            WindowsDomain = view.Fields.FieldByName("WINDOMAIN").Value?.ToString(),
                            WindowsUser = view.Fields.FieldByName("WINUSER").Value?.ToString(),

                            Phone = view.Fields.FieldByName("PHONE").Value?.ToString(),
                            Email1 = view.Fields.FieldByName("EMAIL1").Value?.ToString(),
                            Email2 = view.Fields.FieldByName("EMAIL2").Value?.ToString(),

                            Role = (JobRole)Convert.ToInt32(
                                view.Fields.FieldByName("ROLE").Value
                            ),

                            OptInEmail = Convert.ToInt32(view.Fields.FieldByName("OPTIN").Value) == 1,

                            LocaleId = view.Fields.FieldByName("LOCALEID").Value?.ToString(),

                            Locale = (LocaleList)Convert.ToInt32(
                                view.Fields.FieldByName("LOCALELIST").Value
                            ),

                            Office365UserName = view.Fields.FieldByName("O365USRNAM").Value?.ToString()

                        };
                    }
                    view.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error retrieving user by ID: {ex.Message}");
                    var accpacError = Sage300SessionManager.Instance.accpacSessionErrorsHandler(ex);
                    throw new InvalidOperationException($"Failed to retrive user from Sage 300 \n Accpac Error {accpacError.ToString()}", ex);
                }
                return user;
             });

        }

        public async Task<UserAttributesDto> CreateUser(UserAttributesDto user)
        {
            return await Sage300TaskExecutor.Instance.ExecuteAsync(() =>
            {
                Debug.WriteLine("CreateUser executing on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);

                try
                {
                    var dbLink = Sage300SessionManager.Instance.GetSystemDBLink();
                    var view = dbLink.OpenView("AS0003");

                    view.Fields.FieldByName("USERID").SetValue(user.UserId, false);
                    view.Fields.FieldByName("USERNAME").SetValue(user.UserName, false);
                    view.Fields.FieldByName("PASSWORD").SetValue(user.Password, false);

                    view.Insert();
                    view.Dispose();

                    //sanitize use password
                    user.Password = "******************";

                    return user;

                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"Error creating a new user: {ex.Message}");
                    var accpacError = Sage300SessionManager.Instance.accpacSessionErrorsHandler(ex);
                    throw new InvalidOperationException($"Failed to create user with id {user.UserId} \n Accapc Error {accpacError.ToString()}",ex);
                }

            });

        }

        public async Task DisableUser(String userID)
        {
            await Sage300TaskExecutor.Instance.ExecuteAsync(() =>{

            Debug.WriteLine("DisableUser executing on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);

            try
            {
                var dbLink = Sage300SessionManager.Instance.GetSystemDBLink();
                var view = dbLink.OpenView("AS0003");

                view.Fields.FieldByName("USERID").SetValue(userID, false);

                bool found = view.Read(false);

                if (found)
                {
                    view.Fields.FieldByName("DISABLED").SetValue(1, false);
                    view.Update();
                }
                else
                {
                     view.Dispose();
                    throw new InvalidOperationException($"User with id {userID} not found");
                }

                view.Dispose();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error disabling user: {ex.Message}");
                var accpacError = Sage300SessionManager.Instance.accpacSessionErrorsHandler(ex);
                throw new InvalidOperationException($"Failed to disable user with id {userID} \n Accapc Error {accpacError.ToString()}", ex);
            }

            return;

            });
        }

        public async Task EnableUser(String userID)
        {
            await Sage300TaskExecutor.Instance.ExecuteAsync(() => {

                Debug.WriteLine("EnableUser executing on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);

                try
                {
                    var dbLink = Sage300SessionManager.Instance.GetSystemDBLink();
                    var view = dbLink.OpenView("AS0003");

                    view.Fields.FieldByName("USERID").SetValue(userID, false);

                    bool found = view.Read(false);

                    if (found)
                    {
                        view.Fields.FieldByName("DISABLED").SetValue(0, false);
                        view.Update();
                    }
                    else
                    {
                        view.Dispose();
                        throw new InvalidOperationException($"User with id {userID} not found");
                    }

                    view.Dispose();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error enabling user: {ex.Message}");
                    var accpacError = Sage300SessionManager.Instance.accpacSessionErrorsHandler(ex);
                    throw new InvalidOperationException($"Failed to enable user with id {userID} \n Accapc Error {accpacError.ToString()}", ex);
                }

                return;

            });
        }

        internal async Task<List<RolesDto>> GetUserRolesAsync()
        {
            return await Sage300TaskExecutor.Instance.ExecuteAsync(() =>
            {
                Debug.WriteLine("GetUserRolesAsync executing on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
                var userRoles = new List<RolesDto>();
                try
                {
                    var dbLink = Sage300SessionManager.Instance.GetSystemDBLink();
                    var view = dbLink.OpenView("AS0001");
                    view.Browse("PGMVER = 71A AND  RESOURCEID=\"\" ", true);
                    while (view.Fetch(false))
                    {
                        var userRole = new RolesDto
                        {
                            ProgramId = view.Fields.FieldByName("PGMID").Value?.ToString(),
                            ProgramVersion = view.Fields.FieldByName("PGMVER").Value?.ToString(),
                            GroupId = view.Fields.FieldByName("PROFILEID").Value?.ToString(),
                            ResourceId = view.Fields.FieldByName("RESOURCEID").Value?.ToString(),
                            GroupDescription = view.Fields.FieldByName("PROFDESC").Value?.ToString()
                        };
                        userRoles.Add(userRole);
                    }
                    view.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error retrieving roles: {ex.Message}");
                    var accpacError = Sage300SessionManager.Instance.accpacSessionErrorsHandler(ex);
                    throw new InvalidOperationException($"Failed to retrive roles from Sage 300 \n Accpac Error {accpacError.ToString()}", ex);
                }
                return userRoles;
             });
        }

        public async Task<List<UserRolesDto>> GetUserRoles()
        {
            return await Sage300TaskExecutor.Instance.ExecuteAsync(() =>
            {
                var userRolesMap = new Dictionary<string, UserRolesDto>();
                try
                {
                    var dbLink = Sage300SessionManager.Instance.GetSystemDBLink();
                    using (var view = dbLink.OpenView("AS0002"))
                    {
                        view.Browse("", true);

                        while (view.Fetch(false))
                        {
                            var userId = view.Fields.FieldByName("USERID").Value?.ToString();

                            if (string.IsNullOrWhiteSpace(userId))
                                continue;

                            if (!userRolesMap.TryGetValue(userId, out var userDto))
                            {
                                userDto = new UserRolesDto
                                {
                                    UserId = userId,
                                    Roles = new List<RolesDto>()
                                };

                                userRolesMap[userId] = userDto;
                            }

                            var role = new RolesDto
                            {
                                ProgramId = view.Fields.FieldByName("PGMID").Value?.ToString(),
                                ProgramVersion = "71A",
                                GroupId = view.Fields.FieldByName("PROFILEID").Value?.ToString()
                            };

                            userDto.Roles.Add(role);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error retrieving user roles: {ex.Message}");
                    var accpacError = Sage300SessionManager.Instance.accpacSessionErrorsHandler(ex);
                    throw new InvalidOperationException($"Failed to retrive user roles from Sage 300 \n Accpac Error {accpacError.ToString()}", ex);

                }
                return userRolesMap.Values.ToList();
            });
        }




    }

    


}