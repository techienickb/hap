using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class SMTP
    {        
        private XmlDocument doc;
        private XmlElement el;
        public SMTP(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/SMTP") == null) Initialize();
            this.el = (XmlElement)doc.SelectSingleNode("/hapConfig/SMTP");
        }
        public void Initialize()
        {
            XmlElement e = doc.CreateElement("SMTP");
            e.SetAttribute("server", "");
            e.SetAttribute("port", "25");
            e.SetAttribute("enabled", "False");
            e.SetAttribute("ssl", "False");
            e.SetAttribute("from", "admin");
            e.SetAttribute("fromaddress", "admin@localhost.com");
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }
        public string Server
        {
            get { return el.GetAttribute("server"); }
            set { el.SetAttribute("server", value); }
        }
        public int Port
        {
            get { return int.Parse(el.GetAttribute("port")); }
            set { el.SetAttribute("port", value.ToString()); }
        }
        public bool Enabled
        {
            get { return bool.Parse(el.GetAttribute("enabled")); }
            set { el.SetAttribute("enabled", value.ToString()); }
        }
        public bool SSL
        {
            get { return bool.Parse(el.GetAttribute("ssl")); }
            set { el.SetAttribute("ssl", value.ToString()); }
        }
        public string User
        {
            get { return el.GetAttribute("user"); }
            set { el.SetAttribute("user", value); }
        }
        public string Password
        {
            get { return el.GetAttribute("password"); }
            set { el.SetAttribute("password", value); }
        }
        public string FromUser
        {
            get { return el.GetAttribute("from"); }
            set { el.SetAttribute("from", value); }
        }
        public string FromEmail
        {
            get { return el.GetAttribute("fromaddress"); }
            set { el.SetAttribute("fromaddress", value); }
        }
    }
}
