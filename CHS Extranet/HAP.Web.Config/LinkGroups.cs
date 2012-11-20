using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class LinkGroups : Dictionary<string, LinkGroup>
    {
        private XmlDocument doc;
        private XmlNode node;
        public LinkGroups(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/Homepage/Links");
            foreach (XmlNode n in node.SelectNodes("Group")) base.Add(n.Attributes["name"].Value, new LinkGroup(ref doc, n.Attributes["name"].Value));
        }
        public void Add(string Name, string ShowTo, string SubTitle, bool HideHomePage, bool HideTopMenu)
        {
            XmlElement e = doc.CreateElement("Group");
            e.SetAttribute("name", Name);
            e.SetAttribute("showto", ShowTo);
            e.SetAttribute("subtitle", SubTitle);
            e.SetAttribute("hidehomepage", HideHomePage.ToString());
            e.SetAttribute("hidetopmenu", HideTopMenu.ToString());
            doc.SelectSingleNode("/hapConfig/Homepage/Links").AppendChild(e);
            base.Add(Name, new LinkGroup(ref doc, Name));
        }
        public new void Remove(string Name)
        {
            base.Remove(Name);
            doc.SelectSingleNode("/hapConfig/Homepage/Links").RemoveChild(doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + Name + "']"));
        }
        public void UpdateGroup(string Name, LinkGroup group)
        {
            base.Remove(Name);
            XmlNode e = doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + Name + "']");
            e.Attributes["name"].Value = group.Name;
            e.Attributes["showto"].Value = group.ShowTo;
            e.Attributes["subtitle"].Value = group.SubTitle;
            e.Attributes["hidehomepage"].Value = group.HideHomePage.ToString();
            e.Attributes["hidetopmenu"].Value = group.HideTopMenu.ToString();

            //doc.SelectSingleNode("/hapConfig/Homepage/Links").ReplaceChild(e, doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + Name + "']"));
            base.Add(group.Name, new LinkGroup(ref doc, group.Name));
        }

        public void ReOrder(string[] Names)
        {
            foreach (string name in Names)
            {
                string n = name.Remove(0, 9).Replace("_", " ");
                XmlNode tempnode = doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + n + "']");
                doc.SelectSingleNode("/hapConfig/Homepage/Links").RemoveChild(doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + n + "']"));
                doc.SelectSingleNode("/hapConfig/Homepage/Links").AppendChild(tempnode);
            }
        }
    }
}
