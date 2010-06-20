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

namespace HAP.Web.API
{
    public class DeleteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new Delete(path, drive);
        }
    }

    public class Delete : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public Delete(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }
        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            Context = context;
            config = hapConfig.Current;

            context.Response.Clear();
            context.Response.Headers.Add("HAP:API", "Delete");
            context.Response.ContentType = "text/plain";
            try
            {

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
                if (userhome.EndsWith("\\")) userhome = userhome.Remove(userhome.LastIndexOf('\\'));
                string path = "";
                uncpath unc = null;
                if (RoutingDrive == "N") path = up.HomeDirectory + RoutingPath;
                else
                {
                    unc = config.MyComputer.UNCPaths[RoutingDrive];
                    if (unc == null || !isWriteAuth(unc)) { context.Response.Write("ERROR: Unauthorised"); context.Response.End(); return; }
                    else
                    {
                        path = string.Format(unc.UNC, Username) + RoutingPath;
                    }
                }

                path = path.TrimEnd(new char[] { '\\' }).Replace('^', '&').Replace('/', '\\');

                try { File.Delete(path); }
                catch { Directory.Delete(path, true); }

                context.Response.Write("DONE");
            }
            catch (Exception e)
            {
                context.Response.Write("ERROR: " + e.ToString() + "\\n" + e.Message);
            }
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

        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
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