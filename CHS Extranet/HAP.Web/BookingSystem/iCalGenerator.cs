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
using HAP.AD;

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
            if (string.IsNullOrEmpty(booking.User.Email)) return;
            hapConfig config = hapConfig.Current;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, config.BookingSystem.Lessons.Get(booking.Lesson).StartTime.Hour, config.BookingSystem.Lessons.Get(booking.Lesson).StartTime.Minute, 0);
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, config.BookingSystem.Lessons.Get(booking.Lesson).EndTime.Hour, config.BookingSystem.Lessons.Get(booking.Lesson).EndTime.Minute, 0);
            string location = "";
            Resource resource = config.BookingSystem.Resources[booking.Room];

            Templates t = new Templates();
            Template template = t["general"];
            if (t.ContainsKey(resource.Name)) template = t[resource.Name];
            string ltcount = "";
            if (resource.Type == ResourceType.Room) location = booking.Room;
            else if (resource.Type == ResourceType.Laptops) { location = booking.LTRoom; ltcount = booking.LTCount.ToString(); }
            else if (resource.Type == ResourceType.Equipment) location = booking.EquipRoom;
            
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
            sb.AppendLine("ATTENDEE;ROLE=REQ-PARTICIPANT;CN=" + booking.User.DisplayName + ":MAILTO:" + booking.User.Email);
            sb.AppendLine("ORGANIZER;CN=" + booking.User.DisplayName + ":MAILTO:" + booking.User.Email);
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

            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("~/App_Data/ITBooking.ics"));
            if (file.Exists) file.Delete();
            StreamWriter sr = file.CreateText();
            sr.Write(sb.ToString());
            sr.Flush();
            sr.Close();
            sr.Dispose();

            MailMessage mes = new MailMessage();
            IFormatProvider culture = new CultureInfo("en-gb");
            mes.Subject = summary;
            mes.From = mes.Sender = new MailAddress(config.SMTP.FromEmail, config.SMTP.FromUser);
            mes.ReplyToList.Add(mes.From);
            mes.To.Add(new MailAddress(booking.User.Email, booking.User.DisplayName));

            mes.Body = description;

            AlternateView av = AlternateView.CreateAlternateViewFromString(sb.ToString(), new ContentType("text/calendar; method=REQUEST; name=ITBooking.ics"));
            av.TransferEncoding = TransferEncoding.SevenBit;
            mes.AlternateViews.Add(av);

            //mes.Attachments.Add(new Attachment(file.FullName, "text/calendar; method=REQUEST; name=ITBooking.ics"));
            SmtpClient client = new SmtpClient(config.SMTP.Server);
            if (!string.IsNullOrEmpty(config.SMTP.User))
                client.Credentials = new NetworkCredential(config.SMTP.User, config.SMTP.Password);
            client.EnableSsl = config.SMTP.SSL;
            client.Port = config.SMTP.Port;
            client.Send(mes);
        }

        public static void Generate(Booking booking, DateTime date, bool emailadmins)
        {
            hapConfig config = hapConfig.Current;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, config.BookingSystem.Lessons.Get(booking.Lesson).StartTime.Hour, config.BookingSystem.Lessons.Get(booking.Lesson).StartTime.Minute, 0);
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, config.BookingSystem.Lessons.Get(booking.Lesson).EndTime.Hour, config.BookingSystem.Lessons.Get(booking.Lesson).EndTime.Minute, 0);
            string location = "";
            Resource resource = config.BookingSystem.Resources[booking.Room];
            Templates t = new Templates();
            Template template = t["generaladmin"];
            if (t.ContainsKey(resource.Name)) template = t[resource.Name + "admin"];
            string ltcount = "";
            if (resource.Type == ResourceType.Room) location = booking.Room;
            else if (resource.Type == ResourceType.Laptops) { location = booking.LTRoom; ltcount = booking.LTCount.ToString(); }
            else if (resource.Type == ResourceType.Equipment) location = booking.EquipRoom;
            
            string summary = string.Format(template.Subject, booking.Username, booking.User.DisplayName, booking.Room, booking.Name, booking.Date.ToShortDateString(), booking.Day, booking.Lesson, location, ltcount);
            string description = string.Format(template.Content, booking.Username, booking.User.DisplayName, booking.Room, booking.Name, booking.Date.ToShortDateString(), booking.Day, booking.Lesson, location, ltcount);

            List<UserInfo> uis = new List<UserInfo>();

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//chsit/CalendarAppointment");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:REQUEST");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("DTSTART:" + startDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("DTEND:" + endDate.ToUniversalTime().ToString(DateFormat));
            sb.AppendLine("ORGANIZER;CN=" + booking.User.DisplayName + ":MAILTO:" + booking.User.Email);
            sb.AppendLine("ATTENDEE;ROLE=REQ-PARTICIPANT;CN=" + booking.User.DisplayName + ":MAILTO:" + booking.User.Email);
            foreach (string s in hapConfig.Current.BookingSystem.Resources[booking.Room].Admins.Split(new char[] { ',' }))
            {
                UserInfo ui = ADUtils.FindUserInfos(s)[0];
                uis.Add(ui);
                if (!string.IsNullOrEmpty(ui.Email))
                    sb.AppendLine("ATTENDEE;ROLE=REQ-PARTICIPANT;CN=" + ui.DisplayName + ":MAILTO:" + ui.Email);
            }
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

            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("~/App_Data/ITBooking.ics"));
            if (file.Exists) file.Delete();
            StreamWriter sr = file.CreateText();
            sr.Write(sb.ToString());
            sr.Flush();
            sr.Close();
            sr.Dispose();

            MailMessage mes = new MailMessage();
            IFormatProvider culture = new CultureInfo("en-gb");
            mes.Subject = summary;
            mes.From = mes.Sender = new MailAddress(config.SMTP.FromEmail, config.SMTP.FromUser);
            mes.ReplyToList.Add(mes.From);
            foreach (UserInfo u1 in uis.Where(u => !string.IsNullOrEmpty(u.Email)))
                mes.To.Add(new MailAddress(u1.Email, u1.DisplayName));

            mes.Body = description;

            AlternateView av = AlternateView.CreateAlternateViewFromString(sb.ToString(), new ContentType("text/calendar; method=REQUEST; name=ITBooking.ics"));
            av.TransferEncoding = TransferEncoding.SevenBit;
            mes.AlternateViews.Add(av);

            //mes.Attachments.Add(new Attachment(file.FullName, "text/calendar; method=REQUEST; name=ITBooking.ics"));
            SmtpClient client = new SmtpClient(config.SMTP.Server);
            if (!string.IsNullOrEmpty(config.SMTP.User))
                client.Credentials = new NetworkCredential(config.SMTP.User, config.SMTP.Password);
            client.EnableSsl = config.SMTP.SSL;
            client.Port = config.SMTP.Port;
            client.Send(mes);
        }

        public static void GenerateCancel(Booking booking, DateTime date)
        {
            if (string.IsNullOrEmpty(booking.User.Email)) return;
            hapConfig config = hapConfig.Current;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, config.BookingSystem.Lessons.Get(booking.Lesson).StartTime.Hour, config.BookingSystem.Lessons.Get(booking.Lesson).StartTime.Minute, 0);
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, config.BookingSystem.Lessons.Get(booking.Lesson).EndTime.Hour, config.BookingSystem.Lessons.Get(booking.Lesson).EndTime.Minute, 0);
            string location = "";
            Resource resource = config.BookingSystem.Resources[booking.Room];
            if (resource.Type == ResourceType.Room) location = booking.Room;
            else if (resource.Type == ResourceType.Laptops) location = booking.LTRoom;
            else if (resource.Type == ResourceType.Equipment) location = booking.EquipRoom;
            string summary = "Cancellation of " + booking.Name + " in " + location;
            string description = "Cancellation of " + booking.Name + " in " + location + " during " + booking.Lesson + " on " + booking.Date.ToShortDateString();
            if (resource.Type == ResourceType.Laptops)
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
            sb.AppendLine("ORGANIZER:MAILTO:" + booking.User.Email);
            sb.AppendLine("LOCATION:" + location);
            sb.AppendLine("UID:" + booking.uid);
            sb.AppendLine("DTSTAMP:" + DateTime.Now.ToString(DateFormat));
            sb.AppendLine("SUMMARY:" + summary);
            sb.AppendLine("DESCRIPTION:" + description);
            sb.AppendLine("STATUS:CANCELLED");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("~/App_Data/ITBooking.ics"));
            if (file.Exists) file.Delete();
            StreamWriter sr = file.CreateText();
            sr.Write(sb.ToString());
            sr.Flush();
            sr.Close();
            sr.Dispose();

            MailMessage mes = new MailMessage();
            IFormatProvider culture = new CultureInfo("en-gb");
            mes.Subject = summary;
            mes.From = mes.Sender = new MailAddress(config.SMTP.FromEmail, config.SMTP.FromUser);
            mes.ReplyToList.Add(mes.From);
            mes.To.Add(new MailAddress(booking.User.Email, booking.User.DisplayName));

            mes.Body = description;

            AlternateView av = AlternateView.CreateAlternateViewFromString(sb.ToString(), new ContentType("text/calendar; method=CANCEL; name=ITBooking.ics"));
            av.TransferEncoding = TransferEncoding.SevenBit;
            mes.AlternateViews.Add(av);

            //mes.Attachments.Add(new Attachment(file.FullName, "text/calendar; method=REQUEST; name=ITBooking.ics"));
            SmtpClient client = new SmtpClient(config.SMTP.Server);
            if (!string.IsNullOrEmpty(config.SMTP.User))
                client.Credentials = new NetworkCredential(config.SMTP.User, config.SMTP.Password);
            client.EnableSsl = config.SMTP.SSL;
            client.Port = config.SMTP.Port;
            client.Send(mes);
        }

        public static void GenerateCancel(Booking booking, DateTime date, bool emailadmins)
        {
            hapConfig config = hapConfig.Current;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, config.BookingSystem.Lessons.Get(booking.Lesson).StartTime.Hour, config.BookingSystem.Lessons.Get(booking.Lesson).StartTime.Minute, 0);
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, config.BookingSystem.Lessons.Get(booking.Lesson).EndTime.Hour, config.BookingSystem.Lessons.Get(booking.Lesson).EndTime.Minute, 0);
            string location = "";
            Resource resource = config.BookingSystem.Resources[booking.Room];
            if (resource.Type == ResourceType.Room) location = booking.Room;
            else if (resource.Type == ResourceType.Laptops) location = booking.LTRoom;
            else if (resource.Type == ResourceType.Equipment) location = booking.EquipRoom;
            string summary = "Cancellation of " + booking.Name + " in " + location;
            string description = "Cancellation of " + booking.Name + " in " + location + " during " + booking.Lesson + " on " + booking.Date.ToShortDateString();
            if (resource.Type == ResourceType.Laptops)
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
            sb.AppendLine("ORGANIZER:MAILTO:" + booking.User.Email);
            sb.AppendLine("LOCATION:" + location);
            sb.AppendLine("UID:" + booking.uid);
            sb.AppendLine("DTSTAMP:" + DateTime.Now.ToString(DateFormat));
            sb.AppendLine("SUMMARY:" + summary);
            sb.AppendLine("STATUS:CANCELLED");
            sb.AppendLine("DESCRIPTION:" + description);
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath("~/App_Data/ITBooking.ics"));
            if (file.Exists) file.Delete();
            StreamWriter sr = file.CreateText();
            sr.Write(sb.ToString());
            sr.Flush();
            sr.Close();
            sr.Dispose();

            MailMessage mes = new MailMessage();
            IFormatProvider culture = new CultureInfo("en-gb");
            mes.Subject = summary;
            mes.From = mes.Sender = new MailAddress(config.SMTP.FromEmail, config.SMTP.FromUser);
            mes.ReplyToList.Add(mes.From);

            foreach (string s in hapConfig.Current.BookingSystem.Resources[booking.Room].Admins.Split(new char[] { ',' }))
            {
                UserInfo ui = ADUtils.FindUserInfos(s)[0];
                if (!string.IsNullOrEmpty(ui.Email)) mes.To.Add(new MailAddress(ui.Email, ui.DisplayName));
            }

            mes.Body = description;

            AlternateView av = AlternateView.CreateAlternateViewFromString(sb.ToString(), new ContentType("text/calendar; method=CANCEL; name=ITBooking.ics"));
            av.TransferEncoding = TransferEncoding.SevenBit;
            mes.AlternateViews.Add(av);

            //mes.Attachments.Add(new Attachment(file.FullName, "text/calendar; method=REQUEST; name=ITBooking.ics"));
            SmtpClient client = new SmtpClient(config.SMTP.Server);
            if (!string.IsNullOrEmpty(config.SMTP.User))
                client.Credentials = new NetworkCredential(config.SMTP.User, config.SMTP.Password);
            client.EnableSsl = config.SMTP.SSL;
            client.Port = config.SMTP.Port;
            client.Send(mes);
        }
    }
}