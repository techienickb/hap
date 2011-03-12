using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class mycomputer : ConfigurationElement
    {
        [ConfigurationProperty("uncpaths", IsDefaultCollection = false)]
        public uncpaths UNCPaths
        {
            get { return (uncpaths)base["uncpaths"]; }
        }

        [ConfigurationProperty("uploadfilters", IsDefaultCollection = false)]
        public uploadfilters UploadFilters
        {
            get { return (uploadfilters)base["uploadfilters"]; }
        }

        [ConfigurationProperty("hideextensions", DefaultValue = "", IsRequired = false)]
        public string HideExtensions
        {
            get { return (string)this["hideextensions"]; }
            set { this["hideextensions"] = value; }
        }
    }
}