using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP.Data.Tracker;
using System.Configuration;
using HAP.Web.Configuration;

namespace HAP.Data.SQL
{
    public class Tracker
    {
        public static void Clear(string Computer, string DomainName)
        {
            sql2linqDataContext sdc = new sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.Tracker.Provider].ConnectionString);
            foreach (TrackerEvent te in sdc.TrackerEvents.Where(t => t.ComputerName == Computer && t.domainname == DomainName && !t.LogoffDateTime.HasValue))
                te.LogoffDateTime = DateTime.Now;
            sdc.SubmitChanges();
        }


        public static trackerlogentrysmall[] Poll(string Username, string Computer, string DomainName)
        {
            sql2linqDataContext sdc = new sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.Tracker.Provider].ConnectionString);
            List<trackerlogentrysmall> ll = new List<trackerlogentrysmall>();
            foreach (TrackerEvent te in sdc.TrackerEvents.Where(t => t.Username == Username && t.domainname == DomainName && !t.LogoffDateTime.HasValue && t.ComputerName != Computer))
                ll.Add(new trackerlogentrysmall(te.ComputerName, te.Username, te.domainname, te.LogonDateTime));
            return ll.ToArray();
        }

        public static trackerlogentrysmall[] Logon(string Username, string Computer, string DomainName, string IP, string LogonServer, string OS)
        {
            sql2linqDataContext sdc = new sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.Tracker.Provider].ConnectionString);
            if (sdc.TrackerEvents.Count(t => t.Username == Username && t.domainname == DomainName && !t.LogoffDateTime.HasValue && t.ComputerName == Computer) > 0)
                Clear(Computer, DomainName);
            TrackerEvent newe = new TrackerEvent();
            newe.LogonDateTime = DateTime.Now;
            newe.logonserver = LogonServer;
            newe.ip = IP;
            newe.ComputerName = Computer;
            newe.Username = Username;
            newe.domainname = DomainName;
            newe.os = OS;
            sdc.TrackerEvents.InsertOnSubmit(newe);
            sdc.SubmitChanges();
            return Poll(Username, Computer, DomainName);
        }

        public static trackerlogentry[] GetLogs(bool loadall)
        {
            sql2linqDataContext sdc = new sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.Tracker.Provider].ConnectionString);
            List<trackerlogentry> tle = new List<trackerlogentry>();
            foreach (TrackerEvent te in sdc.TrackerEvents) {
                trackerlogentry tle1 = new trackerlogentry(te.ip, te.ComputerName, te.Username, te.domainname, te.logonserver, te.os, te.LogonDateTime);
                if (te.LogoffDateTime.HasValue) tle1.LogOffDateTime = te.LogoffDateTime.Value;
                tle.Add(tle1);
            }
            return tle.ToArray();
        }

        public static void UpgradeFromXML()
        {
            sql2linqDataContext sdc = new sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.Tracker.Provider].ConnectionString);
            foreach (trackerlogentry tle in HAP.Data.Tracker.xml.GetLogs(true).OrderBy(t => t.LogOnDateTime))
            {
                TrackerEvent newe = new TrackerEvent();
                newe.LogonDateTime = tle.LogOnDateTime;
                newe.logonserver = tle.LogonServer;
                newe.ip = tle.IP;
                newe.ComputerName = tle.ComputerName;
                newe.Username = tle.UserName;
                newe.domainname = tle.DomainName;
                newe.os = tle.OS;
                if (tle.LogOffDateTime != null) newe.LogoffDateTime = tle.LogOffDateTime;
                sdc.TrackerEvents.InsertOnSubmit(newe);
            }
            sdc.SubmitChanges();
            xml.DeleteAll();
        }
    }
}
