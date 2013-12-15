using System;
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
    public partial class live : HAP.Web.Controls.Page
    {
        public live() { this.SectionTitle = Localize("tracker/logontracker") + " - " + Localize("tracker/livelogons"); }

        //protected void ListView1_ItemCommand(object sender, ListViewCommandEventArgs e)
        //{
        //    string Computer = e.CommandArgument.ToString().Split(new char[] { '|' })[0];
        //    string DomainName = e.CommandArgument.ToString().Split(new char[] { '|' })[1];
        //    try
        //    {
        //        ConnectionOptions connoptions = new ConnectionOptions();
        //        connoptions.Username = hapConfig.Current.AD.User;
        //        connoptions.Password = hapConfig.Current.AD.Password;
        //        ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", Computer), connoptions);
        //        scope.Connect();
        //        ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
        //        ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
        //        foreach (ManagementObject o in q.Get())
        //            o.InvokeMethod("Win32Shutdown", new object[] { 4 });
        //    }
        //    catch { }
        //    if (hapConfig.Current.Tracker.Provider == "XML") xml.Clear(Computer, DomainName);
        //    else HAP.Data.SQL.Tracker.Clear(Computer, DomainName);
        //    ListView1.DataSource = trackerlog.Current;
        //    ListView1.DataBind();
        //}

        //protected void logalloff_Click(object sender, EventArgs e)
        //{
        //    foreach (trackerlogentry entry in trackerlog.Current)
        //    {
        //        try
        //        {
        //            ConnectionOptions connoptions = new ConnectionOptions();
        //            connoptions.Username = hapConfig.Current.AD.User;
        //            connoptions.Password = hapConfig.Current.AD.Password;
        //            ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", entry.ComputerName), connoptions);
        //            scope.Connect();
        //            ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
        //            ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
        //            foreach (ManagementObject o in q.Get())
        //                o.InvokeMethod("Win32Shutdown", new object[] { 4 });
        //        }
        //        catch { }
        //        if (hapConfig.Current.Tracker.Provider == "XML") xml.Clear(entry.ComputerName, entry.DomainName);
        //        else HAP.Data.SQL.Tracker.Clear(entry.ComputerName, entry.DomainName);
        //    }
        //    ListView1.DataSource = trackerlog.Current;
        //    ListView1.DataBind();
        //}

        //protected void removeall_Click(object sender, EventArgs e)
        //{
        //    foreach (trackerlogentry entry in trackerlog.Current)
        //        if (hapConfig.Current.Tracker.Provider == "XML") xml.Clear(entry.ComputerName, entry.DomainName);
        //        else HAP.Data.SQL.Tracker.Clear(entry.ComputerName, entry.DomainName);
        //    ListView1.DataSource = trackerlog.Current;
        //    ListView1.DataBind();
        //}
    }
}