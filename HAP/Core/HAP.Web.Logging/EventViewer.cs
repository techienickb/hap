using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Web;

namespace HAP.Web.Logging
{
    public class EventViewer
    {
        public static void Log(string source, string message, EventLogEntryType type, [Optional] bool noweblog)
        {
            try
            {
                EventLog myLog = new EventLog("Application", ".", "Home Access Plus+");
                myLog.EnableRaisingEvents = true;
                if (type == EventLogEntryType.Error) myLog.WriteEntry("An error occurred in Home Access Plus+\r\n\r\nPage: " + source + "\r\n\r\n" + message, type);
                else myLog.WriteEntry("Home Access Plus+ Info\r\n\r\nPage: " + source + "\r\n\r\n" + message, type);
                if (!noweblog) HAP.Data.SQL.WebEvents.Log(DateTime.Now, type.ToString(), HttpContext.Current.User.Identity.IsAuthenticated ? HttpContext.Current.User.Identity.Name : "", HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, message);
                myLog.Close();
            }
            catch { }
        }

    }
}
