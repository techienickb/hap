using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.Web.routing
{
    interface ITicketDisplay : IHttpHandler
    {
        string TicketID { get; set; }
    }
}