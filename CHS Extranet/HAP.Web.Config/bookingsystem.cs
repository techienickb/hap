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
        public int MaxBookingsPerWeek { get { return int.Parse(doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["maxbookingsperweek"].Value); } set { doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["maxbookingsperweek"].Value = value.ToString(); } }
        public int MaxDays { get { return int.Parse(doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["maxdays"].Value); } set { doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["maxdays"].Value = value.ToString(); } }
        public bool KeepXmlClean { get { return bool.Parse(doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["keepxmlclean"].Value); } set { doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["keepxmlclean"].Value = value.ToString(); } }
        public bool TwoWeekTimetable { get { return bool.Parse(doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["twoweektimetable"].Value); } set { doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["twoweektimetable"].Value = value.ToString(); } }
        public string Admins { get { return doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["admins"].Value; } set { doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["admins"].Value = value; } }

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
            e.SetAttribute("admins", "");
            e.SetAttribute("keepxmlclean", "true");
            e.SetAttribute("twoweektimetable", "true");
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }
    }
}
