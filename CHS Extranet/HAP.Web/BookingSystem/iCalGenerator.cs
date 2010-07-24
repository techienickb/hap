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
            if (resource.ResourceType == ResourceType.ITRoom) location = booking.Room;
            else if (resource.ResourceType == ResourceType.Laptops) location = booking.LTRoom;
            string summary = booking.Name + " in " + location;
            string description = booking.Name + " in " + location + " during " + booking.Lesson + " on Day " + booking.Day;
            if (resource.ResourceType == ResourceType.Laptops)
            {
                summary += " with the " + booking.Room + " [" + booking.LTCount.ToString() + "]";
                description += " with the " + booking.Room + " [" + booking.LTCount.ToString() + "]";
            }

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//chsit/CalendarAppointment");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("DTSTART:" + startDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("DTEND:" + endDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("ORGANIZER:MAILTO:" + booking.User.EmailAddress);
            sb.AppendLine("LOCATION:" + location);
            sb.AppendLine("UID:" + booking.Username + startDate.ToString(DateFormat));
            sb.AppendLine("DTSTAMP:" + DateTime.Now.ToString(DateFormat));
            sb.AppendLine("SUMMARY:" + summary);
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

        public static void Generate(Booking booking, DateTime date, String username)
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
            string summary = booking.Name + " in " + location;
            string description = booking.Name + " in " + location + " during " + booking.Lesson + " on Day " + booking.Day;
            if (resource.ResourceType == ResourceType.Laptops)
            {
                summary += " with the " + booking.Room + " [" + booking.LTCount.ToString() + "]";
                description += " with the " + booking.Room + " [" + booking.LTCount.ToString() + "]";
            }

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//chsit/CalendarAppointment");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("DTSTART:" + startDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("DTEND:" + endDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("ORGANIZER:MAILTO:" + booking.User.EmailAddress);
            sb.AppendLine("LOCATION:" + location);
            sb.AppendLine("UID:" + booking.Username + startDate.ToString(DateFormat));
            sb.AppendLine("DTSTAMP:" + DateTime.Now.ToString(DateFormat));
            sb.AppendLine("SUMMARY:" + summary);
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

            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            string _DomainDN = connObj.ConnectionString.Remove(0, connObj.ConnectionString.IndexOf("DC="));
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            UserPrincipal up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, username);

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
    }
}