using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using HAP.AD;
using Microsoft.Exchange.WebServices.Data;
using System.Security.Principal;
using System.Web.Security;
using HAP.Web.Configuration;
using System.Text.RegularExpressions;

namespace HAP.Web.LiveTiles
{
    public class ExchangeConnector
    {
        public static int Unread()
        {
            ServicePointManager.ServerCertificateValidationCallback = 
            delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                // Replace this line with code to validate server certificate.
                return true;
            };
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange))
                service.AutodiscoverUrl(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, (string redirectionUrl) => { return true; });
            else service.Url = new Uri("https://" + HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange + "/ews/exchange.asmx");
            HAP.AD.User u = ((HAP.AD.User)Membership.GetUser());
            if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == Configuration.AuthMode.Forms && string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser))
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                service.Credentials = new NetworkCredential((hapConfig.Current.SMTP.EWSUseEmailoverAN ? u.Email : u.UserName), TokenGenerator.ConvertToPlain(token.Value), HAP.Web.Configuration.hapConfig.Current.AD.UPN);
            }
            else
            {
                if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser))
                {
                    u.ImpersonateContained();
                    service.UseDefaultCredentials = true;
                    //service.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                else
                {
                    if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain))
                        service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword);
                    else
                        service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain);

                    service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, u.Email);
                }
            }
            
            Folder inbox = Folder.Bind(service, WellKnownFolderName.Inbox);
            SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));
            ItemView view = new ItemView(10);
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, sf, view);
            if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == Configuration.AuthMode.Windows || !string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser))
                u.EndContainedImpersonate();
            return findResults.TotalCount;
        }

        public static string[] Appointments()
        {
            ServicePointManager.ServerCertificateValidationCallback =
            delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                // Replace this line with code to validate server certificate.
                return true;
            };
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange))
                service.AutodiscoverUrl(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, (string redirectionUrl) => { return true; });
            else service.Url = new Uri("https://" + HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange + "/ews/exchange.asmx");
            HAP.AD.User u = ((HAP.AD.User)Membership.GetUser());
            if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == Configuration.AuthMode.Forms &&  string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser))
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                service.Credentials = new NetworkCredential((hapConfig.Current.SMTP.EWSUseEmailoverAN ? u.Email : u.UserName), TokenGenerator.ConvertToPlain(token.Value), HAP.Web.Configuration.hapConfig.Current.AD.UPN);
            }
            else
            {
                if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser))
                {
                    u.ImpersonateContained();
                    service.UseDefaultCredentials = true;
                    //service.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                else
                {
                    if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain))
                        service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword);
                    else
                        service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain);

                    service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, u.Email);
                }
            }
            List<string> s = new List<string>();
            foreach (Appointment a in service.FindAppointments(WellKnownFolderName.Calendar, new CalendarView(DateTime.Now, DateTime.Now.AddDays(1))))
                s.Add(a.Subject + "<br />" + (a.Start.Date > DateTime.Now.Date ? "Tomorrow: " : "") + ((a.Start.AddDays(1) == a.End) ? "All Day" : a.Start.AddDays(5) == a.End ? "All Week" : (a.Start.ToShortTimeString() + " - " + a.End.ToShortTimeString())));
            if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == Configuration.AuthMode.Windows || !string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser)) 
                u.EndContainedImpersonate();
            return s.ToArray();
        }

        public static Appointment[] Appointments(DateTime From, DateTime To)
        {
            ServicePointManager.ServerCertificateValidationCallback =
            delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                // Replace this line with code to validate server certificate.
                return true;
            };
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange))
                service.AutodiscoverUrl(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, (string redirectionUrl) => { return true; });
            else service.Url = new Uri("https://" + HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange + "/ews/exchange.asmx");
            HAP.AD.User u = ((HAP.AD.User)Membership.GetUser());
            if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == Configuration.AuthMode.Forms && string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser))
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                service.Credentials = new NetworkCredential((hapConfig.Current.SMTP.EWSUseEmailoverAN ? u.Email : u.UserName), TokenGenerator.ConvertToPlain(token.Value), HAP.Web.Configuration.hapConfig.Current.AD.UPN);
            }
            else
            {
                if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser))
                {
                    u.ImpersonateContained();
                    service.UseDefaultCredentials = true;
                    //service.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                else
                {
                    if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain))
                        service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword);
                    else
                        service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain);

                    service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, u.Email);
                }
            }
            Appointment[] app = service.FindAppointments(WellKnownFolderName.Calendar, new CalendarView(From, To.AddDays(1))).ToArray();
            if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == Configuration.AuthMode.Windows || !string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser))
                u.EndContainedImpersonate();
            return app;
        }

        public static Appointment[] Appointments(DateTime From, DateTime To, string mailbox)
        {
            ServicePointManager.ServerCertificateValidationCallback =
            delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                // Replace this line with code to validate server certificate.
                return true;
            };
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange))
                service.AutodiscoverUrl(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, (string redirectionUrl) => { return true; });
            else service.Url = new Uri("https://" + HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange + "/ews/exchange.asmx");

            service.Credentials = new NetworkCredential(HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password, HAP.Web.Configuration.hapConfig.Current.AD.UPN);
            FolderId fid = new FolderId(WellKnownFolderName.Calendar, new Mailbox(mailbox));
            return service.FindAppointments(fid, new CalendarView(From, To.AddDays(1))).ToArray();
        }

        public static string[] Appointments(string mailbox)
        {
            ServicePointManager.ServerCertificateValidationCallback =
            delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                // Replace this line with code to validate server certificate.
                return true;
            };
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange))
                service.AutodiscoverUrl(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, (string redirectionUrl) => { return true; });
            else service.Url = new Uri("https://" + HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange + "/ews/exchange.asmx");

            if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain))
                service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword);
            else
                service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain);

            FolderId fid = new FolderId(WellKnownFolderName.Calendar, new Mailbox(mailbox));
            List<string> s = new List<string>();
            foreach (Appointment a in service.FindAppointments(fid, new CalendarView(DateTime.Now, DateTime.Now.AddDays(1))))
                s.Add(a.Subject + "<br />" + (a.Start.Date > DateTime.Now.Date ? "Tomorrow: " : "") + ((a.Start.AddDays(1) == a.End) ? "All Day" : a.Start.AddDays(5) == a.End ? "All Week" : (a.Start.ToShortTimeString() + " - " + a.End.ToShortTimeString())));
            return s.ToArray();
        }

        public static EWSAppointmentInfo[] AppointmentsInfoWeek(string mailbox)
        {
            ServicePointManager.ServerCertificateValidationCallback =
            delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                // Replace this line with code to validate server certificate.
                return true;
            };
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange))
                service.AutodiscoverUrl(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, (string redirectionUrl) => { return true; });
            else service.Url = new Uri("https://" + HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange + "/ews/exchange.asmx");

            if (string.IsNullOrEmpty(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain))
                service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword);
            else
                service.Credentials = new WebCredentials(HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationUser, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationPassword, HAP.Web.Configuration.hapConfig.Current.SMTP.ImpersonationDomain);

            FolderId fid = new FolderId(WellKnownFolderName.Calendar, new Mailbox(mailbox));
            List<EWSAppointmentInfo> s = new List<EWSAppointmentInfo>();
            foreach (Appointment a in service.FindAppointments(fid, new CalendarView(DateTime.Now, DateTime.Now.AddDays(14))))
            {
                s.Add(new EWSAppointmentInfo { Start = a.Start.ToString(), End = a.End.ToString(), Location = a.Location, Subject = a.Subject });
                try
                {
                    a.Load(); 
                    string b = a.Body.Text;
                    Regex r = new Regex("<html>(?:.|\n|\r)+?<body>", RegexOptions.IgnoreCase);
                    b = r.Replace(b, "");
                    b = new Regex("</body>(?:.|\n|\r)+?</html>(?:.|\n|\r)+?", RegexOptions.IgnoreCase).Replace(b, "");
                    s.Last().Body = b;
                }
                catch { }
            }
            return s.ToArray();
        }
    }

    public class EWSAppointmentInfo
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string End { get; set; }
        public string Start { get; set; }
        public string Location { get; set; }
    }
}
