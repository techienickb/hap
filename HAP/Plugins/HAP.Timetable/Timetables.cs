using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace HAP.Timetable
{
	public class Timetables
	{
		public static TimetableDay[] getRecords(string UPN)
		{
			List<TimetableRecord> records = new List<TimetableRecord>();
			XmlDocument doc = new XmlDocument();
			doc.Load(HttpContext.Current.Server.MapPath("~/app_data/timetables.xml"));
			List<TimetableDay> days = new List<TimetableDay>();
			foreach (XmlNode n in doc.SelectNodes("/timetables/record[@upn='" + UPN + "']"))
			{
				TimetableRecord tr = TimetableRecord.Prase(n);
				try
				{
                    int day = 0;
                    if (new Regex("\\d\\w{3}:\\d").IsMatch(tr.SortRef))
                    {
                        int weeknum = int.Parse(tr.SortRef.Substring(0, 1));
                        day = DayToInt(tr.SortRef.Substring(1, 3));
                        day = day + (weeknum * 5) - 5;
                    }
                    else day = int.Parse(tr.SortRef.Split(new char[] { ':' })[0].Remove(0, 3));
                    if (days.Count(d => d.Day == day) == 0)
                    {
                        TimetableDay day1 = new TimetableDay();
                        day1.Day = day;
                        days.Add(day1);
                    }
                    TimetableDay td = days.Single(d => d.Day == day);
					if (td.Lessons.Count(t => t.SortRef == tr.SortRef) == 0) td.Lessons.Add(tr);
					td.Lessons.Sort();
				}
				catch
				{
				}
			}
			days.Sort();
            List<JSTimetableDay> days2 = new List<JSTimetableDay>();
            foreach (TimetableDay d in days)
                days2.Add(JSTimetableDay.Parse(d));
			return days.ToArray();
		}

        static int DayToInt(string day)
        {
            switch (day.ToLower())
            {
                case "mon": return 1;
                case "tue": return 2;
                case "wed": return 3;
                case "thu": return 4;
                default: return 5;
            }
        }
	}
}