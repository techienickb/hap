using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.BookingSystem
{
    public class Templates : Dictionary<string, Template>
    {
        public Templates() : base()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/BookingTemplates.xml"));
            foreach (XmlNode node in doc.SelectNodes("/templates/template"))
                Add(node.Attributes["id"].Value, new Template(node));
        }

        public void Save()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/BookingTemplates.xml"));
            doc.SelectSingleNode("/templates").RemoveAll();
            foreach (Template t in this.Values)
            {
                XmlElement t1 = doc.CreateElement("template");
                t1.SetAttribute("id", t.ID);
                t1.SetAttribute("subject", t.Subject);
                t1.InnerXml = t.Content.Replace("\n", "");
                doc.SelectSingleNode("/templates").AppendChild(t1);
            }
            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/BookingTemplates.xml"));
        }
    }
}
