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
    public class Home_PermaLinkHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            Home_PermaLink myu = new Home_PermaLink { Hash =  requestContext.RouteData.GetRequiredString("Hash");}
            return myu;
        }
    }

    public class Home_PermaLink : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public string Hash { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect(VirtualPathUtility.ToAbsolute("~/#") + this.Hash);
        }
    }



}