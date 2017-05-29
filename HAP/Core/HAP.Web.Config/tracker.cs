using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class Tracker
    {
        private XmlElement el;
        private XmlDocument doc;
        public Tracker(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/Tracker") == null) Initialize();
            this.el = (XmlElement)doc.SelectSingleNode("/hapConfig/Tracker");
        }

        public void Initialize()
        {
            XmlElement e = doc.CreateElement("Tracker");
            e.SetAttribute("maxstudentlogons", "1");
            e.SetAttribute("maxstafflogons", "4");
            e.SetAttribute("overridecode", "3600");
            e.SetAttribute("provider", "XML");
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }

        public int MaxStudentLogons
        {
            get { return int.Parse(el.GetAttribute("maxstudentlogons")); }
            set { el.SetAttribute("maxstudentlogons", value.ToString()); }
        }
        public int MaxStaffLogons
        {
            get { return int.Parse(el.GetAttribute("maxstafflogons")); }
            set { el.SetAttribute("maxstafflogons", value.ToString()); }
        }
        public string Provider
        {
            get { return el.GetAttribute("provider"); }
            set { el.SetAttribute("provider", value); }
        }
        public string OverrideCode
        {
            get { return el.GetAttribute("overridecode"); }
            set { el.SetAttribute("overridecode", value); }
        }
    }
}
