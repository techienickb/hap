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
    public class UploadCheckerHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new UploadChecker(path, drive);
        }
    }

    public class UploadChecker : IHttpHandler
    {
        public UploadChecker(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }

        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            FileInfo file = new FileInfo(Converter.DriveToUNC(RoutingPath, RoutingDrive));
            context.Response.Clear();
            context.Response.ExpiresAbsolute = DateTime.Now;
            context.Response.ContentType = "text/plain";
            context.Response.Write(file.Exists ? "EXISTS" : "OK");
            context.Response.Write("\n");
            context.Response.Write("images/icons/" + MyComputerItem.ParseForImage(file));
        }
    }
}