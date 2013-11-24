using HAP.Data.Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.Tracker
{
    public class LogonsList
    {
        public UT UserType { get; set; }
        public int MaxLogons { get; set; }
        public string OverrideCode { get; set; }
        public trackerlogentrysmall[] Logons { get; set; }
        public static LogonsList Convert(HAP.Data.Tracker.LogonsList ll)
        {
            LogonsList l = new LogonsList();
            l.UserType = ll.UserType;
            l.OverrideCode = ll.OverrideCode;
            l.MaxLogons = ll.MaxLogons;
            l.Logons = trackerlogentrysmall.Convert(ll.Logons);
            return l;
        }
    }
}