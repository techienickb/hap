using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class uploadfilter : ConfigurationElement
    {
        public uploadfilter(string name, string filter, string enablefor)
        {
            this.Name = name; this.Filter = filter; this.EnableFor = enablefor;
        }

        public uploadfilter() { }
        public uploadfilter(string elementName) { this.Name = elementName; }

        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("filter", DefaultValue = "*.*", IsRequired = true)]
        public string Filter
        {
            get { return (string)this["filter"]; }
            set { this["filter"] = value; }
        }
        [ConfigurationProperty("enablefor", DefaultValue = "All", IsRequired = false)]
        public string EnableFor
        {
            get { return (string)this["enablefor"]; }
            set { this["enablefor"] = value; }
        }
        public override string ToString()
        {
            return string.Format("{0} ({2})|{1}", this.Name, this.Filter, this.Filter.Replace(";", "\\ "));
        }
    }
}