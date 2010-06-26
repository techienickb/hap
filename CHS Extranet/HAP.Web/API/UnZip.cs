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
using ICSharpCode.SharpZipLib.Zip;

namespace HAP.Web.API
{
    public class UnZipHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new UnZip(path, drive);
        }
    }

    public class UnZip : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public UnZip(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }
        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.Headers.Add("HAP:API", "UNZIP");
            context.Response.ContentType = "text/plain";

            try
            {
                string path = Converter.DriveToUNC(RoutingPath, RoutingDrive);
                StreamReader sr = new StreamReader(context.Request.InputStream);
                string c = sr.ReadToEnd();
                c = Converter.DriveToUNC(c);
                if (!Directory.Exists(c)) Directory.CreateDirectory(c);
                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(path, c, "");
                context.Response.Write("DONE");
            }
            catch (Exception e)
            {
                context.Response.Write("ERROR: " + e.ToString() + "\\n" + e.Message);
            }
        }


    }
}