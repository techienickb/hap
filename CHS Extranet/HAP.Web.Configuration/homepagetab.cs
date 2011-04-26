using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class homepagetab : ConfigurationElement
    {
        public homepagetab(string name, string showto)
        {
            this.Name = name; this.ShowTo = showto;
        }

        public homepagetab() { }
        public homepagetab(string elementName) { this.Name = elementName; }

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
        [ConfigurationProperty("allowupdateto", DefaultValue = "", IsRequired = false)]
        public string AllowUpdateTo
        {
            get { return (string)this["allowupdateto"]; }
            set { this["allowupdateto"] = value; }
        }
        [ConfigurationProperty("showspace", DefaultValue = false, IsRequired = false)]
        public bool ShowSpace
        {
            get { return (bool)this["showspace"]; }
            set { this["showspace"] = value; }
        }
    }
}
