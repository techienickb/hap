using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Tracker
{
    public class webtrackerlogentry
    {
        public string DateTime { get; set; }
        public string Username { get; set; }
        public string IP { get; set; }
        public string ComputerName { get; set; }
        public string OS { get; set; }
        public string Browser { get; set; }
        public string Details { get; set; }
        public string EventType { get; set; }

        public static webtrackerlogentry Convert(HAP.Data.SQL.WebTrackerEvent e)
        {
            return new webtrackerlogentry { Browser = e.Browser, ComputerName = e.ComputerName, DateTime = e.DateTime.ToString(), Details = e.Details, EventType = e.EventType, IP = e.IP, OS = e.OS, Username = e.Username };
        }
    }
}
