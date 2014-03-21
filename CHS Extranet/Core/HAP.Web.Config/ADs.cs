using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class ADs: Dictionary<string, Domain>
    {
        private XmlDocument doc;
        private XmlNode node;
        public ADs(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/AD/ADs");
            foreach (XmlNode n in node.ChildNodes) this.Add(n.Attributes["domain"].Value, new Domain(n));
        }
        public void Add(string Domain, string UPN, string Password, string Username, string StudentsGroup)
        {
            XmlElement e = doc.CreateElement("AD");
            e.SetAttribute("domain", Domain);
            e.SetAttribute("upn", UPN);
            e.SetAttribute("username", Username);
            e.SetAttribute("studentsgroup", StudentsGroup);
            doc.SelectSingleNode("/hapConfig/AD/ADs").AppendChild(e);
            Domain d = new Domain(e);
            d.Password = Password;
            base.Add(Domain, d);
        }

        public new void Remove(string Domain)
        {
            base.Remove(Domain);
            doc.SelectSingleNode("/hapConfig/AD/ADs").RemoveChild(doc.SelectSingleNode("/hapConfig/AD/ADs/AD[@domain='" + Domain + "']"));
        }
    }
}
