using HAP.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Timetable
{
    public class Config :IConfig
    {
        public string Name
        {
            get { return "Timetable"; }
        }

        public void Save(System.Xml.XmlElement xmlElement, ref System.Xml.XmlDocument doc)
        {
            throw new NotImplementedException();
        }

        public void Load(System.Xml.XmlElement xmlElement)
        {
            List<OptionalCalendar> cals = new List<OptionalCalendar>();
            foreach (XmlNode n in xmlElement.SelectSingleNode("optionalCalendars").ChildNodes)
                cals.Add(new OptionalCalendar { Calendar = n.Attributes["calendar"].Value, Color = n.Attributes["color"].Value, Roles = n.Attributes["roles"].Value });
            OptionalCalendars = cals.ToArray();
        }

        public void Init(System.Xml.XmlElement xmlElement, ref System.Xml.XmlDocument doc)
        {
            XmlElement e = doc.CreateElement("optionalCalendars");
            xmlElement.AppendChild(e);
        }

        public OptionalCalendar[] OptionalCalendars;
    }
}
