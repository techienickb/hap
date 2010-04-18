using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class bookingResource : ConfigurationElement
    {
        public bookingResource(string name, ResourceType type)
        {
            this.Name = name;
            this.ResourceType = type;
        }

        public bookingResource() { }
        public bookingResource(string elementName) { this.Name = elementName; }

        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("type", DefaultValue = "ITRoom", IsRequired = true)]
        public ResourceType ResourceType
        {
            get { return (ResourceType)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("emailadmin", DefaultValue = false)]
        public bool EmailAdmin
        {
            get { return (bool)this["emailadmin"]; }
            set { this["emailadmin"] = value; }
        }

        [ConfigurationProperty("enablecharging", DefaultValue = false)]
        public bool EnableCharging
        {
            get { return (bool)this["enablecharging"]; }
            set { this["enablecharging"] = value; }
        }
    }

    public enum ResourceType { ITRoom, Laptops, Equipment, Other }
}