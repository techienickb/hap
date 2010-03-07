﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace CHS_Extranet.Configuration
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
        [ConfigurationProperty("studentemailformat", DefaultValue = "", IsRequired = false)]
        public string StudentEmailFormat
        {
            get { return (string)this["studentemailformat"]; }
            set { this["studentemailformat"] = value; }
        }
        [ConfigurationProperty("adminemailaddress", DefaultValue = "", IsRequired = false)]
        public string AdminEmailAddress
        {
            get { return (string)this["adminemailaddress"]; }
            set { this["adminemailaddress"] = value; }
        }

        [ConfigurationProperty("smtpserver", DefaultValue = "", IsRequired = false)]
        public string SMTPServer
        {
            get { return (string)this["smtpserver"]; }
            set { this["smtpserver"] = value; }
        }
    }
}