using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.Security;
using System.Net;
using HAP.Web.routing;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Xml;
using System.IO;

namespace HAP.Web.API
{
    public class DeleteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new Delete(path, drive);
        }
    }

    public class Delete : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public Delete(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }
        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ExpiresAbsolute = DateTime.Now;
            context.Response.ContentType = "text/plain";
            try
            {

                string path = Converter.DriveToUNC(RoutingPath, RoutingDrive);

                path = path.TrimEnd(new char[] { '\\' }).Replace('^', '&').Replace('/', '\\');

                try { File.Delete(path); }
                catch { Directory.Delete(path, true); }

                context.Response.Write("DONE");
            }
            catch (Exception e)
            {
                context.Response.Write("ERROR: " + e.ToString() + "\\n" + e.Message);
            }
        }
    }
}