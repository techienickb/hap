using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHS_Extranet.routing
{
    interface IBookingSystemDisplay : IHttpHandler
    {
        string Room { get; set; }
    }
}