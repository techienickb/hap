using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.Security;
using System.Net;

namespace HAP.Web.routing
{
    public class MyComputerRoutingHandler : IRouteHandler
    {
        private bool _drive;
        public MyComputerRoutingHandler(bool drive)
        {
            _drive = drive;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (!UrlAuthorizationModule.CheckUrlAccessForPrincipal("~/MyComputer.aspx", requestContext.HttpContext.User, requestContext.HttpContext.Request.HttpMethod))
            {
                requestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                requestContext.HttpContext.Response.End();
            }

            IMyComputerDisplay display = BuildManager.CreateInstanceFromVirtualPath("~/MyComputer.aspx", typeof(Page)) as IMyComputerDisplay;
            if (requestContext.RouteData.Values.ContainsKey("path")) display.RoutingPath = requestContext.RouteData.Values["path"] as string;
            else display.RoutingPath = string.Empty;
            display.RoutingDrive = requestContext.RouteData.Values["drive"] as string;
            display.RoutingDrive = display.RoutingDrive.ToUpper();

            return display;
        }
    }
}