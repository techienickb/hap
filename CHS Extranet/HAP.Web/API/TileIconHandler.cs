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
            Icon = icon;
        }
        public TileIcon(string width, string height, string icon)
        {
            Size = new Size(int.Parse(width), int.Parse(height));
            Icon = icon;
        }
        public string Icon { get; set; }
        public Size Size { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/png";
            MemoryStream m = new MemoryStream();
            Image.FromFile(context.Server.MapPath(HAP.Web.LiveTiles.IconCache.GetIcon(Icon, Size))).Save(m, ImageFormat.Png);
            m.WriteTo(context.Response.OutputStream);
        }

    }
}