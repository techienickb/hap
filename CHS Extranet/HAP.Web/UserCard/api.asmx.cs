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
using System.Web.SessionState;

namespace HAP.Web.UserCard
{
    /// <summary>
    /// Summary description for api
    /// </summary>
    [WebService(Namespace = "http://hap.codeplex.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class api : System.Web.Services.WebService, IRequiresSessionState
    {

        #region Usercard API
        [WebMethod]
        public Init getInit(string username)
        {
            return new Init(username);
        }

        [WebMethod]
        public string getPhoto(string upn)
        {
            return Pupils.getPhoto(upn);
        }

        [WebMethod]
        public void ResetPassword(string username)
        {
            User u = new AD.User(username);
            u.ResetPassword();
        }

        [WebMethod]
        public OU[] getControlledOUs(string OuDn)
        {
            List<OU> alObjects = new List<OU>();
            DirectoryEntry directoryObject = new DirectoryEntry(string.Format("LDAP://{0}", OuDn));
            foreach (DirectoryEntry child in directoryObject.Children)
            {
                string childPath = child.Path.ToString();
                OU ou = new OU(childPath.Remove(0, 7), !childPath.Contains("CN"));
                if (child.SchemaClassName == "organizationalUnit") ou.OUs = getControlledOUs(ou.OUPath);
                else ou.OUPath = ou.Name;
                alObjects.Add(ou);
                //remove the LDAP prefix from the path

                child.Close();
                child.Dispose();
            }
            directoryObject.Close();
            directoryObject.Dispose();
            return alObjects.ToArray();
        }

        [WebMethod]
        public void Enable(string[] oupaths)
        {
            PrincipalContext pcontext = HAP.AD.ADUtils.GetPContext();
            foreach (string s in oupaths)
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(pcontext, s);
                user.Enabled = true;
                user.Save(pcontext);
            }
        }

        [WebMethod]
        public void Disable(string[] oupaths)
        {
            PrincipalContext pcontext = HAP.AD.ADUtils.GetPContext();
            foreach (string s in oupaths)
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(pcontext, s);
                user.Enabled = false;
                user.Save();
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
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            string xpath = string.Format("/Tickets/Ticket[@status!='Fixed']");
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(HAP.AD.ADUtils.GetPContext(), "Domain Admins");
            bool ia = false;
            try
            {
                ia = new User(username).IsMemberOf(gp);
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
            XmlDocument doc = new XmlDocument();
            User u = new AD.User(username);
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
            mes.From = new MailAddress(u.Email, u.DisplayName);
            mes.Sender = mes.From;
            mes.ReplyToList.Add(mes.From);

            mes.To.Add(new MailAddress(config.SMTP.FromEmail, config.SMTP.FromUser));

            mes.IsBodyHtml = true;
            FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newuserticket.htm"));
            StreamReader fs = template.OpenText();
            mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}",
                subject).Replace("{2}",
                note).Replace("{3}",
                room).Replace("{4}",
                u.DisplayName).Replace("{5}",
                HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath);

            SmtpClient smtp = new SmtpClient(config.SMTP.Server, config.SMTP.Port);
            if (!string.IsNullOrEmpty(config.SMTP.User))
                smtp.Credentials = new NetworkCredential(config.SMTP.User, config.SMTP.Password);
            smtp.EnableSsl = config.SMTP.SSL;
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
            User = new User(node.Attributes["username"].Value).DisplayName;
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
            User = new User(node.SelectNodes("Note")[0].Attributes["username"].Value).DisplayName;
            List<Note> notes = new List<Note>();
            foreach (XmlNode n in node.SelectNodes("Note"))
                notes.Add(new Note(n));
            Notes = notes.ToArray();
        }
    }
}
