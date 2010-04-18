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
    public class HelpDeskRoutingHandler : IRouteHandler
    {
        public HelpDeskRoutingHandler()
        {
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (!UrlAuthorizationModule.CheckUrlAccessForPrincipal("~/HelpDesk/Default.aspx", requestContext.HttpContext.User, requestContext.HttpContext.Request.HttpMethod))
            {
                requestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                requestContext.HttpContext.Response.End();
            }

            ITicketDisplay display = BuildManager.CreateInstanceFromVirtualPath("~/HelpDesk/Default.aspx", typeof(Page)) as ITicketDisplay;
            display.TicketID = requestContext.RouteData.Values["ticket"] as string;

            return display;
        }
    }
}