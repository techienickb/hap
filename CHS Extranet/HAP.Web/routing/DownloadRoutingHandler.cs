using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Compilation;
using System.Net;
using System.DirectoryServices.AccountManagement;
using HAP.Web.Configuration;
using System.Configuration;
using System.IO;
using Microsoft.Win32;
using HAP.Data.ComputerBrowser;
using System.Text;
using System.Web.SessionState;
using HAP.AD;

namespace HAP.Web.routing
{
    public class DownloadRoutingHandler : IRouteHandler
    {
        public DownloadRoutingHandler()
        {
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (!UrlAuthorizationModule.CheckUrlAccessForPrincipal("~/f.ashx", requestContext.HttpContext.User, requestContext.HttpContext.Request.HttpMethod))
            {
                requestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                requestContext.HttpContext.Response.End();
            }

            Downloader downloader = new Downloader();
            if (requestContext.RouteData.Values.ContainsKey("path")) downloader.RoutingPath = requestContext.RouteData.Values["path"] as string;
            else downloader.RoutingPath = string.Empty;
            downloader.RoutingDrive = requestContext.RouteData.Values["drive"] as string;
            downloader.RoutingDrive = downloader.RoutingDrive.ToUpper();

            return downloader;
        }
    }

    public class Downloader : RangeRequestHandlerBase, IMyComputerDisplay
    {
        public override FileInfo GetRequestedFileInfo(HttpContext context)
        {
            config = hapConfig.Current;
            string path = RoutingPath.Replace('^', '&').Replace("%20", " ");
            DriveMapping unc = config.MySchoolComputerBrowser.Mappings[RoutingDrive.ToCharArray()[0]];
            if (unc == null || !isAuth(unc)) context.Response.Redirect(context.Request.ApplicationPath + "/unauthorised.aspx", true);
            else path = Converter.FormatMapping(unc.UNC, ADUser) + '\\' + path.Replace('/', '\\');
            return new FileInfo(path);
        }

        public override string GetRequestedFileMimeType(HttpContext context)
        {
            config = hapConfig.Current;
            string path = RoutingPath.Replace('^', '&').Replace("%20", " ");
            DriveMapping unc = config.MySchoolComputerBrowser.Mappings[RoutingDrive.ToCharArray()[0]];
            if (unc == null || !isAuth(unc)) context.Response.Redirect(context.Request.ApplicationPath + "/unauthorised.aspx", true);
            else path = Converter.FormatMapping(unc.UNC, ADUser) + '\\' + path.Replace('/', '\\');
            return MimeType(Path.GetExtension(path));
        }

        private hapConfig config;

        private bool isAuth(DriveMapping path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        private bool isWriteAuth(DriveMapping path)
        {
            if (path == null) return true;
            if (path.EnableWriteTo == "All") return true;
            else if (path.EnableWriteTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableWriteTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        public static string MimeType(string Extension)
        {
            string mime = "application/octetstream";
            if (string.IsNullOrEmpty(Extension))
                return mime;
            string ext = Extension.ToLower();
            FileIcon fi;
            if (FileIcon.TryGet(ext, out fi)) mime = fi.ContentType;
            if (mime == "application/octetstream")
            {
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(ext);
                if (rk != null && rk.GetValue("Content Type") != null)
                    mime = rk.GetValue("Content Type").ToString();
            }
            if (mime.Length == 0) mime = "application/octetstream";
            return mime;

        }

        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }
    }
}