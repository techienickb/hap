using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.BookingSystem
{
    public class Template
    {
        public string ID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public Template() {}
        public Template(XmlNode node)
        {
            this.ID = node.Attributes["id"].Value;
            this.Subject = node.Attributes["subject"].Value;
            this.Content = node.InnerXml;
        }
    }
}
