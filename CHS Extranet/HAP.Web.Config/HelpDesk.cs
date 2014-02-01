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
            e.SetAttribute("provider", "xml");
            e.SetAttribute("priorities", "Low, Normal, High");
            e.SetAttribute("useropenstates", "New, Updated");
            e.SetAttribute("userclosedstates", "Fixed, No Action Needed, Self Fixed");
            e.SetAttribute("openstates", "New, Investigating, User Attention Needed, With 1st Line Support, With 2nd Line Support, With 3rd Line Support, Item Ordered, Waiting");
            e.SetAttribute("closedstates", "Resolved, Fixed, Timed Out, No Action Needed");
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }
        public string Priorities
        {
            get { return el.GetAttribute("priorities"); }
            set { el.SetAttribute("priorities", value); }
        }
        public string UserClosedStates
        {
            get { return el.GetAttribute("userclosedstates"); }
            set { el.SetAttribute("userclosedstates", value); }
        }
        public string UserOpenStates
        {
            get { return el.GetAttribute("useropenstates"); }
            set { el.SetAttribute("useropenstates", value); }
        }
        public string ClosedStates
        {
            get { return el.GetAttribute("closedstates"); }
            set { el.SetAttribute("closedstates", value); }
        }
        public string OpenStates
        {
            get { return el.GetAttribute("openstates"); }
            set { el.SetAttribute("openstates", value); }
        }
        public string Provider
        {
            get { return el.GetAttribute("provider"); }
            set { el.SetAttribute("provider", value); }
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
