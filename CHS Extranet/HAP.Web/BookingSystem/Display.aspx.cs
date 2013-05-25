using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Configuration;
using HAP.Web.routing;
using HAP.BookingSystem;

namespace HAP.Web.BookingSystem
{
    public partial class Display : System.Web.UI.Page, IBookingSystemDisplay
    {
        protected hapConfig config;
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ExpiresAbsolute = DateTime.Now;
            bs = new HAP.BookingSystem.BookingSystem();
            config = hapConfig.Current;
            if (Page.FindControl(Room) != null && Page.FindControl(Room) is Panel)
            {
                Panel room = Page.FindControl(Room) as Panel;
                room.Visible = true;
                if (Room.Contains('_'))
                    foreach (string s in Room.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Repeater r = room.FindControl(s) as Repeater;
                        List<Booking> bookings = new List<Booking>();
                        foreach (Lesson lesson in config.BookingSystem.Lessons)
                            foreach (Booking b in bs.getBooking(s, lesson.Name))
                            {
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
                                foreach (Booking b in bs.getBooking(Room, lesson.Name))
                                {
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
                            foreach (Booking b in bs.getBooking(Room, lesson.Name))
                            {
                                bookings.Add(b);
                            }
                        r.DataSource = bookings.ToArray();
                        r.DataBind();
                    }
            }
        }
        protected HAP.BookingSystem.BookingSystem bs;

        protected string getName(object o)
        {
            Booking b = o as Booking;
            if (b.Name == "FREE")
                return b.Username;
            else return b.User.Notes;
        }

        protected string getJSTimings()
        {
            List<string> s = new List<string>();
            foreach (HAP.Web.Configuration.Lesson l in config.BookingSystem.Lessons)
                s.Add("{ ID: \"" + l.Name.ToLower().Replace(" ", "").Trim() + "\", Name: \"" + l.Name + "\", StartTime: { Hour: " + 
                    l.StartTime.Hour + ", Minute: " + 
                    l.StartTime.Minute + "}, EndTime: { Hour: " + 
                    l.EndTime.Hour + ", Minute: " + 
                    l.EndTime.Minute + " } }");
            return string.Join(", ", s.ToArray());
        }

        public string Room { get; set; }
    }
}