using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class Rooms : List<string>
    {
        private XmlNode node;
        public Rooms(XmlNode root) : base()
        {
            this.node = root;
            foreach (XmlNode n in node.ChildNodes) base.Add(n.InnerText);
        }
        public new void Add(string Name)
        {
            XmlElement e = node.OwnerDocument.CreateElement("room");
            e.InnerText = Name;
            node.AppendChild(e);
            base.Add(Name);
            Sort();
        }
        public bool Inherit { get { return bool.Parse(node.Attributes["inherit"].Value); } set { node.Attributes["inherit"].Value = value.ToString(); } }
        public void Delete(string name)
        {
            base.Remove(name);
            XmlNode n = null;
            foreach (XmlNode n1 in node.ChildNodes)
                if (n1.InnerText == name) { n = n1; break; }
            node.RemoveChild(n);
        }
        public void Update(string name, string newname)
        {
            base.Remove(name);
            XmlNode n = null;
            foreach (XmlNode n1 in node.ChildNodes)
                if (n1.InnerText == name) { n = n1; break; }
            n.InnerText = newname;
            base.Add(newname);
            Sort();
        }
    }

}
