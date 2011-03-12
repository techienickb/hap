using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class ouobject : ConfigurationElement
    {
        public ouobject(string name, string value)
        {
            this.Name = name;
            this.Path = value;
        }

        public ouobject() { }
        public ouobject(string elementName) { this.Name = elementName; }

        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("ignore", IsRequired = false, DefaultValue=false)]
        public bool Ignore
        {
            get { return (bool)this["ignore"]; }
            set { this["ignore"] = value; }
        }
    }
}