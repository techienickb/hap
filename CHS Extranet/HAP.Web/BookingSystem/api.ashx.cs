using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.Configuration;
using HAP.BookingSystem;

namespace HAP.Web.BookingSystem
{
    /// <summary>
    /// Summary description for api
    /// </summary>
    public class api : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            hapConfig config = hapConfig.Current;
            if (context.Request.QueryString["op"] == "getBookings")
            {
                DateTime date = DateTime.Parse(context.Request.QueryString["d"]);
                HAP.BookingSystem.BookingSystem bs = new HAP.BookingSystem.BookingSystem(date);
                string format = "{0}:{1}:{2}:{3}";

                List<string> bookings = new List<string>();
                foreach (Resource br in config.BookingSystem.Resources.Values)
                    foreach (Lesson l in config.BookingSystem.Lessons)
                    {
                        Booking b = bs.getBooking(br.Name, l.Name);
                        bookings.Add(string.Format(format, b.Lesson, b.Name, b.Room, b.Username == "Not Booked" ? "" : b.User.Notes));
                    }
                context.Response.Write(string.Join("\n", bookings.ToArray()));
            }
            else if (context.Request.QueryString["op"] == "getTimes")
            {
                foreach (Lesson l in config.BookingSystem.Lessons)
                    context.Response.Write(string.Format("{0},{1},{2}\n", l.Name, l.StartTime, l.EndTime));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}