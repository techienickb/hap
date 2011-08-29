using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;

namespace HAP.Web.Configuration
{
    public class Subjects : List<string>
    {
        private XmlDocument doc;
        private XmlNode node;
        public Subjects(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/bookingsystem/subjects");
            foreach (XmlNode n in node.ChildNodes) base.Add(n.Attributes["name"].Value);
        }
        public new void Add(string Subject)
        {
            XmlElement e = doc.CreateElement("subject");
            e.SetAttribute("name", Subject);
            doc.SelectSingleNode("/hapConfig/bookingsystem/subjects").AppendChild(e);
            base.Add(Subject);
        }
        public void Delete(string name)
        {
            base.Remove(name);
            doc.SelectSingleNode("/hapConfig/bookingsystem/subjects").RemoveChild(doc.SelectSingleNode("/hapConfig/bookingsystem/subjects/subject[@name='" + name + "']"));
        }
        public void Update(string name, string subject)
        {
            int index = IndexOf(name);
            base.Remove(name);
            doc.SelectSingleNode("/hapConfig/bookingsystem/subjects/subject[@name='" + name + "']").Attributes["name"].Value = subject;
            base.Insert(index, subject);
        }
    }
}
