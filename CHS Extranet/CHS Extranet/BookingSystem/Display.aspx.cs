using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Configuration;
using HAP.Web.routing;

namespace HAP.Web.BookingSystem
{
    public partial class Display : System.Web.UI.Page, IBookingSystemDisplay
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ExpiresAbsolute = DateTime.Now;
            bs = new BookingSystem();
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
                        foreach (lesson lesson in config.BookingSystem.Lessons)
                        {
                            Booking b = bs.getBooking(s, lesson.OldID.ToString());
                            if (b.Name == "FREE" || lesson.OldID == -1) b = bs.getBooking(s, lesson.Name);
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
                            foreach (lesson lesson in config.BookingSystem.Lessons)
                            {
                                Booking b = bs.getBooking(Room, lesson.OldID.ToString());
                                if (b.Name == "FREE" || lesson.OldID == -1) b = bs.getBooking(Room, lesson.Name);
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
                        foreach (lesson lesson in config.BookingSystem.Lessons)
                        {
                            Booking b = bs.getBooking(Room, lesson.OldID.ToString());
                            if (b.Name == "FREE" || lesson.OldID == -1) b = bs.getBooking(Room, lesson.Name);
                            bookings.Add(b);
                        }
                        r.DataSource = bookings.ToArray();
                        r.DataBind();
                    }
            }
        }
        protected BookingSystem bs;

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
                foreach (lesson lesson in config.BookingSystem.Lessons)
                {
                    string[] s1 = lesson.StartTime.Trim().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] s2 = lesson.EndTime.Trim().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime starttime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(s1[0]), int.Parse(s1[1]), 0);
                    DateTime endtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(s2[0]), int.Parse(s2[1]), 0);
                    if (DateTime.Now >= starttime && DateTime.Now < endtime) return lesson.Name;
                }
                return "N/A";
            }
        }

        public string Room { get; set; }
    }
}