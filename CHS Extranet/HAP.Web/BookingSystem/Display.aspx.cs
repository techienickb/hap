using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Configuration;
using HAP.Web.routing;
using HAP.Data.BookingSystem;

namespace HAP.Web.BookingSystem
{
    public partial class Display : System.Web.UI.Page, IBookingSystemDisplay
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ExpiresAbsolute = DateTime.Now;
            bs = new HAP.Data.BookingSystem.BookingSystem();
            hapConfig config = hapConfig.Current;
            if (Page.FindControl(Room) != null)
            {
                Panel room = Page.FindControl(Room) as Panel;
                room.Visible = true;
                if (Room.Contains('_'))
                    foreach (string s in Room.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Repeater r = room.FindControl(s) as Repeater;
                        List<Booking> bookings = new List<Booking>();
                        foreach (Lesson lesson in config.BookingSystem.Lessons)
                        {
                            Booking b = bs.getBooking(s, lesson.Name);
                            bookings.Add(b);
                        }
                        r.DataSource = bookings.ToArray();
                        r.DataBind();
                    }
                else
                {
                    foreach (Control c in room.Controls)
                        if (c.GetType() == typeof(Repeater))
                        {
                            Repeater r = c as Repeater;
                            List<Booking> bookings = new List<Booking>();
                            foreach (Lesson lesson in config.BookingSystem.Lessons)
                            {
                                Booking b = bs.getBooking(Room, lesson.Name);
                                bookings.Add(b);
                            }
                            r.DataSource = bookings.ToArray();
                            r.DataBind();
                        }
                }

            }
            else
            {
                roomlabel.Text = Room;
                defaultview.Visible = true;
                foreach (Control c in defaultview.Controls)
                    if (c.GetType() == typeof(Repeater))
                    {
                        Repeater r = c as Repeater;
                        List<Booking> bookings = new List<Booking>();
                        foreach (Lesson lesson in config.BookingSystem.Lessons)
                        {
                            Booking b = bs.getBooking(Room, lesson.Name);
                            bookings.Add(b);
                        }
                        r.DataSource = bookings.ToArray();
                        r.DataBind();
                    }
            }
        }
        protected HAP.Data.BookingSystem.BookingSystem bs;

        protected string getName(object o)
        {
            Booking b = o as Booking;
            if (b.Name == "FREE")
                return b.Username;
            else return b.User.Notes;
        }

        protected string currentLesson
        {
            get
            {
                hapConfig config = hapConfig.Current;
                foreach (Lesson lesson in config.BookingSystem.Lessons)
                {
                    DateTime starttime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, lesson.StartTime.Hour, lesson.StartTime.Minute, 0);
                    DateTime endtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, lesson.EndTime.Hour, lesson.EndTime.Minute, 0);
                    if (DateTime.Now >= starttime && DateTime.Now < endtime) return lesson.Name;
                }
                return "N/A";
            }
        }

        public string Room { get; set; }
    }
}