using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CHS_Extranet.Configuration;
using System.Configuration;
using CHS_Extranet.routing;

namespace CHS_Extranet.BookingSystem
{
    public partial class Display : System.Web.UI.Page, IBookingSystemDisplay
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bs = new BookingSystem();
            extranetConfig config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
            if (Page.FindControl(Room) != null)
            {
                Panel room = Page.FindControl(Room) as Panel;
                room.Visible = true;
                if (Room.Contains('_'))
                    foreach (string s in Room.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Repeater r = room.FindControl(s) as Repeater;
                        List<Booking> bookings = new List<Booking>();
                        for (int i = 0; i < config.BookingSystem.LessonsPerDay; i++)
                            bookings.Add(bs.getBooking(s, i + 1));
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
                            for (int i = 0; i < config.BookingSystem.LessonsPerDay; i++)
                                bookings.Add(bs.getBooking(Room, i + 1));
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
                        for (int i = 0; i < config.BookingSystem.LessonsPerDay; i++)
                            bookings.Add(bs.getBooking(Room, i + 1));
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

        protected int currentLesson
        {
            get
            {
                extranetConfig config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
                int cl = 0;
                foreach (string s in config.BookingSystem.LessonTimesArray)
                {
                    cl++;
                    string[] s1 = s.Trim().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] s2 = config.BookingSystem.LessonLength.Trim().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime starttime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(s1[0]), int.Parse(s1[1]), 0);
                    DateTime endtime = starttime.AddHours(int.Parse(s2[0])).AddMinutes(int.Parse(s2[1]));
                    if (DateTime.Now >= starttime && DateTime.Now < endtime) return cl;
                }
                return 0;
            }
        }

        public string Room { get; set; }
    }
}