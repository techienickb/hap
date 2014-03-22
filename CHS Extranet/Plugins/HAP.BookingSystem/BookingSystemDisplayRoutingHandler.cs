using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.Security;
using System.Net;

namespace HAP.BookingSystem
{
    public class BookingSystemDislayRoutingHandler : IRouteHandler
    {
        public BookingSystemDislayRoutingHandler()
        {
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            IBookingSystemDisplay display = BuildManager.CreateInstanceFromVirtualPath("~/BookingSystem/Display.aspx", typeof(Page)) as IBookingSystemDisplay;
            display.Room = requestContext.RouteData.Values["room"] as string;

            return display;
        }
    }
}