using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.Web.Configuration
{
    public class Tabs : Dictionary<TabType, Tab>
    {
        private XmlDocument doc;
        private XmlNode node;
        public Tabs(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/Homepage/Tabs");
            foreach (XmlNode n in node.ChildNodes) base.Add((TabType)Enum.Parse(typeof(TabType), n.Attributes["type"].Value), new Tab(n));
        }
        public void Add(string Name, string ShowTo, TabType Type)
        {
            XmlElement e = doc.CreateElement("tab");
            e.SetAttribute("name", Name);
            e.SetAttribute("showto", ShowTo);
            e.SetAttribute("type", Type.ToString());
            doc.SelectSingleNode("/hapConfig/Homepage/Tabs").AppendChild(e);
            base.Add(Type, new Tab(e));
        }
        public void Add(string Name, string ShowTo, TabType Type, string UpdateTo, bool ShowSpace)
        {
            XmlElement e = doc.CreateElement("tab");
            e.SetAttribute("name", Name);
            e.SetAttribute("showto", ShowTo);
            e.SetAttribute("type", Type.ToString());
            e.SetAttribute("allowupdateto", UpdateTo);
            e.SetAttribute("showspace", ShowSpace.ToString());
            doc.SelectSingleNode("/hapConfig/Homepage/Tabs").AppendChild(e);
            base.Add(Type, new Tab(e));
        }
        public void UpdateTab(TabType Type, Tab tab)
        {
            base.Remove(Type);
            XmlNode e = doc.SelectSingleNode("/hapConfig/Homepage/Tabs/tab[@type='" + Type.ToString() + "']");
            e.Attributes["name"].Value = tab.Name;
            e.Attributes["showto"].Value = tab.ShowTo;
            if (tab.Type == TabType.Me)
            {
                e.Attributes["allowupdateto"].Value = tab.AllowUpdateTo;
                e.Attributes["showspace"].Value = tab.ShowSpace.HasValue ? tab.ShowSpace.Value.ToString() : true.ToString();
            }
            base.Add(Type, new Tab(e));
        }
        public void ReOrder(string[] Names)
        {
            XmlElement tempnode = doc.CreateElement("Tabs");
            foreach (string name in Names)
            {
                XmlElement e = doc.CreateElement("tab");
                TabType tt = (TabType)Enum.Parse(typeof(TabType), name);
                e.SetAttribute("name", this[tt].Name);
                e.SetAttribute("showto", this[tt].ShowTo);
                e.SetAttribute("type", this[tt].Type.ToString());
                if (this[tt].Type == TabType.Me)
                {
                    e.SetAttribute("allowupdateto", this[tt].AllowUpdateTo);
                    e.SetAttribute("showspace", this[tt].ShowSpace.ToString());
                }
                tempnode.AppendChild(e);
            }
            doc.SelectSingleNode("/hapConfig/Homepage").ReplaceChild(tempnode, node);
            node = tempnode;
            this.Clear();
            foreach (XmlNode n in node.ChildNodes) this.Add((TabType)Enum.Parse(typeof(TabType), n.Attributes["type"].Value), new Tab(n));
        }

        public Dictionary<TabType, Tab> FilteredTabs
        {
            get
            {
                Dictionary<TabType, Tab> tabs = new Dictionary<TabType, Tab>();
                foreach (Tab tab in this.Values)
                    if (tab.ShowTo == "All") tabs.Add(tab.Type, tab);
                    else if (tab.ShowTo != "None")
                    {
                        bool vis = false;
                        foreach (string s in tab.ShowTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                            if (!vis) vis = HttpContext.Current.User.IsInRole(s);
                        if (vis) tabs.Add(tab.Type, tab);
                    }
                return tabs;
            }
        }
    }
}
