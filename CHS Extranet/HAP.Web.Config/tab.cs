using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    [Serializable]
    public class Tab
    {
        private XmlNode node;
        public Tab(XmlNode node)
        {
            this.node = node;
            Name = node.Attributes["name"].Value;
            ShowTo = node.Attributes["showto"].Value;
            Type = (TabType)Enum.Parse(typeof(TabType), node.Attributes["type"].Value);
            if (Type == TabType.Me)
            {
                this.ShowSpace = bool.Parse(node.Attributes["showspace"].Value);
                this.AllowUpdateTo = node.Attributes["allowupdateto"].Value;
            }
            else
            {
                this.ShowSpace = null;
                this.AllowUpdateTo = "";
            }
        }
        public Tab() { }

        public string Name { get; set; }
        public string ShowTo { get; set; }
        public TabType Type { get; set; }
        public string AllowUpdateTo { get; set; }
        public Nullable<bool> ShowSpace { get; set; }
    }
    public enum TabType { Me, Password, Bookings, Tickets }
}
