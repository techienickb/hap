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

    public class Downloader : IMyComputerDisplay, IHttpHandler
    {

        private PrincipalContext pcontext;
        private UserPrincipal up;
        private hapConfig config;

        private bool isAuth(uncpath path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s);
                return vis;
            }
            return false;
        }

        private bool isWriteAuth(uncpath path)
        {
            if (path == null) return true;
            if (path.EnableWriteTo == "All") return true;
            else if (path.EnableWriteTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableWriteTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s);
                return vis;
            }
            return false;
        }


        public void ProcessRequest(HttpContext context)
        {
            config = hapConfig.Current;
            pcontext = HAP.AD.ADUtil.PContext;
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, HAP.AD.ADUtil.Username);

            string userhome = up.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            string path = RoutingPath.Replace('^', '&').Replace("%20", " ");
            uncpath unc = null;
            unc = config.MyComputer.UNCPaths[RoutingDrive];
            if (unc == null || !isAuth(unc)) context.Response.Redirect(context.Request.ApplicationPath + "/unauthorised.aspx", true);
            else path = string.Format(unc.UNC.Replace("%homepath%", up.HomeDirectory), HAP.AD.ADUtil.Username) + '\\' + path.Replace('/', '\\');
            FileInfo file = new FileInfo(path);
            context.Response.ContentType = MimeType(file.Extension);
            context.Response.Buffer = true;
            if (string.IsNullOrEmpty(context.Request.QueryString["inline"]))
                context.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + file.Name + "\"");
            else context.Response.AppendHeader("Content-Disposition", "inline; filename=\"" + file.Name + "\"");
            context.Response.AddHeader("Content-Length", file.Length.ToString());
            context.Response.Clear();
            context.Response.TransmitFile(file.FullName);
            context.Response.Flush();
            context.Response.Close();
            context.Response.End();
        }

        public static string MimeType(string Extension)
        {
            string mime = "application/octetstream";
            if (string.IsNullOrEmpty(Extension))
                return mime;
            string ext = Extension.ToLower();
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(ext);
            if (rk != null && rk.GetValue("Content Type") != null)
                mime = rk.GetValue("Content Type").ToString();
            return mime;

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }
    }
}