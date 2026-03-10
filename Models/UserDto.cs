using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccpacUserManagement_Wrapper.Models
{

    public enum AccountType
    {
        User = 0,
        Timecard = 2
    }

    public enum AccountStatus
    {
        Active = 0,
        Inactive = 1
    }

    public enum Language
    {
        English = 0,
        ChineseSimplified = 1,
        ChineseTraditional = 2,
        Spanish = 3,
        French = 4
    }

    public enum AuthenticationMethod
    {
        Sage300 = 1,
        Windows = 2,
        Both = 3
    }

    public enum JobRole
    {
        None = 0,
        Owner = 1,
        CEO = 2,
        CFO = 3,
        Director = 4,
        AccountingManager = 5,
        Accountant = 6,
        AdminAssistant = 7,
        OperationsAssociate = 8,
        Salesperson = 9,
        MarketingAnalyst = 10,
        CustomerService = 11,
        Consultant = 12,
        HRPayrollAdmin = 13,
        ITAnalyst = 14,
        Other = 15
    }

    public enum LocaleList
    {
        English = 0,
        EnglishUS = 1,
        EnglishCanada = 2,
        EnglishAustralia = 3,
        EnglishSouthAfrica = 4,
        ChineseSimplifiedChina = 5,
        ChineseSimplifiedSingapore = 6,
        ChineseTraditionalHongKong = 7,
        ChineseTraditionalTaiwan = 8,
        Spanish = 9,
        SpanishSpain = 10,
        FrenchCanada = 11
    }
    public class UserDto
    {
        public string UserId { get; set; }            // USERID (String*8)
        public string UserName { get; set; }          // USERNAME (String*60)
        //public byte[] Password { get; set; }          // PASSWORD (Binary*64)

        public string Password { get; set; }

        public AccountType AccountType { get; set; }  // ACCTTYPE
        public AccountStatus AccountStatus { get; set; } // ACCTSTATUS

        public string LanguageCode { get; set; }      // LANGUAGE (String*3)
        public Language Language { get; set; }        // LANGLIST

        public bool MustChangePassword { get; set; }  // CHNGEPASS
        public bool CannotChangePassword { get; set; }// NOCHNGPASS
        public bool PasswordNeverExpires { get; set; }// PASSNOEXP
        public bool Disabled { get; set; }            // DISABLED
        public bool LockedOut { get; set; }           // LOCKEDOUT
        public bool Restricted { get; set; }          // RESTRICTED

        public bool OkMonday { get; set; }
        public bool OkTuesday { get; set; }
        public bool OkWednesday { get; set; }
        public bool OkThursday { get; set; }
        public bool OkFriday { get; set; }
        public bool OkSaturday { get; set; }
        public bool OkSunday { get; set; }

        /*public TimeSpan TimeBegin { get; set; }       // TIMEBEGIN
        public TimeSpan TimeEnd { get; set; }         // TIMEEND*/

        public string TimeBegin { get; set; }       // TIMEBEGIN
        public string TimeEnd { get; set; }         // TIMEEND

        public AuthenticationMethod AuthenticationMethod { get; set; } // AUTHMETH

        public string WindowsDomain { get; set; }     // WINDOMAIN
        public string WindowsUser { get; set; }       // WINUSER

        public string Phone { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }

        public JobRole Role { get; set; }

        public bool OptInEmail { get; set; }          // OPTIN

        public string LocaleId { get; set; }
        public LocaleList Locale { get; set; }

        public string Office365UserName { get; set; } // O365USRNAM
    }
}