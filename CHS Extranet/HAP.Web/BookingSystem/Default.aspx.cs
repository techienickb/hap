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

namespace HAP.Web.BookingSystem
{
    public partial class Default : System.Web.UI.Page
    {
        private hapConfig config;

        public string Username
        {
            get
            {
                if (User.Identity.Name.Contains('\\'))
                    return User.Identity.Name.Remove(0, User.Identity.Name.IndexOf('\\') + 1);
                else return User.Identity.Name;
            }
        }

        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - IT Booking System", config.BaseSettings.EstablishmentName);
        }

        public bool isAdmin { get; set; }

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
            }
            else d = Calendar1.SelectedDates[0];
            Calendar1.SelectedDates.Clear();
            Calendar1.SelectedDates.Add(d);
            if (!IsPostBack) Calendar1.DataBind();
            DataBind();
            isAdmin = User.IsInRole("Domain Admins");
            adminlink.Visible = isAdmin;
        }

        public override void DataBind()
        {
            daylist.Date = bookingpopup.Date = Calendar1.SelectedDates[0];
            daylist.DataBind(); bookingpopup.DataBind();
            weeknum.Text = new BookingSystem(Calendar1.SelectedDates[0]).WeekNumber.ToString();
        }

        protected void remove_Click(object sender, EventArgs e)
        {
            string room = removevars.Value.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            string lesson = removevars.Value.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[1];
            XmlDocument doc = BookingSystem.BookingsDoc;
            doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + Calendar1.SelectedDate.ToShortDateString() + "' and @lesson='" + lesson.ToString() + "' and @room='" + room + "']"));
            if (config.BookingSystem.Resources[room].ResourceType == ResourceType.Laptops)
            {
                if (lesson.Length == 1)
                    foreach (lesson l in config.BookingSystem.Lessons)
                        if (l.OldID.ToString() == lesson) lesson = l.Name;
                int index = config.BookingSystem.Lessons.IndexOf(config.BookingSystem.Lessons[lesson]) + 1;
                if (index >= config.BookingSystem.Lessons.Count) index--;
                lesson nextlesson = config.BookingSystem.Lessons[index];

                if (nextlesson.Type != lessontype.Lesson) nextlesson = config.BookingSystem.Lessons[config.BookingSystem.Lessons.IndexOf(nextlesson) + 1];
                if (doc.SelectSingleNode("/Bookings/Booking[@date='" + Calendar1.SelectedDate.ToShortDateString() + "' and @lesson='" + nextlesson.Name + "' and @room='" + room.ToString() + "']") != null)
                    doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + Calendar1.SelectedDate.ToShortDateString() + "' and @lesson='" + nextlesson.Name + "' and @room='" + room.ToString() + "']"));
                else if (doc.SelectSingleNode("/Bookings/Booking[@date='" + Calendar1.SelectedDate.ToShortDateString() + "' and @lesson='" + nextlesson.OldID.ToString() + "' and @room='" + room.ToString() + "']") != null)
                    doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + Calendar1.SelectedDate.ToShortDateString() + "' and @lesson='" + nextlesson.OldID.ToString() + "' and @room='" + room.ToString() + "']"));

                index = config.BookingSystem.Lessons.IndexOf(config.BookingSystem.Lessons[lesson]) - 1;
                if (index < 0) index++;
                lesson previouslesson = config.BookingSystem.Lessons[index];
                if (doc.SelectSingleNode("/Bookings/Booking[@date='" + Calendar1.SelectedDate.ToShortDateString() + "' and @lesson='" + previouslesson.Name + "' and @room='" + room.ToString() + "' and @name='UNAVAILABLE']") != null)
                    doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + Calendar1.SelectedDate.ToShortDateString() + "' and @lesson='" + previouslesson.Name + "' and @room='" + room.ToString() + "' and @name='UNAVAILABLE']"));
                else if (doc.SelectSingleNode("/Bookings/Booking[@date='" + Calendar1.SelectedDate.ToShortDateString() + "' and @lesson='" + previouslesson.OldID.ToString() + "' and @room='" + room.ToString() + "' and @name='UNAVAILABLE']") != null)
                    doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + Calendar1.SelectedDate.ToShortDateString() + "' and @lesson='" + previouslesson.OldID.ToString() + "' and @room='" + room.ToString() + "' and @name='UNAVAILABLE']"));
            }
            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/Bookings.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();
            DataBind();
        }

        #region Calendar

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void Calendar1_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            DataBind();
        }

        #endregion
    }
}