using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using HAP.Web.Configuration;
using System.Configuration;
using System.Net.Mail;
using System.Globalization;
using System.Net.Mime;
using System.Net;
using System.DirectoryServices.AccountManagement;
using HAP.Data.BookingSystem;

namespace HAP.Web.BookingSystem
{
    public class iCalGenerator
    {
        public static string DateFormat
        {
            get { return "yyyyMMddTHHmmssZ"; /* 20060215T092000Z */ }
        }

        public static void Generate(Booking booking, DateTime date)
        {
            hapConfig config = hapConfig.Current;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, int.Parse(config.BookingSystem.Lessons[booking.Lesson].StartTime.Substring(0, config.BookingSystem.Lessons[booking.Lesson].StartTime.IndexOf(':'))), int.Parse(config.BookingSystem.Lessons[booking.Lesson].StartTime.Substring(config.BookingSystem.Lessons[booking.Lesson].StartTime.IndexOf(':') + 1, 2)), 0);
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, int.Parse(config.BookingSystem.Lessons[booking.Lesson].EndTime.Substring(0, config.BookingSystem.Lessons[booking.Lesson].EndTime.IndexOf(':'))), int.Parse(config.BookingSystem.Lessons[booking.Lesson].EndTime.Substring(config.BookingSystem.Lessons[booking.Lesson].EndTime.IndexOf(':') + 1, 2)), 0);
            string location = "";
            bookingResource resource = config.BookingSystem.Resources[booking.Room];

            Templates t = new Templates();
            Template template = t["general"];
            if (t.ContainsKey(resource.Name)) template = t[resource.Name];
            string ltcount = "";
            if (resource.ResourceType == ResourceType.ITRoom) location = booking.Room;
            else if (resource.ResourceType == ResourceType.Laptops) { location = booking.LTRoom; ltcount = booking.LTCount.ToString(); }
            else if (resource.ResourceType == ResourceType.Equipment) location = booking.EquipRoom;
            
