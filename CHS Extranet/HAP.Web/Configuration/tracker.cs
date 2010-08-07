using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class tracker : ConfigurationElement
    {
        [ConfigurationProperty("maxstudentlogons", DefaultValue = 1, IsRequired = false)]
        public int MaxStudentLogons
        {
            get { return (int)this["maxstudentlogons"]; }
            set { this["maxstudentlogons"] = value; }
        }

        [ConfigurationProperty("maxstafflogons", DefaultValue = 0, IsRequired = false)]
        public int MaxStaffLogons
        {
            get { return (int)this["maxstafflogons"]; }
            set { this["maxstafflogons"] = value; }
        }
    }
}