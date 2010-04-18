using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class announcementBox : ConfigurationElement
    {
        [ConfigurationProperty("showto", DefaultValue = "All", IsRequired = true)]
        public string ShowTo
        {
            get { return (string)this["showto"]; }
            set { this["showto"] = value; }
        }

        [ConfigurationProperty("enableeditto", DefaultValue = "Domain Admins", IsRequired = true)]
        public string EnableEditTo
        {
            get { return (string)this["enableeditto"]; }
            set { this["enableeditto"] = value; }
        }
    }
}