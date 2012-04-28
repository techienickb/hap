﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using HAP.AD;
using Microsoft.Exchange.WebServices.Data;

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

            HttpCookie token = HttpContext.Current.Request.Cookies["token"];
            if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
 
            ExchangeService service = new ExchangeService();
            service.Url = new Uri("https://" + HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange + "/ews/exchange.asmx");
            service.Credentials = new NetworkCredential(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value), HAP.Web.Configuration.hapConfig.Current.AD.UPN);
            Folder inbox = Folder.Bind(service, WellKnownFolderName.Inbox);
            SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));
            ItemView view = new ItemView(10);
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, sf, view);
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

            HttpCookie token = HttpContext.Current.Request.Cookies["token"];
            if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");

            ExchangeService service = new ExchangeService();
            service.Url = new Uri("https://" + HAP.Web.Configuration.hapConfig.Current.SMTP.Exchange + "/ews/exchange.asmx");
            service.Credentials = new NetworkCredential(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value), HAP.Web.Configuration.hapConfig.Current.AD.UPN);
            List<string> s = new List<string>();
            foreach (Appointment a in service.FindAppointments(WellKnownFolderName.Calendar, new CalendarView(DateTime.Now, DateTime.Now.AddDays(1))))
                s.Add(a.Subject + "<br />" + (a.Start.Date > DateTime.Now.Date ? "Tomorrow: " : "") + a.Start.ToShortTimeString() + " - " + a.End.ToShortTimeString());
            return s.ToArray();
        }
    }
}
