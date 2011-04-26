using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class baseSettings : ConfigurationElement
    {
        [ConfigurationProperty("establishmentname", DefaultValue = "", IsRequired = true)]
        public string EstablishmentName
        {
            get { return (string)this["establishmentname"]; }
            set { this["establishmentname"] = value; }
        }
        [ConfigurationProperty("establishmentcode", DefaultValue = "", IsRequired = true)]
        public string EstablishmentCode
        {
            get { return (string)this["establishmentcode"]; }
            set { this["establishmentcode"] = value; }
        }
        [ConfigurationProperty("studentphotohandler", DefaultValue = "", IsRequired = false)]
        public string StudentPhotoHandler
        {
            get { return (string)this["studentphotohandler"]; }
            set { this["studentphotohandler"] = value; }
        }
        [ConfigurationProperty("adminemailaddress", DefaultValue = "", IsRequired = false)]
        public string AdminEmailAddress
        {
            get { return (string)this["adminemailaddress"]; }
            set { this["adminemailaddress"] = value; }
        }

        [ConfigurationProperty("adminemailuser", DefaultValue = "admin", IsRequired = false)]
        public string AdminEmailUser
        {
            get { return (string)this["adminemailuser"]; }
            set { this["adminemailuser"] = value; }
        }

        [ConfigurationProperty("smtpserver", DefaultValue = "", IsRequired = false)]
        public string SMTPServer
        {
            get { return (string)this["smtpserver"]; }
            set { this["smtpserver"] = value; }
        }

        [ConfigurationProperty("smtpserverusername", DefaultValue = "", IsRequired = false)]
        public string SMTPServerUsername
        {
            get { return (string)this["smtpserverusername"]; }
            set { this["smtpserverusername"] = value; }
        }

        [ConfigurationProperty("smtpserverpassword", DefaultValue = "", IsRequired = false)]
        public string SMTPServerPassword
        {
            get { return (string)this["smtpserverpassword"]; }
            set { this["smtpserverpassword"] = value; }
        }

        [ConfigurationProperty("smtpserverssl", DefaultValue = false, IsRequired = false)]
        public bool SMTPServerSSL
        {
            get { return bool.Parse(this["smtpserverssl"].ToString()); }
            set { this["smtpserverssl"] = value.ToString(); }
        }

        [ConfigurationProperty("smtpserverport", DefaultValue = 25, IsRequired = false)]
        public int SMTPServerPort
        {
            get { return int.Parse(this["smtpserverport"].ToString()); }
            set { this["smtpserverport"] = value.ToString(); }
        }
    }
}