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
            if (!UrlAuthorizationModule.CheckUrlAccessForPrincipal("~/Download/", requestContext.HttpContext.User, requestContext.HttpContext.Request.HttpMethod))
            {
                requestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                requestContext.HttpContext.Response.End();
            }

            hapConfig config = hapConfig.Current;
            string path = HttpUtility.UrlDecode(((string)requestContext.RouteData.Values["path"]).Replace('^', '&').Replace("|", "%"));
            DriveMapping unc = config.MyFiles.Mappings[((string)requestContext.RouteData.Values["drive"]).ToUpper().ToCharArray()[0]];
            path = Converter.FormatMapping(unc.UNC, ADUser) + '\\' + path.Replace('/', '\\');
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Download", requestContext.HttpContext.User.Identity.Name, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Downloading: " + path);

            Downloader downloader = new Downloader();
            if (requestContext.RouteData.Values.ContainsKey("path")) downloader.RoutingPath = requestContext.RouteData.Values["path"] as string;
            else downloader.RoutingPath = string.Empty;
            downloader.RoutingDrive = requestContext.RouteData.Values["drive"] as string;
            downloader.RoutingDrive = downloader.RoutingDrive.ToUpper();
            return downloader;
        }

        private HAP.AD.User _ADUser = null;
        public HAP.AD.User ADUser
        {
            get
            {
                if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == HAP.Web.Configuration.AuthMode.Windows) return ((HAP.AD.User)Membership.GetUser());
                if (_ADUser == null)
                {
                    _ADUser = new User();
                    HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                    if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                    _ADUser.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
                }
                return _ADUser;
            }
        }
    }

    public class Downloader : RangeRequestHandlerBase, IMyComputerDisplay
    {
        public override FileInfo GetRequestedFileInfo(HttpContext context)
        {
            config = hapConfig.Current;
            string path = HttpUtility.UrlDecode(RoutingPath.Replace('^', '&').Replace("|", "%"));
            DriveMapping unc = config.MyFiles.Mappings[RoutingDrive.ToCharArray()[0]];
            if (unc == null || !isAuth(unc)) context.Response.Redirect(VirtualPathUtility.ToAbsolute("~/unauthorised.aspx"), true);
            else path = Converter.FormatMapping(unc.UNC, ADUser) + '\\' + path.Replace('/', '\\');
            return new FileInfo(path);
        }

        public override string GetRequestedFileMimeType(HttpContext context)
        {
            config = hapConfig.Current;
            string path = HttpUtility.UrlDecode(RoutingPath.Replace('^', '&').Replace("|", "%"));
            DriveMapping unc = config.MyFiles.Mappings[RoutingDrive.ToCharArray()[0]];
            if (unc == null || !isAuth(unc)) context.Response.Redirect(VirtualPathUtility.ToAbsolute("~/unauthorised.aspx"), true);
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
            string mime = "application/octet-stream";
            //if (string.IsNullOrEmpty(Extension))
            //    return mime;
            //string ext = Extension.ToLower();
            //FileIcon fi;
            //if (FileIcon.TryGet(ext, out fi)) mime = fi.ContentType;
            //if (mime == "application/octetstream")
            //{
            //    RegistryKey rk = Registry.ClassesRoot.OpenSubKey(ext);
            //    if (rk != null && rk.GetValue("Content Type") != null)
            //        mime = rk.GetValue("Content Type").ToString();
            //}
            //if (mime.Length == 0) mime = "application/octet-stream";
            return mime;

        }

        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }
    }
}