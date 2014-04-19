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
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading;
using System.Configuration;

namespace HAP.HelpDesk
{
    [ServiceAPI("api/helpdesk")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class API
    {
        [OperationContract]
        [WebGet(UriTemplate="/Permalink/{Id}")]
        public void Permalink(string Id)
        {
            HttpContext.Current.Response.Redirect("~/HelpDesk/#ticket-" + Id);
        }

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Ticket/{Id}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public FullTicket UpdateTicket(string Id, string Note, string State, string Priority, string ShowTo, string FAQ, string Subject, string AssignTo)
        {
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "HelpDesk.Update", HttpContext.Current.User.Identity.Name, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Updating Ticket " + Id);

            if (hapConfig.Current.HelpDesk.Provider.ToLower() == "xml")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
                XmlElement ticket = doc.SelectSingleNode("/Tickets/Ticket[@id='" + Id + "']") as XmlElement;
                ticket.SetAttribute("status", "New");
                if (!string.IsNullOrEmpty(State)) ticket.Attributes["status"].Value = State;
                ticket.SetAttribute("readby", HttpContext.Current.User.Identity.Name);
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
            }
            else
            {
                HAP.Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.HelpDesk.Provider].ConnectionString);
                Data.SQL.Ticket tick = sql.Tickets.Single(t => t.Id == int.Parse(Id));
                tick.Status = string.IsNullOrEmpty(State) ? "New" : State;
                tick.ReadBy = HttpContext.Current.User.Identity.Name;
                if (string.IsNullOrEmpty(Note)) Note = string.IsNullOrEmpty(AssignTo) ? "No Note Information Added" : "Assigned to: " + AssignTo;
                else Note = HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />");
                tick.Notes.Add(new Data.SQL.Note { DateTime = DateTime.Now, Hide = false, Username = HttpContext.Current.User.Identity.Name, Content = Note });
                sql.SubmitChanges();
            }
            if (hapConfig.Current.SMTP.Enabled)
            {
                MailMessage mes = new MailMessage();

                mes.Subject = Localizable.Localize("helpdesk/ticketupdated").Replace("#", "#" + Id);
                try
                {
                    mes.From = mes.Sender = new MailAddress(ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].Email, ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName);
                }
                catch { mes.From = new MailAddress(hapConfig.Current.SMTP.FromEmail, hapConfig.Current.SMTP.FromUser); }
                mes.ReplyToList.Add(mes.From);
                foreach (string s in hapConfig.Current.HelpDesk.FirstLineEmails.Split(new char[] { ',' }))
                    mes.To.Add(new MailAddress(s.Trim()));

                mes.IsBodyHtml = true;

                FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newusernote.htm"));
                StreamReader fs = template.OpenText();

