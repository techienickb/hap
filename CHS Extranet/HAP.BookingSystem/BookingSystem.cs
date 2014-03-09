using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using HAP.Web.Configuration;
using System.Configuration;
using System.IO;
using System.Threading;

namespace HAP.BookingSystem
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

        public static XmlDocument BookingsDoc
        {
            get
            {
                XmlDocument doc;
                if (HttpContext.Current.Cache["bookings"] == null)
                {
                    doc = new XmlDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Bookings.xml"));

                    HttpContext.Current.Cache.Insert("bookings", doc, new System.Web.Caching.CacheDependency(HttpContext.Current.Server.MapPath("~/App_Data/Bookings.xml")));

                }
                else doc = HttpContext.Current.Cache["bookings"] as XmlDocument;
                return doc;
            }
            set
            {
                if (hapConfig.Current.BookingSystem.KeepXmlClean)
                {
                    List<XmlNode> nodes = new List<XmlNode>();
                    foreach (XmlNode node in value.SelectNodes("/Bookings/Booking"))
                        if (DateTime.Parse(node.Attributes["date"].Value) < DateTime.Now.AddDays(-7))
                            nodes.Add(node);
                    if (hapConfig.Current.BookingSystem.ArchiveXml && nodes.Count > 0)
                        new Thread(new ParameterizedThreadStart(ArchiveXml)).Start(new Obj { Nodes = nodes.ToArray(), Path = HttpContext.Current.Server.MapPath("~/app_data/bookingarchive.xml") }); //archive ASYNC
                    foreach (XmlNode node in nodes)
                        value.SelectSingleNode("/Bookings").RemoveChild(node);
                }
                XmlWriterSettings set = new XmlWriterSettings();
                set.Indent = true;
                set.IndentChars = "   ";
                set.Encoding = System.Text.Encoding.UTF8;
                XmlWriter writer = XmlWriter.Create(HttpContext.Current.Server.MapPath("~/App_Data/Bookings.xml"), set);
                value.Save(writer);
                writer.Flush();
                writer.Close();
                HttpContext.Current.Cache.Remove("bookings");
            }
        }

        public static void ArchiveXml(object obj)
        {
            Obj o = obj as Obj;
            if (!File.Exists(o.Path))
            {
                StreamWriter sw = File.CreateText(o.Path);
                sw.WriteLine("<?xml version=\"1.0\"?>");
                sw.WriteLine("</Bookings>");
                sw.Close();
                sw.Dispose();
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(o.Path);
            foreach (XmlNode node in o.Nodes)
                doc.SelectSingleNode("/Bookings").AppendChild(node);
            doc.Save(o.Path);
        }

        public Booking[] getBooking(string room, string lesson)
        {
            XmlDocument doc = BookingsDoc;
            if (!islessonFree(room, lesson))
            {
                List<Booking> bookings = new List<Booking>();
                foreach (XmlNode node in doc.SelectNodes("/Bookings/Booking[@date='" + Date.ToShortDateString() + "' and @lesson[contains(.,'" + lesson + "')] and @room='" + room + "']"))
                    bookings.Add(new Booking(node, DayNumber));
                return bookings.ToArray();
            }
            else if (isStatic(room, lesson)) return new Booking[] { StaticBookings[BookingKey.parseBooking(DayNumber, lesson, room)] };
            return new Booking[] { new Booking(DayNumber, lesson, room, "FREE", "Not Booked") };
        }

        public static Dictionary<BookingKey, Booking> StaticBookings
        {
            get
            {
                Dictionary<BookingKey, Booking> staticbookings;
                if (HttpContext.Current.Cache["staticbookings"] == null)
                {
                    staticbookings = new Dictionary<BookingKey, Booking>();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));

                    foreach (XmlNode node in doc.SelectNodes("/Bookings/Booking"))
                        staticbookings.Add(new BookingKey(node), new Booking(node));

                    HttpContext.Current.Cache.Insert("staticbookings", staticbookings, new System.Web.Caching.CacheDependency(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml")));

                }
                else staticbookings = HttpContext.Current.Cache["staticbookings"] as Dictionary<BookingKey, Booking>;
                return staticbookings;
            }
        }

        public bool isStatic(string room, string lesson)
        {
            foreach (BookingKey key in StaticBookings.Keys)
                if (key.Lesson.Contains(lesson) && key.Day == DayNumber && key.Room == room)
                {
                    if (StaticBookings[key].StartDate.HasValue && StaticBookings[key].EndDate.HasValue)
                        return (StaticBookings[key].StartDate.Value.Date <= DateTime.Now.Date && StaticBookings[key].EndDate >= DateTime.Now.Date);
                    else if (StaticBookings[key].StartDate.HasValue && !StaticBookings[key].EndDate.HasValue)
                        return (StaticBookings[key].StartDate.Value.Date <= DateTime.Now.Date);
                    else if (!StaticBookings[key].StartDate.HasValue && StaticBookings[key].EndDate.HasValue)
                        return (StaticBookings[key].EndDate.Value.Date >= DateTime.Now.Date);
                    else return true;
                }
            return false;
        }

        public bool islessonFree(string room, string lesson)
        {
            XmlDocument doc = BookingsDoc;
            if (doc.SelectSingleNode("/Bookings/Booking[@date='" + Date.ToShortDateString() + "' and @lesson[contains(.,'" + lesson + "')] and @room='" + room + "']") != null)
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
                List<AdvancedBookingRight> abr;
                if (HttpContext.Current.Cache["abr"] == null)
                {
                    abr = new List<AdvancedBookingRight>();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));

                    foreach (XmlNode node in doc.SelectNodes("/ABR/Right"))
                        abr.Add(new AdvancedBookingRight(node.Attributes["user"].Value, int.Parse(node.Attributes["weeksinadvanced"].Value), int.Parse(node.Attributes["bookingsperweek"].Value)));

                    HttpContext.Current.Cache.Insert("abr", abr, new System.Web.Caching.CacheDependency(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml")));

                }
                else abr = HttpContext.Current.Cache["abr"] as List<AdvancedBookingRight>;
                return abr;
            }
        }

        public AdvancedBookingRight[] getBookingRights()
        {
            return BookingRights.ToArray();
        }

        public void updateBookingRights(AdvancedBookingRight right)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));

            XmlNode node = doc.SelectSingleNode("/ABR/Right[@user='" + right.Username + "']");
            node.Attributes["bookingsperweek"].Value = right.Numperweek.ToString();
            node.Attributes["weeksinadvanced"].Value = right.Weeksahead.ToString();

            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/abr.xml"));
            HttpContext.Current.Cache.Remove("abr");
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
            HttpContext.Current.Cache.Remove("abr");
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
            HttpContext.Current.Cache.Remove("abr");
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

            HttpContext.Current.Cache["staticbookings"] = null;
        }
        #endregion
    }

    public class Obj
    {
        public XmlNode[] Nodes { get; set; }
        public string Path { get; set; }
    }
}