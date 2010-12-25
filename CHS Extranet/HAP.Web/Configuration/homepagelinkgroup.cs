using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class homepagelinkgroup : ConfigurationElement
    {
        public homepagelinkgroup(string name, string showto)
        {
            this.Name = name; this.ShowTo = showto;
        }

        public homepagelinkgroup() { }
        public homepagelinkgroup(string elementName) { this.Name = elementName; }

        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("showto", DefaultValue = "", IsRequired = true)]
        public string ShowTo
        {
            get { return (string)this["showto"]; }
            set { this["showto"] = value; }
        }

        [ConfigurationProperty("links", IsDefaultCollection = true, IsRequired = true)]
        public homepagelinkgrouplinks Links
        {
            get { return (homepagelinkgrouplinks)base["links"]; }
        }
    }
}