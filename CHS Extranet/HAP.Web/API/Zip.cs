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
    public class ZipHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new Zip(path, drive);
        }
    }

    public class Zip : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public Zip(string path, string drive)
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
                StreamReader sr = new StreamReader(context.Request.InputStream);
                string c = sr.ReadToEnd();
                ZipFile zf;
                if (File.Exists(path))
                    zf = new ZipFile(path);
                else zf = ZipFile.Create(path);
                zf.BeginUpdate();
                foreach (string s in c.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string p = Converter.DriveToUNC(s);
                    if (File.Exists(p))
                        zf.Add(p);
                    else if (Directory.Exists(p))
                        zf.AddDirectory(p);
                }
                zf.CommitUpdate();
                zf.Close();
                context.Response.Write("DONE");
            }
            catch (Exception e)
            {
                context.Response.Write("ERROR: " + e.ToString() + "\\n" + e.Message);
            }
        }


    }
}