using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class HelpDesk
    {        
        private XmlDocument doc;
        private XmlElement el;
        public HelpDesk(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/HelpDesk") == null) Initialize();
            this.el = (XmlElement)doc.SelectSingleNode("/hapConfig/HelpDesk");
        }
        public void Initialize()
        {
            XmlElement e = doc.CreateElement("HelpDesk");
            e.SetAttribute("admins", "Domain Admins");
            e.SetAttribute("firstlineemails", "");
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }
        public string Admins
        {
            get { return el.GetAttribute("admins"); }
            set { el.SetAttribute("admins", value); }
        }
        public string FirstLineEmails
        {
            get { return el.GetAttribute("firstlineemails"); }
            set { el.SetAttribute("firstlineemails", value); }
        }
    }
}
