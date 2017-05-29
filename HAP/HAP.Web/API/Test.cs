using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Web.Routing;
using HAP.Data.ComputerBrowser;
using System.Web.Security;
using System.Web.SessionState;

namespace HAP.Web.API
{
    public class TestHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new Test();
        }
    }

    public class Test : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(context.Request.QueryString["1"]))
                {
                    File.CreateText(context.Server.MapPath("~/app_data/test.tmp")).Close();
                    File.Delete(context.Server.MapPath("~/app_data/test.tmp"));
                }
                else
                {
                    if (!context.User.Identity.IsAuthenticated) context.Response.Redirect("~/unauthorised.aspx");
                }
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                context.Response.Write("OK");
            }
            catch 
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                context.Response.Write("WriteAccess");
            }
        }
    }
}