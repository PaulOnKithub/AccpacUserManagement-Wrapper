using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccpacUserManagement_Wrappe.Models
{
    public class AccpacErrorDto
    {
        public string Message { get; set; }

        public string Source { get; set; }

        public override string ToString()
        {
            return $"Accpac Error, Message {Message}, Source -{Source}";
        }
    }
}