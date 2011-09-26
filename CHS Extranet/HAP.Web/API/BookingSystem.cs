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

    }
}
