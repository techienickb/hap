using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class homepage : ConfigurationElement
    {
        [ConfigurationProperty("linkgroups", IsDefaultCollection = false)]
        public homepagelinkgroups Groups
        {
            get { return (homepagelinkgroups)base["linkgroups"]; }
        }

        [ConfigurationProperty("tabs", IsDefaultCollection = true, IsRequired = true)]
        public homepagetabs Tabs
        {
            get { return (homepagetabs)base["tabs"]; }
        }
    }
}