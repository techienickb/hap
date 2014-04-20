using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.routing;
using System.Web.Routing;
using System.Web.Compilation;
using System.Web.Security;
using System.IO;
using HAP.Data.ComputerBrowser;
using HAP.Web.Configuration;
using System.Xml;
using HAP.AD;

namespace HAP.Web.API
{
    public class HelpDesk_UploadHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HelpDesk_Upload();
        }
    }

    public class HelpDesk_Upload : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request.Headers["X_FILENAME"]))
            {
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "HelpDesk.Upload", context.User.Identity.Name, context.Request.UserHostAddress, context.Request.Browser.Platform, context.Request.Browser.Browser + " " + context.Request.Browser.Version, context.Request.UserHostName, "Uploading of: " + context.Request.Headers["X_FILENAME"]);
                Stream inputStream = context.Request.InputStream;
                MemoryStream memory = new MemoryStream();
                inputStream.CopyTo(memory);
                HttpContext.Current.Cache.Insert("hap-HD-" + context.Request.Headers["X_FILENAME"], memory.ToArray(), null, DateTime.UtcNow.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration);
                context.Response.Write(context.Request.Headers["X_FILENAME"]);
            }
            else throw new ArgumentNullException("No File Attached!");
        }
    }



}