using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HAP.Web.Logging
{
    public class EventViewer
    {
        public static void Log(string source, string message, EventLogEntryType type)
        {
            try
            {
                if (!EventLog.SourceExists("Home Access Plus+")) return;
                EventLog myLog = new EventLog("Application");
                myLog.Source = "Home Access Plus+";
                if (type == EventLogEntryType.Error)
                    myLog.WriteEntry("An error occurred in Home Access Plus+\r\n\r\nPage: " + source + "\r\n\r\n" + message, type);
                else myLog.WriteEntry("Home Access Plus+ Info\r\n\r\nPage: " + source + "\r\n\r\n" + message, type);
            }
            catch { }
        }
    }
}
