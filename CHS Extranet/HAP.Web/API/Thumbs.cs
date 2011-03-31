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
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using HAP.Data.ComputerBrowser;

namespace HAP.Web.API
{
    public class ThumbsHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new Thumbs(path, drive);
        }
    }

    public class Thumbs : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public Thumbs(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }
        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            Context = context;
            config = hapConfig.Current;
            uncpath unc; string userhome;
            string path = Converter.DriveToUNC(RoutingPath, RoutingDrive, out unc, out userhome);
            FileInfo file = new FileInfo(path);

            Image image = Image.FromFile(file.FullName);
            Image thumb = image.GetThumbnailImage(64, 64, new Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            MemoryStream memstr = new MemoryStream();
            thumb.Save(memstr, ImageFormat.Png);
            context.Response.Clear();
            context.Response.ExpiresAbsolute = DateTime.Now;
            context.Response.ContentType = Converter.MimeType(".png");
            context.Response.Buffer = true;
            context.Response.AppendHeader("Content-Disposition", "inline; filename=\"" + file.Name + "\"");
            context.Response.AddHeader("Content-Length", memstr.Length.ToString());
            context.Response.Clear();
            memstr.WriteTo(context.Response.OutputStream);
            context.Response.Flush();
            context.Response.Close();
            context.Response.End();
        }

        public bool ThumbnailCallback()
        {
            return true;
        }

        private hapConfig config;
        private HttpContext Context;
    }
}