using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Net;
using System.IO;

namespace HAP.Data.Tracker
{
    public class trackerlogentry
    {
        public string IP { get; set; }
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string DomainName { get; set; }
        public string LogonServer { get; set; }
        public string OS { get; set; }
        public DateTime LogOnDateTime { get; set; }
        public Nullable<DateTime> LogOffDateTime { get; set; }

        public trackerlogentry(XmlNode node)
        {
            IP = node.Attributes["ip"].Value;
            ComputerName = node.Attributes["computername"].Value;
            UserName = node.Attributes["username"].Value;
            DomainName = node.Attributes["domainname"].Value;
            LogonServer = node.Attributes["logonserver"].Value;
            OS = node.Attributes["os"].Value;
            if (!string.IsNullOrWhiteSpace(node.Attributes["logoffdatetime"].Value))
                LogOffDateTime = DateTime.Parse(node.Attributes["logoffdatetime"].Value);
            else LogOffDateTime = null;
            LogOnDateTime = DateTime.Parse(node.Attributes["logondatetime"].Value);
        }
        public trackerlogentry(string IP, string Computer, string User, string Domain, string LogonServer, string os, DateTime LogonDateTime)
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
        public DateTime LogOnDateTime { get; set; }

        public trackerlogentrysmall(XmlNode node)
        {
            ComputerName = node.Attributes["computername"].Value;
            UserName = node.Attributes["username"].Value;
            DomainName = node.Attributes["domainname"].Value;
            LogOnDateTime = DateTime.Parse(node.Attributes["logondatetime"].Value);
        }
        public trackerlogentrysmall(string Computer, string User, string Domain, DateTime LogonDateTime)
        {
            this.ComputerName = Computer;
            this.UserName = User;
            this.DomainName = Domain;
            this.LogOnDateTime = LogonDateTime;
        }

        public trackerlogentrysmall() { }
    }
}