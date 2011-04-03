using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class quotaserver : ConfigurationElement
    {
        public quotaserver(string expression, string server, string drive)
        {
            this.Expression = expression;
            this.Drive = drive;
            this.Server = server;
        }

        public quotaserver() { }
        public quotaserver(string elementName) { this.Expression = elementName; }

        [ConfigurationProperty("expression", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Expression
        {
            get { return (string)this["expression"]; }
            set { this["expression"] = value; }
        }
        [ConfigurationProperty("drive", IsRequired = true)]
        public string Drive
        {
            get { return (string)this["drive"]; }
            set { this["drive"] = value; }
        }

        [ConfigurationProperty("server", IsRequired = true)]
        public string Server
        {
            get { return (string)this["server"]; }
            set { this["server"] = value; }
        }
    }
}
