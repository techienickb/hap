using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.Web.Tracker
{
    public class LogonsList
    {
        public UT UserType { get; set; }
        public int MaxLogons { get; set; }
        public string OverrideCode { get; set; }
        public trackerlogentry[] Logons { get; set; }
    }

    public enum UT { Admin, Staff, Student }
}