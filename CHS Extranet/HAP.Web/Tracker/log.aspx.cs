using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Xml;
using System.Net;
using System.IO;

namespace HAP.Web.Tracker
{
    public partial class log : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                computerfilter.Items.Add("All");
                ipfilter.Items.Add("All");
                userfilter.Items.Add("All");
                domainfilter.Items.Add("All");
                lsfilter.Items.Add("All");
                logondt.Items.Add("All");
                logoffdt.Items.Add("All");

                tlog = trackerlog.Current;
                foreach (trackerlogentry entry in tlog)
                {
                    if (!computerfilter.Items.Contains(new ListItem(entry.ComputerName))) computerfilter.Items.Add(new ListItem(entry.ComputerName));
                    if (!ipfilter.Items.Contains(new ListItem(entry.IP.ToString()))) ipfilter.Items.Add(new ListItem(entry.IP.ToString()));
                    if (!userfilter.Items.Contains(new ListItem(entry.UserName))) userfilter.Items.Add(new ListItem(entry.UserName));
                    if (!domainfilter.Items.Contains(new ListItem(entry.DomainName))) domainfilter.Items.Add(new ListItem(entry.DomainName));
                    if (!lsfilter.Items.Contains(new ListItem(entry.LogonServer))) lsfilter.Items.Add(new ListItem(entry.LogonServer));
                    if (!logondt.Items.Contains(new ListItem(entry.LogOnDateTime.ToShortDateString()))) logondt.Items.Add(new ListItem(entry.LogOnDateTime.ToShortDateString()));
                    if (entry.LogOffDateTime.Year == 1)
                    {
                        if (!logoffdt.Items.Contains(new ListItem("Not Logged Off", entry.LogOffDateTime.ToShortDateString()))) logoffdt.Items.Add(new ListItem("Not Logged Off", entry.LogOffDateTime.ToShortDateString()));
                    }
                    else
                    {
                        if (!logoffdt.Items.Contains(new ListItem(entry.LogOffDateTime.ToShortDateString()))) logoffdt.Items.Add(new ListItem(entry.LogOffDateTime.ToShortDateString()));
                    }
                }

                ListView1.DataSource = tlog.ToArray();
                ListView1.DataBind();
            }
            else
            {
                if (tlog == null) tlog = trackerlog.Current;
                if (computerfilter.SelectedValue != "All")
                    tlog.Filter(TrackerStringValue.ComputerName, computerfilter.SelectedValue);
                if (ipfilter.SelectedValue != "All")
                    tlog.Filter(IPAddress.Parse(ipfilter.SelectedValue));
                if (domainfilter.SelectedValue != "All")
                    tlog.Filter(TrackerStringValue.DomainName, domainfilter.SelectedValue);
                if (userfilter.SelectedValue != "All")
                    tlog.Filter(TrackerStringValue.UserName, userfilter.SelectedValue);
                if (lsfilter.SelectedValue != "All")
                    tlog.Filter(TrackerStringValue.LogonServer, lsfilter.SelectedValue);
                if (logondt.SelectedValue != "All")
                    tlog.Filter(TrackerDateTimeValue.LogOff, DateTime.Parse(logondt.SelectedValue));
                if (logoffdt.SelectedValue != "All")
                    tlog.Filter(TrackerDateTimeValue.LogOff, DateTime.Parse(logoffdt.SelectedValue));

                ListView1.DataSource = tlog.ToArray();
                ListView1.DataBind();
            }
        }
        private trackerlog tlog;
        hapConfig config;
        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - Logon Tracker - Historic Logs", config.BaseSettings.EstablishmentName);
        }

        protected void computerfilter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void sort_Command(object sender, CommandEventArgs e)
        {
            if (tlog == null) tlog = trackerlog.Current;
            switch (e.CommandName)
            {
                case "ComputerName": 
                    tlog.Sort(delegate(trackerlogentry e1, trackerlogentry e2) { return e1.ComputerName.CompareTo(e2.ComputerName); });
                    break;
                case "Username":
                    tlog.Sort(delegate(trackerlogentry e1, trackerlogentry e2) { return e1.UserName.CompareTo(e2.UserName); });
                    break;
                case "Domain":
                    tlog.Sort(delegate(trackerlogentry e1, trackerlogentry e2) { return e1.DomainName.CompareTo(e2.DomainName); });
                    break;
                case "Server":
                    tlog.Sort(delegate(trackerlogentry e1, trackerlogentry e2) { return e1.LogonServer.CompareTo(e2.LogonServer); });
                    break;
                case "LogonDT":
                    tlog.Sort(delegate(trackerlogentry e1, trackerlogentry e2) { return e1.LogOnDateTime.CompareTo(e2.LogOnDateTime); });
                    break;
                case "LogoffDT":
                    tlog.Sort(delegate(trackerlogentry e1, trackerlogentry e2) { return e1.LogOffDateTime.CompareTo(e2.LogOffDateTime); });
                    break;
            }

            ListView1.DataSource = tlog.ToArray();
            ListView1.DataBind();
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