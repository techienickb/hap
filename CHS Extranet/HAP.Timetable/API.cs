using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using System.Xml;
using HAP.Web.Configuration;
using HAP.AD;
using Microsoft.Exchange.WebServices.Data;

namespace HAP.Timetable
{
    [ServiceAPI("api/timetable")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class API
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Load/{UPN}")]
        public TimetableDay[] Load(string UPN)
        {
            return Timetables.getRecords(UPN);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/LoadUser")]
        public TimetableDay[] LoadUser()
        {
            User u = new User(HttpContext.Current.User.Identity.Name);
            if (HttpContext.Current.User.IsInRole(hapConfig.Current.AD.StudentsGroup))
                return Timetables.getRecords(u.EmployeeID);
            return new TimetableDay[] { };
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/LoadUser/{Username}")]
        public TimetableDay[] LoadUserByUser(string Username)
        {
            return Timetables.getRecords(new User(Username).EmployeeID);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Week/{Date}")]
        public int WeekNumber(string date)
        {
            int ret = -1;
            for (int i = 0; i < 5; i++)
            {
                ret = new BookingSystem.BookingSystem(DateTime.Parse(date).AddDays(i).Date).WeekNumber;
                if (ret > -1) break;
            }
            return ret;
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Load/{From}/{To}")]
        public hapAppointment[] LoadFromTo(string From, string To)
        {
            List<hapAppointment> apps = new List<hapAppointment>();
            foreach (Appointment a in HAP.Web.LiveTiles.ExchangeConnector.Appointments(DateTime.Parse(From), DateTime.Parse(To)))
                apps.Add(hapAppointment.Parse(a));
            return apps.ToArray();
        }

        [OperationContract]
        [WebInvoke(Method = "POST",ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/Load/{From}/{To}")]
        public hapAppointment[] LoadFromToCal(string From, string To, OptionalCalendar[] Calendar)
        {
            List<hapAppointment> apps = new List<hapAppointment>();
            foreach (OptionalCalendar cal in Calendar)
            {
                string[] roles = cal.Roles.Split(new char[] { ',' });
                bool con = true;
                foreach (string s in roles)
                {
                    string s1 = s.Trim();
                    bool ans = true;
                    if (s1.StartsWith("!")) { ans = false; s1 = s1.Remove(0, 1); }
                    if (s1.StartsWith("[")) s1 = hapConfig.Current.AD.StudentsGroup;
                    con = HttpContext.Current.User.IsInRole(s1) == ans;
                    if (con) break;
                }

                if (con) foreach (Appointment a in HAP.Web.LiveTiles.ExchangeConnector.Appointments(DateTime.Parse(From), DateTime.Parse(To), cal.Calendar))
                        apps.Add(hapAppointment.Parse(a, cal.Color));
            }
            return apps.ToArray();
        }

    }

    public class OptionalCalendar
    {
        public string Calendar { get; set; }
        public string Roles { get; set; }
        public string Color { get; set; }
    }
}