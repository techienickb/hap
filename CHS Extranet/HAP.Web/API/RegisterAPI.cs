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
            RouteTable.Routes.Add(new Route("api/mycomputer/listdrives", new ListDrivesHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/list/{*path}", new ListHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/delete/{*path}", new DeleteHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/check/{*path}", new UploadCheckerHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/new/{*path}", new NewFolderHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/save/{*path}", new SaveHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/thumb/{*path}", new ThumbsHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/upload/{*path}", new UploadHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/zip/{*path}", new ZipHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/unzip/{*path}", new UnZipHandler()));
            RouteTable.Routes.Add(new Route("api/inf", new InfHandler()));
        }
    }
}