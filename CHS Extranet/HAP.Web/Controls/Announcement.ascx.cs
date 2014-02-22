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
using System.Text.RegularExpressions;

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
            string anan = FilterAnan(htmlannouncement.Text);
            if (ShowAnnouncement.Checked && !string.IsNullOrWhiteSpace(anan))
                AnnouncementText.Text = string.Format("<h1 class=\"Announcement\"><b>Announcement</b><br />{0}</h1>", anan);
        }

        private string FilterAnan(string anan)
        {
            string result = anan;
            Regex r = new Regex("\\[Filter:\\!?.*\\].*\\[/Filter]");
            foreach (Match m in r.Matches(anan))
            {
                Regex r2 = new Regex("\\[Filter:|[\\!\\w\\d\\s\\,\\-\\.]*]");
                string group = r2.Matches(m.Value)[1].Value;
                group = group.Remove(group.Length - 1);
                string[] groups = group.Split(new char[] { ',' });
                if (isVisible(groups))
                {
                    result = result.Replace(m.Value, new Regex("\\[Filter:[\\!\\w\\d\\s\\,\\-\\.]*\\]|\\[\\/Filter\\]").Replace(m.Value, ""));
                }
                else result = result.Replace(m.Value, "");
            }
            return result.Replace("<p><br></p>", "").Replace("<p></p>", "");
        }

        private bool isVisible(string[] groups)
        {
            bool vis = false;
            foreach (string s in groups)
            {
                bool res = s.StartsWith("!") ? !Page.User.IsInRole(s.Remove(0, 1).Replace("Students", hapConfig.Current.AD.StudentsGroup)) : Page.User.IsInRole(s.Replace("Students", hapConfig.Current.AD.StudentsGroup));
                if (!vis) vis = res;
            }
            return vis;
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