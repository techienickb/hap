using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using HAP.Web.routing;
using System.Diagnostics;

namespace HAP.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            HttpContext.Current.Cache.Insert("hapBannedIps", new List<Banned>());
            API.APIRoutes.Register(RouteTable.Routes);
            RouteTable.Routes.Add(new Route("download/{drive}/{*path}", new DownloadRoutingHandler()));
            RouteTable.Routes.Add(new Route("bookingsystem/r-{resource}", new PageRouteHandler("~/bookingsystem/default.aspx", true)));
            RouteTable.Routes.Add(new Route("bookingsystem/t-{type}", new PageRouteHandler("~/bookingsystem/default.aspx", true)));
            RouteTable.Routes.Add(new Route("bookingsystem/{room}/display", new BookingSystemDislayRoutingHandler()));
            RouteTable.Routes.Add(new Route("tracker/web/{year}/{month}/", new PageRouteHandler("~/tracker/weblog.aspx", true)));
            RouteTable.Routes.Add(new Route("tracker/web/{year}/{month}/d/{day}", new PageRouteHandler("~/tracker/weblog.aspx", true)));
            RouteTable.Routes.Add(new Route("tracker/{year}/{month}/", new PageRouteHandler("~/tracker/log.aspx", true)));
            RouteTable.Routes.Add(new Route("tracker/{year}/{month}/c/{computer}/", new PageRouteHandler("~/tracker/log.aspx", true)));
            RouteTable.Routes.Add(new Route("tracker/{year}/{month}/d/{day}/", new PageRouteHandler("~/tracker/log.aspx", true)));
            RouteTable.Routes.Add(new Route("tracker/{year}/{month}/c/{computer}/d/{day}/", new PageRouteHandler("~/tracker/log.aspx", true)));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Context.Request.Path.StartsWith(Context.Request.ApplicationPath + "/api/"))
                Context.SetSessionStateBehavior(SessionStateBehavior.Disabled);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception myError = null;
            if (HttpContext.Current.Server.GetLastError() != null)
            {
                string eventSource = HttpContext.Current.Request.RawUrl;
                string myErrorMessage = "";

                myError = Server.GetLastError();

                while (myError.InnerException != null)
                {
                    myErrorMessage += "Message\r\n" +
                        myError.Message.ToString() + "\r\n\r\n";
                    myErrorMessage += "Source\r\n" +
                        myError.Source + "\r\n\r\n";
                    myErrorMessage += "Target site\r\n" +
                        myError.TargetSite.ToString() + "\r\n\r\n";
                    myErrorMessage += "Stack trace\r\n" +
                        myError.StackTrace + "\r\n\r\n";
                    myErrorMessage += "ToString()\r\n\r\n" +
                        myError.ToString();

                    // Assign the next InnerException
                    // to catch the details of that exception as well
                    myError = myError.InnerException;
                }

                HAP.Web.Logging.EventViewer.Log(eventSource, myErrorMessage, EventLogEntryType.Error);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}