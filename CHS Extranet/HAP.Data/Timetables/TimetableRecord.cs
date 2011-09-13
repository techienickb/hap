using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace HAP.Data.Timetables
{
    public class TimetableRecord : IComparable
    {
        public TimetableRecord(XmlNode node)
        {
            this.node = node;
        }
        private XmlNode node;
        public string UPN { get { return node.Attributes["year"].Value; } }
        public string YearGroup { get { return node.Attributes["year"] == null ? "" : node.Attributes["year"].Value; } }
        public string Name { get { return node.Attributes["name"] == null ? "" : node.Attributes["name"].Value; } }
        public string Description { get { return node.Attributes["description"] == null ? "" : node.Attributes["description"].Value; } }
        public string Room { get { return node.Attributes["room"] == null ? "" : node.Attributes["room"].Value; } }
        public string Teacher { get { return node.Attributes["teacher"] == null ? "" : node.Attributes["teacher"].Value; } }
        public string Period { get { return node.Attributes["period"] == null ? "" : node.Attributes["period"].Value; } }
        public string SortRef 
        { 
            get 
            {
                string p = Period;
                if (p.StartsWith("Da10")) p = p.Replace("Da10", "Day10");
                if (p.EndsWith("Reg") && StartTime == "08:40") p = p.Replace("Reg", "0");
                else if (p.EndsWith("Reg")) p = p.Replace("Reg", "5");
                else if (p.EndsWith(":5")) p = p.Replace(":5", ":6");
                return p;
            } 
        }
        public string StartTime { get { return node.Attributes["starttime"] == null ? "" : node.Attributes["starttime"].Value; } }
        public string EndTime { get { return node.Attributes["endtime"] == null ? "" : node.Attributes["endtime"].Value; } }

        public int CompareTo(object obj)
        {
            return SortRef.CompareTo(((TimetableRecord)obj).SortRef);
        }
    }
}