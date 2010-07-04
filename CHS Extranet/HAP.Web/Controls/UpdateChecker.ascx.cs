using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using System.Reflection;
using HAP.Web.Configuration;

namespace HAP.Web.Controls
{
    public partial class UpdateChecker : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = Page.User.IsInRole("Domain Admins");
            if (this.Visible)
            {
                WebClient client = new WebClient();
                if (!string.IsNullOrEmpty(hapConfig.Current.AnnouncementBox.ProxyAddress))
                    client.Proxy = new WebProxy(hapConfig.Current.AnnouncementBox.ProxyAddress, hapConfig.Current.AnnouncementBox.ProxyPort);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(client.DownloadString("http://chsextranet.codeplex.com/Project/ProjectRss.aspx?ProjectRSSFeed=codeplex://release/chsextranet"));

                XmlNode latest = xmldoc.SelectNodes("/rss/channel/item")[0];
                XmlNode title = latest.SelectSingleNode("title");
                Regex reg = new Regex("Release: v([\\d\\.])+");
                string versioninfo = reg.Match(title.InnerText).Value.Replace("Release: ", "").TrimStart(new char[] { 'v' });

                Version NeededUpdate = Version.Parse(versioninfo);

                this.Visible = false;

                if (NeededUpdate.Major > Assembly.GetExecutingAssembly().GetName().Version.Major) this.Visible = true;
                if (NeededUpdate.Minor > Assembly.GetExecutingAssembly().GetName().Version.Minor) this.Visible = true;
                if (NeededUpdate.Build > Assembly.GetExecutingAssembly().GetName().Version.Build) this.Visible = true;
                if (NeededUpdate.Revision > Assembly.GetExecutingAssembly().GetName().Version.Revision) this.Visible = true;

                currentv.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                latestv.Text = NeededUpdate.ToString();
            }

        }

    }
}