                mes.Body = fs.ReadToEnd().Replace("{0}", Id).Replace("{1}",
                    HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />")).Replace("{2}",
                    ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName).Replace("{3}",
                    HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath);
                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                    smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                smtp.Send(mes);
            }
            return Ticket("", Id);
        }

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/AdminTicket/{Id}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public FullTicket UpdateAdminTicket(string Id, string Note, string State, string Priority, string ShowTo, string FAQ, string Subject, string AssignTo, bool HideNote)
        {
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "HelpDesk.UpdateAdmin", HttpContext.Current.User.Identity.Name, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Updating Admin Ticket " + Id);


            if (hapConfig.Current.HelpDesk.Provider.ToLower() == "xml")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
                XmlNode ticket = doc.SelectSingleNode("/Tickets/Ticket[@id='" + Id + "']");

                if (ticket.Attributes["assignedto"] != null && ticket.Attributes["assignedto"].Value.ToLower() == AssignTo.ToLower()) AssignTo = "";

                if (!string.IsNullOrEmpty(Subject)) ticket.Attributes["subject"].Value = Subject;
                XmlElement node = doc.CreateElement("Note");
                node.SetAttribute("datetime", DateTime.Now.ToString("u"));
                node.SetAttribute("username", HttpContext.Current.User.Identity.Name);
                node.SetAttribute("hide", HideNote.ToString());
                if (string.IsNullOrEmpty(Note)) node.InnerXml = string.IsNullOrEmpty(AssignTo) ? "<![CDATA[No Note Information Added]]>" : "<![CDATA[Assigned to: " + AssignTo + "]]>";
                else node.InnerXml = "<![CDATA[" + HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />") + "]]>";
                ticket.AppendChild(node);

                if (!string.IsNullOrEmpty(State)) ticket.Attributes["status"].Value = State;
                if (node.Attributes["assignedto"] == null) ticket.Attributes.Append(doc.CreateAttribute("assignedto"));
                ticket.Attributes["assignedto"].Value = string.IsNullOrEmpty(AssignTo) ? HttpContext.Current.User.Identity.Name : AssignTo;
                ticket.Attributes["priority"].Value = string.IsNullOrEmpty(Priority) ? ticket.Attributes["priority"].Value : Priority;
                if (ticket.Attributes["showto"] == null) ticket.Attributes.Append(doc.CreateAttribute("showto"));
                if (!string.IsNullOrEmpty(ShowTo)) ticket.Attributes["showto"].Value = ShowTo;
                if (ticket.Attributes["faq"] == null) ticket.Attributes.Append(doc.CreateAttribute("faq"));
                ticket.Attributes["faq"].Value = string.IsNullOrWhiteSpace(FAQ) ? "false" : FAQ;

                ((XmlElement)ticket).SetAttribute("readby", HttpContext.Current.User.Identity.Name);

                XmlWriterSettings set = new XmlWriterSettings();
                set.Indent = true;
                set.IndentChars = "   ";
                set.Encoding = System.Text.Encoding.UTF8;
                XmlWriter writer = XmlWriter.Create(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"), set);
                doc.Save(writer);
                writer.Flush();
                writer.Close();
            }
            else
            {
                HAP.Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.HelpDesk.Provider].ConnectionString);
                Data.SQL.Ticket tick = sql.Tickets.Single(t => t.Id == int.Parse(Id));
                if (tick.AssignedTo.ToLower() == AssignTo.ToLower()) AssignTo = "";
                if (!string.IsNullOrEmpty(Subject)) tick.Title = Subject;
                if (!string.IsNullOrEmpty(State)) tick.Status = State;
                tick.AssignedTo = string.IsNullOrEmpty(AssignTo) ? HttpContext.Current.User.Identity.Name : AssignTo;
                tick.Priority = string.IsNullOrEmpty(Priority) ? tick.Priority : Priority;
                if (!string.IsNullOrEmpty(ShowTo)) tick.ShowTo = ShowTo;
                tick.Faq = string.IsNullOrWhiteSpace(FAQ) ? false : bool.Parse(FAQ);

                if (string.IsNullOrEmpty(Note)) Note = string.IsNullOrEmpty(AssignTo) ? "No Note Information Added" : "Assigned to: " + AssignTo;
                else Note = HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />");
                tick.Notes.Add(new Data.SQL.Note { DateTime = DateTime.Now, Hide = HideNote, Username = HttpContext.Current.User.Identity.Name, Content = Note });
                sql.SubmitChanges();
            }

            string emailnote = "";
            FullTicket ft = Ticket("", Id);
            foreach (Note not in ft.Notes.Where(n => !n.Hide))
                emailnote += not.DisplayName + " on " + not.Date.ToString() + "<br />" + HttpUtility.UrlDecode(not.NoteText, System.Text.Encoding.Default).Replace("\n", "<br />") + "<hr />";
            

            UserInfo user = ADUtils.FindUserInfos(ft.Notes[0].Username)[0];
            UserInfo currentuser = ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0];
            if (hapConfig.Current.SMTP.Enabled && user.Email != null && !string.IsNullOrEmpty(user.Email) && !HideNote)
            {
                MailMessage mes = new MailMessage();
                mes.Subject = Localizable.Localize("helpdesk/tickedhasbeen").Replace("#", "#" + Id).Replace("%", !isOpen(State) ? Localizable.Localize("helpdesk/closed") : Localizable.Localize("helpdesk/updated"));
                mes.From = mes.Sender = new MailAddress(currentuser.Email, currentuser.DisplayName);
                mes.ReplyToList.Add(mes.From);

                mes.To.Add(new MailAddress(user.Email, user.DisplayName));

                mes.IsBodyHtml = true;

                FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newadminnote.htm"));
                StreamReader fs = template.OpenText();

                mes.Body = fs.ReadToEnd().Replace("{0}", Id).Replace("{1}", (!isOpen(State) ? Localizable.Localize("helpdesk/closed") : Localizable.Localize("helpdesk/updated"))).Replace("{2}", emailnote).Replace("{3}", (!isOpen(State) ? Localizable.Localize("helpdesk/reopen") : Localizable.Localize("helpdesk/update"))).Replace("{4}", HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath).Replace("{5}", currentuser.UserName).Replace("{6}", AssignTo == "" ? "" : ADUtils.FindUserInfos(AssignTo)[0].DisplayName).Replace("{7}", currentuser.DisplayName).Replace("{8}", ft.Subject);
                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                    smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                smtp.Send(mes);
            }
            if (hapConfig.Current.SMTP.Enabled && !string.IsNullOrWhiteSpace(ShowTo) && !HideNote)
                foreach (string s in ShowTo.Split(new char[] { ',' }))
                {

                    MailMessage mes = new MailMessage();

                    mes.Subject = Localizable.Localize("helpdesk/ticketupdated").Replace("#", "#" + Id);
                    mes.From = new MailAddress(hapConfig.Current.SMTP.FromEmail, hapConfig.Current.SMTP.FromUser);
                    mes.Sender = mes.From;
                    mes.ReplyToList.Add(mes.From);

                    mes.To.Add(new MailAddress(ADUtils.FindUserInfos(s)[0].Email, ADUtils.FindUserInfos(s)[0].DisplayName));

                    mes.IsBodyHtml = true;
                    FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newadminnote.htm"));
                    StreamReader fs = template.OpenText();

                    mes.Body = fs.ReadToEnd().Replace("{0}", Id).Replace("{1}", (!isOpen(State) ? Localizable.Localize("helpdesk/closed") : Localizable.Localize("helpdesk/updated"))).Replace("{2}", emailnote).Replace("{3}", (!isOpen(State) ? Localizable.Localize("helpdesk/reopen") : Localizable.Localize("helpdesk/update"))).Replace("{4}", HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath).Replace("{5}", currentuser.UserName).Replace("{6}", AssignTo == "" ? "" : ADUtils.FindUserInfos(AssignTo)[0].DisplayName).Replace("{7}", currentuser.DisplayName).Replace("{8}", ft.Subject);
                    ServicePointManager.ServerCertificateValidationCallback = delegate(object s1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                    if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                        smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                    smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                    smtp.Send(mes);
                }
            if (hapConfig.Current.SMTP.Enabled && !string.IsNullOrEmpty(AssignTo) && AssignTo.ToLower() != HttpContext.Current.User.Identity.Name.ToLower())
            {
                MailMessage mes = new MailMessage();

                mes.Subject = Localizable.Localize("helpdesk/assignticket").Replace("#", "#" + Id);
                mes.From = new MailAddress(hapConfig.Current.SMTP.FromEmail, hapConfig.Current.SMTP.FromUser);
                mes.Sender = mes.From;
                mes.ReplyToList.Add(mes.From);

                mes.To.Add(new MailAddress(ADUtils.FindUserInfos(AssignTo)[0].Email, ADUtils.FindUserInfos(AssignTo)[0].DisplayName));

                mes.IsBodyHtml = true;
                FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newassignticket.htm"));
                StreamReader fs = template.OpenText();

                mes.Body = fs.ReadToEnd().Replace("{0}", Id).Replace("{1}", (!isOpen(State) ? Localizable.Localize("helpdesk/closed") : Localizable.Localize("helpdesk/updated"))).Replace("{2}", emailnote).Replace("{3}", (!isOpen(State) ? Localizable.Localize("helpdesk/reopen") : Localizable.Localize("helpdesk/update"))).Replace("{4}", HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath).Replace("{5}", currentuser.UserName).Replace("{6}", AssignTo == "" ? "" : ADUtils.FindUserInfos(AssignTo)[0].DisplayName).Replace("{7}", currentuser.DisplayName).Replace("{8}", ft.Subject);
                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                    smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                smtp.Send(mes);
            }
            return Ticket("", Id);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Ticket", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public FullTicket FileTicket(string Subject, string Room, string Note)
        {
            int x;
            if (hapConfig.Current.HelpDesk.Provider.ToLower() == "xml")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
                if (doc.SelectSingleNode("/Tickets").ChildNodes.Count > 0)
                {
                    XmlNodeList tickets = doc.SelectNodes("/Tickets/Ticket");
                    x = int.Parse(tickets[tickets.Count - 1].Attributes["id"].Value) + 1;
                }
                else x = 1;
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "HelpDesk.New", HttpContext.Current.User.Identity.Name, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Creating Ticket " + x + " (" + Subject + ")");
                XmlElement ticket = doc.CreateElement("Ticket");
                ticket.SetAttribute("id", x.ToString());
                ticket.SetAttribute("subject", HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default));
                ticket.SetAttribute("priority", "Normal");
                ticket.SetAttribute("status", "New");
                ticket.SetAttribute("readby", HttpContext.Current.User.Identity.Name);
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
            }
            else
            {
                HAP.Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.HelpDesk.Provider].ConnectionString);
                Data.SQL.Ticket tick = new Data.SQL.Ticket { Archive = "", Faq = false, Status = "New", Priority = "Normal", AssignedTo = "", HideAssignedTo = false, ShowTo = "", ReadBy = HttpContext.Current.User.Identity.Name, Title = HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default) };
                tick.Notes.Add(new Data.SQL.Note { DateTime = DateTime.Now, Hide = false, Username = HttpContext.Current.User.Identity.Name, Content = "Room: " + Room + "\n\n" + HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />") });
                sql.Tickets.InsertOnSubmit(tick);
                sql.SubmitChanges();
                x = tick.Id;
            }

            if (hapConfig.Current.SMTP.Enabled)
            {
                MailMessage mes = new MailMessage();

                mes.Subject = Localizable.Localize("helpdesk/ticketcreated").Replace("#", "#" + x);
                try
                {
                    mes.From = new MailAddress(ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].Email, ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName);
                }
                catch { mes.From = new MailAddress(hapConfig.Current.SMTP.FromEmail, hapConfig.Current.SMTP.FromUser); }
                mes.Sender = mes.From;
                mes.ReplyToList.Add(mes.From);

                foreach (string s in hapConfig.Current.HelpDesk.FirstLineEmails.Split(new char[] { ',' }))
                    mes.To.Add(new MailAddress(s.Trim()));

                mes.IsBodyHtml = true;
                FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath("~/HelpDesk/newuserticket.htm"));
                StreamReader fs = template.OpenText();
                mes.Body = fs.ReadToEnd().Replace("{0}", x.ToString()).Replace("{1}",
                    HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default)).Replace("{2}",
                    HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />")).Replace("{3}",
                    Room).Replace("{4}",
                    ADUtils.FindUserInfos(HttpContext.Current.User.Identity.Name)[0].DisplayName).Replace("{5}",
                    HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath);
                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                    smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                smtp.Send(mes);
            }
            return Ticket("", x.ToString());
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AdminTicket", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public FullTicket FileAdminTicket(string Subject, string Room, string Note, string ShowTo, string Priority, string User)
        {
            int x = 0;
            if (hapConfig.Current.HelpDesk.Provider.ToLower() == "xml")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
                if (doc.SelectSingleNode("/Tickets").ChildNodes.Count > 0)
                {
                    XmlNodeList tickets = doc.SelectNodes("/Tickets/Ticket");
                    x = int.Parse(tickets[tickets.Count - 1].Attributes["id"].Value) + 1;
                }
                else x = 1;
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "HelpDesk.NewAdmin", HttpContext.Current.User.Identity.Name, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Creating Ticket " + x + " (" + Subject + ")");
                XmlElement ticket = doc.CreateElement("Ticket");
                ticket.SetAttribute("id", x.ToString());
                ticket.SetAttribute("subject", HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default));
                ticket.SetAttribute("priority", Priority == "" ? "Normal" : Priority);
                ticket.SetAttribute("status", "New");
                ticket.SetAttribute("readby", HttpContext.Current.User.Identity.Name);
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
            }
            else
            {
                HAP.Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.HelpDesk.Provider].ConnectionString);
                Data.SQL.Ticket tick = new Data.SQL.Ticket { Archive = "", Faq = false, Status = "New", AssignedTo = "", HideAssignedTo = false, ShowTo = ShowTo, ReadBy = HttpContext.Current.User.Identity.Name, Title = HttpUtility.UrlDecode(Subject, System.Text.Encoding.Default) };
                tick.Priority = Priority == "" ? "Normal" : Priority;
                tick.Notes.Add(new Data.SQL.Note { DateTime = DateTime.Now, Hide = false, Username = HttpContext.Current.User.Identity.Name, Content = "Room: " + Room + "\n\n" + HttpUtility.UrlDecode(Note, System.Text.Encoding.Default).Replace("\n", "<br />") });
                sql.Tickets.InsertOnSubmit(tick);
                sql.SubmitChanges();
                x = tick.Id;
            }

            if (hapConfig.Current.SMTP.Enabled && ADUtils.FindUserInfos(User)[0].Email != null && !string.IsNullOrEmpty(ADUtils.FindUserInfos(User)[0].Email))
            {
                MailMessage mes = new MailMessage();

                mes.Subject = Localizable.Localize("helpdesk/ticketlogged").Replace("#", "#" + x);

                try
                {
                    mes.From = mes.Sender = new MailAddress(ADUtils.FindUserInfos(User)[0].Email, ADUtils.FindUserInfos(User)[0].DisplayName);
                }
                catch { mes.From = new MailAddress(hapConfig.Current.SMTP.FromEmail, hapConfig.Current.SMTP.FromUser); }
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
                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                    smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                smtp.Send(mes);
            }
            if (hapConfig.Current.SMTP.Enabled && !string.IsNullOrWhiteSpace(ShowTo))
                foreach (string s in ShowTo.Split( new char[] {','}))
                {
                    try
                    {

                        MailMessage mes = new MailMessage();

                        mes.Subject = Localizable.Localize("helpdesk/ticketcreated").Replace("#", "#" + x);
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
                        ServicePointManager.ServerCertificateValidationCallback = delegate(object s1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        SmtpClient smtp = new SmtpClient(hapConfig.Current.SMTP.Server, hapConfig.Current.SMTP.Port);
                        if (!string.IsNullOrEmpty(hapConfig.Current.SMTP.User))
                            smtp.Credentials = new NetworkCredential(hapConfig.Current.SMTP.User, hapConfig.Current.SMTP.Password);
                        smtp.EnableSsl = hapConfig.Current.SMTP.SSL;
                        smtp.Send(mes);
                    }
                    catch { }
                }
            return Ticket("", x.ToString());
        }

        [OperationContract]
        [WebGet(UriTemplate = "Tickets/{State}", ResponseFormat = WebMessageFormat.Json)]
        public Ticket[] AllShortTickets(string State)
        {
            return AllTickets("", State);
        }

        public bool isOpen(string state)
        {
            foreach (string s in hapConfig.Current.HelpDesk.UserOpenStates.Split(new char[] { ',' }))
                if (state == s.Trim()) return true;
            foreach (string s in hapConfig.Current.HelpDesk.OpenStates.Split(new char[] { ',' }))
                if (state == s.Trim()) return true;
            return false;
        }

        [OperationContract]
        [WebGet(UriTemplate = "ATickets/{Archive}/{State}", ResponseFormat = WebMessageFormat.Json)]
        public Ticket[] AllTickets(string Archive, string State)
        {
            List<Ticket> tickets = new List<Ticket>();
            if (hapConfig.Current.HelpDesk.Provider.ToLower() == "xml")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets" + Archive + ".xml"));
                string xpath = string.Format("/Tickets/Ticket");
                foreach (XmlNode node in doc.SelectNodes(xpath))
                {
                    Ticket t = new Ticket(node);
                    bool add = false;
                    if (State == "Open")
                    {
                        foreach (string s in hapConfig.Current.HelpDesk.UserOpenStates.Split(new char[] { ',' }))
                            if (t.Status == s.Trim()) { add = true; break; }
                        if (!add) foreach (string s in hapConfig.Current.HelpDesk.OpenStates.Split(new char[] { ',' }))
                                if (t.Status == s.Trim()) { add = true; break; }
                    }
                    else
                    {
                        foreach (string s in hapConfig.Current.HelpDesk.UserClosedStates.Split(new char[] { ',' }))
                            if (t.Status == s.Trim()) { add = true; break; }
                        if (!add) foreach (string s in hapConfig.Current.HelpDesk.ClosedStates.Split(new char[] { ',' }))
                                if (t.Status == s.Trim()) { add = true; break; }
                    }
                    if (add) tickets.Add(t);
                }
            }
            else
            {
                HAP.Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.HelpDesk.Provider].ConnectionString);
                foreach (HAP.Data.SQL.Ticket tick in sql.Tickets.Where(t => t.Archive == Archive))
                {
                    Ticket t = new Ticket(tick);
                    bool add = false;
                    if (State == "Open")
                    {
                        foreach (string s in hapConfig.Current.HelpDesk.UserOpenStates.Split(new char[] { ',' }))
                            if (t.Status == s.Trim()) { add = true; break; }
                        if (!add) foreach (string s in hapConfig.Current.HelpDesk.OpenStates.Split(new char[] { ',' }))
                                if (t.Status == s.Trim()) { add = true; break; }
                    }
                    else
                    {
                        foreach (string s in hapConfig.Current.HelpDesk.UserClosedStates.Split(new char[] { ',' }))
                            if (t.Status == s.Trim()) { add = true; break; }
                        if (!add) foreach (string s in hapConfig.Current.HelpDesk.ClosedStates.Split(new char[] { ',' }))
                                if (t.Status == s.Trim()) { add = true; break; }
                    }
                    if (add) tickets.Add(t);
                }
            }
            return tickets.ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate="Tickets/{State}/{Username}", ResponseFormat=WebMessageFormat.Json)]
        public Ticket[] ShortTickets(string State, string Username)
        {
            return Tickets("", State, Username);
        }

        [OperationContract]
        [WebGet(UriTemplate = "ATickets/{Archive}/{State}/{Username}", ResponseFormat = WebMessageFormat.Json)]
        public Ticket[] Tickets(string Archive, string State, string Username)
        {
            List<Ticket> tickets = new List<Ticket>();
            tickets.AddRange(AllTickets(Archive, State).Where(t => t.Username.ToLower() == Username.ToLower() || t.AssignedTo.ToLower().Contains(Username.ToLower()) || t.ShowTo.ToLower().Contains(Username.ToLower())));
            if (State != "Open") tickets.Reverse();
            return tickets.ToArray();
        }

        private bool contains(string a, string b)
        {
            foreach (string s in a.Split(new char[] { ',' }))
                if (s.ToLower().Trim().Equals(b.ToLower())) return true;
            return false;
        }

        [OperationContract]
        [WebGet(UriTemplate = "ATicket/{Archive}/{TicketId}", ResponseFormat = WebMessageFormat.Json)]
        public FullTicket Ticket(string Archive, string TicketId)
        {
            if (hapConfig.Current.HelpDesk.Provider.ToLower() == "xml")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets" + Archive + ".xml"));
                XmlElement ticket = doc.SelectSingleNode("/Tickets/Ticket[@id='" + TicketId + "']") as XmlElement;
                FullTicket ft = new FullTicket(ticket);
                bool needed = true;
                if (ticket.HasAttribute("readby"))
                {
                    string[] s = ticket.GetAttribute("readby").Split(new char[] { ',' });
                    foreach (string a in s)
                        if (a.Trim().ToLower() == HttpContext.Current.User.Identity.Name.ToLower()) needed = false;
                    if (needed)
                    {
                        List<string> s1 = new List<string>();
                        s1.AddRange(s);
                        s1.Add(HttpContext.Current.User.Identity.Name);
                        ticket.SetAttribute("readby", string.Join(", ", s1.ToArray()));
                    }
                }
                else
                {
                    ticket.SetAttribute("readby", HttpContext.Current.User.Identity.Name);
                }
                if (needed) doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/Tickets" + Archive + ".xml"));
                return ft;
            }
            else
            {
                HAP.Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.HelpDesk.Provider].ConnectionString);
                Data.SQL.Ticket tick = sql.Tickets.Single(t => t.Archive == Archive && t.Id == int.Parse(TicketId));
                FullTicket ft = new FullTicket(tick);
                string[] s = tick.ReadBy.Split(new char[] { ',' });
                bool needed = true;
                foreach (string a in s)
                    if (a.Trim().ToLower() == HttpContext.Current.User.Identity.Name.ToLower()) needed = false;
                if (needed)
                {
                    List<string> s1 = new List<string>();
                    s1.AddRange(s);
                    s1.Add(HttpContext.Current.User.Identity.Name);
                    tick.ReadBy = string.Join(", ", s1.ToArray());
                    sql.SubmitChanges();
                }
                return ft;
            }
        }

        [OperationContract]
        [WebGet(UriTemplate = "Ticket/{TicketId}", ResponseFormat = WebMessageFormat.Json)]
        public FullTicket ShortTicket(string TicketId)
        {
            return Ticket("", TicketId);
        }

        [OperationContract]
        [WebGet(UriTemplate = "FAQs", ResponseFormat = WebMessageFormat.Json)]
        public Ticket[] FAQs()
        {
            List<Ticket> tickets = new List<Ticket>();
            if (hapConfig.Current.HelpDesk.Provider.ToLower() == "xml")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
                foreach (XmlNode node in doc.SelectNodes("/Tickets/Ticket[@faq='true']"))
                    tickets.Add(new Ticket(node));
            }
            else
            {
                HAP.Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.HelpDesk.Provider].ConnectionString);
                foreach (Data.SQL.Ticket tick in sql.Tickets.Where(t => t.Faq == true))
                    tickets.Add(new Ticket(tick));
            }
            return tickets.ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate = "Stats", ResponseFormat = WebMessageFormat.Json)]
        public Stats Stats()
        {
            return PeriodStats("7");
        }

        [OperationContract]
        [WebGet(UriTemplate = "Stats/{Period}", ResponseFormat = WebMessageFormat.Json)]
        public Stats PeriodStats(string Period)
        {
            int p = int.Parse("-" + Period);
            Stats s = new Stats();
            Dictionary<string, int> highusers = new Dictionary<string, int>();
            List<FullTicket> fulltickets = new List<FullTicket>();
            XmlDocument doc = new XmlDocument();
            s.NewTickets = s.ClosedTickets = s.OpenTickets = 0;
            if (hapConfig.Current.HelpDesk.Provider.ToLower() == "xml")
            {
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
                foreach (XmlNode node in doc.SelectNodes("/Tickets/Ticket"))
                {
                    fulltickets.Add(new FullTicket(node));
                }
            }
            else
            {
                HAP.Data.SQL.sql2linqDataContext sql = new Data.SQL.sql2linqDataContext(ConfigurationManager.ConnectionStrings[hapConfig.Current.HelpDesk.Provider].ConnectionString);
                foreach (Data.SQL.Ticket tick in sql.Tickets.Where(t => t.Archive != ""))
                    fulltickets.Add(new FullTicket(tick));
            }
            foreach (FullTicket tick in fulltickets)
            {
                if (DateTime.Parse(tick.Date).Date >= DateTime.Now.AddDays(p).Date && DateTime.Parse(tick.Date).Date <= DateTime.Now.Date)
                {
                    if (highusers.ContainsKey(tick.Username)) highusers[tick.Username]++;
                    else highusers.Add(tick.Username, 1);
                    s.NewTickets++;
                }
                if (DateTime.Parse(tick.Notes[tick.Notes.Count - 1].Date).Date >= DateTime.Now.AddDays(p).Date && DateTime.Parse(tick.Notes[tick.Notes.Count - 1].Date).Date <= DateTime.Now.Date && !isOpen(tick.Status))
                {
                    s.ClosedTickets++;
                }
                if (isOpen(tick.Status)) s.OpenTickets++;
            }
            if (highusers.Keys.Count > 0)
            {
                var a = highusers.OrderByDescending(h => h.Value).First();
                s.HighestUser = new UserStats();
                s.HighestUser.Username = a.Key;
                s.HighestUser.Tickets = a.Value;
            }
            return s;
        }
    }
}