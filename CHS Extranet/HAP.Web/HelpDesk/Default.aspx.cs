using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.AD;
using System.IO;
using System.Xml;
using System.Net;

namespace HAP.Web.HelpDesk
{
    public partial class Default : HAP.Web.Controls.Page
    {
        public Default() { this.SectionTitle = Localize("helpdesk/helpdesk"); }

        public bool isHDAdmin
        {
            get
            {
                foreach (string s in config.HelpDesk.Admins.Split(new char[] { ',' }))
                    if (s.Trim().ToLower().Equals(ADUser.UserName.ToLower())) return true;
                    else if (User.IsInRole(s.Trim())) return true;
                return false;
            }
        }

        public bool isUpgrade
        {
            get
            {
                return (Directory.GetFiles(HttpContext.Current.Server.MapPath("~/app_data/"), "Tickets.xml").Length > 0) && isHDAdmin;
            }
        }

        public bool hasArch { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<string> users = new List<string>();
            foreach (string s in config.HelpDesk.Admins.Split(new char[] { ',' }))
                try 
                { 
                    foreach (string s2 in System.Web.Security.Roles.GetUsersInRole(s.Trim())) if (!users.Contains(s2.ToLower())) users.Add(s2.ToLower()); 
                } 
                catch  
                { 
                    users.Add(s.Trim().ToLower()); 
                }
            foreach (FileInfo f in new DirectoryInfo(Server.MapPath("~/app_data/")).GetFiles("Tickets_*.xml", SearchOption.TopDirectoryOnly))
                archiveddates.Items.Add(new ListItem(f.Name.Remove(f.Name.LastIndexOf('.')).Remove(0, 8).Replace("_", " to "), f.Name.Remove(f.Name.LastIndexOf('.')).Remove(0, 7)));
            hasArch = archiveddates.Items.Count > 0;
            if (hasArch) archiveddates.Items.Insert(0, new ListItem("--- Select ---", ""));
            if (isHDAdmin)
            {
                userlist.Items.Clear();
                userlist2.Items.Clear();
                foreach (UserInfo user in ADUtils.FindUsers(Configuration.OUVisibility.HelpDesk))
                {
                    if (user.DisplayName == user.UserName) userlist.Items.Add(new ListItem(user.UserName, user.UserName.ToLower()));
                    else userlist.Items.Add(new ListItem(string.Format("{0} - ({1})", user.UserName, user.DisplayName), user.UserName.ToLower()));
                    if (users.Contains(user.UserName.ToLower()))
                    {
                        if (user.DisplayName == user.UserName) userlist2.Items.Add(new ListItem(user.UserName, user.UserName.ToLower()));
                        else userlist2.Items.Add(new ListItem(string.Format("{0} - ({1})", user.UserName, user.DisplayName), user.UserName.ToLower()));
                    }
                }
                userlist.SelectedValue = userlist2.SelectedValue = ADUser.UserName.ToLower();
            }
            if (!Request.Browser.Browser.Contains("Chrome"))
            {
                foreach (string ip in config.AD.InternalIP)
                {
                    if (new IPSubnet(ip).Contains(Request.UserHostAddress) && Dns.GetHostEntry(Request.UserHostAddress).HostName.ToLower().EndsWith(config.AD.UPN.ToLower())) 
                        newticket_pc.Value = Dns.GetHostEntry(Request.UserHostAddress).HostName.ToLower().Remove(Dns.GetHostEntry(Request.UserHostAddress).HostName.IndexOf('.'));
                }
            }
        }


        public void Archive()
        {
        }

        protected void archivetickets_Click(object sender, EventArgs e)
        {
            DateTime datefrom = DateTime.Parse(archivefrom.Text);
            DateTime dateto = DateTime.Parse(archiveto.Text);
            StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("~/app_data/Tickets_" + datefrom.ToString("dd-MM-yy") + "_" + dateto.ToString("dd-MM-yy") + ".xml"));
            sw.WriteLine("<?xml version=\"1.0\"?>");
            sw.WriteLine("<Tickets/>");
            sw.Close();
            sw.Dispose();
            XmlDocument doc2 = new XmlDocument();
            doc2.Load(HttpContext.Current.Server.MapPath("~/app_data/Tickets_" + datefrom.ToString("dd-MM-yy") + "_" + dateto.ToString("dd-MM-yy") + ".xml"));
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            foreach (XmlNode node in doc.SelectNodes("/Tickets/Ticket[@status='Fixed']"))
            {
                DateTime d = DateTime.Parse(node.SelectNodes("Note")[node.SelectNodes("Note").Count - 1].Attributes["datetime"].Value);
                bool faq = node.Attributes["faq"] != null;
                if (faq) faq = bool.Parse(node.Attributes["faq"].Value);
                if (datefrom.Date <= d.Date && dateto.Date > d.Date && !faq)
                {
                    doc2.SelectSingleNode("/Tickets").AppendChild(doc2.ImportNode(node.Clone(), true));
                    doc.SelectSingleNode("/Tickets").RemoveChild(node);
                }
            }
            doc.Save(HttpContext.Current.Server.MapPath("~/app_data/Tickets.xml"));
            doc2.Save(HttpContext.Current.Server.MapPath("~/app_data/Tickets_" + datefrom.ToString("dd-MM-yy") + "_" + dateto.ToString("dd-MM-yy") + ".xml"));
        }

        protected void migrate_Click(object sender, EventArgs e)
        {

        }
    }
}