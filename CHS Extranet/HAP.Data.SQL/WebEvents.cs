using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using HAP.Web.Configuration;
using System.Threading;

namespace HAP.Data.SQL
{
    public class WebEvents
    {
        public static void Log(WebTrackerEvent Event)
        {
            if (hapConfig.Current.Tracker.Provider == "XML") return;
            sql2linqDataContext sdc = new sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.Tracker.Provider].ConnectionString);
            sdc.WebTrackerEvents.InsertOnSubmit(Event);
            sdc.SubmitChanges();
        }

        public static void Log(object o)
        {
            Log(o as WebTrackerEvent);
        }

        public static void Log(DateTime datetime, string type, string Username, string IP, string OS, string Browser, string ComputerName, string Details)
        {
            WebTrackerEvent Event = new WebTrackerEvent();
            Event.EventType = type;
            Event.DateTime = datetime;
            Event.Username = Username;
            Event.IP = IP;
            Event.OS = OS;
            Event.Browser = Browser;
            Event.ComputerName = ComputerName;
            Event.Details = Details;
            Log(Event);
        }

        public static WebTrackerEvent[] Events
        {
            get
            {
                if (hapConfig.Current.Tracker.Provider == "XML") return new WebTrackerEvent[] { };
                sql2linqDataContext sdc = new sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.Tracker.Provider].ConnectionString);
                return sdc.WebTrackerEvents.ToArray();
            }
        }
    }
}
