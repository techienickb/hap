using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.Web.Configuration
{
    public class LinkGroup : List<Link>
    {
        private XmlDocument doc;
        private XmlNode node;
        public string Name { get; set; }
        public string ShowTo { get; set; }
        public string SubTitle { get; set; }
        public bool HideHomePage { get; set; }
        public bool HideTopMenu { get; set; }
        public bool HideHomePageLink { get; set; }
        public LinkGroup(ref XmlDocument doc, string Name) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + Name + "']");
            this.Name = Name;
            this.SubTitle = node.Attributes["subtitle"].Value;
            this.ShowTo = node.Attributes["showto"].Value;
            this.HideHomePage = bool.Parse(node.Attributes["hidehomepage"].Value);
            this.HideTopMenu = bool.Parse(node.Attributes["hidetopmenu"].Value);
            this.HideHomePageLink = bool.Parse(node.Attributes["hidehomepagelink"].Value);

            foreach (XmlNode n in node.ChildNodes) base.Add(new Link(n));
        }
        public Link[] FilteredLinks
        {
            get
            {
                List<Link> Links = new List<Link>();
                foreach (Link l in this)
                    if (l.ShowTo == "All"|| l.ShowTo == "Inherit") Links.Add(l);
                    else if (l.ShowTo != "None")
                    {
                        bool vis = false;
                        foreach (string s in l.ShowTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                        if (vis) Links.Add(l);
                    }
                return Links.ToArray();
            }
        }
        public void Add(string Name, string ShowTo, string Description, string Url, string Icon, string Target, string width, string height)
        {
            XmlElement e = doc.CreateElement("Link");
            e.SetAttribute("name", Name);
            e.SetAttribute("showto", ShowTo);
            e.SetAttribute("description", Description);
            e.SetAttribute("url", Url);
            e.SetAttribute("icon", Icon);
            e.SetAttribute("target", Target);
            e.SetAttribute("width", width);
            e.SetAttribute("height", height);
            doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']").AppendChild(e);
            base.Add(new Link(e));
        }
        public void Add(string Name, string ShowTo, string Description, string Url, string Icon, string Target, string width, string height, string Type)
        {
            XmlElement e = doc.CreateElement("Link");
            e.SetAttribute("name", Name);
            e.SetAttribute("showto", ShowTo);
            e.SetAttribute("description", Description);
            e.SetAttribute("url", Url);
            e.SetAttribute("icon", Icon);
            e.SetAttribute("target", Target);
            e.SetAttribute("type", Type);
            e.SetAttribute("width", width);
            e.SetAttribute("height", height);
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
            e.Attributes["width"].Value = link.Width;
            e.Attributes["height"].Value = link.Height;
            base.Insert(x, new Link(e));
        }

        public void ReOrder(string[] Names)
        {
            foreach (string name in Names)
            {
                string n = name.Remove(0, 4).Replace('_', ' ');
                XmlNode tempnode = doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']/Link[@name='" + n + "']").Clone();
                doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']").RemoveChild(doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']/Link[@name='" + n + "']"));
                doc.SelectSingleNode("/hapConfig/Homepage/Links/Group[@name='" + this.Name + "']").AppendChild(tempnode);
            }
        }
    }
}
