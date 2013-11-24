using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Net;
using System.IO;

namespace HAP.Tracker
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

        public trackerlogentry(XmlNode node)
        {
            IP = node.Attributes["ip"].Value;
            ComputerName = node.Attributes["computername"].Value;
            UserName = node.Attributes["username"].Value;
            DomainName = node.Attributes["domainname"].Value;
            LogonServer = node.Attributes["logonserver"].Value;
            OS = node.Attributes["os"].Value;
            if (!string.IsNullOrWhiteSpace(node.Attributes["logoffdatetime"].Value))
                LogOffDateTime = node.Attributes["logoffdatetime"].Value;
            else LogOffDateTime = "";
            LogOnDateTime = node.Attributes["logondatetime"].Value;
        }
        public trackerlogentry(string IP, string Computer, string User, string Domain, string LogonServer, string os, string LogonDateTime)
        {
            this.IP = IP;
            this.ComputerName = Computer;
            this.UserName = User;
            this.DomainName = Domain;
            this.OS = os;
            this.LogonServer = LogonServer;
            this.LogOnDateTime = LogonDateTime;
            this.LogOffDateTime = null;
        }



        public trackerlogentry()
        {
            this.LogOffDateTime = null;
        }
    }


    public class trackerlogentrysmall
    {
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string DomainName { get; set; }
        public string LogOnDateTime { get; set; }

        public trackerlogentrysmall(XmlNode node)
        {
            ComputerName = node.Attributes["computername"].Value;
            UserName = node.Attributes["username"].Value;
            DomainName = node.Attributes["domainname"].Value;
            LogOnDateTime = node.Attributes["logondatetime"].Value;
        }
        public trackerlogentrysmall(string Computer, string User, string Domain, string LogonDateTime)
        {
            this.ComputerName = Computer;
            this.UserName = User;
            this.DomainName = Domain;
            this.LogOnDateTime = LogonDateTime;
        }

        public static trackerlogentrysmall Convert(HAP.Data.Tracker.trackerlogentrysmall o)
        {
            return new trackerlogentrysmall(o.ComputerName, o.UserName, o.DomainName, o.LogOnDateTime.ToString("f"));
        }
        public static trackerlogentrysmall[] Convert(HAP.Data.Tracker.trackerlogentrysmall[] o)
        {
            List<trackerlogentrysmall> ls = new List<trackerlogentrysmall>();
            foreach (HAP.Data.Tracker.trackerlogentrysmall o1 in o) ls.Add(Convert(o1));
            return ls.ToArray();
        }
        public trackerlogentrysmall() { }
    }
}