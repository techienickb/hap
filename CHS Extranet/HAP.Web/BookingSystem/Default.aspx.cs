using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using HAP.Web.Configuration;
using System.Configuration;
using System.Xml;
using HAP.Data.BookingSystem;
using HAP.AD;

namespace HAP.Web.BookingSystem
{
    public partial class Default : HAP.Web.Controls.Page
    {

        public Default()
        {
            this.SectionTitle = "IT Booking System";
        }

        protected void sub1_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        protected bool isAdmin
        {
            get
            {
                bool vis = false;
                foreach (string s in hapConfig.Current.BookingSystem.Admins.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                if (vis) return true;
                return HttpContext.Current.User.IsInRole("Domain Admins");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime d;
            if (!IsPostBack)
            {
                d = DateTime.Now;
                if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                    d = d.AddDays(2);
                else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    d = d.AddDays(1);
                datestamp.Value = d.ToShortDateString();
            }
            else d = DateTime.Parse(datestamp.Value);
            Calendar1.VisibleDate = DateTime.Parse(datestamp.Value);
            DataBind();
            adminlink.Visible = isAdmin;
        }

        public override void DataBind()
        {
            daylist.Date = bookingpopup.Date = DateTime.Parse(datestamp.Value);
            daylist.DataBind(); bookingpopup.DataBind();
            weeknum.Text = new HAP.Data.BookingSystem.BookingSystem(DateTime.Parse(datestamp.Value)).WeekNumber.ToString();
        }

        protected void remove_Click(object sender, EventArgs e)
        {
            string room = removevars.Value.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            string lesson = removevars.Value.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[1];
            HAP.Data.BookingSystem.BookingSystem bs = new HAP.Data.BookingSystem.BookingSystem(DateTime.Parse(datestamp.Value));
            Booking b = bs.getBooking(room, lesson);
            if (!string.IsNullOrEmpty(b.uid) && config.SMTP.Enabled)
            {
                iCalGenerator.GenerateCancel(b, DateTime.Parse(datestamp.Value));
                if (config.BookingSystem.Resources[room].EmailAdmins) iCalGenerator.GenerateCancel(b, DateTime.Parse(datestamp.Value), true);
            }
            XmlDocument doc = HAP.Data.BookingSystem.BookingSystem.BookingsDoc;
            doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(datestamp.Value).ToShortDateString() + "' and @lesson='" + lesson.ToString() + "' and @room='" + room + "']"));
            if (config.BookingSystem.Resources[room].EnableCharging)
            {
                if (lesson.Length == 1)
                    foreach (Lesson l in config.BookingSystem.Lessons)
                        lesson = l.Name;
                int index = config.BookingSystem.Lessons.FindIndex(l1 => l1.Name == lesson) + 1;
                if (index >= config.BookingSystem.Lessons.Count) index--;
                Lesson nextlesson = config.BookingSystem.Lessons[index];

                if (nextlesson.Type != LessonType.Lesson) nextlesson = config.BookingSystem.Lessons[config.BookingSystem.Lessons.IndexOf(nextlesson) + 1];
                if (doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(datestamp.Value).ToShortDateString() + "' and @lesson='" + nextlesson.Name + "' and @room='" + room.ToString() + "']") != null)
                    doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(datestamp.Value).ToShortDateString() + "' and @lesson='" + nextlesson.Name + "' and @room='" + room.ToString() + "']"));

                index = config.BookingSystem.Lessons.FindIndex(l1 => l1.Name == lesson) - 1;
                if (index < 0) index++;
                Lesson previouslesson = config.BookingSystem.Lessons[index];
                if (doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(datestamp.Value).ToShortDateString() + "' and @lesson='" + previouslesson.Name + "' and @room='" + room.ToString() + "' and @name='UNAVAILABLE']") != null)
                    doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(datestamp.Value).ToShortDateString() + "' and @lesson='" + previouslesson.Name + "' and @room='" + room.ToString() + "' and @name='UNAVAILABLE']"));
            }
            HAP.Data.BookingSystem.BookingSystem.BookingsDoc = doc;
            DataBind();
        }

        #region Calendar

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            if (Calendar1.SelectedDates.Count > 1) Response.Redirect("weekview.aspx?d=" + Calendar1.SelectedDates[0].ToShortDateString());
            DataBind();
        }

        protected void Calendar1_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            DataBind();
        }

        #endregion
    }
}