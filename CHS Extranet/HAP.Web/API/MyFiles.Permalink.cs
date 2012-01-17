using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.routing;
using System.Web.Routing;
using System.Web.Compilation;
using System.Web.Security;
using System.IO;
using HAP.Data.ComputerBrowser;
using HAP.Web.Configuration;
using System.Xml;

namespace HAP.Web.API
{
    public class MyFiles_PermaLinkHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            MyFiles_PermaLink myu = new MyFiles_PermaLink();
            if (requestContext.RouteData.Values.ContainsKey("path")) myu.RoutingPath = requestContext.RouteData.Values["path"] as string;
            else myu.RoutingPath = string.Empty;
            myu.RoutingDrive = requestContext.RouteData.Values["drive"] as string;
            myu.RoutingDrive = myu.RoutingDrive.ToUpper();
            return myu;
        }
    }

    public class MyFiles_PermaLink : IHttpHandler, IMyComputerDisplay
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect(VirtualPathUtility.ToAbsolute("~/MyFiles/#") + RoutingDrive + "\\" + RoutingPath);
        }

        public string RoutingPath { get; set;}

        public string RoutingDrive { get; set; }
    }



}