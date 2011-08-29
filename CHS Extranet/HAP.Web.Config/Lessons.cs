using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;

namespace HAP.Web.Configuration
{
    public class Lessons : Dictionary<string, Lesson>
    {
        private XmlDocument doc;
        private XmlNode node;
        public Lessons(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/bookingsystem/lessons");
            foreach (XmlNode n in node.ChildNodes) base.Add(n.Attributes["name"].Value, new Lesson(n));
        }
        public void Add(string Name, LessonType Type, DateTime StartTime, DateTime EndTime)
        {
            XmlElement e = doc.CreateElement("lesson");
            e.SetAttribute("name", Name);
            e.SetAttribute("type", Type.ToString());
            e.SetAttribute("starttime", StartTime.ToShortTimeString());
            e.SetAttribute("endtime", EndTime.ToShortTimeString());
            doc.SelectSingleNode("/hapConfig/bookingsystem/lessons").AppendChild(e);
            base.Add(Name, new Lesson(e));
        }
        public new void Remove(string name)
        {
            base.Remove(name);
            doc.SelectSingleNode("/hapConfig/bookingsystem/lessons").RemoveChild(node.SelectSingleNode("lesson[@name='" + name + "']"));
        }
        public void Update(string name, Lesson l)
        {
            base.Remove(name);
            XmlNode e = doc.SelectSingleNode("/hapConfig/bookingsystem/lessons/lesson[@name='" + name + "']");
            e.Attributes["name"].Value = l.Name;
            e.Attributes["type"].Value = l.Type.ToString();
            e.Attributes["starttime"].Value = l.StartTime.ToShortTimeString();
            e.Attributes["endtime"].Value = l.EndTime.ToShortTimeString();
            base.Add(l.Name, new Lesson(e));
        }

        public void ReOrder(string[] Names)
        {
            foreach (string name in Names)
            {
                string n = name.Remove(0, 6).Replace('_', ' ');
                XmlNode tempnode = doc.SelectSingleNode("/hapConfig/bookingsystem/lesson/lesson[@name='" + n + "']");
                doc.SelectSingleNode("/hapConfig/bookingsystem/lessons").RemoveChild(doc.SelectSingleNode("/hapConfig/bookingsystem/lessons/lesson[@name='" + n + "']"));
                doc.SelectSingleNode("/hapConfig/bookingsystem/lessons").AppendChild(tempnode);
            }
        }
    }
}
