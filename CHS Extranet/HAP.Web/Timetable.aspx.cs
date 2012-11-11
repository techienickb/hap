using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.AD;
using System.Xml;
using System.Web.Security;

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
                if (User.IsInRole(HAP.Web.Configuration.hapConfig.Current.AD.StudentsGroup))
                {
                    RenderTimetable(ADUser.EmployeeID);
                    tt.Visible = true;
                }
                else
                {
                    message.Text = "You are a staff user, you will not get a timetable on this system, please use SIMS";
                    tt.Visible = false;
                }
            }
        }

        private void RenderTimetable(string _upn)
        {
            ttds.SelectParameters[0].DefaultValue = _upn;
            tt.DataBind();
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

        protected void impersonate_Click(object sender, EventArgs e)
        {
            if (upn.Text.Length == 0) RenderTimetable(new User(un.Text).EmployeeID);
            else RenderTimetable(upn.Text);
            tt.Visible = true;
        }
    }
}