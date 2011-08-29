using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;

namespace HAP.Web.Configuration
{
    public class Resources:Dictionary<string, Resource>
    {
        private XmlDocument doc;
        private XmlNode node;
        public Resources(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/bookingsystem/resources");
            foreach (XmlNode n in node.ChildNodes) base.Add(n.Attributes["name"].Value, new Resource(n));
        }
        public void Add(string Name, ResourceType Type, string Admins, bool Enabled, bool EmailAdmins, bool EnableCharging)
        {
            XmlElement e = doc.CreateElement("resource");
            e.SetAttribute("name", Name);
            e.SetAttribute("type", Type.ToString());
            e.SetAttribute("admins", Admins);
            e.SetAttribute("enabled", Enabled.ToString());
            e.SetAttribute("emailadmins", EmailAdmins.ToString());
            e.SetAttribute("enablecharging", EnableCharging.ToString());
            doc.SelectSingleNode("/hapConfig/bookingsystem/resources").AppendChild(e);
            base.Add(Name, new Resource(e));
        }
        public void Delete(string name)
        {
            base.Remove(name);
            doc.SelectSingleNode("/hapConfig/bookingsystem/resources").RemoveChild(node.SelectSingleNode("resource[@name='" + name + "']"));
        }
        public void Update(string name, Resource r)
        {
            base.Remove(name);
            XmlNode e = doc.SelectSingleNode("/hapConfig/bookingsystem/resources/resource[@name='" + name + "']");
            e.Attributes["name"].Value = r.Name;
            e.Attributes["type"].Value = r.Type.ToString();
            e.Attributes["enabled"].Value = r.Enabled.ToString();
            e.Attributes["admins"].Value = r.Admins;
            e.Attributes["emailadmins"].Value = r.EmailAdmins.ToString();
            e.Attributes["enablecharging"].Value = r.EnableCharging.ToString();
            base.Add(r.Name, new Resource(e));
        }

        public void ReOrder(string[] Names)
        {
            foreach (string name in Names)
            {
                string n = name.Remove(0, 3).Replace('_', ' ');
                XmlNode tempnode = doc.SelectSingleNode("/hapConfig/bookingsystem/resources/resource[@name='" + n + "']");
                doc.SelectSingleNode("/hapConfig/bookingsystem/resources").RemoveChild(doc.SelectSingleNode("/hapConfig/bookingsystem/resources/resource[@name='" + n + "']"));
                doc.SelectSingleNode("/hapConfig/bookingsystem/resources").AppendChild(tempnode);
            }
        }
    }
}
