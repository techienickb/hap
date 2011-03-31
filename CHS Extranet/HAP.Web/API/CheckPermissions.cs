using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.AccessControl;
using System.Web.Routing;
using HAP.Web.Configuration;
using System.IO;
using HAP.Data.ComputerBrowser;

namespace HAP.Web.API
{
    public class CheckPermissions : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new PermissionHandler(path, drive);
        }
    }

    public class PermissionHandler : IHttpHandler
    {
        public PermissionHandler(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }
        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }
        private string CheckSecurity(DirectoryInfo info)
        {
            DirectorySecurity DirSec = info.GetAccessControl(AccessControlSections.Access);
            string rule = "";
            foreach (FileSystemAccessRule FSAR in DirSec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
            {
                rule += string.Format("Account: {0}\nType: {1}\nRights: {2}\nInherited: {3}\nIs User: {4}\n\n", FSAR.IdentityReference.Value, FSAR.AccessControlType, FSAR.FileSystemRights, FSAR.IsInherited, isUser(FSAR.IdentityReference.Value));
            }

            return rule;
        }

        private bool isUser(string reference)
        {
            if (reference.ToUpper() == Context.User.Identity.Name.ToUpper() || reference == "NT AUTHORITY\\Authenticated Users" || reference == "Everyone") return true;
            else
            {
                string r = reference;
                if (r.Contains('\\')) r = r.Remove(0, r.IndexOf('\\') + 1);
                return Context.User.IsInRole(r);
            }
        }

        private hapConfig config;
        private HttpContext Context;

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            Context = context;
            config = hapConfig.Current;
            uncpath unc; string userhome;
            string path = Converter.DriveToUNC(RoutingPath, RoutingDrive, out unc, out userhome);
            DirectoryInfo dir = new DirectoryInfo(path);
            context.Response.Write(CheckSecurity(dir));
            context.Response.ContentType = "text/plain";
        }
    }
}