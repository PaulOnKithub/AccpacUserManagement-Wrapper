using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccpacUserManagement_Wrapper.Models
{
    public class UserRolesDto
    {
        public string UserId { get; set; }            // USERID (String*8)
        public List<RolesDto> Roles { get; set; }     // List of Roles associated with the user
    }
}