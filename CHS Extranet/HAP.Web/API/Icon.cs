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
    public class IconHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new Icon(requestContext.RouteData.GetRequiredString("ext"));
        }
    }

    public class Icon : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public Icon(string extension)
        {
            Extension = extension;
        }
        public string Extension { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/png";
            try
            {
                Bitmap b = new Bitmap(48, 48);
                Graphics g = Graphics.FromImage(b);
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Icon i = IconHelper.ExtractIconForExtension("." + Extension, true);
                g.DrawIcon(i, new Rectangle(0, 0, 48, 48));
                g.Flush();
                System.IO.MemoryStream mem = new System.IO.MemoryStream();
                b.Save(mem, ImageFormat.Png);
                mem.WriteTo(context.Response.OutputStream);
            }
            catch { context.Response.TransmitFile(context.Server.MapPath("~/images/icons/file.png")); }
        }

    }
}