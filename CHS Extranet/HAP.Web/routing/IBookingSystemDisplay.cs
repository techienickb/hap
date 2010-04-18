using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.Web.routing
{
    interface IBookingSystemDisplay : IHttpHandler
    {
        string Room { get; set; }
    }
}