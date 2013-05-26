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
            if (node.SelectSingleNode("rooms") != null) Rooms = new Rooms(node.SelectSingleNode("rooms"));
            else Rooms = null;
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
            MaxMultiLesson = node.Attributes["maxmultilesson"] == null ? 0 : int.Parse(node.Attributes["maxmultilesson"].Value);
            Disclaimer = node.Attributes["disclaimer"] == null ? "" : node.Attributes["disclaimer"].Value;
            CanShare = node.Attributes["canshare"] == null ? false : bool.Parse(node.Attributes["canshare"].Value);
            ChargingPeriods = node.Attributes["chargingperiods"] == null ? 1 : int.Parse(node.Attributes["chargingperiods"].Value);
        }

        public Rooms Rooms { get; set; }
        public string Name { get; set; }
        public ResourceType Type { get; set; }
        public bool EmailAdmins { get; set; }
        public string Admins { get; set; }
        public bool EnableCharging { get; set; }
        public int ChargingPeriods { get; set; }
        public bool Enabled { get; set; }
        public string ShowTo { get; set; }
        public string HideFrom { get; set; }
        public string Years { get; set; }
        public string Quantities { get; set; }
        public string ReadOnlyTo { get; set; }
        public string ReadWriteTo { get; set; }
        public string MultiLessonTo { get; set; }
        public int MaxMultiLesson { get; set; }
        public bool CanShare { get; set; }
        public string Disclaimer { get; set; }
        public void InitRooms()
        {
            XmlElement e = node.OwnerDocument.CreateElement("rooms");
            e.SetAttribute("inherit", "False");
            Rooms = new Rooms(node.AppendChild(e));
        }

        public void RemoveRooms()
        {
            node.RemoveChild(node.SelectSingleNode("rooms"));
            Rooms = null;
        }
    }

    public enum ResourceType { Room, Laptops, Equipment, Loan, Other }
}
