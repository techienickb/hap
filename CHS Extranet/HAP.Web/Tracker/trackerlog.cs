using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Net;

namespace HAP.Web.Tracker
{
    public class trackerlog : List<trackerlogentry>
    {
        public trackerlog() : base()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/tracker.xml"));
            foreach (XmlNode node in doc.SelectNodes("/Tracker/Event"))
                this.Add(new trackerlogentry(node));
        }

        public void Filter(IPAddress IP)
        {
            List<trackerlogentry> removeentries = new List<trackerlogentry>();
            foreach (trackerlogentry entry in this)
                if (entry.IP.ToString() != IP.ToString()) removeentries.Add(entry);
            foreach (trackerlogentry entry in removeentries) this.Remove(entry);
        }

        public void Filter(TrackerStringValue Property, string Value)
        {
            List<trackerlogentry> removeentries = new List<trackerlogentry>();
            foreach (trackerlogentry entry in this)
                switch (Property)
                {
                    case TrackerStringValue.ComputerName:
                        if (entry.ComputerName != Value) removeentries.Add(entry);
                        break;
                    case TrackerStringValue.UserName:
                        if (entry.UserName != Value) removeentries.Add(entry);
                        break;
                    case TrackerStringValue.DomainName:
                        if (entry.DomainName != Value) removeentries.Add(entry);
                        break;
                    case TrackerStringValue.LogonServer:
                        if (entry.LogonServer != Value) removeentries.Add(entry);
                        break;
            }
            foreach (trackerlogentry entry in removeentries) this.Remove(entry);
        }

        public void Filter(TrackerDateTimeValue Property, DateTime Value)
        {
            List<trackerlogentry> removeentries = new List<trackerlogentry>();
            foreach (trackerlogentry entry in this)
                switch (Property)
                {
                    case TrackerDateTimeValue.LogOn:
                        if (entry.LogOnDateTime.Date != Value.Date) removeentries.Add(entry);
                        break;
                    case TrackerDateTimeValue.LogOff:
                        if (entry.LogOffDateTime.Date != Value.Date) removeentries.Add(entry);
                        break;
                }
            foreach (trackerlogentry entry in removeentries) this.Remove(entry);
        }

        public static trackerlog Current { get { return new trackerlog(); } }
    }

    public enum TrackerStringValue { ComputerName, UserName, DomainName, LogonServer }
    public enum TrackerDateTimeValue { LogOn, LogOff }

    public class trackerlogentry
    {
        public IPAddress IP { get; set; }
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string DomainName { get; set; }
        public string LogonServer { get; set; }
        public DateTime LogOnDateTime { get; set; }
        public DateTime LogOffDateTime { get; set; }

        public trackerlogentry(XmlNode node)
        {
            IP = IPAddress.Parse(node.Attributes["ip"].Value);
            ComputerName = node.Attributes["computername"].Value;
            UserName = node.Attributes["username"].Value;
            DomainName = node.Attributes["domainname"].Value;
            LogonServer = node.Attributes["logonserver"].Value;
            if (!string.IsNullOrWhiteSpace(node.Attributes["logoffdatetime"].Value))
                LogOffDateTime = DateTime.Parse(node.Attributes["logoffdatetime"].Value);
            if (!string.IsNullOrWhiteSpace(node.Attributes["logondatetime"].Value))
                LogOnDateTime = DateTime.Parse(node.Attributes["logondatetime"].Value);
        }
    }
}