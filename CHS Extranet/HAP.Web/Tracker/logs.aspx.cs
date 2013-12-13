using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using HAP.Web.Configuration;
using HAP.Data.Tracker;

namespace HAP.Web.Tracker
{
    public partial class logs : HAP.Web.Controls.Page
    {
        public logs()
        {
            SectionTitle = Localize("tracker/logontracker") + " - " + Localize("tracker/historiclogs");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //archive.Visible = hapConfig.Current.Tracker.Provider == "XML";
            Dictionary<DateTime, int> data = new Dictionary<DateTime, int>();
            foreach (trackerlogentry entry in trackerlog.CurrentFull)
            {
                DateTime dt = new DateTime(entry.LogOnDateTime.Year, entry.LogOnDateTime.Month, 1);
                DateTime dt1 = new DateTime(entry.LogOnDateTime.Year, entry.LogOnDateTime.Month, entry.LogOnDateTime.Day, 12, 0, 0);
                if (!data.ContainsKey(dt1)) data.Add(dt1, 1);
                else data[dt1]++;
            }
            List<string> s = new List<string>();
            foreach (DateTime dt2 in data.Keys)
                s.Add(string.Format("['{0}', {1}]", dt2.ToString("yyyy-MM-dd h:mmtt"), data[dt2]));
            Data = string.Join(", ", s.ToArray());
        }

        protected string Data { get; set; }

        //protected void archivelogsb_Click(object sender, EventArgs e)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    XmlDocument archdoc = new XmlDocument();
        //    XmlElement rootNode = archdoc.CreateElement("Tracker");
        //    archdoc.InsertBefore(archdoc.CreateXmlDeclaration("1.0", "utf-8", null), archdoc.DocumentElement);
        //    doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/tracker.xml"));
        //    XmlNode el = doc.SelectSingleNode("/Tracker");
        //    foreach (XmlNode node in el.SelectNodes("Event"))
        //    {
        //        if (!string.IsNullOrWhiteSpace(node.Attributes["logoffdatetime"].Value))
        //        {
        //            DateTime logoffdt = DateTime.Parse(node.Attributes["logoffdatetime"].Value);
        //            if (logoffdt.Date >= DateTime.Parse(startdate.Text).Date && logoffdt.Date <= DateTime.Parse(enddate.Text).Date)
        //            {
        //                XmlElement ev = archdoc.CreateElement("Event");
        //                foreach (XmlAttribute at in node.Attributes)
        //                    ev.SetAttribute(at.Name, at.Value);
        //                rootNode.AppendChild(ev);
        //                el.RemoveChild(node);
        //            }
        //        }
        //    }
        //    archdoc.AppendChild(rootNode);

        //    XmlWriterSettings set = new XmlWriterSettings();
        //    set.Indent = true;
        //    set.IndentChars = "   ";
        //    set.Encoding = System.Text.Encoding.UTF8;
        //    XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/tracker-archive-" + startdate.Text.Replace('/', '-') + "-" + enddate.Text.Replace('/', '-') + ".xml"), set);
        //    try
        //    {
        //        archdoc.Save(writer);
        //        writer.Flush();
        //        writer.Close();
        //    }
        //    catch { writer.Close(); }

        //    File.Delete(Server.MapPath("~/App_Data/Tracker.xml"));
        //    writer = XmlWriter.Create(Server.MapPath("~/App_Data/Tracker.xml"), set);
        //    try
        //    {
        //        doc.Save(writer);
        //        writer.Flush();
        //        writer.Close();
        //    }
        //    catch { writer.Close(); }

        //    Response.Redirect("log.aspx");
        //}
    }
}