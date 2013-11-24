using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.Tracker.UI
{
    public class LogonsList
    {
        public UT UserType { get; set; }
        public int MaxLogons { get; set; }
        public string OverrideCode { get; set; }
        public trackerlogentrysmall[] Logons { get; set; }
    }

    public enum UT { Admin, Staff, Student }
}