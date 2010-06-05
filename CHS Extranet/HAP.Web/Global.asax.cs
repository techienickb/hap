using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using HAP.Web.routing;

namespace HAP.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            API.APIRoutes.Register(RouteTable.Routes);
            RouteTable.Routes.Add(new Route("mycomputer/{drive}", new MyComputerRoutingHandler(true)));
            RouteTable.Routes.Add(new Route("mycomputer/{drive}/{*path}", new MyComputerRoutingHandler(false)));
            RouteTable.Routes.Add(new Route("download/{drive}/{*path}", new DownloadRoutingHandler()));
            RouteTable.Routes.Add(new Route("upload/check/{drive}/{*path}", new UploadHandler("check")));
            RouteTable.Routes.Add(new Route("upload/transfer/{*path}", new UploadHandler("transfer")));
            RouteTable.Routes.Add(new Route("preview/{drive}/{*path}", new PreviewRoutingHandler()));
            RouteTable.Routes.Add(new Route("helpdesk/ticket/{ticket}", new HelpDeskRoutingHandler()));
            RouteTable.Routes.Add(new Route("bookingsystem/{room}/display", new BookingSystemDislayRoutingHandler()));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}