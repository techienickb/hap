using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using HAP.Web.Configuration;
using System.Configuration;
using System.Xml;

namespace HAP.Web.Controls
{
    public partial class Announcement : System.Web.UI.UserControl
    {
        private hapConfig config;
        protected void Page_Load(object sender, EventArgs e)
        {
            config = hapConfig.Current;
            adminbox.Visible = isEdit();
            AnnouncementText.Visible = isShowTo();
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Announcement.xml"));
            XmlNode node = doc.SelectSingleNode("/announcement");
            ShowAnnouncement.Checked = bool.Parse(node.Attributes[0].Value);
            htmlannouncement.Text = node.InnerXml.Replace("<![CDATA[ ", "").Replace(" ]]>", "");
            if (ShowAnnouncement.Checked)
                AnnouncementText.Text = string.Format("<h1 class=\"Announcement\"><b>Announcement</b><br />{0}</h1>", htmlannouncement.Text);
        }

        private bool isShowTo()
        {
            if (config.Homepage.AnnouncementBox.ShowTo == "All") return true;
            else if (config.Homepage.AnnouncementBox.ShowTo != "None")
            {
                bool vis = false;
                foreach (string s in config.Homepage.AnnouncementBox.ShowTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Page.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        private bool isEdit()
        {
            if (config.Homepage.AnnouncementBox.EnableEditTo == "All") return true;
            else if (config.Homepage.AnnouncementBox.EnableEditTo != "None")
            {
                bool vis = false;
                foreach (string s in config.Homepage.AnnouncementBox.EnableEditTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Page.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }
    }
}