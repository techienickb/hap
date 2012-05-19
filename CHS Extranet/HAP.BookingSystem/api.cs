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
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using HAP.BookingSystem;
using System.Xml;

namespace HAP.Web.API
{
    [ServiceAPI("api/bookingsystem")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BookingSystem
    {
        [OperationContract]
        [WebInvoke(Method="POST", UriTemplate="/Search", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
        public JSONBooking[] Search(string Query)
        {
            List<JSONBooking> bookings = new List<JSONBooking>();
            XmlDocument doc = HAP.BookingSystem.BookingSystem.BookingsDoc;
            foreach (XmlNode n in doc.SelectNodes("/Bookings/Booking"))
            {
                Booking b = new Booking(n, false);
                if (b.Username.ToLower().StartsWith(Query.ToLower()))
                    bookings.Add(new JSONBooking(b));
            }
            foreach (Booking sb in HAP.BookingSystem.BookingSystem.StaticBookings.Values)
                if (sb.Username.ToLower().StartsWith(Query.ToLower()))
                    bookings.Add(new JSONBooking(sb));
            bookings.Sort();
            return bookings.ToArray();
        }

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/Booking/{Date}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public JSONBooking[] RemoveBooking(string Date, JSONBooking booking)
        {
#if DEBUG
            HAP.Web.Logging.EventViewer.Log("Booking System JSON API", "Loading Booking from XML Datasource", System.Diagnostics.EventLogEntryType.Information);
#endif
            HAP.BookingSystem.BookingSystem bs = new HAP.BookingSystem.BookingSystem(DateTime.Parse(Date));
            Booking b = bs.getBooking(booking.Room, booking.Lesson);
            try
            {
#if DEBUG
                HAP.Web.Logging.EventViewer.Log("Booking System JSON API", "Checking if Email is Enabled and a UID exists on the booking", System.Diagnostics.EventLogEntryType.Information);
#endif
                if (!string.IsNullOrEmpty(b.uid) && hapConfig.Current.SMTP.Enabled)
                {
#if DEBUG
                    HAP.Web.Logging.EventViewer.Log("Booking System JSON API", "Asking for an iCal Cancel to be Generated and Sent", System.Diagnostics.EventLogEntryType.Information);
#endif
                    iCalGenerator.GenerateCancel(b, DateTime.Parse(Date));
#if DEBUG
                    HAP.Web.Logging.EventViewer.Log("Booking System JSON API", "Asking for an iCan Admin Cancel to be Generated and Sent", System.Diagnostics.EventLogEntryType.Information);
#endif
                    if (hapConfig.Current.BookingSystem.Resources[booking.Room].EmailAdmins) iCalGenerator.GenerateCancel(b, DateTime.Parse(Date), true);
                }
            }
            catch (Exception ex) { HAP.Web.Logging.EventViewer.Log("Booking System JSON API", ex.ToString() + "\nMessage:\n" + ex.Message + "\nStack Trace:\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error); }
            XmlDocument doc = HAP.BookingSystem.BookingSystem.BookingsDoc;
#if DEBUG
            HAP.Web.Logging.EventViewer.Log("Booking System JSON API", "Remove XML Element from XML Datasource", System.Diagnostics.EventLogEntryType.Information);
#endif
            doc.SelectSingleNode("/Bookings").RemoveChild(doc.SelectSingleNode("/Bookings/Booking[@date='" + DateTime.Parse(Date).ToShortDateString() + "' and @lesson='" + booking.Lesson + "' and @room='" + booking.Room + "']"));
#if DEBUG
            HAP.Web.Logging.EventViewer.Log("Booking System JSON API", "Checking to see if the Room has a Charging Flag", System.Diagnostics.EventLogEntryType.Information);
#endif
            if (hapConfig.Current.BookingSystem.Resources[booking.Room].EnableCharging)
            {
#if DEBUG
                HAP.Web.Logging.EventViewer.Log("Booking System JSON API", "Removing the Charging and Unavailable Items", System.Diagnostics.EventLogEntryType.Information);
#endif
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
#if DEBUG
            HAP.Web.Logging.EventViewer.Log("Booking System JSON API", "Save XML Datasource", System.Diagnostics.EventLogEntryType.Information);
#endif
            HAP.BookingSystem.BookingSystem.BookingsDoc = doc;
#if DEBUG
            HAP.Web.Logging.EventViewer.Log("Booking System JSON API", "Loading Resource from XML Datasource for the specific day", System.Diagnostics.EventLogEntryType.Information);
#endif
            return LoadRoom(Date, booking.Room);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Booking/{Date}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public JSONBooking[] Book(string Date, JSONBooking booking)
        {
            try
            {
                XmlDocument doc = HAP.BookingSystem.BookingSystem.BookingsDoc;
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
                    HAP.BookingSystem.BookingSystem bs = new HAP.BookingSystem.BookingSystem(DateTime.Parse(Date));
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
                HAP.BookingSystem.BookingSystem.BookingsDoc = doc;
                Booking b = new HAP.BookingSystem.BookingSystem(DateTime.Parse(Date)).getBooking(booking.Room, booking.Lesson);
                if (config.SMTP.Enabled)
                {
                    iCalGenerator.Generate(b, DateTime.Parse(Date));
                    if (config.BookingSystem.Resources[booking.Room].EmailAdmins) iCalGenerator.Generate(b, DateTime.Parse(Date), true);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                HAP.Web.Logging.EventViewer.Log(HttpContext.Current.Request.RawUrl, e.ToString() + "\nMessage:\n" + e.Message + "\n\nStack Trace:\n" + e.StackTrace, System.Diagnostics.EventLogEntryType.Error);
#endif
            }
            return LoadRoom(Date, booking.Room);
        }

        [OperationContract]
        [WebGet(ResponseFormat=WebMessageFormat.Json, UriTemplate="/LoadRoom/{Date}/{Resource}")]
        public JSONBooking[] LoadRoom(string Date, string Resource)
        {
            Resource = HttpUtility.UrlDecode(Resource, System.Text.Encoding.Default).Replace("%20", " ");
            List<JSONBooking> bookings = new List<JSONBooking>();
            HAP.BookingSystem.BookingSystem bs = new HAP.BookingSystem.BookingSystem(DateTime.Parse(Date));
            foreach (Lesson lesson in hapConfig.Current.BookingSystem.Lessons)
                bookings.Add(new JSONBooking(bs.getBooking(Resource, lesson.Name)));
            WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
            return bookings.ToArray();
        }

        [OperationContract]
        [WebGet(ResponseFormat=WebMessageFormat.Json, UriTemplate="/Initial/{Date}/{Username}")]
        public int[] Initial(string Username, string Date)
        {
            if (!isAdmin(Username))
            {
                XmlDocument doc = HAP.BookingSystem.BookingSystem.BookingsDoc;
                int max = hapConfig.Current.BookingSystem.MaxBookingsPerWeek;
                foreach (AdvancedBookingRight right in HAP.BookingSystem.BookingSystem.BookingRights)
                    if (right.Username.ToLower() == Username.ToLower())
                        max = right.Numperweek;
                int x = 0;
                foreach (DateTime d in getWeekDates(DateTime.Parse(Date)))
                    x += doc.SelectNodes("/Bookings/Booking[@date='" + d.ToShortDateString() + "' and @username='" + Username + "']").Count;
                return new int[] { max - x, new HAP.BookingSystem.BookingSystem(DateTime.Parse(Date)).WeekNumber };
            }
            WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
            return new int[] { 0, new HAP.BookingSystem.BookingSystem(DateTime.Parse(Date)).WeekNumber };
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
                else if (HttpContext.Current.User.IsInRole(s.Trim())) return true;
            return false;
        }
    }
}
