using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Xml;
using System.Net;

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
            tlog = trackerlog.Current;
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
}