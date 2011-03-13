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
using System.Web.UI.DataVisualization.Charting;
using HAP.Data.Tracker;

namespace HAP.Web.Tracker
{
    public enum mode { month, day, pc }
    public partial class log : System.Web.UI.Page
    {
        private mode Mode;
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

                if (Mode == mode.month)
                    tlog = new trackerlog(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")));
                else if (Mode == mode.pc)
                    tlog = new trackerlog(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), RouteData.GetRequiredString("computer"));
                else if (Mode == mode.day)
                {
                    if (RouteData.Values["computer"] != null) tlog = new trackerlog(new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day"))), RouteData.GetRequiredString("computer"));
                    else tlog = new trackerlog(new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day"))));
                }
                foreach (trackerlogentry entry in tlog)
                {
                    if (!computerfilter.Items.Contains(new ListItem(entry.ComputerName))) computerfilter.Items.Add(new ListItem(entry.ComputerName));
                    if (!ipfilter.Items.Contains(new ListItem(entry.IP))) ipfilter.Items.Add(new ListItem(entry.IP));
                    if (!userfilter.Items.Contains(new ListItem(entry.UserName))) userfilter.Items.Add(new ListItem(entry.UserName));
                    if (!domainfilter.Items.Contains(new ListItem(entry.DomainName))) domainfilter.Items.Add(new ListItem(entry.DomainName));
                    if (!lsfilter.Items.Contains(new ListItem(entry.LogonServer))) lsfilter.Items.Add(new ListItem(entry.LogonServer));
                    if (!logondt.Items.Contains(new ListItem(entry.LogOnDateTime.ToShortDateString()))) logondt.Items.Add(new ListItem(entry.LogOnDateTime.ToShortDateString()));
                    if (entry.LogOffDateTime.HasValue)
                    {
                        if (!logoffdt.Items.Contains(new ListItem(entry.LogOffDateTime.Value.ToShortDateString()))) logoffdt.Items.Add(new ListItem(entry.LogOffDateTime.Value.ToShortDateString()));
                    }
                    else
                    {
                        if (!logoffdt.Items.Contains(new ListItem("Not Logged Off", entry.LogOffDateTime.Value.ToShortDateString()))) logoffdt.Items.Add(new ListItem("Not Logged Off", entry.LogOffDateTime.Value.ToShortDateString()));
                    }
                }
                int dim = 30;
                if (Mode == mode.month || Mode == mode.pc) dim = DateTime.DaysInMonth(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")));
                else dim = 24;
                if (Mode == mode.pc)
                {
                    pcchart.Titles[0].Text = "Tracker Data on " + RouteData.GetRequiredString("computer") + " for " + new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), 1).ToString("MMMM yyyy");
                    pcchart.ChartAreas[0].AxisX.Title = "Day";
                }
                else if (Mode == mode.month)
                {
                    pcchart.Titles[0].Text = "Tracker Data for " + new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), 1).ToString("MMMM yyyy");
                    pcchart.ChartAreas[0].AxisX.Title = "Day";
                }
                else
                {
                    if (RouteData.Values["computer"] != null) pcchart.Titles[0].Text = "Tracker Data on " + RouteData.GetRequiredString("computer") + " for " + new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day"))).ToString("dd MMMM yyyy");
                    else pcchart.Titles[0].Text = "Tracker Data for " + new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day"))).ToString("dd MMMM yyyy");
                    pcchart.ChartAreas[0].AxisX.Title = "Hour";
                }
                for (int x = 0; x < dim; x++)
                {
                    int y = 0;
                    if (Mode == mode.pc || Mode == mode.month)
                    {
                        y = tlog.Count(t => t.LogOnDateTime >= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 0, 0, 0) && t.LogOnDateTime <= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 23, 59, 59));
                        DataPoint p = new DataPoint(x + 1, y);
                        if (Mode == mode.pc)
                        {
                            p.Url = string.Format("~/tracker/{0}/{1}/c/{2}/d/{3}/", RouteData.GetRequiredString("year"), RouteData.GetRequiredString("month"), RouteData.GetRequiredString("computer"), x + 1);
                            p.ToolTip = y + " Logons - Click to view more info for " + new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1).ToLongDateString() + " for " + RouteData.GetRequiredString("computer");
                        }
                        else
                        {
                            p.Url = string.Format("~/tracker/{0}/{1}/d/{2}/", RouteData.GetRequiredString("year"), RouteData.GetRequiredString("month"), x + 1);
                            p.ToolTip = y + " Logons - Click to view more info for " + new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1).ToLongDateString();
                        }
                        pcchart.Series[0].Points.Add(p);
                    }
                    else if (Mode == mode.day)
                    {
                        y = tlog.Count(t => t.LogOnDateTime >= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 0, 0) && t.LogOnDateTime <= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 59, 59));
                        DataPoint p = new DataPoint(x, y);
                        p.ToolTip = y + " Logons";
                        pcchart.Series[0].Points.Add(p);
                    }
                }
            }
            else
            {
                if (tlog == null)
                {
                    if (Mode == mode.month)
                        tlog = new trackerlog(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")));
                    else if (Mode == mode.pc)
                        tlog = new trackerlog(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), RouteData.GetRequiredString("computer"));
                    else if (Mode == mode.day)
                    {
                        if (RouteData.Values["computer"] != null) tlog = new trackerlog(new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day"))), RouteData.GetRequiredString("computer"));
                        else tlog = new trackerlog(new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day"))));
                    }
                }
                if (computerfilter.SelectedValue != "All")
                    tlog.Filter(TrackerStringValue.ComputerName, computerfilter.SelectedValue);
                if (ipfilter.SelectedValue != "All")
                    tlog.Filter(TrackerStringValue.IP, ipfilter.SelectedValue);
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
        protected string showtable = " style=\"display: none;\"";
        hapConfig config;
        protected override void OnInitComplete(EventArgs e)
        {
            Mode = RouteData.Values["day"] != null ? mode.day : RouteData.Values["computer"] != null ? mode.pc : mode.month;
            config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - Logon Tracker - Historic Logs", config.BaseSettings.EstablishmentName);
        }

        protected void showdata_Click(object sender, EventArgs e)
        {
            showdata.Visible = false;
            showtable = "";
        }

        protected void computerfilter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void sort_Command(object sender, CommandEventArgs e)
        {
            if (tlog == null)
            {
                if (Mode == mode.month)
                    tlog = new trackerlog(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")));
                else if (Mode == mode.pc)
                    tlog = new trackerlog(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), RouteData.GetRequiredString("computer"));
                else if (Mode == mode.day)
                {
                    if (RouteData.Values["computer"] != null) tlog = new trackerlog(new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day"))), RouteData.GetRequiredString("computer"));
                    else tlog = new trackerlog(new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day"))));
                }
            }
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
                    tlog.Sort(delegate(trackerlogentry e1, trackerlogentry e2) { return e1.LogOffDateTime.Value.CompareTo(e2.LogOffDateTime.Value); });
                    break;
            }

            ListView1.DataSource = tlog.ToArray();
            ListView1.DataBind();
        }
    }
}