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
        [WebGet(ResponseFormat=WebMessageFormat.Json, UriTemplate="/LoadRoom/{Date}/{Room}")]
        public JSONBooking[] LoadRoom(string Date, string Room)
        {
            List<JSONBooking> bookings = new List<JSONBooking>();
            HAP.Data.BookingSystem.BookingSystem bs = new HAP.Data.BookingSystem.BookingSystem(DateTime.Parse(Date));
            foreach (Lesson lesson in hapConfig.Current.BookingSystem.Lessons)
                bookings.Add(new JSONBooking(bs.getBooking(Room, lesson.Name)));
            return bookings.ToArray();
        }

        [OperationContract]
        [WebGet(ResponseFormat=WebMessageFormat.Json, UriTemplate="/BookingCount/{Date}/{Username}")]
        public int BookingCount(string Username, string Date)
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
                return max - x;
            }

            return 0;
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
