using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.BookingSystem
{
    interface IBookingSystemDisplay : IHttpHandler
    {
        string Room { get; set; }
    }
}