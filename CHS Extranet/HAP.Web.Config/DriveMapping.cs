using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.Web.Configuration
{
    public class DriveMapping
    {
        private XmlNode node;
        public DriveMapping(XmlNode node)
        {
            this.node = node;
            Name = node.Attributes["name"].Value;
            Drive = node.Attributes["drive"].Value.ToCharArray()[0];
            UNC = HttpContext.Current.Server.HtmlDecode(node.InnerText);
            EnableReadTo = node.Attributes["enablereadto"].Value;
            EnableWriteTo = node.Attributes["enablewriteto"].Value;
            EnableMove = bool.Parse(node.Attributes["enablemove"].Value);
            UsageMode = (MappingUsageMode)Enum.Parse(typeof(MappingUsageMode), node.Attributes["usagemode"].Value);
        }

        public string Name { get; set; }
        public char Drive { get; set; }
        public string UNC { get; set; }
        public string EnableReadTo { get; set; }
        public string EnableWriteTo { get; set; }
        public bool EnableMove { get; set; }
        public MappingUsageMode UsageMode { get; set; }
    }

    public enum MappingUsageMode { None, Quota, DriveSpace }
}
