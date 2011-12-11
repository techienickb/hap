using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Data.SQL;
using System.Web.UI.DataVisualization.Charting;

namespace HAP.Web.Tracker
{
    public partial class WebLog : HAP.Web.Controls.Page
    {
        public WebLog()
        {
            this.SectionTitle = "HAP+ Web Logon Tracker - Logs";
        }
        private mode Mode;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Mode = RouteData.Values["day"] == null ? mode.month : mode.day;
            if (!IsPostBack)
            {
                computerfilter.Items.Add("All");
                userfilter.Items.Add("All");
                eventfilter.Items.Add("All");
                osfilter.Items.Add("All");
                browserfilter.Items.Add("All");

                if (Mode == mode.month) tlog = WebEvents.Events.Where(we => we.DateTime.Year == int.Parse(RouteData.GetRequiredString("year")) && we.DateTime.Month == int.Parse(RouteData.GetRequiredString("month"))).ToArray();
                else tlog = WebEvents.Events.Where(we => we.DateTime.Year == int.Parse(RouteData.GetRequiredString("year")) && we.DateTime.Month == int.Parse(RouteData.GetRequiredString("month")) && we.DateTime.Day == int.Parse(RouteData.GetRequiredString("day"))).ToArray();
                foreach (WebTrackerEvent entry in tlog)
                {
                    if (!computerfilter.Items.Contains(new ListItem(entry.ComputerName))) computerfilter.Items.Add(new ListItem(entry.ComputerName));
                    if (!userfilter.Items.Contains(new ListItem(entry.Username))) userfilter.Items.Add(new ListItem(entry.Username));
                    if (!eventfilter.Items.Contains(new ListItem(entry.EventType))) eventfilter.Items.Add(new ListItem(entry.EventType));
                    if (!osfilter.Items.Contains(new ListItem(entry.OS))) osfilter.Items.Add(new ListItem(entry.OS));
                    if (!browserfilter.Items.Contains(new ListItem(entry.Browser))) browserfilter.Items.Add(new ListItem(entry.Browser));
                }
                int dim = 30;
                if (Mode == mode.month) dim = DateTime.DaysInMonth(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")));
                else dim = 24;
                if (Mode == mode.month)
                {
                    pcchart.Titles[0].Text = "Web Tracker Data for " + new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), 1).ToString("MMMM yyyy");
                    pcchart.ChartAreas[0].AxisX.Title = "Day";
                }
                else
                {
                    pcchart.Titles[0].Text = "Web Tracker Data for " + new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day"))).ToString("dd MMMM yyyy");
                    pcchart.ChartAreas[0].AxisX.Title = "Hour";
                }
                for (int x = 0; x < dim; x++)
                {
                    int y = 0;
                    if (Mode == mode.month)
                    {
                        y = tlog.Count(t => t.DateTime >= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 0, 0, 0) && t.DateTime <= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1, 23, 59, 59));
                        DataPoint p = new DataPoint(x + 1, y);
                        p.Url = string.Format("~/tracker/web/{0}/{1}/d/{2}/", RouteData.GetRequiredString("year"), RouteData.GetRequiredString("month"), x + 1);
                        p.ToolTip = y + " Click to view more info for " + new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), x + 1).ToLongDateString();
                        pcchart.Series[0].Points.Add(p);
                    }
                    else if (Mode == mode.day)
                    {
                        y = tlog.Count(t => t.DateTime >= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 0, 0) && t.DateTime <= new DateTime(int.Parse(RouteData.GetRequiredString("year")), int.Parse(RouteData.GetRequiredString("month")), int.Parse(RouteData.GetRequiredString("day")), x, 59, 59));
                        DataPoint p = new DataPoint(x, y);
                        p.ToolTip = y + " Events";
                        pcchart.Series[0].Points.Add(p);
                    }
                }
            }
            else
            {
                if (tlog == null)
                {
                    if (Mode == mode.month) tlog = WebEvents.Events.Where(we => we.DateTime.Year == int.Parse(RouteData.GetRequiredString("year")) && we.DateTime.Month == int.Parse(RouteData.GetRequiredString("month"))).ToArray();
                    else if (Mode == mode.day)
                        tlog = WebEvents.Events.Where(we => we.DateTime.Year == int.Parse(RouteData.GetRequiredString("year")) && we.DateTime.Month == int.Parse(RouteData.GetRequiredString("month")) && we.DateTime.Day == int.Parse(RouteData.GetRequiredString("day"))).ToArray();
                }
                if (computerfilter.SelectedValue != "All")
                    tlog = tlog.Where(w => w.ComputerName == computerfilter.SelectedValue).ToArray();
                if (eventfilter.SelectedValue != "All")
                    tlog = tlog.Where(w => w.EventType == eventfilter.SelectedValue).ToArray();
                if (userfilter.SelectedValue != "All")
                    tlog = tlog.Where(w => w.Username == userfilter.SelectedValue).ToArray();
                if (osfilter.SelectedValue != "All")
                    tlog = tlog.Where(w => w.OS == osfilter.SelectedValue).ToArray();
                if (browserfilter.SelectedValue != "All")
                    tlog = tlog.Where(w => w.Browser == browserfilter.SelectedValue).ToArray();
                ListView1.DataSource = tlog.ToArray();
                ListView1.DataBind();
            }
        }
        private WebTrackerEvent[] tlog;
        protected string showtable = " style=\"display: none;\"";

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
                if (Mode == mode.month) tlog = WebEvents.Events.Where(we => we.DateTime.Year == int.Parse(RouteData.GetRequiredString("year")) && we.DateTime.Month == int.Parse(RouteData.GetRequiredString("month"))).ToArray();
                else if (Mode == mode.day)
                    tlog = WebEvents.Events.Where(we => we.DateTime.Year == int.Parse(RouteData.GetRequiredString("year")) && we.DateTime.Month == int.Parse(RouteData.GetRequiredString("month")) && we.DateTime.Day == int.Parse(RouteData.GetRequiredString("day"))).ToArray();
            }
            switch (e.CommandName)
            {
                case "ComputerName":
                    tlog.OrderBy(w => w.ComputerName);
                    break;
                case "Username":
                    tlog.OrderBy(w => w.ComputerName);
                    break;
                case "Browser":
                    tlog.OrderBy(w => w.Browser);
                    break;
                case "OS":
                    tlog.OrderBy(w => w.OS);
                    break;
                case "EventType":
                    tlog.OrderBy(w => w.EventType);
                    break;
                case "DT":
                    tlog.OrderByDescending(w => w.DateTime);
                    break;
            }

            ListView1.DataSource = tlog.ToArray();
            ListView1.DataBind();
        }
    }
}