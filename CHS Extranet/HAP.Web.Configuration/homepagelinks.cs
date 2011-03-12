using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class homepagelinks : ConfigurationElement
    {
        [ConfigurationProperty("groups", IsDefaultCollection = false)]
        public homepagelinkgroups Groups
        {
            get { return (homepagelinkgroups)base["groups"]; }
        }

        [ConfigurationProperty("buttons", IsDefaultCollection = true, IsRequired = true)]
        public homepagelinkgrouplinks Buttons
        {
            get { return (homepagelinkgrouplinks)base["buttons"]; }
        }
    }
}