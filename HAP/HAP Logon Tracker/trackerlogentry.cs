using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Net;
using System.IO;

namespace HAP.Tracker.UI
{
    public class trackerlogentry
    {
        public string IP { get; set; }
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string DomainName { get; set; }
        public string LogonServer { get; set; }
        public string OS { get; set; }
        public string LogOnDateTime { get; set; }
        public string LogOffDateTime { get; set; }

        public trackerlogentry()
        {
        }
    }


    public class trackerlogentrysmall
    {
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string DomainName { get; set; }
        public string LogOnDateTime { get; set; }

        public trackerlogentrysmall() { }
    }
}