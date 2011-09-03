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
            EditAnnouncement.Visible = isEdit();
            AnnouncementText.Visible = isShowTo();
            if (!Page.IsPostBack)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Server.MapPath("~/App_Data/Announcement.xml"));
                XmlNode node = doc.SelectSingleNode("/announcement");
                ShowAnnouncement.Checked = bool.Parse(node.Attributes[0].Value);
                Editor1.Content = node.InnerXml.Replace("<![CDATA[ ", "").Replace(" ]]>", "");
                Editor1.DataBind();
                if (ShowAnnouncement.Checked)
                    AnnouncementText.Text = string.Format("<h1 class=\"Announcement\"><b>Announcement</b><br />{0}</h1>", Editor1.Content);
            }
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

        protected void saveann_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Announcement.xml"));
            XmlNode node = doc.SelectSingleNode("/announcement");
            node.Attributes[0].Value = ShowAnnouncement.Checked.ToString();
            node.InnerXml = string.Format("<![CDATA[ {0} ]]>", Editor1.Content);
            doc.Save(Server.MapPath("~/App_Data/Announcement.xml"));
            if (ShowAnnouncement.Checked)
                AnnouncementText.Text = string.Format("<h1 class=\"Announcement\"><b>Announcement</b><br />{0}</h1>", Editor1.Content);
            else AnnouncementText.Text = "";
        }
    }
}