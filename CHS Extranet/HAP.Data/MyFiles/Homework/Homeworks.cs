using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.Data.MyFiles.Homework
{
    public class Homeworks
    {
        private XmlDocument _doc;
        public Homework[] Homework
        {
            get
            {
                List<Homework> res = new List<Homework>();
                foreach (XmlNode n in _doc.SelectNodes("/homeworks/teacher"))
                    foreach (XmlNode node in n.SelectNodes("homework"))
                        res.Add(new Homework(node, n.Attributes["user"].Value));
                return res.ToArray();
            }
        }

        public void Add(Homework homework)
        {
            XmlNode teacher = _doc.SelectSingleNode("/homeworks/teacher[@user='" + homework.Teacher + "']");
            if (teacher == null)
            {
                XmlElement e = _doc.CreateElement("teacher");
                e.SetAttribute("user", homework.Teacher);
                teacher = _doc.AppendChild(e);
            }
            XmlElement h = _doc.CreateElement("homework");
            h.SetAttribute("name", homework.Name);
            h.SetAttribute("start", homework.Start);
            h.SetAttribute("end", homework.End);
            XmlElement d = _doc.CreateElement("description");
            d.InnerText = "<![CDATA[" + homework.Description + "]]>";
            h.AppendChild(d);
            foreach (UserNode u in homework.UserNodes)
            {
                XmlElement un = _doc.CreateElement(u.Method == UserNodeMethod.Add ? "add" : "remove");
                un.SetAttribute("type", u.Type.ToString());
                if (u.Method == UserNodeMethod.Add) un.SetAttribute("mode", u.Mode.ToString());
                un.InnerText = u.Value;
                h.AppendChild(un);
            }
            teacher.AppendChild(h);
            _doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/Homework.xml"));
        }

        public void Update(Homework orighomework, Homework homework)
        {
            XmlElement h = (XmlElement)_doc.SelectSingleNode("/homeworks/teacher[@user='" + orighomework.Teacher + "']/homework[@name='" + orighomework.Name + "' AND @start='" + orighomework.Start + "' AND @end='" + orighomework.End + "']");
            h.RemoveAll();
            h.SetAttribute("name", homework.Name);
            h.SetAttribute("start", homework.Start);
            h.SetAttribute("end", homework.End);
            XmlElement d = _doc.CreateElement("description");
            d.InnerText = "<![CDATA[" + homework.Description + "]]>";
            h.AppendChild(d);
            foreach (UserNode u in homework.UserNodes)
            {
                XmlElement un = _doc.CreateElement(u.Method == UserNodeMethod.Add ? "add" : "remove");
                un.SetAttribute("type", u.Type.ToString());
                if (u.Method == UserNodeMethod.Add) un.SetAttribute("mode", u.Mode.ToString());
                un.InnerText = u.Value;
                h.AppendChild(un);
            }
            _doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/Homework.xml"));
        }

        public void Remove(Homework homework)
        {
            XmlNode teacher = _doc.SelectSingleNode("/homeworks/teacher[@user='" + homework.Teacher + "']");
            teacher.RemoveChild(teacher.SelectSingleNode("homework[@name='" + homework.Name + "' AND @start='" + homework.Start + "' AND @end='" + homework.End + "']"));
            _doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/Homework.xml"));
        }

        public Homeworks()
        {
            _doc = new XmlDocument();
            _doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Homework.xml"));
            
        }
    }
}
