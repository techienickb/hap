using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class Resource
    {
        private XmlNode node;
        public Resource(XmlNode node)
        {
            this.node = node;
            Name = node.Attributes["name"].Value;
            Type = (ResourceType)Enum.Parse(typeof(ResourceType), node.Attributes["type"].Value);
            Enabled = bool.Parse(node.Attributes["enabled"].Value);
            EnableCharging = bool.Parse(node.Attributes["enablecharging"].Value);
            Admins = node.Attributes["admins"].Value != "" ? node.Attributes["admins"].Value : "Inherit";
            EmailAdmins = bool.Parse(node.Attributes["emailadmins"].Value);
            ShowTo = node.Attributes["showto"].Value;
            HideFrom = node.Attributes["hidefrom"].Value;
            Years = node.Attributes["years"].Value;
            Quantities = node.Attributes["quantities"].Value;
            ReadOnlyTo = node.Attributes["readonlyto"].Value;
            ReadWriteTo = node.Attributes["readwriteto"].Value;
            MultiLessonTo = node.Attributes["multilessonto"] == null ? "" : node.Attributes["multilessonto"].Value;
        }

        public string Name { get; set; }
        public ResourceType Type { get; set; }
        public bool EmailAdmins { get; set; }
        public string Admins { get; set; }
        public bool EnableCharging { get; set; }
        public bool Enabled { get; set; }
        public string ShowTo { get; set; }
        public string HideFrom { get; set; }
        public string Years { get; set; }
        public string Quantities { get; set; }
        public string ReadOnlyTo { get; set; }
        public string ReadWriteTo { get; set; }
        public string MultiLessonTo { get; set; }
    }

    public enum ResourceType { Room, Laptops, Equipment, Other }
}
