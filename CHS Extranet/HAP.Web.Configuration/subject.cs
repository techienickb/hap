using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class subject : ConfigurationElement, IComparable
    {
        public subject() { }
        public subject(string elementName) { this.Name = elementName; }

        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public int CompareTo(object obj)
        {
            return this.Name.CompareTo(((subject)obj).Name);
        }
    }
}