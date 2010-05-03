using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using HAP.Web.Configuration;
using System.Configuration;

namespace HAP.Web.BookingSystem
{
    public class BookingSystem
    {
        public BookingSystem()
        {
            this.date = DateTime.Now.Date;
        }
        public BookingSystem(DateTime date)
        {
            this.date = date;
        }

        private DateTime date;
        public DateTime Date
        {
            get
            {
                if (date.DayOfWeek == DayOfWeek.Saturday) date = date.AddDays(2);
                else if (date.DayOfWeek == DayOfWeek.Sunday) date = date.AddDays(1);
                return date;
            }
        }

        public Booking getBooking(string room, string lesson)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Bookings.xml"));
            if (!islessonFree(room, lesson))
            {
                XmlNode node = doc.SelectSingleNode("/Bookings/Booking[@date='" + Date.ToShortDateString() + "' and @lesson='" + lesson + "' and @room='" + room + "']");
                return new Booking(node, DayNumber);
            }
            else if (isStatic(room, lesson)) return StaticBookings[BookingKey.parseBooking(DayNumber, lesson, room)];
            return new Booking(DayNumber, lesson, room, "FREE", "Not Booked");
        }

        public Dictionary<BookingKey, Booking> StaticBookings
        {
            get
            {
                Dictionary<BookingKey, Booking> staticbookings = new Dictionary<BookingKey, Booking>();

                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));

                foreach (XmlNode node in doc.SelectNodes("/Bookings/Booking"))
                    staticbookings.Add(new BookingKey(node), new Booking(node));
                return staticbookings;
            }
        }

        public bool isStatic(string room, string lesson)
        {
            return StaticBookings.ContainsKey(new BookingKey(DayNumber, lesson, room));
        }

        public bool islessonFree(string room, string lesson)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Bookings.xml"));
            if (doc.SelectSingleNode("/Bookings/Booking[@date='" + Date.ToShortDateString() + "' and @lesson='" + lesson + "' and @room='" + room + "']") != null)
                return false;
            return true;
        }

        #region Day/Week Nuber
        public int DayNumber
        {
            get
            {
                int x = 1;
                if (Date.DayOfWeek == DayOfWeek.Monday)
                    x = 1;
                else if (Date.DayOfWeek == DayOfWeek.Tuesday)
                    x = 2;
                else if (Date.DayOfWeek == DayOfWeek.Wednesday)
                    x = 3;
                else if (Date.DayOfWeek == DayOfWeek.Thursday)
                    x = 4;
                else if (Date.DayOfWeek == DayOfWeek.Friday)
                    x = 5;
                if (WeekNumber == 2) x += 5;
                return x;
            }
        }

        public int WeekNumber { get { return Terms.getTerm(Date).WeekNum(Date); } }

        #endregion

        #region AdminPanel
        public static List<AdvancedBookingRight> BookingRights
        {
            get
            {
                List<AdvancedBookingRight> abr = new List<AdvancedBookingRight>();
                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));

                foreach (XmlNode node in doc.SelectNodes("/ABR/Right"))
                    abr.Add(new AdvancedBookingRight(node.Attributes["user"].Value, int.Parse(node.Attributes["weeksinadvanced"].Value), int.Parse(node.Attributes["bookingsperweek"].Value)));
                return abr;
            }
        }

        public AdvancedBookingRight[] getBookingRights()
        {
            List<AdvancedBookingRight> abr = new List<AdvancedBookingRight>();
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));

            foreach (XmlNode node in doc.SelectNodes("/ABR/Right"))
                abr.Add(new AdvancedBookingRight(node.Attributes["user"].Value, int.Parse(node.Attributes["weeksinadvanced"].Value), int.Parse(node.Attributes["bookingsperweek"].Value)));
            return abr.ToArray();
        }

        public void updateBookingRights(AdvancedBookingRight right)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));

            XmlNode node = doc.SelectSingleNode("/ABR/Right[@user='" + right.Username + "']");
            node.Attributes["bookingsperweek"].Value = right.Numperweek.ToString();
            node.Attributes["weeksinadvanced"].Value = right.Weeksahead.ToString();

            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));
        }

        public void deleteBookingRights(AdvancedBookingRight right)
        {
        }

        public void deleteBookingRights1(string Username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));

            XmlNode node = doc.SelectSingleNode("/ABR/Right[@user='" + Username + "']");
            doc.SelectSingleNode("/ABR").RemoveChild(node);

            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));
        }

        public void addBookingRights(AdvancedBookingRight right)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));
            XmlNode bookings = doc.SelectSingleNode("/ABR");
            XmlNode node = doc.CreateElement("Right");
            XmlAttribute at = doc.CreateAttribute("user");
            at.Value = right.Username;
            node.Attributes.Append(at);
            at = doc.CreateAttribute("weeksinadvanced");
            at.Value = right.Weeksahead.ToString();
            node.Attributes.Append(at);
            at = doc.CreateAttribute("bookingsperweek");
            at.Value = right.Numperweek.ToString();
            node.Attributes.Append(at);
            bookings.AppendChild(node);

            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));
        }

        public Booking[] getStaticBookingsArray()
        {
            List<Booking> bookings = new List<Booking>();
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));

            foreach (XmlNode node in doc.SelectNodes("/Bookings/Booking"))
                bookings.Add(new Booking(node));
            return bookings.ToArray();
        }

        public void updateStaticBooking(Booking booking)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));

            XmlNode node = doc.SelectSingleNode("/Bookings/Booking[@lesson='" + booking.Lesson + "' and @room='" + booking.Room + "' and @day='" + booking.Day + "']");
            node.Attributes["name"].Value = booking.Name;
            node.Attributes["username"].Value = booking.Username;

            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));
        }

        public void deleteStaticBooking(Booking booking)
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load(HttpContext.Current.Server.MapPath("~/StaticBookings.xml"));

            //XmlNode node = doc.SelectSingleNode("/Bookings/Booking[@lesson='" + booking.Lesson + "' and @room='" + booking.Room + "' and @day='" + booking.Day + "']");
            //doc.SelectSingleNode("/Bookings").RemoveChild(node);

            //doc.Save(HttpContext.Current.Server.MapPath("~/StaticBookings.xml"));
        }

        public void deleteStaticBooking1(string lesson, string room, int day)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));

            XmlNode node = doc.SelectSingleNode("/Bookings/Booking[@lesson='" + lesson + "' and @room='" + room + "' and @day='" + day + "']");
            //HttpContext.Current.Response.Write("/Bookings/Booking[@lesson='" + lesson + "' and @room='" + room + "' and @day='" + day + "']  " + (node != null));
            doc.SelectSingleNode("/Bookings").RemoveChild(node);

            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));
        }

        public void addStaticBooking(Booking booking)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));
            XmlNode bookings = doc.SelectSingleNode("/Bookings");
            XmlNode node = doc.CreateElement("Booking");
            XmlAttribute at = doc.CreateAttribute("day");
            at.Value = booking.Day.ToString();
            node.Attributes.Append(at);
            at = doc.CreateAttribute("lesson");
            at.Value = booking.Lesson.ToString();
            node.Attributes.Append(at);
            at = doc.CreateAttribute("room");
            at.Value = booking.Room;
            node.Attributes.Append(at);
            at = doc.CreateAttribute("name");
            at.Value = booking.Name;
            node.Attributes.Append(at);
            at = doc.CreateAttribute("username");
            at.Value = booking.Username;
            node.Attributes.Append(at);
            bookings.AppendChild(node);

            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));
        }
        #endregion
    }
}