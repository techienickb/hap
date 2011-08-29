using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class BookingSystem
    {
        private XmlDocument doc;
        public BookingSystem(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/bookingsystem") == null) Initialize();
        }

        public Resources Resources { get { return new Resources(ref doc); } }
        public Subjects Subjects { get { return new Subjects(ref doc); } }
        public Lessons Lessons { get { return new Lessons(ref doc); } }
        public int MaxBookingsPerWeek { get; set; }
        public int MaxDays { get; set; }
        public bool KeepXmlClean { get; set; }
        public bool TwoWeekTimetable { get; set; }

        public void Initialize()
        {
            XmlElement e = doc.CreateElement("bookingsystem");
            e.AppendChild(doc.CreateElement("resources"));
            e.AppendChild(doc.CreateElement("lessons"));
            XmlElement s = doc.CreateElement("subjects");
            XmlElement ss = doc.CreateElement("subject");
            ss.SetAttribute("name", "General");
            s.AppendChild(ss);
            e.AppendChild(s);
            e.SetAttribute("maxbookingsperweek", "3");
            e.SetAttribute("maxdays", "14");
            e.SetAttribute("keepxmlclean", "true");
            e.SetAttribute("twoweektimetable", "true");
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }
    }
}
