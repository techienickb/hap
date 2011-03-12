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
        public DateTime LogOffDateTime { get; set; }

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
            if (!string.IsNullOrWhiteSpace(node.Attributes["logondatetime"].Value))
                LogOnDateTime = DateTime.Parse(node.Attributes["logondatetime"].Value);
        }
        public trackerlogentry(string IP, string Computer, string User, string Domain, string LogonServer, string os, DateTime LogonDateTime)
        {
            this.IP = IP;
            ComputerName = Computer;
            UserName = User;
            DomainName = DomainName;
            OS = os;
            this.LogonServer = LogonServer;
            LogOnDateTime = LogOnDateTime;
        }

        public trackerlogentry()
        {
        }
    }
}