using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class uncpath : ConfigurationElement
    {
        public uncpath(string drive, string name, string enablereadto, string enablewriteto, string unc)
        {
            this.Drive = drive; this.Name = name; this.EnableReadTo = enablereadto; this.EnableWriteTo = enablewriteto; this.UNC = unc;
        }

        public uncpath() { }
        public uncpath(string elementName) { this.Drive = elementName; }

        [ConfigurationProperty("drive", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Drive
        {
            get { return (string)this["drive"]; }
            set { this["drive"] = value; }
        }
        [ConfigurationProperty("enablereadto", DefaultValue = "", IsRequired = true)]
        public string EnableReadTo
        {
            get { return (string)this["enablereadto"]; }
            set { this["enablereadto"] = value; }
        }
        [ConfigurationProperty("enablewriteto", DefaultValue = "", IsRequired = true)]
        public string EnableWriteTo
        {
            get { return (string)this["enablewriteto"]; }
            set { this["enablewriteto"] = value; }
        }
        [ConfigurationProperty("name", DefaultValue = "", IsRequired = false)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("unc", DefaultValue = "", IsRequired = true)]
        public string UNC
        {
            get { return (string)this["unc"]; }
            set { this["unc"] = value; }
        }
        [ConfigurationProperty("enablemove", DefaultValue = false, IsRequired = false)]
        public bool EnableMove
        {
            get { return (bool)this["enablemove"]; }
            set { this["enablemove"] = value; }
        }

        [ConfigurationProperty("usage", DefaultValue = UsageMode.DriveSpace, IsRequired = false)]
        public UsageMode Usage
        {
            get { return (UsageMode)this["enablemove"]; }
            set { this["enablemove"] = value; }
        }
    }

    public enum UsageMode { DriveSpace, Quota }
}