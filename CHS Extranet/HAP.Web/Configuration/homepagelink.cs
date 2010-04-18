using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class homepagelink : ConfigurationElement
    {
        public homepagelink(string name, string description, string showto, string linklocation)
        {
            this.Name = name; this.Description = description; this.ShowTo = showto; this.LinkLocation = linklocation;
        }

        public homepagelink() { }
        public homepagelink(string elementName) { this.Name = elementName; }

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
        [ConfigurationProperty("description", DefaultValue = "", IsRequired = false)]
        public string Description
        {
            get { return (string)this["description"]; }
            set { this["description"] = value; }
        }
        [ConfigurationProperty("linklocation", DefaultValue = "", IsRequired = false)]
        public string LinkLocation
        {
            get { return (string)this["linklocation"]; }
            set { this["linklocation"] = value; }
        }
        [ConfigurationProperty("icon", DefaultValue = "~/images/icons/net.png", IsRequired = false)]
        public string Icon
        {
            get { return (string)this["icon"]; }
            set { this["icon"] = value; }
        }
    }
}