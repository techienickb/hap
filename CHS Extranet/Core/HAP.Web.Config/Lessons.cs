using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;

namespace HAP.Web.Configuration
{
    public class Lessons : List<Lesson>
    {
        private XmlDocument doc;
        private XmlNode node;
        public Lessons(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/bookingsystem/lessons");
            foreach (XmlNode n in node.ChildNodes) base.Add(new Lesson(n));
        }
        public void Add(string Name, LessonType Type, DateTime StartTime, DateTime EndTime)
        {
            XmlElement e = doc.CreateElement("lesson");
            e.SetAttribute("name", Name);
            e.SetAttribute("type", Type.ToString());
            e.SetAttribute("starttime", StartTime.ToShortTimeString());
            e.SetAttribute("endtime", EndTime.ToShortTimeString());
            doc.SelectSingleNode("/hapConfig/bookingsystem/lessons").AppendChild(e);
            base.Add(new Lesson(e));
        }
        public void Remove(string name)
        {
            base.Remove(Get(name));
            doc.SelectSingleNode("/hapConfig/bookingsystem/lessons").RemoveChild(node.SelectSingleNode("lesson[@name='" + name + "']"));
        }

        public Lesson Get(string name)
        {
            return this.Single(l => l.Name == name);
        }
        public void Update(string name, Lesson l)
        {
            int x = IndexOf(Get(name));
            base.Remove(Get(name));
            XmlNode e = doc.SelectSingleNode("/hapConfig/bookingsystem/lessons/lesson[@name='" + name + "']");
            e.Attributes["name"].Value = l.Name;
            e.Attributes["type"].Value = l.Type.ToString();
            e.Attributes["starttime"].Value = l.StartTime.ToShortTimeString();
            e.Attributes["endtime"].Value = l.EndTime.ToShortTimeString();
            base.Insert(x, new Lesson(e));
        }

        public void ReOrder(string[] Names)
        {
            base.Clear();
            foreach (string name in Names)
            {
                string n = name.Remove(0, 6).Replace('_', ' ');
                XmlNode tempnode = doc.SelectSingleNode("/hapConfig/bookingsystem/lesson/lesson[@name='" + n + "']");
                doc.SelectSingleNode("/hapConfig/bookingsystem/lessons").RemoveChild(doc.SelectSingleNode("/hapConfig/bookingsystem/lessons/lesson[@name='" + n + "']"));
                doc.SelectSingleNode("/hapConfig/bookingsystem/lessons").AppendChild(tempnode);
                base.Add(new Lesson(tempnode));
            }
        }
    }
}
