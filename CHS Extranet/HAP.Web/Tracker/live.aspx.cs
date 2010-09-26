using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Management;
using System.Xml;

namespace HAP.Web.Tracker
{
    public partial class live : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        hapConfig config;
        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - Logon Tracker - Live Tracker", config.BaseSettings.EstablishmentName);
        }

        protected void refreshtimer_Tick(object sender, EventArgs e)
        {
            ListView1.DataBind();
        }

        protected void ListView1_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/tracker.xml"));
            try
            {
                ConnectionOptions connoptions = new ConnectionOptions();
                connoptions.Username = hapConfig.Current.ADSettings.ADUsername;
                connoptions.Password = hapConfig.Current.ADSettings.ADPassword;
                ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", e.CommandArgument), connoptions);
                scope.Connect();
                ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
                ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
                foreach (ManagementObject o in q.Get())
                    o.InvokeMethod("Win32Shutdown", new object[] { 4 });
            }
            catch { }
            foreach (XmlNode node in doc.SelectNodes(string.Format("/Tracker/Event[@logoffdatetime='' and @computername='{0}']", e.CommandArgument)))
                node.Attributes["logoffdatetime"].Value = DateTime.Now.ToString("s");
            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/Tracker.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();
            ListView1.DataBind();
        }

        protected void logalloff_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/tracker.xml"));
            foreach (XmlNode node in doc.SelectNodes("/Tracker/Event[@logoffdatetime='']"))
            {
                try
                {
                    ConnectionOptions connoptions = new ConnectionOptions();
                    connoptions.Username = hapConfig.Current.ADSettings.ADUsername;
                    connoptions.Password = hapConfig.Current.ADSettings.ADPassword;
                    ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", node.Attributes["computername"].Value), connoptions);
                    scope.Connect();
                    ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
                    ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
                    foreach (ManagementObject o in q.Get())
                        o.InvokeMethod("Win32Shutdown", new object[] { 4 });
                }
                catch { }
                node.Attributes["logoffdatetime"].Value = DateTime.Now.ToString("s");
                XmlWriterSettings set = new XmlWriterSettings();
                set.Indent = true;
                set.IndentChars = "   ";
                set.Encoding = System.Text.Encoding.UTF8;
                XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/Tracker.xml"), set);
                doc.Save(writer);
                writer.Flush();
                writer.Close();
            }
            ListView1.DataBind();
        }
    }
}