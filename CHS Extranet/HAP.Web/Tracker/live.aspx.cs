﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Management;
using System.Xml;
using HAP.Data.Tracker;

namespace HAP.Web.Tracker
{
    public partial class live : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ListView1.DataSource = trackerlog.Current;
                ListView1.DataBind();
            }
        }

        hapConfig config;
        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - Logon Tracker - Live Tracker", config.BaseSettings.EstablishmentName);
        }

        protected void refreshtimer_Tick(object sender, EventArgs e)
        {
            ListView1.DataSource = trackerlog.Current;
            ListView1.DataBind();
        }

        protected void ListView1_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            string Computer = e.CommandArgument.ToString().Split(new char[] { '|' })[0];
            string DomainName = e.CommandArgument.ToString().Split(new char[] { '|' })[1];
            try
            {
                ConnectionOptions connoptions = new ConnectionOptions();
                connoptions.Username = hapConfig.Current.ADSettings.ADUsername;
                connoptions.Password = hapConfig.Current.ADSettings.ADPassword;
                ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", Computer), connoptions);
                scope.Connect();
                ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
                ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
                foreach (ManagementObject o in q.Get())
                    o.InvokeMethod("Win32Shutdown", new object[] { 4 });
            }
            catch { }
            if (hapConfig.Current.Tracker.Provider == "XML") xml.Clear(Computer, DomainName);
            else HAP.Data.SQL.Tracker.Clear(Computer, DomainName);
            ListView1.DataSource = trackerlog.Current;
            ListView1.DataBind();
        }

        protected void logalloff_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/tracker.xml"));
            foreach (trackerlogentry entry in trackerlog.Current)
            {
                try
                {
                    ConnectionOptions connoptions = new ConnectionOptions();
                    connoptions.Username = hapConfig.Current.ADSettings.ADUsername;
                    connoptions.Password = hapConfig.Current.ADSettings.ADPassword;
                    ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", entry.ComputerName), connoptions);
                    scope.Connect();
                    ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
                    ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
                    foreach (ManagementObject o in q.Get())
                        o.InvokeMethod("Win32Shutdown", new object[] { 4 });
                }
                catch { }
                if (hapConfig.Current.Tracker.Provider == "XML") xml.Clear(entry.ComputerName, entry.DomainName);
                else HAP.Data.SQL.Tracker.Clear(entry.ComputerName, entry.DomainName);
            }
            ListView1.DataSource = trackerlog.Current;
            ListView1.DataBind();
        }
    }
}