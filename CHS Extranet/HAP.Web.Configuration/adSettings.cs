using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class adSettings : ConfigurationElement
    {
        [ConfigurationProperty("adusername", DefaultValue = "", IsRequired = true)]
        public string ADUsername
        {
            get { return (string)this["adusername"]; }
            set { this["adusername"] = value; }
        }
        [ConfigurationProperty("adpassword", DefaultValue = "", IsRequired = true)]
        public string ADPassword
        {
            get { return (string)this["adpassword"]; }
            set { this["adpassword"] = value; }
        }
        [ConfigurationProperty("adconnectionstring", DefaultValue = "ADConnectionString", IsRequired = true)]
        public string ADConnectionString
        {
            get { return (string)this["adconnectionstring"]; }
            set { this["adconnectionstring"] = value; }
        }
        [ConfigurationProperty("studentsgroupname", DefaultValue = "", IsRequired = true)]
        public string StudentsGroupName
        {
            get { return (string)this["studentsgroupname"]; }
            set { this["studentsgroupname"] = value; }
        }

        [ConfigurationProperty("ouobjects", IsDefaultCollection = false)]
        public ouobjects OUObjects
        {
            get { return (ouobjects)base["ouobjects"]; }
        }

        public string DomainName
        {
            get
            {
                string _ActiveDirectoryConnectionString = ConfigurationManager.ConnectionStrings[this.ADConnectionString].ConnectionString;
                _ActiveDirectoryConnectionString = _ActiveDirectoryConnectionString.Remove(_ActiveDirectoryConnectionString.LastIndexOf(','));
                return _ActiveDirectoryConnectionString.Substring(_ActiveDirectoryConnectionString.LastIndexOf(@"/") + 1, _ActiveDirectoryConnectionString.Length - _ActiveDirectoryConnectionString.LastIndexOf(@"/") - 1).Replace("DC=", "");
            }
        }
    }
}