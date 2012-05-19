using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using System.Xml;
using HAP.AD;
using System.DirectoryServices.AccountManagement;
using HAP.Web.Configuration;
using System.Net.Mail;
using System.Net;

namespace HAP.HelpDesk
{
    [ServiceAPI("api/helpdesk")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class API
    {
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Ticket/{Id}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public FullTicket UpdateTicket(string Id, string Note)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            XmlNode ticket = doc.SelectSingleNode("/Tickets/Ticket[@id='" + Id + "']");
            ticket.Attributes["status"].Value = "New";
            XmlElement node = doc.CreateElement("Note");
            node.SetAttribute("datetime", DateTime.Now.ToString("u"));
            node.SetAttribute("username", HttpContext.Current.User.Identity.Name);
            if (string.IsNullOrEmpty(Note)) node.InnerXml = "<![CDATA[No Note Information Added]]>";
            else node.InnerXml = "<![CDATA[" + HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />") + "]]>";
            ticket.AppendChild(node);

            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();

            if (hapConfig.Current.SMTP.Enabled)
            {
                MailMessage mes = new MailMessage();

                mes.Subject = "Ticket (#" + Id + ") has been Updated";
                mes.From = mes.Sender = new MailAddress(ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].Email, ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName);
                mes.ReplyToList.Add(mes.From);
                mes.To.Add(new MailAddress(hapConfig.Current.SMTP.FromEmail, hapConfig.Current.SMTP.FromUser));

                mes.IsBodyHtml = true;

                FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newusernote.htm"));
                StreamReader fs = template.OpenText();

                mes.Body = fs.ReadToEnd().Replace("{0}", Id).Replace("{1}",
                    HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />")).Replace("{2}",
                    ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName).Replace("{3}",
                    HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath);

                SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                    smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                smtp.Send(mes);
            }
            return new FullTicket(doc.SelectSingleNode("/Tickets/Ticket[@id='" + Id + "']"));
        }

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/AdminTicket/{Id}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public FullTicket UpdateAdminTicket(string Id, string Note, string State, string Priority, string ShowTo, string FAQ)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            XmlNode ticket = doc.SelectSingleNode("/Tickets/Ticket[@id='" + Id + "']");
            ticket.Attributes["status"].Value = "New";
            XmlElement node = doc.CreateElement("Note");
            node.SetAttribute("datetime", DateTime.Now.ToString("u"));
            node.SetAttribute("username", HttpContext.Current.User.Identity.Name);
            if (string.IsNullOrEmpty(Note)) node.InnerXml = "<![CDATA[No Note Information Added]]>";
            else node.InnerXml = "<![CDATA[" + HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />") + "]]>";
            ticket.AppendChild(node);

            ticket.Attributes["status"].Value = State;
            ticket.Attributes["priority"].Value = Priority;
            if (ticket.Attributes["showto"] == null) ticket.Attributes.Append(doc.CreateAttribute("showto"));
            ticket.Attributes["showto"].Value = ShowTo;
            if (ticket.Attributes["faq"] == null) ticket.Attributes.Append(doc.CreateAttribute("faq"));
            ticket.Attributes["faq"].Value = string.IsNullOrWhiteSpace(FAQ) ? "false" : FAQ;

            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();

            UserInfo user = ADUtils.FindUserInfos(ticket.SelectNodes("Note")[0].Attributes["username"].Value)[0];
            UserInfo currentuser = ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0];
            if (hapConfig.Current.SMTP.Enabled && user.Email != null && !string.IsNullOrEmpty(user.Email))
            {
                MailMessage mes = new MailMessage();
                mes.Subject = "Your Ticket (#" + Id + ") has been " + (State == "Fixed" ? "Closed" : "Updated");
                mes.From = mes.Sender = new MailAddress(currentuser.Email, currentuser.DisplayName);
                mes.ReplyToList.Add(mes.From);

                mes.To.Add(new MailAddress(user.Email, user.DisplayName));

                mes.IsBodyHtml = true;

                FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newadminnote.htm"));
                StreamReader fs = template.OpenText();

                mes.Body = fs.ReadToEnd().Replace("{0}", Id).Replace("{1}",
                    (State == "Fixed" ? "Closed" : "Updated")).Replace("{2}",
                    HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />")).Replace("{3}",
                    (State == "Fixed" ? "reopen" : "update")).Replace("{4}",
                    HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath).Replace("{5}", HttpContext.Current.User.Identity.Name).Replace("{6}", user.DisplayName).Replace("{7}", currentuser.DisplayName);

                SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                    smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                smtp.Send(mes);
            }
            if (hapConfig.Current.SMTP.Enabled && !string.IsNullOrWhiteSpace(ShowTo))
                foreach (string s in ShowTo.Split(new char[] { ',' }))
                {

                    MailMessage mes = new MailMessage();

                    mes.Subject = "Ticket (#" + Id + ") has been Updated";
                    mes.From = new MailAddress(hapConfig.Current.SMTP.FromEmail, hapConfig.Current.SMTP.FromUser);
                    mes.Sender = mes.From;
                    mes.ReplyToList.Add(mes.From);

                    mes.To.Add(new MailAddress(ADUtils.FindUserInfos(s)[0].Email, ADUtils.FindUserInfos(s)[0].DisplayName));

                    mes.IsBodyHtml = true;
                    FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newadminnote.htm"));
                    StreamReader fs = template.OpenText();

                    mes.Body = fs.ReadToEnd().Replace("{0}", Id).Replace("{1}",
                        (State == "Fixed" ? "Closed" : "Updated")).Replace("{2}",
                        HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />")).Replace("{3}",
                        (State == "Fixed" ? "reopen" : "update")).Replace("{4}",
                        HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath);

                    SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                    if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                        smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                    smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                    smtp.Send(mes);
                }
            return new FullTicket(doc.SelectSingleNode("/Tickets/Ticket[@id='" + Id + "']"));
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Ticket", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public FullTicket FileTicket(string Subject, string Room, string Note)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            int x;
            if (doc.SelectSingleNode("/Tickets").ChildNodes.Count > 0)
            {
                XmlNodeList tickets = doc.SelectNodes("/Tickets/Ticket");
                x = int.Parse(tickets[tickets.Count - 1].Attributes["id"].Value) + 1;
            }
            else x = 1;
            XmlElement ticket = doc.CreateElement("Ticket");
            ticket.SetAttribute("id", x.ToString());
            ticket.SetAttribute("subject", HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default));
            ticket.SetAttribute("priority", "Normal");
            ticket.SetAttribute("status", "New");
            XmlElement node = doc.CreateElement("Note");
            node.SetAttribute("datetime", DateTime.Now.ToUniversalTime().ToString("u"));
            node.SetAttribute("username", HttpContext.Current.User.Identity.Name);
            node.InnerXml = "<![CDATA[Room: " + Room + "<br />\n" + HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />") + "]]>";
            ticket.AppendChild(node);
            doc.SelectSingleNode("/Tickets").AppendChild(ticket);

            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();

            if (hapConfig.Current.SMTP.Enabled)
            {
                MailMessage mes = new MailMessage();

                mes.Subject = "A Ticket (#" + x + ") has been Created";
                mes.From = new MailAddress(ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].Email, ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName);
                mes.Sender = mes.From;
                mes.ReplyToList.Add(mes.From);

                mes.To.Add(new MailAddress(hapConfig.Current.SMTP.FromEmail, hapConfig.Current.SMTP.FromUser));

                mes.IsBodyHtml = true;
                FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newuserticket.htm"));
                StreamReader fs = template.OpenText();
                mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}",
                    HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default)).Replace("{2}",
                    HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />")).Replace("{3}",
                    Room).Replace("{4}",
                    ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName).Replace("{5}",
                    HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath);

                SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                    smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                smtp.Send(mes);
            }
            return new FullTicket(doc.SelectSingleNode("/Tickets/Ticket[@id='" + x + "']"));
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AdminTicket", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public FullTicket FileAdminTicket(string Subject, string Room, string Note, string ShowTo, string Priority, string User)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            int x;
            if (doc.SelectSingleNode("/Tickets").ChildNodes.Count > 0)
            {
                XmlNodeList tickets = doc.SelectNodes("/Tickets/Ticket");
                x = int.Parse(tickets[tickets.Count - 1].Attributes["id"].Value) + 1;
            }
            else x = 1;
            XmlElement ticket = doc.CreateElement("Ticket");
            ticket.SetAttribute("id", x.ToString());
            ticket.SetAttribute("subject", HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default));
            ticket.SetAttribute("priority", Priority == "" ? "Normal": Priority);
            ticket.SetAttribute("status", "New");
            XmlElement node = doc.CreateElement("Note");
            node.SetAttribute("datetime", DateTime.Now.ToUniversalTime().ToString("u"));
            node.SetAttribute("username", User);
            node.InnerXml = "<![CDATA[Room: " + Room + "\n\n" + HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />") + "]]>";
            ticket.AppendChild(node);
            doc.SelectSingleNode("/Tickets").AppendChild(ticket);

            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();

            if (hapConfig.Current.SMTP.Enabled && ADUtils.FindUserInfos(User)[0].Email != null && !string.IsNullOrEmpty(ADUtils.FindUserInfos(User)[0].Email))
            {
                MailMessage mes = new MailMessage();

                mes.Subject = "A Support Ticket (#" + x + ") has been Logged";

                mes.From = mes.Sender = new MailAddress(ADUtils.FindUserInfos(User)[0].Email, ADUtils.FindUserInfos(User)[0].DisplayName);
                mes.ReplyToList.Add(mes.From);

                mes.To.Add(new MailAddress(ADUtils.FindUserInfos(User)[0].Email, ADUtils.FindUserInfos(User)[0].DisplayName));

                mes.IsBodyHtml = true;

                FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newadminticket.htm"));
                StreamReader fs = template.OpenText();
                mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}",
                    HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default)).Replace("{2}",
                    HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />")).Replace("{3}",
                    ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName).Replace("{4}",
                    HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath);

                SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                    smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                smtp.Send(mes);
            }
            if (hapConfig.Current.SMTP.Enabled && !string.IsNullOrWhiteSpace(ShowTo))
                foreach (string s in ShowTo.Split( new char[] {','}))
                {
                
                    MailMessage mes = new MailMessage();

                    mes.Subject = "A Ticket (#" + x + ") has been Created";
                    mes.From = new MailAddress(hapConfig.Current.SMTP.FromEmail, hapConfig.Current.SMTP.FromUser);
                    mes.Sender = mes.From;
                    mes.ReplyToList.Add(mes.From);

                    mes.To.Add(new MailAddress(ADUtils.FindUserInfos(s)[0].Email, ADUtils.FindUserInfos(s)[0].DisplayName));

                    mes.IsBodyHtml = true;
                    FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newuserticket.htm"));
                    StreamReader fs = template.OpenText();
                    mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}",
                        HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default)).Replace("{2}",
                        HttpUtility.UrlDecode(Note, System.Text.Encoding.Default)).Replace("{3}",
                        Room).Replace("{4}",
                        ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName).Replace("{5}",
                        HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath);

                    SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                    if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                        smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                    smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                    smtp.Send(mes);
                }
            return new FullTicket(doc.SelectSingleNode("/Tickets/Ticket[@id='" + x + "']"));
        }

        [OperationContract]
        [WebGet(UriTemplate = "Tickets/{State}", ResponseFormat = WebMessageFormat.Json)]
        public Ticket[] AllTickets(string State)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            List<Ticket> tickets = new List<Ticket>();
            string xpath = string.Format("/Tickets/Ticket[@status{0}]", State == "Open" ? "!='Fixed'" : "='Fixed'");
            foreach (XmlNode node in doc.SelectNodes(xpath))
                tickets.Add(new Ticket(node));
            return tickets.ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate="Tickets/{State}/{Username}", ResponseFormat=WebMessageFormat.Json)]
        public Ticket[] Tickets(string State, string Username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            List<Ticket> tickets = new List<Ticket>();
            string xpath = string.Format("/Tickets/Ticket[@status{0}]", State == "Open" ? "!='Fixed'" : "='Fixed'");

            foreach (XmlNode node in doc.SelectNodes(xpath))
                if (node.SelectNodes("Note")[0].Attributes["username"].Value.ToLower() == Username.ToLower() || (node.Attributes["showto"] != null && contains(node.Attributes["showto"].Value, Username)))
                    tickets.Add(new Ticket(node));
            return tickets.ToArray();
        }

        private bool contains(string a, string b)
        {
            foreach (string s in a.Split(new char[] { ',' }))
                if (s.ToLower().Trim().Equals(b.ToLower())) return true;
            return false;
        }

        [OperationContract]
        [WebGet(UriTemplate = "Ticket/{TicketId}", ResponseFormat = WebMessageFormat.Json)]
        public FullTicket Ticket(string TicketId)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            return new FullTicket(doc.SelectSingleNode("/Tickets/Ticket[@id='" + TicketId + "']"));
        }

        [OperationContract]
        [WebGet(UriTemplate = "FAQs")]
        public Ticket[] FAQs()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            List<Ticket> tickets = new List<Ticket>();
            foreach (XmlNode node in doc.SelectNodes("/Tickets/Ticket[@faq='true']"))
                tickets.Add(new Ticket(node));
            return tickets.ToArray();
        }
    }
}