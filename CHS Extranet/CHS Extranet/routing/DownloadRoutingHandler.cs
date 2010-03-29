using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Compilation;
using System.Net;
using System.DirectoryServices.AccountManagement;
using CHS_Extranet.Configuration;
using System.Configuration;
using System.IO;
using Microsoft.Win32;

namespace CHS_Extranet.routing
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

        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private extranetConfig config;

        private bool isAuth(uncpath path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, s);
                    if (!vis) vis = up.IsMemberOf(gp);
                }
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
                {
                    GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, s);
                    if (!vis) vis = up.IsMemberOf(gp);
                }
                return vis;
            }
            return false;
        }

        public string Username
        {
            get
            {
                if (HttpContext.Current.User.Identity.Name.Contains('\\'))
                    return HttpContext.Current.User.Identity.Name.Remove(0, HttpContext.Current.User.Identity.Name.IndexOf('\\') + 1);
                else return HttpContext.Current.User.Identity.Name;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);

            string userhome = up.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            string path = RoutingPath.Replace('^', '&');
            uncpath unc = null;
            if (RoutingDrive == "N") path = up.HomeDirectory + '\\' + path.Replace('/', '\\');
            else
            {
                unc = config.UNCPaths[RoutingDrive];
                if (unc == null || !isAuth(unc)) context.Response.Redirect("/Extranet/unauthorised.aspx", true);
                else
                {
                    path = string.Format(unc.UNC, Username) + '\\' + path.Replace('/', '\\');
                }
            }
            FileInfo file = new FileInfo(path);
            context.Response.ContentType = MimeType(file.Extension);
            if (string.IsNullOrEmpty(context.Request.QueryString["inline"]))
                context.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + file.Name + "\"");
            else context.Response.AppendHeader("Content-Disposition", "inline; filename=\"" + file.Name + "\"");
            context.Response.AddHeader("Content-Length", file.Length.ToString("F0"));
            context.Response.Clear();
            context.Response.TransmitFile(file.FullName);
            context.Response.Flush();
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