using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccpacUserManagement_Wrapper.Models
{
    public class RolesDto
    {
      public string ProgramId { get; set; }         // PROGRAMID (String*2)

      public string ProgramVersion { get; set; }      // PROGRAMVERSION (String*3)

      public string GroupId { get; set; }            // GROUPID (String*8)

      public string ResourceId { get; set; }         // RESOURCEID (String*10)

      public string GroupDescription { get; set; }    // GROUPDESCRIPTION (String*60)


    }
}