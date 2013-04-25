using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class School
    {        
        private XmlDocument doc;
        private XmlElement el;
        public School(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/School") == null) Initialize();
            this.el = (XmlElement)doc.SelectSingleNode("/hapConfig/School");
        }
        public void Initialize()
        {
            XmlElement e = doc.CreateElement("School");
            e.SetAttribute("name", "");
            e.SetAttribute("website", "");
            e.SetAttribute("photohandler", "");
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }
        public string Name
        {
            get { return el.GetAttribute("name"); }
            set { el.SetAttribute("name", value); }
        }
        public string WebSite
        {
            get { return el.GetAttribute("website"); }
            set { el.SetAttribute("website", value); }
        }

        public string PhotoHandler
        {
            get { return el.GetAttribute("photohandler"); }
            set { el.SetAttribute("photohandler", value); }
        }
        public bool HidePhotoErrors
        {
            get { if (el.Attributes["hidephotoerror"] != null) return bool.Parse(el.GetAttribute("hidephotoerror")); return false; }
            set { el.SetAttribute("hidephotoerror", value.ToString()); }
        }
    }
}
