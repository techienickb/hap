using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class adDomains : Dictionary<string, adDomain>
    {
        private XmlDocument doc;
        private XmlNode node;
        public adDomains(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/AD/Domains");
            foreach (XmlNode n in node.ChildNodes) this.Add(n.Attributes["name"].Value, new adDomain(n));
        }
        public void Add(string Name, string Username, string Password)
        {
            XmlElement e = doc.CreateElement("Domain");
            e.SetAttribute("name", Name);
            e.SetAttribute("username", Username);
            e.SetAttribute("password", Password);
            doc.SelectSingleNode("/hapConfig/AD/Domains").AppendChild(e);
            base.Add(Name, new adDomain(e));
        }

        public void Update(string OriginalName, string Name, string Username, string Password)
        {
            base.Remove(OriginalName);
            XmlElement e = (XmlElement)doc.SelectSingleNode("/hapConfig/AD/Domains/Domain[@name='" + OriginalName + "']");
            e.SetAttribute("name", Name);
            e.SetAttribute("username", Username);
            e.SetAttribute("password", Password);
            base.Add(Name, new adDomain(e));
        }

        public new void Remove(string Name)
        {
            base.Remove(Name);
            doc.SelectSingleNode("/hapConfig/AD/Domains").RemoveChild(doc.SelectSingleNode("/hapConfig/AD/Domains/Domain[@name='" + Name + "']"));
        }
    }
}
