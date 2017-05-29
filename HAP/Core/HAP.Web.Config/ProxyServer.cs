using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class ProxyServer
    {
        private XmlElement el;
        private XmlDocument doc;
        public ProxyServer(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/ProxyServer") == null) Initialize();
            this.el = (XmlElement)doc.SelectSingleNode("/hapConfig/ProxyServer");
        }

        public void Initialize()
        {
            XmlElement e = doc.CreateElement("ProxyServer");
            e.SetAttribute("address", "");
            e.SetAttribute("port", "0");
            e.SetAttribute("enabled", "False");
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
        }
        public bool Enabled
        {
            get { return bool.Parse(el.GetAttribute("enabled")); }
            set { el.SetAttribute("enabled", value.ToString()); }
        }
        public int Port
        {
            get { return int.Parse(el.GetAttribute("port")); }
            set { el.SetAttribute("port", value.ToString()); }
        }
        public string Address
        {
            get { return el.GetAttribute("address"); }
            set { el.SetAttribute("address", value); }
        }
    }
}
