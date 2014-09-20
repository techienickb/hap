using HAP.AD;
using HAP.Web.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace HAP.HelpDesk.HelpDesk
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
                return (Directory.GetFiles(HttpContext.Current.Server.MapPath("~/app_data/"), "Tickets.xml").Length > 0) && isHDAdmin && config.HelpDesk.Provider != "xml";
            }
        }

        public bool hasArch { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<string> users = new List<string>();
            foreach (string s in config.HelpDesk.Admins.Split(new char[] { ',' }))
                if (s.StartsWith("!")) continue;
                else if (!System.Web.Security.Roles.RoleExists(s.Trim())) users.Add(s.Trim().ToLower());
                else foreach (string s2 in System.Web.Security.Roles.GetUsersInRole(s.Trim())) if (!users.Contains(s2.ToLower())) users.Add(s2.ToLower());
            foreach (string s in config.HelpDesk.Admins.Split(new char[] { ',' }))
                if (s.StartsWith("!") && users.Contains(s.Trim().Substring(1).ToLower())) users.Remove(s.Trim().Substring(1).ToLower());
            if (config.HelpDesk.Provider == "xml") 
                foreach (FileInfo f in new DirectoryInfo(Server.MapPath("~/app_data/")).GetFiles("Tickets_*.xml", SearchOption.TopDirectoryOnly))
                    archiveddates.Items.Add(new ListItem(f.Name.Remove(f.Name.LastIndexOf('.')).Remove(0, 8).Replace("_", " to "), f.Name.Remove(f.Name.LastIndexOf('.')).Remove(0, 7)));
            else
            {
                Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(WebConfigurationManager.ConnectionStrings[config.HelpDesk.Provider].ConnectionString);
                foreach (var tick in sql.Tickets.Where(t => t.Archive != "").GroupBy(t => t.Archive))
                    archiveddates.Items.Add(new ListItem(tick.Key.Replace("_", " to "), tick.Key));
            }
            hasArch = archiveddates.Items.Count > 0;
            if (hasArch) archiveddates.Items.Insert(0, new ListItem("--- Select ---", ""));
            adminbookingpanel.Visible = archiveadmin.Visible = isHDAdmin;
            if (isHDAdmin)
            {
                userlist.Items.Clear();
                userlist2.Items.Clear();
                foreach (UserInfo user in ADUtils.FindUsers(OUVisibility.HelpDesk))
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
                try
                {
                    foreach (string ip in config.AD.InternalIP)
                    {
                        if (new IPSubnet(ip).Contains(Request.UserHostAddress) && Dns.GetHostEntry(Request.UserHostAddress).HostName.ToLower().EndsWith(config.AD.UPN.ToLower()))
                            newticket_pc.Value = Dns.GetHostEntry(Request.UserHostAddress).HostName.ToLower().Remove(Dns.GetHostEntry(Request.UserHostAddress).HostName.IndexOf('.'));
                    }
                }
                catch { }
            }
            migrate.Visible = isUpgrade;
        }


        public void Archive()
        {
        }

        protected void archivetickets_Click(object sender, EventArgs e)
        {
            DateTime datefrom = DateTime.Parse(archivefrom.Text);
            DateTime dateto = DateTime.Parse(archiveto.Text);
            if (config.HelpDesk.Provider == "xml")
            {
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
            else
            {
                Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(WebConfigurationManager.ConnectionStrings[config.HelpDesk.Provider].ConnectionString);
                foreach (Data.SQL.Ticket tick in sql.Tickets.Where(t => t.Archive == "" && !t.Faq))
                {
                    if (!API.isOpen(tick.Status))
                    {
                        tick.Archive = datefrom.ToString("dd-MM-yy") + "_" + dateto.ToString("dd-MM-yy");
                    }
                }
                sql.SubmitChanges();
            }
        }

        protected void migrate_Click(object sender, EventArgs e)
        {
            Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(WebConfigurationManager.ConnectionStrings[config.HelpDesk.Provider].ConnectionString);
            foreach (FileInfo f in new DirectoryInfo(Server.MapPath("~/app_data/")).GetFiles("Tickets_*.xml", SearchOption.TopDirectoryOnly))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(f.OpenRead());
                Migrate(f.Name.Remove(f.Name.LastIndexOf('.')).Remove(0, 8), doc, sql);
            }
            XmlDocument doc2 = new XmlDocument();
            doc2.Load(Server.MapPath("~/App_Data/tickets.xml"));
            Migrate("", doc2, sql);
        }

        protected void Migrate(string archive, XmlDocument doc, Data.SQL.sql2linqDataContext sql)
        {
            foreach (XmlNode node in doc.SelectNodes("/Tickets/Ticket"))
            {
                FullTicket ticket = new FullTicket(node);
                Data.SQL.Ticket tick = new Data.SQL.Ticket { Archive = archive, Faq = ticket.FAQ, Status = ticket.Status, AssignedTo = ticket.AssignedTo, ShowTo = ticket.ShowTo, ReadBy = ticket.ReadBy, Title = ticket.Subject, Priority = ticket.Priority };
                foreach (Note n in ticket.Notes)
                {
                    tick.Notes.Add(new Data.SQL.Note { DateTime = DateTime.Parse(n.Date), Hide = n.Hide, Username = n.Username, Content = n.NoteText });
                }
                sql.Tickets.InsertOnSubmit(tick);
            }
            sql.SubmitChanges();
        }
    }
}