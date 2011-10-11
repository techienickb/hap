using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using HAP.Web.Configuration;
using System.Web;
using HAP.Data;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using HAP.Data.BookingSystem;
using System.Xml;
using HAP.Web.BookingSystem;

namespace HAP.Web.API
{
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BookingSystem
    {
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/Booking/{Date}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public JSONBooking[] RemoveBooking(string Date, JSONBooking booking)
        {
            HAP.Data.BookingSystem.BookingSystem bs = new HAP.Data.BookingSystem.BookingSystem(DateTime.Parse(Date));
            Booking b = bs.getBooking(booking.Room, booking.Lesson);
            if (!string.IsNullOrEmpty(b.uid) && hapConfig.Current.SMTP.Enabled)
            {
                iCalGenerator.GenerateCancel(b, DateTime.Parse(Date));
                if (hapConfig.Current.BookingSystem.Resources[booking.Room].EmailAdmins) iCalGenerator.GenerateCancel(b, DateTime.Parse(Date), true);
            }
            XmlDocument doc = HAP.Data.BookingSystem.BookingSystem.BookingsDoc;
            doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(Date).ToShortDateString() + "' and @lesson='" + booking.Lesson + "' and @room='" + booking.Room + "']"));
            if (hapConfig.Current.BookingSystem.Resources[booking.Room].EnableCharging)
            {
                int index = hapConfig.Current.BookingSystem.Lessons.FindIndex(l1 => l1.Name == booking.Lesson) + 1;
                if (index >= hapConfig.Current.BookingSystem.Lessons.Count) index--;
                Lesson nextlesson = hapConfig.Current.BookingSystem.Lessons[index];

                if (nextlesson.Type != LessonType.Lesson) nextlesson = hapConfig.Current.BookingSystem.Lessons[hapConfig.Current.BookingSystem.Lessons.IndexOf(nextlesson) + 1];
                if (doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(Date).ToShortDateString() + "' and @lesson='" + nextlesson.Name + "' and @room='" + booking.Room + "']") != null)
                    doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(Date).ToShortDateString() + "' and @lesson='" + nextlesson.Name + "' and @room='" + booking.Room + "']"));

                index = hapConfig.Current.BookingSystem.Lessons.FindIndex(l1 => l1.Name == booking.Lesson) - 1;
                if (index < 0) index++;
                Lesson previouslesson = hapConfig.Current.BookingSystem.Lessons[index];
                if (doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(Date).ToShortDateString() + "' and @lesson='" + previouslesson.Name + "' and @room='" + booking.Room + "' and @name='UNAVAILABLE']") != null)
                    doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(Date).ToShortDateString() + "' and @lesson='" + previouslesson.Name + "' and @room='" + booking.Room + "' and @name='UNAVAILABLE']"));
            }
            HAP.Data.BookingSystem.BookingSystem.BookingsDoc = doc;
            return LoadRoom(Date, booking.Room);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Booking/{Date}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public JSONBooking[] Book(string Date, JSONBooking booking)
        {
            XmlDocument doc = HAP.Data.BookingSystem.BookingSystem.BookingsDoc;
            XmlElement node = doc.CreateElement("Booking");
            node.SetAttribute("date", DateTime.Parse(Date).ToShortDateString());
            node.SetAttribute("lesson", booking.Lesson);
            hapConfig config = hapConfig.Current;
            if (config.BookingSystem.Resources[booking.Room].Type == ResourceType.Laptops)
            {
                node.SetAttribute("ltroom", booking.LTRoom);
                node.SetAttribute("ltcount", booking.LTCount.ToString());
                node.SetAttribute("ltheadphones", booking.LTHeadPhones.ToString());
            }
            else if (config.BookingSystem.Resources[booking.Room].Type == ResourceType.Equipment)
                node.SetAttribute("equiproom", booking.EquipRoom);
            node.SetAttribute("room", booking.Room);
            node.SetAttribute("uid", booking.Username + DateTime.Now.ToString(iCalGenerator.DateFormat));
            node.SetAttribute("username", booking.Username);
            node.SetAttribute("name", booking.Name);
            doc.SelectSingleNode("/Bookings").AppendChild(node);
            #region Charging
            if (config.BookingSystem.Resources[booking.Room].EnableCharging)
            {
                HAP.Data.BookingSystem.BookingSystem bs = new HAP.Data.BookingSystem.BookingSystem(DateTime.Parse(Date));
                int index = config.BookingSystem.Lessons.FindIndex(l => l.Name == booking.Lesson);
                if (index > 0 && bs.islessonFree(booking.Room, config.BookingSystem.Lessons[index - 1].Name))
                {
                    node = doc.CreateElement("Booking");
                    node.SetAttribute("date", DateTime.Parse(Date).ToShortDateString());
                    node.SetAttribute("lesson", config.BookingSystem.Lessons[index - 1].Name);
                    node.SetAttribute("room", booking.Room);
                    node.SetAttribute("ltroom", "--");
                    node.SetAttribute("ltcount", booking.LTCount.ToString());
                    node.SetAttribute("ltheadphones", booking.LTHeadPhones.ToString());
                    node.SetAttribute("username", "systemadmin");
                    node.SetAttribute("name", "UNAVAILABLE");
                    doc.SelectSingleNode("/Bookings").AppendChild(node);
                }
                if (index < config.BookingSystem.Lessons.Count - 1)
                {
                    if (bs.islessonFree(booking.Room, config.BookingSystem.Lessons[index + 1].Name))
                    {
                        node = doc.CreateElement("Booking");
                        node.SetAttribute("date", DateTime.Parse(Date).ToShortDateString());
                        node.SetAttribute("lesson", config.BookingSystem.Lessons[index + 1].Name);
                        node.SetAttribute("room", booking.Room);
                        node.SetAttribute("ltroom", "--");
                        node.SetAttribute("ltcount", booking.LTCount.ToString());
                        node.SetAttribute("ltheadphones", booking.LTHeadPhones.ToString());
                        node.SetAttribute("username", "systemadmin");
                        node.SetAttribute("name", "CHARGING");
                        doc.SelectSingleNode("/Bookings").AppendChild(node);
                    }
                }
            }
            #endregion
            HAP.Data.BookingSystem.BookingSystem.BookingsDoc = doc;
            Booking b = new HAP.Data.BookingSystem.BookingSystem(DateTime.Parse(Date)).getBooking(booking.Room, booking.Lesson);
            if (config.SMTP.Enabled)
            {
                iCalGenerator.Generate(b, DateTime.Parse(Date));
                if (config.BookingSystem.Resources[booking.Room].EmailAdmins) iCalGenerator.Generate(b, DateTime.Parse(Date), true);
            }
            return LoadRoom(Date, booking.Room);
        }

        [OperationContract]
        [WebGet(ResponseFormat=WebMessageFormat.Json, UriTemplate="/LoadRoom/{Date}/{Resource}")]
        public JSONBooking[] LoadRoom(string Date, string Resource)
        {
            List<JSONBooking> bookings = new List<JSONBooking>();
            HAP.Data.BookingSystem.BookingSystem bs = new HAP.Data.BookingSystem.BookingSystem(DateTime.Parse(Date));
            foreach (Lesson lesson in hapConfig.Current.BookingSystem.Lessons)
                bookings.Add(new JSONBooking(bs.getBooking(Resource, lesson.Name)));
            return bookings.ToArray();
        }

        [OperationContract]
        [WebGet(ResponseFormat=WebMessageFormat.Json, UriTemplate="/Initial/{Date}/{Username}")]
        public int[] Initial(string Username, string Date)
        {
            if (!isAdmin(Username))
            {
                XmlDocument doc = HAP.Data.BookingSystem.BookingSystem.BookingsDoc;
                int max = hapConfig.Current.BookingSystem.MaxBookingsPerWeek;
                foreach (AdvancedBookingRight right in HAP.Data.BookingSystem.BookingSystem.BookingRights)
                    if (right.Username == Username)
                        max = right.Numperweek;
                int x = 0;
                foreach (DateTime d in getWeekDates(DateTime.Parse(Date)))
                    x += doc.SelectNodes("/Bookings/Booking[@date='" + d.ToShortDateString() + "' and @username='" + Username + "']").Count;
                return new int[] { max - x, new HAP.Data.BookingSystem.BookingSystem(DateTime.Parse(Date)).WeekNumber };
            }

            return new int[] { 0, new HAP.Data.BookingSystem.BookingSystem(DateTime.Parse(Date)).WeekNumber };
        }

        private DateTime[] getWeekDates(DateTime Date)
        {
            List<DateTime> dates = new List<DateTime>();
            if (Date.DayOfWeek == DayOfWeek.Monday)
            {
                dates.Add(Date); dates.Add(Date.AddDays(1));
                dates.Add(Date.AddDays(2)); dates.Add(Date.AddDays(3));
                dates.Add(Date.AddDays(4));
            }
            else if (Date.DayOfWeek == DayOfWeek.Tuesday)
            {
                dates.Add(Date); dates.Add(Date.AddDays(1));
                dates.Add(Date.AddDays(2)); dates.Add(Date.AddDays(3));
                dates.Add(Date.AddDays(-1));
            }
            else if (Date.DayOfWeek == DayOfWeek.Wednesday)
            {
                dates.Add(Date); dates.Add(Date.AddDays(1));
                dates.Add(Date.AddDays(2)); dates.Add(Date.AddDays(-1));
                dates.Add(Date.AddDays(-2));
            }
            else if (Date.DayOfWeek == DayOfWeek.Tuesday)
            {
                dates.Add(Date); dates.Add(Date.AddDays(1));
                dates.Add(Date.AddDays(-1)); dates.Add(Date.AddDays(-2));
                dates.Add(Date.AddDays(-3));
            }
            else
            {
                dates.Add(Date); dates.Add(Date.AddDays(-1));
                dates.Add(Date.AddDays(-2)); dates.Add(Date.AddDays(-3));
                dates.Add(Date.AddDays(-4));
            }
            return dates.ToArray();
        }

        private bool isAdmin(string Username)
        {
            if (new HAP.AD.User(Username).IsMemberOf(GroupPrincipal.FindByIdentity(AD.ADUtils.GetPContext(), "Domain Admins"))) return true;
            return isBSAdmin(Username);
        }

        private bool isBSAdmin(string Username)
        {
            foreach (string s in hapConfig.Current.BookingSystem.Admins.Split(new char[] { ',' }))
                if (s.Trim().ToLower().Equals(Username.ToLower())) return true;
            return false;
        }
    }
}
