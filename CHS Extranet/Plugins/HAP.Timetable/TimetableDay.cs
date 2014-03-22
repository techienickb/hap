using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.Timetable
{
    public class TimetableDay : IComparable
    {
        public TimetableDay()
        {
            Lessons = new List<TimetableRecord>();
        }

        public int Day { get; set; }
        public List<TimetableRecord> Lessons { get; set; }

        public int CompareTo(object obj)
        {
            return Day.CompareTo(((TimetableDay)obj).Day);
        }
    }

    public class JSTimetableDay
    {
        public JSTimetableDay()
        {
        }

        public int Day { get; set; }
        public TimetableRecord[] Lessons { get; set; }

        public static JSTimetableDay Parse(TimetableDay day)
        {
            return new JSTimetableDay { Day = day.Day, Lessons = day.Lessons.ToArray() };
        }
    }
}