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
using HAP.Data.Tracker;

namespace HAP.Web.Tracker
{
    public enum mode { month, day, pc }
    public partial class log : HAP.Web.Controls.Page
    {
        public log()
        {
            this.SectionTitle = Localize("tracker/logontracker") + " - " + Localize("tracker/historiclogs");
        }
        private mode Mode;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Mode = RouteData.Values["day"] != null ? mode.day : RouteData.Values["computer"] != null ? mode.pc : mode.month;
            Dictionary<DateTime, int> data = new Dictionary<DateTime, int>();
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
                        if (!logoffdt.Items.Contains(new ListItem("Not Logged Off", ""))) logoffdt.Items.Add(new ListItem("Not Logged Off", ""));
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
                    tlog.Filter(TrackerDateTimeValue.LogOff, logoffdt.SelectedValue);
            }
            int dim = 30;
            if (Mode == mode.month || Mode == mode.pc) dim = DateTime.DaysInMonth(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")));
            else dim = 24;
            for (int x = 0; x < dim; x++)
            {
                int y = 0;
                if (Mode == mode.pc || Mode == mode.month)
                {
                    y = tlog.Count(t => t.LogOnDateTime >= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 0, 0, 0) && t.LogOnDateTime <= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 23, 59, 59));
                    DateTime dt = new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 0, 0, 0);
                    data.Add(dt, y);
                }
                else if (Mode == mode.day)
                {
                    y = tlog.Count(t => t.LogOnDateTime >= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 0, 0) && t.LogOnDateTime <= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 59, 59));
                    DateTime dt = new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 0, 0);
                    data.Add(dt, y);
                }
            }

            ListView1.DataSource = tlog.ToArray();
            ListView1.DataBind();
            List<string> s = new List<string>();
            foreach (DateTime dt2 in data.Keys)
                s.Add(string.Format("['{0}', {1}]", dt2.ToString("yyyy-MM-dd h:mmtt"), data[dt2]));
            Data = string.Join(", ", s.ToArray());
        }
        private trackerlog tlog;
        protected string showtable = " style=\"display: none;\"";
        protected string Data { get; set; }

        protected void showdata_Click(object sender, EventArgs e)
        {
            showdata.Visible = false;
            showtable = "";
        }

        protected void computerfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            showdata.Visible = false;
            showtable = "";
        }

        protected void sort_Command(object sender, CommandEventArgs e)
        {
            showdata.Visible = false;
            showtable = "";
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
                    tlog.Sort(delegate(trackerlogentry e1, trackerlogentry e2) { return e1.LogOffDateTime.HasValue ? (e2.LogOffDateTime.HasValue ? e1.LogOffDateTime.Value.CompareTo(e2.LogOffDateTime.Value) : 1) : -1; });
                    break;
            }

            Dictionary<DateTime, int> data = new Dictionary<DateTime, int>();

            int dim = 30;
            if (Mode == mode.month || Mode == mode.pc) dim = DateTime.DaysInMonth(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")));
            else dim = 24;
            for (int x = 0; x < dim; x++)
            {
                int y = 0;
                if (Mode == mode.pc || Mode == mode.month)
                {
                    y = tlog.Count(t => t.LogOnDateTime >= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 0, 0, 0) && t.LogOnDateTime <= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 23, 59, 59));
                    DateTime dt = new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 0, 0, 0);
                    data.Add(dt, y);
                }
                else if (Mode == mode.day)
                {
                    y = tlog.Count(t => t.LogOnDateTime >= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 0, 0) && t.LogOnDateTime <= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 59, 59));
                    DateTime dt = new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 0, 0);
                    data.Add(dt, y);
                }
            }

            ListView1.DataSource = tlog.ToArray();
            ListView1.DataBind();
        }
    }
}