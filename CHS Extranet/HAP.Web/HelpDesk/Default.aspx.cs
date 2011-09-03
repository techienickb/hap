using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Xml;
using System.DirectoryServices.AccountManagement;
using HAP.Web.Configuration;
using System.Net.Mail;
using System.DirectoryServices;
using System.Net;
using System.IO;
using HAP.Web.routing;
using System.Collections;
using System.Collections.Specialized;
using HAP.AD;

namespace HAP.Web.HelpDesk
{
    public partial class Default : HAP.Web.Controls.Page, ITicketDisplay
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - Help Desk", config.School.Name);
            loadtickets();
            if (!Page.IsPostBack)
            {
                canCurrentTicket.Visible = false;
                noCurrentTicket.Visible = true;
                if (!string.IsNullOrEmpty(TicketID))
                {
                    if (int.Parse(TicketID) > 0) loadticket();
                    else
                    {
                        loadnewticket(); if (TicketID == "-2")
                        {
                            newadminsupportticket.Attributes.Add("class", "Selected");
                            userlist.Items.Clear();
                            foreach (User user in ADUtils.FindUsers())
                                userlist.Items.Add(new ListItem(string.Format("{0} - ({1})", user.UserName, user.DisplayName), user.UserName));
                        }
                    }
                }
            }
        }

        private hapConfig config;

        public string isSelected(object o)
        {
            if (o.ToString() == TicketID) return " class=\"Selected\"";
            return "";
        }

        public string getDisplayName(object o)
        {
            User u = o as User;
            if (string.IsNullOrEmpty(u.DisplayName)) return u.UserName;
            return u.DisplayName;
        }


        private void loadnewticket()
        {
            noCurrentTicket.Visible = false;
            newticket.Visible = (int.Parse(TicketID) == -1);
            if (User.IsInRole("Domain Admins")) newadminticket.Visible = (int.Parse(TicketID) == -2);
        }

        private void loadticket()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            List<Ticket> tickets = new List<Ticket>();
            tickets.Add(Ticket.Parse(doc.SelectSingleNode("/Tickets/Ticket[@id='" + TicketID + "']")));
            currentticket.DataSource = tickets.ToArray();
            currentticket.DataBind();
            List<Note> notes = new List<Note>();
            foreach (XmlNode node in doc.SelectNodes("/Tickets/Ticket[@id='" + TicketID + "']/Note"))
                notes.Add(Note.Parse(node));
            ticketnotes.DataSource = notes.ToArray();
            ticketnotes.DataBind();
            canCurrentTicket.Visible = true;
            AddNote.Visible = true;
            AdminNote.Visible = CheckFixed.Visible = PriorityList.Visible = User.IsInRole("Domain Admins");
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
            if (User.IsInRole("Domain Admins"))
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
                    if (node.SelectNodes("Note")[0].Attributes["username"].Value.ToLower() == ADUser.UserName.ToLower())
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
            node.SetAttribute("username", ADUser.UserName);
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
            mes.From = new MailAddress(ADUser.Email, ADUser.DisplayName);
            mes.Sender = mes.From;
            mes.ReplyToList.Add(mes.From);

            mes.To.Add(new MailAddress(config.SMTP.FromEmail, config.SMTP.FromUser));

            mes.IsBodyHtml = true;
            FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newuserticket.htm"));
            StreamReader fs = template.OpenText();
            mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}",
                newticketsubject.Text).Replace("{2}",
                newticketeditor.Content).Replace("{3}",
                newticketroom.Text).Replace("{4}",
                ADUser.DisplayName).Replace("{5}",
                Request.Url.Host + Request.ApplicationPath);

            SmtpClient smtp = new SmtpClient(config.SMTP.Server, config.SMTP.Port);
            if (!string.IsNullOrEmpty(config.SMTP.User))
                smtp.Credentials = new NetworkCredential(config.SMTP.User, config.SMTP.Password);
            smtp.EnableSsl = config.SMTP.SSL;
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
            User user = new User(userlist.SelectedValue);

            mes.From = mes.Sender = new MailAddress(ADUser.UserName, ADUser.DisplayName);
            mes.ReplyToList.Add(mes.From);

            mes.To.Add(new MailAddress(user.Email, user.DisplayName));

            mes.IsBodyHtml = true;

            FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newadminticket.htm"));
            StreamReader fs = template.OpenText();
            mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}", 
                newadminticketsubject.Text).Replace("{2}", 
                newadminticketeditor.Content).Replace("{3}", 
                user.DisplayName).Replace("{4}", 
                Request.Url.Host + Request.ApplicationPath);

            SmtpClient smtp = new SmtpClient(config.SMTP.Server, config.SMTP.Port);
            if (!string.IsNullOrEmpty(config.SMTP.User))
                smtp.Credentials = new NetworkCredential(config.SMTP.User, config.SMTP.Password);
            smtp.EnableSsl = config.SMTP.SSL;
            smtp.Send(mes);

            loadtickets();
            NewTicketFiled.Visible = true;
            newadminticket.Visible = false;
        }

        protected void AddNewNote_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
            XmlNode ticket = doc.SelectSingleNode("/Tickets/Ticket[@id='" + TicketID + "']");
            if (User.IsInRole("Domain Admins"))
            {
                ticket.Attributes["status"].Value = (CheckFixed.Checked ? "Fixed" : "WithIT");
                ticket.Attributes["priority"].Value = PriorityList.SelectedValue;

                MailMessage mes = new MailMessage();
                mes.Subject = "Your Ticket (#" + TicketID + ") has been " + (CheckFixed.Checked ? "Closed" : "Updated");
                mes.From = mes.Sender = new MailAddress(ADUser.Email, ADUser.DisplayName);
                mes.ReplyToList.Add(mes.From);
                User user = new User(ticket.SelectNodes("Note")[0].Attributes["username"].Value);

                mes.To.Add(new MailAddress(user.Email, user.DisplayName));

                mes.IsBodyHtml = true;

                FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newadminnote.htm"));
                StreamReader fs = template.OpenText();

                mes.Body = fs.ReadToEnd().Replace("{0}", TicketID).Replace("{1}",
                    (CheckFixed.Checked ? "Closed" : "Updated")).Replace("{2}",
                    newnote.Content).Replace("{3}",
                    (CheckFixed.Checked ? "reopen" : "update")).Replace("{4}",
                    Request.Url.Host + Request.ApplicationPath);

                SmtpClient smtp = new SmtpClient(config.SMTP.Server, config.SMTP.Port);
                if (!string.IsNullOrEmpty(config.SMTP.User))
                    smtp.Credentials = new NetworkCredential(config.SMTP.User, config.SMTP.Password);
                smtp.EnableSsl = config.SMTP.SSL;
                smtp.Send(mes);
            }
            else
            {
                ticket.Attributes["status"].Value = "New";

                MailMessage mes = new MailMessage();

                mes.Subject = "Ticket (#" + TicketID + ") has been Updated";
                mes.From = mes.Sender = new MailAddress(ADUser.Email, ADUser.DisplayName);
                mes.ReplyToList.Add(mes.From);
                mes.To.Add(new MailAddress(config.SMTP.FromEmail, config.SMTP.FromUser));

                mes.IsBodyHtml = true;

                FileInfo template = new FileInfo(Server.MapPath("~/HelpDesk/newusernote.htm"));
                StreamReader fs = template.OpenText();

                mes.Body = fs.ReadToEnd().Replace("{0}", TicketID).Replace("{1}",
                    newnote.Content).Replace("{2}",
                    ADUser.DisplayName).Replace("{3}",
                    Request.Url.Host + Request.ApplicationPath);

                SmtpClient smtp = new SmtpClient(config.SMTP.Server, config.SMTP.Port);
                if (!string.IsNullOrEmpty(config.SMTP.User))
                    smtp.Credentials = new NetworkCredential(config.SMTP.User, config.SMTP.Password);
                smtp.EnableSsl = config.SMTP.SSL;
                smtp.Send(mes);
            }
            XmlElement node = doc.CreateElement("Note");
            node.SetAttribute("datetime", DateTime.Now.ToString("u"));
            node.SetAttribute("username", ADUser.UserName);
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


        public string TicketID { get; set; }
    }

    public class Note
    {
        public Note() { }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public string NoteText { get; set; }

        public Note(XmlNode node)
        {
            NoteText = node.InnerXml.Replace("<![CDATA[", "").Replace("]]>", "");
            if (node.Attributes["date"] != null && node.Attributes["time"] != null)
                Date = DateTime.Parse(node.Attributes["date"].Value + " " + node.Attributes["time"].Value);
            else Date = DateTime.Parse(node.Attributes["datetime"].Value);
            User = new User(node.Attributes["username"].Value);
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
        public User User { get; set; }
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
            User = new User(node.SelectNodes("Note")[0].Attributes["username"].Value);
        }
    }

}