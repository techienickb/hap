using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace HAP.Timetable
{
    public class TimetableRecord : IComparable
    {
        public TimetableRecord() { }

        public static TimetableRecord Prase(XmlNode node)
        {
            string p = node.Attributes["period"] == null ? "" : node.Attributes["period"].Value;
            if (p.StartsWith("Da10")) p = p.Replace("Da10", "Day10");
            if (p.EndsWith("Reg")) p = p.Replace("Reg", "0");
            else if (p.EndsWith("Reg")) p = p.Replace("Reg", "5");
            else if (p.EndsWith(":5")) p = p.Replace(":5", ":6");

            return new TimetableRecord
            {
                UPN = node.Attributes["upn"].Value,
                YearGroup = node.Attributes["year"] == null ? "" : node.Attributes["year"].Value,
                Name = node.Attributes["name"] == null ? "" : node.Attributes["name"].Value,
                Description = node.Attributes["description"] == null ? "" : node.Attributes["description"].Value,
                Room = node.Attributes["room"] == null ? "" : node.Attributes["room"].Value,
                Teacher = node.Attributes["teacher"] == null ? "" : node.Attributes["teacher"].Value,
                Period = node.Attributes["period"] == null ? "" : node.Attributes["period"].Value,
                StartTime = node.Attributes["starttime"] == null ? "" : node.Attributes["starttime"].Value,
                EndTime =  node.Attributes["endtime"] == null ? "" : node.Attributes["endtime"].Value,
                SortRef = p

            }; 
        }
        public string UPN { get; set; }
        public string YearGroup { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Room { get; set; }
        public string Teacher { get; set; }
        public string Period { get; set; }
        public string SortRef { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public int CompareTo(object obj)
        {
            return SortRef.CompareTo(((TimetableRecord)obj).SortRef);
        }
    }
}