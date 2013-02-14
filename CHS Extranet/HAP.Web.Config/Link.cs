using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class Link
    {
        private XmlNode node;
        public Link(XmlNode node)
        {
            this.node = node;
            Name = node.Attributes["name"].Value;
            ShowTo = node.Attributes["showto"].Value;
            Description = node.Attributes["description"].Value;
            Url = node.Attributes["url"].Value;
            Target = node.Attributes["target"].Value;
            Icon = node.Attributes["icon"].Value;
            Type = node.Attributes["type"] != null ? node.Attributes["type"].Value : "";
            Width = node.Attributes["width"].Value;
            Height = node.Attributes["height"].Value;
        }

        public string Name { get; set; }
        public string ShowTo { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Target { get; set; }
        public string Type { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }
}
