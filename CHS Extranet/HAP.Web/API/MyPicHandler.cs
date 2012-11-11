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
using System.DirectoryServices;

namespace HAP.Web.API
{
    public class MyPicHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MyPic();
        }
    }

    public class MyPic : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                using (DirectorySearcher dsSearcher = new DirectorySearcher())
                {
                    dsSearcher.Filter = "(&(objectClass=user) (cn=" + context.User.Identity.Name + "))";
                    SearchResult result = dsSearcher.FindOne();

                    using (DirectoryEntry user = new DirectoryEntry(result.Path))
                    {
                        byte[] data = user.Properties["jpegPhoto"].Value as byte[];

                        if (data != null)
                        {
                            using (MemoryStream s = new MemoryStream(data))
                            {
                                context.Response.ContentType = "image/png";
                                MemoryStream m = new MemoryStream();
                                Bitmap.FromStream(s).Save(m, ImageFormat.Png);
                                m.WriteTo(context.Response.OutputStream);
                            }
                        }
                        else context.Response.Redirect("~/api/tiles/icons/128/128/images/icons/metro/folders-os/UserNo-Frame.png");
                    }
                }
            }
            else context.Response.Redirect("~/api/tiles/icons/128/128/images/icons/metro/folders-os/UserNo-Frame.png");
        }

    }
}