using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using HAP.Web.Configuration;
using HAP.Data.UserCard;
using System.Runtime.InteropServices;
using HAP.Data;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Xml;
using System.Net.Mail;
using System.Net;
using System.IO;
using HAP.AD;
using System.DirectoryServices;

namespace HAP.Web.UserCard
{
    /// <summary>
    /// Summary description for api
    /// </summary>
    [WebService(Namespace = "http://hap.codeplex.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class api : System.Web.Services.WebService
    {

        #region Usercard API
        [WebMethod]
        public Init getInit()
        {
            return new Init();
        }

        [WebMethod]
        public string getPhoto(string upn)
        {
            return Pupils.getPhoto(upn);
        }

        [WebMethod]
        public void Enable(string[] oupaths)
        {
            foreach (string s in oupaths)
            {
                try
                {
                    DirectoryEntry user = new DirectoryEntry("LDAP://" + s, hapConfig.Current.ADSettings.ADUsername, hapConfig.Current.ADSettings.ADPassword);
                    int val = (int)user.Properties["userAccountControl"].Value;
                    user.Properties["userAccountControl"].Value = val & ~0x2;
                    //ADS_UF_NORMAL_ACCOUNT;

                    user.CommitChanges();
                    user.Close();
                }
                catch { continue; }
            }
        }

        [WebMethod]
        public void Disable(string[] oupaths)
        {
            foreach (string s in oupaths)
            {
                try
                {
                    DirectoryEntry user = new DirectoryEntry("LDAP://" + s, hapConfig.Current.ADSettings.ADUsername, hapConfig.Current.ADSettings.ADPassword);
                    int val = (int)user.Properties["userAccountControl"].Value;
                    user.Properties["userAccountControl"].Value = val | ~0x2;
                    //ADS_UF_NORMAL_ACCOUNT;

                    user.CommitChanges();
                    user.Close();
                }
                catch { continue; }
            }
        }

        [WebMethod]
        public HAP.Data.Quota.QuotaInfo GetFreeSpacePercentage(string username, string userhome)
        {
            return HAP.Data.ComputerBrowser.Quota.GetQuota(username, userhome);
        }
        #endregion

        #region HelpDesk API

        [WebMethod]
        public Ticket[] getMyTickets(string username)
        {
            hapConfig config = hapConfig.Current;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            string _ActiveDirectoryConnectionString = "";
            string _DomainDN = "";
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            UserPrincipal up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, username);
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            string xpath = string.Format("/Tickets/Ticket[@status!='Fixed']");
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, "Domain Admins");
            bool ia = false;
            try
            {
                ia = up.IsMemberOf(gp);
            }
            catch { }
            if (ia)
            {
                List<Ticket> tickets = new List<Ticket>();
                foreach (XmlNode node in doc.SelectNodes(xpath))
                    tickets.Add(Ticket.Parse(node));
                return tickets.ToArray();
            }
            else
            {
                List<Ticket> tickets = new List<Ticket>();
                foreach (XmlNode node in doc.SelectNodes(xpath))
                    if (node.SelectNodes("Note")[0].Attributes["username"].Value.ToLower() == username.ToLower())
                        tickets.Add(Ticket.Parse(node));
                return tickets.ToArray();
            }
        }

        [WebMethod]
        public Ticket[] setNewTicket(string subject, string note, [Optional]string room, string username)
        {
            hapConfig config = hapConfig.Current;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            string _ActiveDirectoryConnectionString = "";
            string _DomainDN = "";
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            UserPrincipal up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, username);
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            int x;
            if (doc.SelectSingleNode("/Tickets").ChildNodes.Count > 0)
            {
                XmlNodeList tickets = doc.SelectNodes("/Tickets/Ticket");
                x = int.Parse(tickets[tickets.Count - 1].Attributes["id"].Value) + 1;
            }
            else x = 1;
            string _id = x.ToString();
            XmlElement ticket = doc.CreateElement("Ticket");
            ticket.SetAttribute("id", x.ToString());
            ticket.SetAttribute("subject", subject);
            ticket.SetAttribute("priority", "Normal");
            ticket.SetAttribute("status", "New");
            XmlElement node = doc.CreateElement("Note");
            node.SetAttribute("datetime", DateTime.Now.ToUniversalTime().ToString("u"));
            node.SetAttribute("username", username);
            node.InnerXml = "<![CDATA[Room: " + room + "<br />\n" + note + "]]>";
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
            mes.Sender = mes.From;
            mes.ReplyToList.Add(mes.From);

            mes.To.Add(new MailAddress(config.BaseSettings.AdminEmailAddress, "IT Department"));

            mes.IsBodyHtml = true;
            FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newuserticket.htm"));
            StreamReader fs = template.OpenText();
            mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}",
                subject).Replace("{2}",
                note).Replace("{3}",
                room).Replace("{4}",
                up.DisplayName).Replace("{5}",
                HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath);

            SmtpClient smtp = new SmtpClient(config.BaseSettings.SMTPServer);
            if (!string.IsNullOrEmpty(config.BaseSettings.SMTPServerUsername))
                smtp.Credentials = new NetworkCredential(config.BaseSettings.SMTPServerUsername, config.BaseSettings.SMTPServerPassword);
            smtp.EnableSsl = config.BaseSettings.SMTPServerSSL;
            smtp.Port = config.BaseSettings.SMTPServerPort;
            smtp.Send(mes);
            return getMyTickets(username);
        }
        #endregion
    }


    public class Note
    {
        public Note() { }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string NoteText { get; set; }

        public Note(XmlNode node)
        {
            NoteText = node.InnerXml.Replace("<![CDATA[", "").Replace("]]>", "");
            if (node.Attributes["date"] != null && node.Attributes["time"] != null)
                Date = DateTime.Parse(node.Attributes["date"].Value + " " + node.Attributes["time"].Value);
            else Date = DateTime.Parse(node.Attributes["datetime"].Value);
            hapConfig config = hapConfig.Current;
            string _DomainDN = "";
            if (ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.StartsWith("LDAP://"))
                _DomainDN = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.Remove(0, ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.IndexOf("DC="));
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            User = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, node.Attributes["username"].Value).DisplayName;
        }

        public static Note Parse(XmlNode node) { return new Note(node); }
    }

    public class Ticket
    {
        public Ticket() { }
        public static Ticket Parse(XmlNode node) { return new Ticket(node); }
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public Note[] Notes { get; set; }

        public Ticket(XmlNode node)
        {
            Id = int.Parse(node.Attributes["id"].Value);
            Subject = node.Attributes["subject"].Value;
            Priority = node.Attributes["priority"].Value;
            Status = node.Attributes["status"].Value;
            if (node.SelectNodes("Note")[0].Attributes["date"] != null)
                Date = DateTime.Parse(node.SelectNodes("Note")[0].Attributes["date"].Value + " " + node.SelectNodes("Note")[0].Attributes["time"].Value);
            Date = DateTime.Parse(node.SelectNodes("Note")[0].Attributes["datetime"].Value);
            hapConfig config = hapConfig.Current;
            string _DomainDN = "";
            if (ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.StartsWith("LDAP://"))
                _DomainDN = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.Remove(0, ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString.IndexOf("DC="));
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            User = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, node.SelectNodes("Note")[0].Attributes["username"].Value).DisplayName;
            List<Note> notes = new List<Note>();
            foreach (XmlNode n in node.SelectNodes("Note"))
                notes.Add(new Note(n));
            Notes = notes.ToArray();
        }
    }
}
