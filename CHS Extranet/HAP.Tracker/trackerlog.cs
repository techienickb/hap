using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Net;
using System.IO;
using HAP.Web.Configuration;
using HAP.Data.Tracker;

namespace HAP.Tracker
{
    public class trackerlog : List<HAP.Data.Tracker.trackerlogentry>
    {
        public trackerlog():base()
        {
            if (hapConfig.Current.Tracker.Provider == "XML") foreach (HAP.Data.Tracker.trackerlogentry tle in xml.GetLogs(false).Where(l => !l.LogOffDateTime.HasValue)) this.Add(tle);
            else foreach (HAP.Data.Tracker.trackerlogentry tle in HAP.Data.SQL.Tracker.GetLogs(false).Where(l => !l.LogOffDateTime.HasValue)) this.Add(tle);
            this.Sort(delegate(HAP.Data.Tracker.trackerlogentry e1, HAP.Data.Tracker.trackerlogentry e2) { return e1.LogOnDateTime.CompareTo(e2.LogOnDateTime); });
        }

        public trackerlog(bool loadfull) : base()
        {
            if (hapConfig.Current.Tracker.Provider == "XML") foreach (HAP.Data.Tracker.trackerlogentry tle in xml.GetLogs(loadfull)) this.Add(tle);
            else foreach (HAP.Data.Tracker.trackerlogentry tle in HAP.Data.SQL.Tracker.GetLogs(loadfull)) this.Add(tle);
            this.Sort(delegate(HAP.Data.Tracker.trackerlogentry e1, HAP.Data.Tracker.trackerlogentry e2) { return e1.LogOnDateTime.CompareTo(e2.LogOnDateTime); });
        }

        public trackerlog(int year, int month) : base()
        {
            if (hapConfig.Current.Tracker.Provider == "XML") foreach (HAP.Data.Tracker.trackerlogentry tle in xml.GetLogs(true).Where(t => t.LogOnDateTime.Month == month && t.LogOnDateTime.Year == year)) this.Add(tle);
            else foreach (HAP.Data.Tracker.trackerlogentry tle in HAP.Data.SQL.Tracker.GetLogs(true).Where(t => t.LogOnDateTime.Month == month && t.LogOnDateTime.Year == year)) this.Add(tle);
            this.Sort(delegate(HAP.Data.Tracker.trackerlogentry e1, HAP.Data.Tracker.trackerlogentry e2) { return e1.LogOnDateTime.CompareTo(e2.LogOnDateTime); });
        }

        public trackerlog(int year, int month, string pc) : base()
        {
            if (hapConfig.Current.Tracker.Provider == "XML") foreach (HAP.Data.Tracker.trackerlogentry tle in xml.GetLogs(true).Where(t => t.LogOnDateTime.Month == month && t.LogOnDateTime.Year == year && t.ComputerName == pc)) this.Add(tle);
            else foreach (HAP.Data.Tracker.trackerlogentry tle in HAP.Data.SQL.Tracker.GetLogs(true).Where(t => t.LogOnDateTime.Month == month && t.LogOnDateTime.Year == year && t.ComputerName == pc)) this.Add(tle);
            this.Sort(delegate(HAP.Data.Tracker.trackerlogentry e1, HAP.Data.Tracker.trackerlogentry e2) { return e1.LogOnDateTime.CompareTo(e2.LogOnDateTime); });
        }

        public trackerlog(DateTime date, string pc) : base()
        {
            if (hapConfig.Current.Tracker.Provider == "XML") foreach (HAP.Data.Tracker.trackerlogentry tle in xml.GetLogs(true).Where(t => t.LogOnDateTime.Date == date && t.ComputerName == pc)) this.Add(tle);
            else foreach (HAP.Data.Tracker.trackerlogentry tle in HAP.Data.SQL.Tracker.GetLogs(true).Where(t => t.LogOnDateTime.Date == date && t.ComputerName == pc)) this.Add(tle);
            this.Sort(delegate(HAP.Data.Tracker.trackerlogentry e1, HAP.Data.Tracker.trackerlogentry e2) { return e1.LogOnDateTime.CompareTo(e2.LogOnDateTime); });
        }
        public trackerlog(DateTime date): base()
        {
            if (hapConfig.Current.Tracker.Provider == "XML") foreach (HAP.Data.Tracker.trackerlogentry tle in xml.GetLogs(true).Where(t => t.LogOnDateTime.Date == date)) this.Add(tle);
                else foreach (HAP.Data.Tracker.trackerlogentry tle in HAP.Data.SQL.Tracker.GetLogs(true).Where(t => t.LogOnDateTime.Date == date)) this.Add(tle);
            this.Sort(delegate(HAP.Data.Tracker.trackerlogentry e1, HAP.Data.Tracker.trackerlogentry e2) { return e1.LogOnDateTime.CompareTo(e2.LogOnDateTime); });
        }

        public void Filter(TrackerStringValue Property, string Value)
        {
            List<HAP.Data.Tracker.trackerlogentry> removeentries = new List<HAP.Data.Tracker.trackerlogentry>();
            foreach (HAP.Data.Tracker.trackerlogentry entry in this)
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
                    case TrackerStringValue.IP:
                        if (entry.IP != Value) removeentries.Add(entry);
                        break;
            }
            foreach (HAP.Data.Tracker.trackerlogentry entry in removeentries) this.Remove(entry);
        }

        public void Filter(TrackerDateTimeValue Property, DateTime Value)
        {
            List<HAP.Data.Tracker.trackerlogentry> removeentries = new List<HAP.Data.Tracker.trackerlogentry>();
            foreach (HAP.Data.Tracker.trackerlogentry entry in this)
                switch (Property)
                {
                    case TrackerDateTimeValue.LogOn:
                        if (entry.LogOnDateTime.Date != Value.Date) removeentries.Add(entry);
                        break;
                    case TrackerDateTimeValue.LogOff:
                        if (!entry.LogOffDateTime.HasValue) removeentries.Add(entry);
                        else if (entry.LogOffDateTime.Value.Date != Value.Date) removeentries.Add(entry);
                        break;
                }
            foreach (HAP.Data.Tracker.trackerlogentry entry in removeentries) this.Remove(entry);
        }

        public void Filter(TrackerDateTimeValue Property, string Value)
        {
            if (string.IsNullOrEmpty(Value) && Property == TrackerDateTimeValue.LogOff)
            {
                List<HAP.Data.Tracker.trackerlogentry> removeentries = new List<HAP.Data.Tracker.trackerlogentry>();
                foreach (HAP.Data.Tracker.trackerlogentry entry in this)
                    if (entry.LogOffDateTime.HasValue) removeentries.Add(entry);
                foreach (HAP.Data.Tracker.trackerlogentry entry in removeentries) this.Remove(entry);
            }
            else Filter(Property, DateTime.Parse(Value));
        }

        public static trackerlog Current { get { return new trackerlog(); } }

        public static trackerlog CurrentFull { get { return new trackerlog(true); } }
    }

    public enum TrackerStringValue { ComputerName, UserName, DomainName, LogonServer, IP }
    public enum TrackerDateTimeValue { LogOn, LogOff }
}