using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace HAP.Web.API
{
    public class api : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new APIHandler();   
        }
    }

    public class APIHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write("<!DOCTYPE html><html><head><title>Home Access Plus+ - APIs</title><link href=\"" + VirtualPathUtility.ToAbsolute("~/") + "style/basestyle.css\" rel=\"stylesheet\" type=\"text/css\" /></head><body><div id=\"hapContent\"><h1>Home Access Plus+ - APIs</h1>");
            context.Response.Write("<ol>");
            foreach (Route r in RouteTable.Routes)
                if (r is ServiceRoute) context.Response.Write(string.Format("<li><a href=\"../{2}\">{0}</a>: {1}</li>", r.Url.Replace("{*pathInfo}", ""), r.RouteHandler.GetType().Name, r.Url.Replace("{*pathInfo}", "help")));
                else context.Response.Write(string.Format("<li>{0}: {1}</li>", r.Url, r.RouteHandler.GetType().Name));
            context.Response.Write("</ol></div></body></html>");
        }

        public List<string> APIs { get; set; }
    }
}