using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace HAP.Web.API
{
    public class APIRoutes
    {
        public static void Register(RouteCollection collection)
        {
            RouteTable.Routes.Add(new Route("api/mycomputer/check/{*path}", new UploadCheckerHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/thumb/{*path}", new ThumbsHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/{ext}.ico", new IconHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/upload/{*path}", new UploadHandler()));
        }
    }
}