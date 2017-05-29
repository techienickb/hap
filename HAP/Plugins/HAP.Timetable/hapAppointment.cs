using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Timetable
{
    public class hapAppointment
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string Name { get; set; }
        public string Room { get; set; }
        public bool AllDay { get; set; }
        public string Color { get; set; }
        public static hapAppointment Parse(Appointment a)
        {
            return new hapAppointment { Start = a.Start.ToString("o"), End = a.End.ToString("o"), Name = a.Subject, Room = a.Location, AllDay = a.IsAllDayEvent };
        }
        public static hapAppointment Parse(Appointment a, string Color)
        {
            return new hapAppointment { Start = a.Start.ToString("o"), End = a.End.ToString("o"), Name = a.Subject, Room = a.Location, AllDay = a.IsAllDayEvent, Color = Color };
        }
    }
}
