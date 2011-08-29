using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class ous : Dictionary<string, ou>
    {
        private XmlDocument doc;
        private XmlNode node;
        public ous(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/AD/OUs");
            foreach (XmlNode n in node.ChildNodes) this.Add(n.Attributes["name"].Value, new ou(n));
        }
        public void Add(string Name, string Path)
        {
            XmlElement e = doc.CreateElement("OU");
            e.SetAttribute("name", Name);
            e.SetAttribute("path", Path);
            doc.SelectSingleNode("/hapConfig/AD/OUs").AppendChild(e);
            base.Add(Name, new ou(e));
        }
        public void Add(string Name, string Path, bool Ignore)
        {
            XmlElement e = doc.CreateElement("OU");
            e.SetAttribute("name", Name);
            e.SetAttribute("path", Path);
            e.SetAttribute("ignore", Ignore.ToString());
            doc.SelectSingleNode("/hapConfig/AD/OUs").AppendChild(e);
            base.Add(Name, new ou(e));
        }
        public new void Remove(string Name)
        {
            base.Remove(Name);
            doc.SelectSingleNode("/hapConfig/AD/OUs").RemoveChild(doc.SelectSingleNode("/hapConfig/AD/OUs/OU[@name='" + Name + "']"));
        }
    }
}
