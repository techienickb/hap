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
            foreach (TrackerEvent te in sdc.TrackerEvents.Where(t => t.ComputerName == Computer && t.domainname == DomainName))
                te.LogoffDateTime = DateTime.Now;
            sdc.SubmitChanges();
        }

        public static trackerlogentry[] Logon(string Username, string Computer, string DomainName, string IP, string LogonServer, string OS)
        {
            sql2linqDataContext sdc = new sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.Tracker.Provider].ConnectionString);
            List<trackerlogentry> ll = new List<trackerlogentry>();
            foreach (TrackerEvent te in sdc.TrackerEvents.Where(t => t.Username == Username && t.domainname == DomainName && !t.LogoffDateTime.HasValue))
                ll.Add(new trackerlogentry(te.ip, te.ComputerName, te.Username, te.domainname, te.logonserver, te.os, te.LogonDateTime));
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
            return ll.ToArray();
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
    }
}
