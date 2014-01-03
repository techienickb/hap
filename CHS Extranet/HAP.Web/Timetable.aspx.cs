using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.AD;
using System.Xml;
using System.Web.Security;
using HAP.BookingSystem;
using HAP.Web.Configuration;

namespace HAP.Web
{
    public partial class Timetable : HAP.Web.Controls.Page
    {
        public Timetable()
        {
            this.SectionTitle = Localize("timetable/my");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                adminconverter.Visible = User.IsInRole("Domain Admins");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
        }

        protected string JSLessons
        {
            get
            {
                List<string> s = new List<string>();
                foreach (Lesson l in config.BookingSystem.Lessons)
                {
                    s.Add("{" + string.Format("\"Name\": \"{0}\", \"Start\": \"{1}:{2}\", \"End\": \"{3}:{4}\", \"FromStart\": null, \"FromEnd\": null, \"Type\": \"{5}\"", l.Name, l.StartTime.Hour, l.StartTime.Minute, l.EndTime.Hour, l.EndTime.Minute, l.Type) + "}");
                }
                return string.Join(", ", s.ToArray());
            }
        }

        protected string JSTermDates
        {
            get
            {
                List<string> terms = new List<string>();
                foreach (Term t in new HAP.BookingSystem.Terms())
                    terms.Add("{ " + string.Format(" name: '{0}', start: new Date({1}, {2}, {3}), end: new Date({4}, {5}, {6})",
                        t.Name,
                        t.StartDate.Year, t.StartDate.Month - 1, t.StartDate.Day,
                        t.EndDate.Year, t.EndDate.Month - 1, t.EndDate.Day) + ", halfterm: { " +
                        string.Format("start: new Date({0}, {1}, {2}), end: new Date({3}, {4}, {5})",
                        t.HalfTerm.StartDate.Year, t.HalfTerm.StartDate.Month - 1, t.HalfTerm.StartDate.Day,
                        t.HalfTerm.EndDate.Year, t.HalfTerm.EndDate.Month - 1, t.HalfTerm.EndDate.Day) + " } }");
                return string.Join(", ", terms.ToArray());
            }
        }

        protected void convert_Click(object sender, EventArgs e)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Server.MapPath("~/app_data/timetableexport.xml"));
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/app_data/timetables.xml"));
            XmlNode n = doc.SelectSingleNode("timetables");
            n.RemoveAll();
            int x = 0;
            foreach (XmlNode node in xdoc.SelectNodes("/SuperStarReport/Record"))
            {
                x++;
                XmlElement el = doc.CreateElement("record");
                if (node.SelectSingleNode("UPN") != null)
                    el.SetAttribute("upn", node.SelectSingleNode("UPN").InnerText);
                if (node.SelectSingleNode("YearGroup") != null)
                    el.SetAttribute("year", node.SelectSingleNode("YearGroup").InnerText.Remove(0, 6));
                if (node.SelectSingleNode("ShortName") != null)
                    el.SetAttribute("name", node.SelectSingleNode("ShortName").InnerText);
                if (node.SelectSingleNode("Description") != null)
                    el.SetAttribute("description", node.SelectSingleNode("Description").InnerText);
                if (node.SelectSingleNode("Surname") != null)
                    el.SetAttribute("teacher", (node.SelectSingleNode("Title") == null ? "" : node.SelectSingleNode("Title").InnerText) + " " + node.SelectSingleNode("Surname").InnerText);
                if (node.SelectSingleNode("Name1") != null)
                    el.SetAttribute("room", node.SelectSingleNode("Name1").InnerText);
                if (node.SelectSingleNode("Name") != null)
                    el.SetAttribute("period", node.SelectSingleNode("Name").InnerText);
                if (node.SelectSingleNode("StartTime") != null)
                    el.SetAttribute("starttime", node.SelectSingleNode("StartTime").InnerText);
                if (node.SelectSingleNode("EndTime") != null)
                    el.SetAttribute("endtime", node.SelectSingleNode("EndTime").InnerText);
                n.AppendChild(el);
            }
            doc.Save(Server.MapPath("~/app_data/timetables.xml"));
            message.Text = "Import Done";
        }
    }
}