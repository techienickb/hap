using System;
using System.Collections.Generic;
using System.Linq;
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
					if (days.Count(d => d.Day == int.Parse(tr.SortRef.Split(new char[] { ':' })[0].Remove(0, 3))) == 0)
					{
						TimetableDay day = new TimetableDay();
						day.Day = int.Parse(tr.SortRef.Split(new char[] { ':' })[0].Remove(0, 3));
						days.Add(day);
					}
					TimetableDay td = days.Single(d => d.Day == int.Parse(tr.SortRef.Split(new char[] { ':' })[0].Remove(0, 3)));
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
	}
}