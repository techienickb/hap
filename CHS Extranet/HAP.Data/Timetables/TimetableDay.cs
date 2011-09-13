using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.Data.Timetables
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
}