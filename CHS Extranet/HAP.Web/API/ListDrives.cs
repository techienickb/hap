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
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.AccessControl;

namespace HAP.Web.API
{
    public class ListDrivesHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ListDrives();
        }
    }

    public class ListDrives : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            Context = context;
            config = hapConfig.Current;

            context.Response.Clear();
            context.Response.ExpiresAbsolute = DateTime.Now;
            context.Response.ContentType = "text/plain";
            string format = "{0}|{1}|{2}|{3}{4}\n";
            long freeBytesForUser, totalBytes, freeBytes;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            string _ActiveDirectoryConnectionString = "";
            string _DomainDN = "";
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            UserPrincipal up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);

            string userhome = up.HomeDirectory;
            foreach (uncpath path in config.MyComputer.UNCPaths)
            {
                string space = "";
                bool showspace = false;
                if (context.User.IsInRole("Domain Admins") || !path.UNC.Contains("%homepath%")) showspace = isWriteAuth(path);
                if (showspace)
                {
                    if (Win32.GetDiskFreeSpaceEx(string.Format(path.UNC.Replace("%homepath%", userhome), Username), out freeBytesForUser, out totalBytes, out freeBytes))
                        space = "|" + Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                    else space = "";
                }
                if (isAuth(path)) context.Response.Write(string.Format(format, path.Name, "/extranet/images/icons/netdrive.png", string.Format("/Extranet/api/mycomputer/list/{0}", path.Drive), isWriteAuth(path) ? AccessControlActions.Change : AccessControlActions.View, space));
            }

            foreach (uploadfilter filter in config.MyComputer.UploadFilters)
                if (isAuth(filter)) context.Response.Write("FILTER" + filter.ToString().Replace('\\', ',') + "\n");

            context.Response.Write("INFOName:" + Assembly.GetExecutingAssembly().GetName().Name + "|Version:" + Assembly.GetExecutingAssembly().GetName().Version.ToString());

            context.Response.ContentType = "text/plain";
        }

        internal static class Win32
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);

        }

        private bool isAuth(uploadfilter filter)
        {
            if (filter.EnableFor == "All") return true;
            else if (filter.EnableFor != "None")
            {
                bool vis = false;
                foreach (string s in filter.EnableFor.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Context.User.IsInRole(s);
                return vis;
            }
            return false;
        }

        private bool isAuth(uncpath path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Context.User.IsInRole(s);
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
                    if (!vis) vis = Context.User.IsInRole(s);
                return vis;
            }
            return false;
        }

        private hapConfig config;
        private HttpContext Context;

        public string Username
        {
            get
            {
                if (Context.User.Identity.Name.Contains('\\'))
                    return Context.User.Identity.Name.Remove(0, Context.User.Identity.Name.IndexOf('\\') + 1);
                else return Context.User.Identity.Name;
            }
        }
    }
}