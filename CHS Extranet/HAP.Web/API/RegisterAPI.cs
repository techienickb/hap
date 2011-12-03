using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;

namespace HAP.Web.API
{
    public class APIRoutes
    {
        public static void Register(RouteCollection collection)
        {
            RouteTable.Routes.Add(new Route("api/test", new TestHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/check/{*path}", new UploadCheckerHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/thumb/{*path}", new ThumbsHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/{ext}.ico", new IconHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/upload/{*path}", new UploadHandler()));
            WebServiceHostFactory factory = new WebServiceHostFactory();
            RouteTable.Routes.Add(new ServiceRoute("api/setup", factory, typeof(setup)));
            RouteTable.Routes.Add(new ServiceRoute("api/bookingsystem", factory, typeof(BookingSystem)));
            RouteTable.Routes.Add(new ServiceRoute("api/helpdesk", factory, typeof(HelpDesk)));
            RouteTable.Routes.Add(new ServiceRoute("api/myfiles", factory, typeof(MyFiles)));
        }
    }
}