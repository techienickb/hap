using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Web.Routing;

namespace HAP.Web.API
{
    public class NewFolderHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new NewFolder(path, drive);
        }
    }

    public class NewFolder : IHttpHandler
    {
        public NewFolder(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }

        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            string path = Converter.DriveToUNC(RoutingPath, RoutingDrive);
            context.Response.Clear();
            context.Response.ContentType = "text/plain";
            try
            {
                Directory.CreateDirectory(path);
                context.Response.Write("OK");
            }
            catch (Exception e)
            {
                context.Response.Write("ERROR\n");
                context.Response.Write(e.ToString() + "\n");
                context.Response.Write(e.Message);
            }
        }
    }
}