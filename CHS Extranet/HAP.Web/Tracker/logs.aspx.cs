using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using HAP.Web.Configuration;

namespace HAP.Web.Tracker
{
    public partial class logs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<DateTime> d = new List<DateTime>();
            foreach (trackerlogentry entry in trackerlog.CurrentFull)
            {
                DateTime dt = new DateTime(entry.LogOnDateTime.Year, entry.LogOnDateTime.Month, 1);
                if (!d.Contains(dt)) d.Add(dt);
            }
            dates.DataSource = d.ToArray();
            dates.DataBind();
        }

        protected override void OnInitComplete(EventArgs e)
        {
            this.Title = string.Format("{0} - Home Access Plus+ - Logon Tracker - Historic Logs", hapConfig.Current.BaseSettings.EstablishmentName);
        }

        protected void archivelogsb_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            XmlDocument archdoc = new XmlDocument();
            XmlElement rootNode = archdoc.CreateElement("Tracker");
            archdoc.InsertBefore(archdoc.CreateXmlDeclaration("1.0", "utf-8", null), archdoc.DocumentElement);
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/tracker.xml"));
            XmlNode el = doc.SelectSingleNode("/Tracker");
            foreach (XmlNode node in el.SelectNodes("Event"))
            {
                if (!string.IsNullOrWhiteSpace(node.Attributes["logoffdatetime"].Value))
                {
                    DateTime logoffdt = DateTime.Parse(node.Attributes["logoffdatetime"].Value);
                    if (logoffdt.Date >= DateTime.Parse(startdate.Text).Date && logoffdt.Date <= DateTime.Parse(enddate.Text).Date)
                    {
                        XmlElement ev = archdoc.CreateElement("Event");
                        foreach (XmlAttribute at in node.Attributes)
                            ev.SetAttribute(at.Name, at.Value);
                        rootNode.AppendChild(ev);
                        el.RemoveChild(node);
                    }
                }
            }
            archdoc.AppendChild(rootNode);

            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/tracker-archive-" + startdate.Text.Replace('/', '-') + "-" + enddate.Text.Replace('/', '-') + ".xml"), set);
            try
            {
                archdoc.Save(writer);
                writer.Flush();
                writer.Close();
            }
            catch { writer.Close(); }

            File.Delete(Server.MapPath("~/App_Data/Tracker.xml"));
            writer = XmlWriter.Create(Server.MapPath("~/App_Data/Tracker.xml"), set);
            try
            {
                doc.Save(writer);
                writer.Flush();
                writer.Close();
            }
            catch { writer.Close(); }

            Response.Redirect("log.aspx");
        }
    }
}