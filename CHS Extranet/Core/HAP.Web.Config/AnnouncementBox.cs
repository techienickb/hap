using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class AnnouncementBox
    {
        private XmlDocument doc;
        private XmlElement el;
        public AnnouncementBox(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/Homepage/AnnouncementBox") == null) Initialize();
            this.el = (XmlElement)doc.SelectSingleNode("/hapConfig/Homepage/AnnouncementBox");
        }
        public void Initialize()
        {
            XmlElement e = doc.CreateElement("AnnouncementBox");
            e.SetAttribute("showto", "All");
            e.SetAttribute("enableeditto", "Domain Admins");
            doc.SelectSingleNode("/hapConfig/Homepage").AppendChild(e);
        }
        public string ShowTo
        {
            get { return el.GetAttribute("showto"); }
            set { el.SetAttribute("showto", value); }
        }
        public string EnableEditTo
        {
            get { return el.GetAttribute("enableeditto"); }
            set { el.SetAttribute("enableeditto", value); }
        }
    }
}
