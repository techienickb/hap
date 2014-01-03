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
    public class TileIconHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new TileIcon(requestContext.RouteData.GetRequiredString("width"), requestContext.RouteData.GetRequiredString("height"), requestContext.RouteData.GetRequiredString("path"));
        }
    }

    public class TileIcon : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        public TileIcon(Size size, string icon)
        {
            Size = size;
            Icon = "~/" + icon;
        }
        public TileIcon(string width, string height, string icon)
        {
            Size = new Size(int.Parse(width), int.Parse(height));
            Icon = "~/" + icon;
        }
        public string Icon { get; set; }
        public Size Size { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/png";
            string file = HAP.Web.LiveTiles.IconCache.GetIcon(Icon, Size);
            DateTime lastModified = File.GetLastWriteTimeUtc(file);
            lastModified = new DateTime(lastModified.Year, lastModified.Month, lastModified.Day, lastModified.Hour, lastModified.Minute, lastModified.Second, 0, DateTimeKind.Utc);

            context.Response.AddFileDependency(file);
            context.Response.Cache.SetETagFromFileDependencies();
            context.Response.Cache.SetLastModifiedFromFileDependencies();
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            int status = 200;
            if (context.Request.Headers["If-Modified-Since"] != null)
            {
                status = 304;
                DateTime modifiedSinceDate = DateTime.UtcNow;
                if (DateTime.TryParse(context.Request.Headers["If-Modified-Since"], out modifiedSinceDate))
                {
                    if (lastModified != modifiedSinceDate)
                        status = 200;
                }
            }
            context.Response.StatusCode = status;
            if (status == 200)
            {
                MemoryStream m = new MemoryStream();
                Image.FromFile(HAP.Web.LiveTiles.IconCache.GetIcon(Icon, Size)).Save(m, ImageFormat.Png);
                m.WriteTo(context.Response.OutputStream);
            }
        }

    }
}