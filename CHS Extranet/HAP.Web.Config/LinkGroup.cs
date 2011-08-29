using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class LinkGroup : List<Link>
    {
        private XmlDocument doc;
        private XmlNode node;
        public string Name { get; set; }
        public string ShowTo { get; set; }
        public LinkGroup(ref XmlDocument doc, string Name) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + Name + "']");
            this.Name = Name;
            this.ShowTo = node.Attributes["showto"].Value;
            foreach (XmlNode n in node.ChildNodes) base.Add(new Link(n));
        }
        public void Add(string Name, string ShowTo, string Description, string Url, string Icon, string Target)
        {
            XmlElement e = doc.CreateElement("Link");
            e.SetAttribute("name", Name);
            e.SetAttribute("showto", ShowTo);
            e.SetAttribute("description", Description);
            e.SetAttribute("url", Url);
            e.SetAttribute("icon", Icon);
            e.SetAttribute("target", Target);
            doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']").AppendChild(e);
            base.Add(new Link(e));
        }
        private Link get(string name)
        {
            return this.Single(l => l.Name == name);
        }
        public new void Remove(string name)
        {
            base.Remove(get(name));
            doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + Name + "']").RemoveChild(node.SelectSingleNode("Link[@name='" + name + "']"));
        }
        public void UpdateLink(string name, Link link)
        {
            int x = base.IndexOf(get(name));
            base.RemoveAt(x);
            XmlNode e = doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']/Link[@name='" + name + "']");
            e.Attributes["name"].Value = link.Name;
            e.Attributes["showto"].Value = link.ShowTo;
            e.Attributes["description"].Value = link.Description;
            e.Attributes["url"].Value = link.Url;
            e.Attributes["icon"].Value = link.Icon;
            e.Attributes["target"].Value = link.Target;
            base.Insert(x, new Link(e));
        }

        public void ReOrder(string[] Names)
        {
            foreach (string name in Names)
            {
                string n = name.Remove(0, 4).Replace('_', ' ');
                XmlNode tempnode = doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']/Link[@name='" + n + "']");
                doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']").RemoveChild(doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']/Link[@name='" + n + "']"));
                doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']").AppendChild(tempnode);
            }
        }
    }
}
