using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Xml;
using System.DirectoryServices.AccountManagement;
using CHS_Extranet.Configuration;
using System.Net.Mail;
using System.DirectoryServices;
using System.Net;
using System.IO;

namespace CHS_Extranet.HelpDesk
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);
            gp = new GroupPrincipal(pcontext, "Domain Admins");
            this.Title = string.Format("{0} - Home Access Plus+ - Help Desk", config.BaseSettings.EstablishmentName);
            loadtickets();
            if (!Page.IsPostBack)
            {
                canCurrentTicket.Visible = false;
                noCurrentTicket.Visible = true;
                if (!string.IsNullOrEmpty(Request.QueryString["view"]))
                {
                    if (int.Parse(Request.QueryString["view"]) > 0) loadticket();
                    else { loadnewticket(); if (Request.QueryString["view"] == "-2") newadminsupportticket.Attributes.Add("class", "Selected"); }
                }
                GroupPrincipal da = GroupPrincipal.FindByIdentity(pcontext, "Domain Admins");
                if (up.IsMemberOf(da))
                {
                    userlist.Items.Clear();
                    foreach (UserInfo user in ADUtil.FindUsers())
                        userlist.Items.Add(new ListItem(string.Format("{0} - ({1})", user.LoginName, user.DisplayName), user.LoginName));
                }
            }
        }

        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private extranetConfig config;
        private GroupPrincipal gp;

        public string Username
        {
            get
            {
                if (User.Identity.Name.Contains('\\'))
                    return User.Identity.Name.Remove(0, User.Identity.Name.IndexOf('\\') + 1);
                else return User.Identity.Name;
            }
        }

        public string isSelected(object o)
        {
            if (o.ToString() == Request.QueryString["view"]) return " class=\"Selected\"";
            return "";
        }

        public string getDisplayName(object o)
        {
            UserPrincipal u = o as UserPrincipal;
            if (string.IsNullOrEmpty(u.DisplayName)) return Username;
            return u.DisplayName;
        }


        private void loadnewticket()
        {
            noCurrentTicket.Visible = false;
            newticket.Visible = (int.Parse(Request.QueryString["view"]) == -1);
            GroupPrincipal da = GroupPrincipal.FindByIdentity(pcontext, "Domain Admins");
            if (up.IsMemberOf(da)) newadminticket.Visible = (int.Parse(Request.QueryString["view"]) == -2);
        }

        private void loadticket()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            List<Ticket> tickets = new List<Ticket>();
            tickets.Add(Ticket.Parse(doc.SelectSingleNode("/Tickets/Ticket[@id='" + Request.QueryString["view"] + "']")));
            currentticket.DataSource = tickets.ToArray();
            currentticket.DataBind();
            List<Note> notes = new List<Note>();
            foreach (XmlNode node in doc.SelectNodes("/Tickets/Ticket[@id='" + Request.QueryString["view"] + "']/Note"))
                notes.Add(Note.Parse(node));
            ticketnotes.DataSource = notes.ToArray();
            ticketnotes.DataBind();
            canCurrentTicket.Visible = true;
            AddNote.Visible = true;
            GroupPrincipal da = GroupPrincipal.FindByIdentity(pcontext, "Domain Admins");
            AdminNote.Visible = CheckFixed.Visible = PriorityList.Visible = up.IsMemberOf(da);
            CheckFixed.Checked = (tickets[0].Status == "Fixed");
            PriorityList.SelectedValue = tickets[0].Priority;
            noCurrentTicket.Visible = false;
        }

        private void loadtickets()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            openticketcount.Text = doc.SelectNodes("/Tickets/Ticket[@status!='Fixed']").Count.ToString();
            string xpath = string.Format("/Tickets/Ticket[@status{0}]", statusselection.SelectedValue == "Open" ? "!='Fixed'" : "='Fixed'");
            GroupPrincipal da = GroupPrincipal.FindByIdentity(pcontext, "Domain Admins");
            if (up.IsMemberOf(da))
            {
                List<Ticket> tickets = new List<Ticket>();
                foreach (XmlNode node in doc.SelectNodes(xpath))
                    tickets.Add(Ticket.Parse(node));
                ticketsrepeater.DataSource = tickets.ToArray();
                ticketsrepeater.DataBind();
                newadminsupportticket.Visible = true;
            }
            else
            {
                newadminsupportticket.Visible = false;
                List<Ticket> tickets = new List<Ticket>();
                foreach (XmlNode node in doc.SelectNodes(xpath))
                    if (node.SelectNodes("Note")[0].Attributes["username"].Value.ToLower() == Username.ToLower())
                        tickets.Add(Ticket.Parse(node));
                ticketsrepeater.DataSource = tickets.ToArray();
                ticketsrepeater.DataBind();
            }
        }

        protected string _id;

        protected void FileTicket_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            int x;
            if (doc.SelectSingleNode("/Tickets").ChildNodes.Count > 0)
            {
                XmlNodeList tickets = doc.SelectNodes("/Tickets/Ticket");
                x = int.Parse(tickets[tickets.Count - 1].Attributes["id"].Value) + 1;
            }
            else x = 1;
            _id = x.ToString();
            XmlElement ticket = doc.CreateElement("Ticket");
            ticket.SetAttribute("id", x.ToString());
            ticket.SetAttribute("subject", newticketsubject.Text);
            ticket.SetAttribute("priority", "Normal");
            ticket.SetAttribute("status", "New");
            XmlElement node = doc.CreateElement("Note");
            node.SetAttribute("datetime", DateTime.Now.ToUniversalTime().ToString("u"));
            node.SetAttribute("username", Username);
            node.InnerXml = "<![CDATA[Room: " + newticketroom.Text + "<br />\n" + newticketeditor.Content + "]]>";
            ticket.AppendChild(node);
            doc.SelectSingleNode("/Tickets").AppendChild(ticket);

            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/Tickets.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();

            MailMessage mes = new MailMessage();

            mes.Subject = "A Ticket (#" + x + ") has been Created";
            mes.From = new MailAddress(up.EmailAddress, up.DisplayName);
            mes.Sender = mes.ReplyTo = mes.From;

            mes.To.Add(new MailAddress(config.BaseSettings.AdminEmailAddress, "IT Department"));

            mes.IsBodyHtml = true;
            FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newuserticket.htm"));
            StreamReader fs = template.OpenText();
            mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}",
                newticketsubject.Text).Replace("{2}",
                newticketeditor.Content).Replace("{3}",
                newticketroom.Text).Replace("{4}",
                up.DisplayName).Replace("{5}",
                Request.Url.Host);

            SmtpClient smtp = new SmtpClient(config.BaseSettings.SMTPServer);
            if (!string.IsNullOrEmpty(config.BaseSettings.SMTPServerUsername))
                smtp.Credentials = new NetworkCredential(config.BaseSettings.SMTPServerUsername, config.BaseSettings.SMTPServerPassword);
            smtp.EnableSsl = config.BaseSettings.SMTPServerSSL;
            smtp.Port = config.BaseSettings.SMTPServerPort;
            smtp.Send(mes);

            loadtickets();
            newticket.Visible = false;
            NewTicketFiled.Visible = true;
        }

        protected void FileAdminTicket_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            XmlNodeList tickets = doc.SelectNodes("/Tickets/Ticket");
            int x = int.Parse(tickets[tickets.Count - 1].Attributes["id"].Value) + 1;
            _id = x.ToString();
            XmlElement ticket = doc.CreateElement("Ticket");
            ticket.SetAttribute("id", x.ToString());
            ticket.SetAttribute("subject", newadminticketsubject.Text);
            ticket.SetAttribute("priority", newadminticketpriorityList.SelectedValue);
            ticket.SetAttribute("status", "New");
            XmlElement node = doc.CreateElement("Note");
            node.SetAttribute("datetime", DateTime.Now.ToUniversalTime().ToString("u"));
            node.SetAttribute("username", userlist.SelectedValue);
            node.InnerXml = "<![CDATA[" + newadminticketeditor.Content + "]]>";
            ticket.AppendChild(node);
            doc.SelectSingleNode("/Tickets").AppendChild(ticket);

            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/Tickets.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();

            MailMessage mes = new MailMessage();

            mes.Subject = "A Support Ticket (#" + x + ") has been Logged";
            UserPrincipal user = UserPrincipal.FindByIdentity(pcontext, userlist.SelectedValue);

            mes.From = mes.ReplyTo = mes.Sender = new MailAddress(up.EmailAddress, "IT Department");

            mes.To.Add(new MailAddress(user.EmailAddress, user.DisplayName));

            mes.IsBodyHtml = true;

            FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newadminticket.htm"));
            StreamReader fs = template.OpenText();
            mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}", 
                newadminticketsubject.Text).Replace("{2}", 
                newadminticketeditor.Content).Replace("{3}", 
                user.DisplayName).Replace("{4}", 
                Request.Url.Host);
            
            SmtpClient smtp = new SmtpClient(config.BaseSettings.SMTPServer);
            if (!string.IsNullOrEmpty(config.BaseSettings.SMTPServerUsername))
                smtp.Credentials = new NetworkCredential(config.BaseSettings.SMTPServerUsername, config.BaseSettings.SMTPServerPassword);
            smtp.EnableSsl = config.BaseSettings.SMTPServerSSL;
            smtp.Port = config.BaseSettings.SMTPServerPort;
            smtp.Send(mes);

            loadtickets();
            NewTicketFiled.Visible = true;
            newadminticket.Visible = false;
        }

        protected void AddNewNote_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            XmlNode ticket = doc.SelectSingleNode("/Tickets/Ticket[@id='" + Request.QueryString["view"] + "']");
            GroupPrincipal da = GroupPrincipal.FindByIdentity(pcontext, "Domain Admins");
            if (up.IsMemberOf(da))
            {
                ticket.Attributes["status"].Value = (CheckFixed.Checked ? "Fixed" : "WithIT");
                ticket.Attributes["priority"].Value = PriorityList.SelectedValue;

                MailMessage mes = new MailMessage();
                mes.Subject = "Your Ticket (#" + Request.QueryString["view"] + ") has been " + (CheckFixed.Checked ? "Closed" : "Updated");
                mes.From = mes.ReplyTo = mes.Sender = new MailAddress(up.EmailAddress, "IT Department");
                UserPrincipal user = UserPrincipal.FindByIdentity(pcontext, ticket.SelectNodes("Note")[0].Attributes["username"].Value);

                mes.To.Add(new MailAddress(user.EmailAddress, user.DisplayName));

                mes.IsBodyHtml = true;

                FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newadminnote.htm"));
                StreamReader fs = template.OpenText();

                mes.Body = fs.ReadToEnd().Replace("{0}", Request.QueryString["view"]).Replace("{1}",
                    (CheckFixed.Checked ? "Closed" : "Updated")).Replace("{2}",
                    newnote.Content).Replace("{3}",
                    (CheckFixed.Checked ? "reopen" : "update")).Replace("{4}",
                    Request.Url.Host);

                SmtpClient smtp = new SmtpClient(config.BaseSettings.SMTPServer);
                if (!string.IsNullOrEmpty(config.BaseSettings.SMTPServerUsername))
                    smtp.Credentials = new NetworkCredential(config.BaseSettings.SMTPServerUsername, config.BaseSettings.SMTPServerPassword);
                smtp.EnableSsl = config.BaseSettings.SMTPServerSSL;
                smtp.Port = config.BaseSettings.SMTPServerPort;
                smtp.Send(mes);
            }
            else
            {
                ticket.Attributes["status"].Value = "New";

                MailMessage mes = new MailMessage();

                mes.Subject = "Ticket (#" + Request.QueryString["view"] + ") has been Updated";
                mes.From = mes.ReplyTo = mes.Sender = new MailAddress(up.EmailAddress, up.DisplayName);

                mes.To.Add(new MailAddress(config.BaseSettings.AdminEmailAddress, "IT Department"));

                mes.IsBodyHtml = true;

                FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newusernote.htm"));
                StreamReader fs = template.OpenText();

                mes.Body = fs.ReadToEnd().Replace("{0}", Request.QueryString["view"]).Replace("{1}",
                    newnote.Content).Replace("{2}",
                    up.DisplayName).Replace("{3}",
                    Request.Url.Host);

                SmtpClient smtp = new SmtpClient(config.BaseSettings.SMTPServer);
                if (!string.IsNullOrEmpty(config.BaseSettings.SMTPServerUsername))
                    smtp.Credentials = new NetworkCredential(config.BaseSettings.SMTPServerUsername, config.BaseSettings.SMTPServerPassword);
                smtp.EnableSsl = config.BaseSettings.SMTPServerSSL;
                smtp.Port = config.BaseSettings.SMTPServerPort;
                smtp.Send(mes);
            }
            XmlElement node = doc.CreateElement("Note");
            node.SetAttribute("datetime", DateTime.Now.ToString("u"));
            node.SetAttribute("username", Username);
            if (string.IsNullOrEmpty(newnote.Content)) node.InnerXml = "<![CDATA[No Note Information Added]]>";
            else node.InnerXml = "<![CDATA[" + newnote.Content + "]]>";
            ticket.AppendChild(node);


            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/Tickets.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();
            loadtickets();
            loadticket();
            newnote.Content = "";
        }

    }

    public class Note
    {
        public DateTime Date { get; set; }
        public UserPrincipal User { get; set; }
        public string NoteText { get; set; }

        public Note(XmlNode node)
        {
            NoteText = node.InnerXml.Replace("<![CDATA[", "").Replace("]]>", "");
            if (node.Attributes["date"] != null && node.Attributes["time"] != null)
                Date = DateTime.Parse(node.Attributes["date"].Value + " " + node.Attributes["time"].Value);
            else Date = DateTime.Parse(node.Attributes["datetime"].Value);
            extranetConfig config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
            string _DomainDN = "";
            if (ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.StartsWith("LDAP://"))
                _DomainDN = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.Remove(0, ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.IndexOf("DC="));
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            User = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, node.Attributes["username"].Value);
        }

        public static Note Parse(XmlNode node) { return new Note(node); }
    }

    public class Ticket
    {
        public static Ticket Parse(XmlNode node) { return new Ticket(node); }
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public UserPrincipal User { get; set; }
        public DateTime Date { get; set; }

        public Ticket(XmlNode node)
        {
            Id = int.Parse(node.Attributes["id"].Value);
            Subject = node.Attributes["subject"].Value;
            Priority = node.Attributes["priority"].Value;
            Status = node.Attributes["status"].Value;
            if (node.SelectNodes("Note")[0].Attributes["date"] != null)
                Date = DateTime.Parse(node.SelectNodes("Note")[0].Attributes["date"].Value + " " + node.SelectNodes("Note")[0].Attributes["time"].Value);
            Date = DateTime.Parse(node.SelectNodes("Note")[0].Attributes["datetime"].Value);
            extranetConfig config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
            string _DomainDN = "";
            if (ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.StartsWith("LDAP://"))
                _DomainDN = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.Remove(0, ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.IndexOf("DC="));
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            User = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, node.SelectNodes("Note")[0].Attributes["username"].Value);
        }
    }

    // Structures for returning user information
    public struct UserInfo : IComparable
    {
        public string LoginName;
        public string DisplayName;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return LoginName.CompareTo(((UserInfo)obj).LoginName);
        }

        #endregion
    }

    // Static class containing all the supported user property names
    public class UserProperty
    {
        public static string CommonName = "cn";
        public static string UserName = "sAMAccountName";
        public static string Company = "company";
        public static string Department = "department";
        public static string Description = "description";
        public static string DisplayName = "displayName";
        public static string FirstName = "givenName";
        public static string Email = "mail";
        public static string LastName = "sn";
        public static string Notes = "info";
    }

    public class ADUtil
    {
        static public UserInfo[] FindUsers()
        {
            System.Collections.Generic.List<UserInfo> users = new System.Collections.Generic.List<UserInfo>();
            foreach (UserInfo info in FindUsers("Teaching Staff"))
                if  (!users.Contains(info))
                    users.Add(info);
            foreach (UserInfo info in FindUsers("Non-Teaching Staff"))
                if (!users.Contains(info))
                    users.Add(info);
            foreach (UserInfo info in FindUsers("Domain Admins"))
                if (!users.Contains(info))
                    users.Add(info);
            users.Sort();
            return users.ToArray();
        }

        // FindUsers - Returns all users matching a pattern
        static public UserInfo[] FindUsers(string ou)
        {
            List<UserInfo> results = new List<UserInfo>();

            extranetConfig config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
            DirectoryEntry usersDE = new DirectoryEntry(ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(&(objectClass=user)(mail=*)(sAMAccountName=*)(mailNickname=*))";
            ds.PropertiesToLoad.Add(UserProperty.UserName);
            ds.PropertiesToLoad.Add(UserProperty.DisplayName);
            ds.PropertiesToLoad.Add(UserProperty.Email);

            SearchResultCollection sr = ds.FindAll();

            for (int i = 0; i < sr.Count; i++)
            {
                int z = 0;
                if (!int.TryParse(sr[i].Properties[UserProperty.UserName][0].ToString().ToCharArray()[0].ToString(), out z))
                {
                    UserInfo info = new UserInfo();
                    info.LoginName = sr[i].Properties[UserProperty.UserName][0].ToString();
                    if (sr[i].Properties[UserProperty.DisplayName].Count == 0)
                        info.DisplayName = "";
                    else if (sr[i].Properties[UserProperty.DisplayName] != null)
                        info.DisplayName = sr[i].Properties[UserProperty.DisplayName][0].ToString();
                    else
                        info.DisplayName = "";
                    results.Add(info);
                }
            }
            results.Sort();
            return (results.ToArray());
        }
    }

}