            string summary = string.Format(template.Subject, booking.Username, booking.User.DisplayName, booking.Room, booking.Name, booking.Date.ToShortDateString(), booking.Day, booking.Lesson, location, ltcount);
            string description = string.Format(template.Content, booking.Username, booking.User.DisplayName, booking.Room, booking.Name, booking.Date.ToShortDateString(), booking.Day, booking.Lesson, location, ltcount);

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//chsit/CalendarAppointment");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:REQUEST");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("DTSTART:" + startDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("DTEND:" + endDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("ATTENDEE;ROLE=REQ-PARTICIPANT;CN=" + booking.User.DisplayName + ":MAILTO:" + booking.User.EmailAddress);
            sb.AppendLine("ORGANIZER;CN=" + booking.User.DisplayName + ":MAILTO:" + booking.User.EmailAddress);
            sb.AppendLine("LOCATION:" + location);
            sb.AppendLine("UID:" + booking.uid);
            sb.AppendLine("DTSTAMP:" + DateTime.Now.ToString(DateFormat));
            sb.AppendLine("SUMMARY:" + summary);
            sb.AppendLine("DESCRIPTION:" + description);
            sb.AppendLine("BEGIN:VALARM");
            sb.AppendLine("ACTION:DISPLAY");
            sb.AppendLine("DESCRIPTION:" + description);
            sb.AppendLine("TRIGGER:-PT5M");
            sb.AppendLine("END:VALARM");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("~/BookingSystem/ITBooking.ics"));
            if (file.Exists) file.Delete();
            StreamWriter sr = file.CreateText();
            sr.Write(sb.ToString());
            sr.Flush();
            sr.Close();
            sr.Dispose();

            MailMessage mes = new MailMessage();
            IFormatProvider culture = new CultureInfo("en-gb");
            mes.Subject = summary;
            mes.From = mes.Sender = new MailAddress(config.BaseSettings.AdminEmailAddress, "ICT Department");
            mes.ReplyToList.Add(mes.From);
            mes.To.Add(new MailAddress(booking.User.EmailAddress, booking.User.DisplayName));

            mes.Body = description;

            AlternateView av = AlternateView.CreateAlternateViewFromString(sb.ToString(), new ContentType("text/calendar; method=REQUEST; name=ITBooking.ics"));
            av.TransferEncoding = TransferEncoding.SevenBit;
            mes.AlternateViews.Add(av);

            //mes.Attachments.Add(new Attachment(file.FullName, "text/calendar; method=REQUEST; name=ITBooking.ics"));
            SmtpClient client = new SmtpClient(config.BaseSettings.SMTPServer);
            if (!string.IsNullOrEmpty(config.BaseSettings.SMTPServerUsername))
                client.Credentials = new NetworkCredential(config.BaseSettings.SMTPServerUsername, config.BaseSettings.SMTPServerPassword);
            client.EnableSsl = config.BaseSettings.SMTPServerSSL;
            client.Port = config.BaseSettings.SMTPServerPort;
            client.Send(mes);
        }

        public static void Generate(Booking booking, DateTime date, string username)
        {
            hapConfig config = hapConfig.Current;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, int.Parse(config.BookingSystem.Lessons[booking.Lesson].StartTime.Substring(0, config.BookingSystem.Lessons[booking.Lesson].StartTime.IndexOf(':'))), int.Parse(config.BookingSystem.Lessons[booking.Lesson].StartTime.Substring(config.BookingSystem.Lessons[booking.Lesson].StartTime.IndexOf(':') + 1, 2)), 0);
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, int.Parse(config.BookingSystem.Lessons[booking.Lesson].EndTime.Substring(0, config.BookingSystem.Lessons[booking.Lesson].EndTime.IndexOf(':'))), int.Parse(config.BookingSystem.Lessons[booking.Lesson].EndTime.Substring(config.BookingSystem.Lessons[booking.Lesson].EndTime.IndexOf(':') + 1, 2)), 0);
            string location = "";
            bookingResource resource = config.BookingSystem.Resources[booking.Room];
            Templates t = new Templates();
            Template template = t["generaladmin"];
            if (t.ContainsKey(resource.Name)) template = t[resource.Name + "admin"];
            string ltcount = "";
            if (resource.ResourceType == ResourceType.ITRoom) location = booking.Room;
            else if (resource.ResourceType == ResourceType.Laptops) { location = booking.LTRoom; ltcount = booking.LTCount.ToString(); }
            else if (resource.ResourceType == ResourceType.Equipment) location = booking.EquipRoom;
            
            string summary = string.Format(template.Subject, booking.Username, booking.User.DisplayName, booking.Room, booking.Name, booking.Date.ToShortDateString(), booking.Day, booking.Lesson, location, ltcount);
            string description = string.Format(template.Content, booking.Username, booking.User.DisplayName, booking.Room, booking.Name, booking.Date.ToShortDateString(), booking.Day, booking.Lesson, location, ltcount);

            List<UserPrincipal> ups = new List<UserPrincipal>();

            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            string _DomainDN = connObj.ConnectionString.Remove(0, connObj.ConnectionString.IndexOf("DC="));
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            foreach (string user in resource.Admins.Split(new string[] { ", "}, StringSplitOptions.RemoveEmptyEntries))
                if (user == "Inherit") ups.Add(UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, username));
                else ups.Add(UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, user));

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//chsit/CalendarAppointment");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:REQUEST");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("DTSTART:" + startDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("DTEND:" + endDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("ORGANIZER;CN=" + booking.User.DisplayName + ":MAILTO:" + booking.User.EmailAddress);
            sb.AppendLine("ATTENDEE;ROLE=REQ-PARTICIPANT;CN=" + booking.User.DisplayName + ":MAILTO:" + booking.User.EmailAddress);
            foreach (UserPrincipal up in ups)
                sb.AppendLine("ATTENDEE;ROLE=REQ-PARTICIPANT;CN=" + up.DisplayName + ":MAILTO:" + up.EmailAddress);
            sb.AppendLine("LOCATION:" + location);
            sb.AppendLine("UID:" + booking.uid);
            sb.AppendLine("DTSTAMP:" + DateTime.Now.ToString(DateFormat));
            sb.AppendLine("SUMMARY:" + summary);
            sb.AppendLine("DESCRIPTION:" + description);
            sb.AppendLine("BEGIN:VALARM");
            sb.AppendLine("ACTION:DISPLAY");
            sb.AppendLine("DESCRIPTION:" + summary);
            sb.AppendLine("TRIGGER:-PT5M");
            sb.AppendLine("END:VALARM");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("~/BookingSystem/ITBooking.ics"));
            if (file.Exists) file.Delete();
            StreamWriter sr = file.CreateText();
            sr.Write(sb.ToString());
            sr.Flush();
            sr.Close();
            sr.Dispose();

            MailMessage mes = new MailMessage();
            IFormatProvider culture = new CultureInfo("en-gb");
            mes.Subject = summary;
            mes.From = mes.Sender = new MailAddress(config.BaseSettings.AdminEmailAddress, "ICT Department");
            mes.ReplyToList.Add(mes.From);
            foreach (UserPrincipal up in ups)
                mes.To.Add(new MailAddress(up.EmailAddress, up.DisplayName));

            mes.Body = description;

            AlternateView av = AlternateView.CreateAlternateViewFromString(sb.ToString(), new ContentType("text/calendar; method=REQUEST; name=ITBooking.ics"));
            av.TransferEncoding = TransferEncoding.SevenBit;
            mes.AlternateViews.Add(av);

            //mes.Attachments.Add(new Attachment(file.FullName, "text/calendar; method=REQUEST; name=ITBooking.ics"));
            SmtpClient client = new SmtpClient(config.BaseSettings.SMTPServer);
            if (!string.IsNullOrEmpty(config.BaseSettings.SMTPServerUsername))
                client.Credentials = new NetworkCredential(config.BaseSettings.SMTPServerUsername, config.BaseSettings.SMTPServerPassword);
            client.EnableSsl = config.BaseSettings.SMTPServerSSL;
            client.Port = config.BaseSettings.SMTPServerPort;
            client.Send(mes);
        }

        public static void GenerateCancel(Booking booking, DateTime date)
        {
            hapConfig config = hapConfig.Current;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, int.Parse(config.BookingSystem.Lessons[booking.Lesson].StartTime.Substring(0, config.BookingSystem.Lessons[booking.Lesson].StartTime.IndexOf(':'))), int.Parse(config.BookingSystem.Lessons[booking.Lesson].StartTime.Substring(config.BookingSystem.Lessons[booking.Lesson].StartTime.IndexOf(':') + 1, 2)), 0);
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, int.Parse(config.BookingSystem.Lessons[booking.Lesson].EndTime.Substring(0, config.BookingSystem.Lessons[booking.Lesson].EndTime.IndexOf(':'))), int.Parse(config.BookingSystem.Lessons[booking.Lesson].EndTime.Substring(config.BookingSystem.Lessons[booking.Lesson].EndTime.IndexOf(':') + 1, 2)), 0);
            string location = "";
            bookingResource resource = config.BookingSystem.Resources[booking.Room];
            if (resource.ResourceType == ResourceType.ITRoom) location = booking.Room;
            else if (resource.ResourceType == ResourceType.Laptops) location = booking.LTRoom;
            else if (resource.ResourceType == ResourceType.Equipment) location = booking.EquipRoom;
            string summary = "Cancellation of " + booking.Name + " in " + location;
            string description = "Cancellation of " + booking.Name + " in " + location + " during " + booking.Lesson + " on " + booking.Date.ToShortDateString();
            if (resource.ResourceType == ResourceType.Laptops)
            {
                summary += " with the " + booking.Room + " [" + booking.LTCount.ToString() + "]";
                description += " with the " + booking.Room + " [" + booking.LTCount.ToString() + "]";
            }

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//chsit/CalendarAppointment");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:CANCEL");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("DTSTART:" + startDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("DTEND:" + endDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("ORGANIZER:MAILTO:" + booking.User.EmailAddress);
            sb.AppendLine("LOCATION:" + location);
            sb.AppendLine("UID:" + booking.uid);
            sb.AppendLine("DTSTAMP:" + DateTime.Now.ToString(DateFormat));
            sb.AppendLine("SUMMARY:" + summary);
            sb.AppendLine("DESCRIPTION:" + description);
            sb.AppendLine("STATUS:CANCELLED");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("~/BookingSystem/ITBooking.ics"));
            if (file.Exists) file.Delete();
            StreamWriter sr = file.CreateText();
            sr.Write(sb.ToString());
            sr.Flush();
            sr.Close();
            sr.Dispose();

            MailMessage mes = new MailMessage();
            IFormatProvider culture = new CultureInfo("en-gb");
            mes.Subject = summary;
            mes.From = mes.Sender = new MailAddress(config.BaseSettings.AdminEmailAddress, "ICT Department");
            mes.ReplyToList.Add(mes.From);
            mes.To.Add(new MailAddress(booking.User.EmailAddress, booking.User.DisplayName));

            mes.Body = description;

            AlternateView av = AlternateView.CreateAlternateViewFromString(sb.ToString(), new ContentType("text/calendar; method=CANCEL; name=ITBooking.ics"));
            av.TransferEncoding = TransferEncoding.SevenBit;
            mes.AlternateViews.Add(av);

            //mes.Attachments.Add(new Attachment(file.FullName, "text/calendar; method=REQUEST; name=ITBooking.ics"));
            SmtpClient client = new SmtpClient(config.BaseSettings.SMTPServer);
            if (!string.IsNullOrEmpty(config.BaseSettings.SMTPServerUsername))
                client.Credentials = new NetworkCredential(config.BaseSettings.SMTPServerUsername, config.BaseSettings.SMTPServerPassword);
            client.EnableSsl = config.BaseSettings.SMTPServerSSL;
            client.Port = config.BaseSettings.SMTPServerPort;
            client.Send(mes);
        }

        public static void GenerateCancel(Booking booking, DateTime date, string username)
        {
            hapConfig config = hapConfig.Current;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, int.Parse(config.BookingSystem.Lessons[booking.Lesson].StartTime.Substring(0, config.BookingSystem.Lessons[booking.Lesson].StartTime.IndexOf(':'))), int.Parse(config.BookingSystem.Lessons[booking.Lesson].StartTime.Substring(config.BookingSystem.Lessons[booking.Lesson].StartTime.IndexOf(':') + 1, 2)), 0);
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, int.Parse(config.BookingSystem.Lessons[booking.Lesson].EndTime.Substring(0, config.BookingSystem.Lessons[booking.Lesson].EndTime.IndexOf(':'))), int.Parse(config.BookingSystem.Lessons[booking.Lesson].EndTime.Substring(config.BookingSystem.Lessons[booking.Lesson].EndTime.IndexOf(':') + 1, 2)), 0);
            string location = "";
            bookingResource resource = config.BookingSystem.Resources[booking.Room];
            if (resource.ResourceType == ResourceType.ITRoom) location = booking.Room;
            else if (resource.ResourceType == ResourceType.Laptops) location = booking.LTRoom;
            else if (resource.ResourceType == ResourceType.Equipment) location = booking.EquipRoom;
            string summary = "Cancellation of " + booking.Name + " in " + location;
            string description = "Cancellation of " + booking.Name + " in " + location + " during " + booking.Lesson + " on " + booking.Date.ToShortDateString();
            if (resource.ResourceType == ResourceType.Laptops)
            {
                summary += " with the " + booking.Room + " [" + booking.LTCount.ToString() + "]";
                description += " with the " + booking.Room + " [" + booking.LTCount.ToString() + "]";
            }

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//chsit/CalendarAppointment");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:CANCEL");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("DTSTART:" + startDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("DTEND:" + endDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("ORGANIZER:MAILTO:" + booking.User.EmailAddress);
            sb.AppendLine("LOCATION:" + location);
            sb.AppendLine("UID:" + booking.uid);
            sb.AppendLine("DTSTAMP:" + DateTime.Now.ToString(DateFormat));
            sb.AppendLine("SUMMARY:" + summary);
            sb.AppendLine("STATUS:CANCELLED");
            sb.AppendLine("DESCRIPTION:" + description);
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("~/BookingSystem/ITBooking.ics"));
            if (file.Exists) file.Delete();
            StreamWriter sr = file.CreateText();
            sr.Write(sb.ToString());
            sr.Flush();
            sr.Close();
            sr.Dispose();

            MailMessage mes = new MailMessage();
            IFormatProvider culture = new CultureInfo("en-gb");
            mes.Subject = summary;
            mes.From = mes.Sender = new MailAddress(config.BaseSettings.AdminEmailAddress, "ICT Department");
            mes.ReplyToList.Add(mes.From);

            List<UserPrincipal> ups = new List<UserPrincipal>();

            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            string _DomainDN = connObj.ConnectionString.Remove(0, connObj.ConnectionString.IndexOf("DC="));
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            foreach (string user in resource.Admins.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                if (user == "Inherit") ups.Add(UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, username));
                else ups.Add(UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, user));

            foreach (UserPrincipal up in ups)
                mes.To.Add(new MailAddress(up.EmailAddress, up.DisplayName));

            mes.Body = description;

            AlternateView av = AlternateView.CreateAlternateViewFromString(sb.ToString(), new ContentType("text/calendar; method=CANCEL; name=ITBooking.ics"));
            av.TransferEncoding = TransferEncoding.SevenBit;
            mes.AlternateViews.Add(av);

            //mes.Attachments.Add(new Attachment(file.FullName, "text/calendar; method=REQUEST; name=ITBooking.ics"));
            SmtpClient client = new SmtpClient(config.BaseSettings.SMTPServer);
            if (!string.IsNullOrEmpty(config.BaseSettings.SMTPServerUsername))
                client.Credentials = new NetworkCredential(config.BaseSettings.SMTPServerUsername, config.BaseSettings.SMTPServerPassword);
            client.EnableSsl = config.BaseSettings.SMTPServerSSL;
            client.Port = config.BaseSettings.SMTPServerPort;
            client.Send(mes);
        }
    }
}