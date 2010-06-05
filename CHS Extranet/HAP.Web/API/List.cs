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

namespace HAP.Web.API
{
    public class ListHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new List(path, drive);
        }
    }

    public class List : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public List(string path, string drive)
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
                if (unc == null || !isAuth(unc)) { context.Response.Write("ERROR: Unauthorised"); context.Response.End(); return; }
                else
                {
                    path = string.Format(unc.UNC, Username) + RoutingPath;
                }
            }

            path = path.TrimEnd(new char[] { '\\' }).Replace('^', '&').Replace('/', '\\');
            DirectoryInfo dir = new DirectoryInfo(path);
            //name,icon,size,type,path,canwrite
            string format = "{0},/extranet/images/icons/{1},{2},{3},{4},{5}\n";

            context.Response.Clear();
            context.Response.Headers.Add("HAP:API", "List");
            context.Response.ContentType = "text/plain";

            bool allowedit = isWriteAuth(unc);

            if (string.IsNullOrEmpty(RoutingPath))
                context.Response.Write(string.Format(format, "My Computer", "school.png", "Back to My Computer", "Drive", "/Extranet/api/MyComputer/listdrives", false));
            else context.Response.Write(string.Format(format, "..", "folder.png", "Up a Folder", "File Folder", "/Extranet/api/MyComputer/list/" + (RoutingDrive + "/" + RoutingPath).Replace("//", "/").Remove((RoutingDrive + "/" + RoutingPath).LastIndexOf('/') - 1), allowedit));

            try
            {
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                    try
                    {
                        if (!subdir.Name.ToLower().Contains("recycle") && subdir.Attributes != FileAttributes.Hidden && subdir.Attributes != FileAttributes.System && !subdir.Name.ToLower().Contains("system volume info"))
                        {
                            string dirpath = subdir.FullName;
                            if (unc == null) dirpath = dirpath.Replace(userhome, "N");
                            else dirpath = dirpath.Replace(string.Format(unc.UNC, Username), unc.Drive);
                            dirpath = dirpath.Replace('\\', '/').Replace("//", "/"); ;
                            context.Response.Write(string.Format(format, subdir.Name, MyComputerItem.ParseForImage(subdir), "", "File Folder", "/Extranet/api/MyComputer/list/" + dirpath.Replace('&', '^'), allowedit));
                        }
                    }
                    catch { }

                foreach (FileInfo file in dir.GetFiles())
                {
                    try
                    {
                        if (!file.Name.ToLower().Contains("thumbs") && checkext(file.Extension) && file.Attributes != FileAttributes.Hidden && file.Attributes != FileAttributes.System)
                        {
                            string filetype = "File";
                            string filename = file.Name + (file.Name.Contains(file.Extension) ? "" : file.Extension);
                            try
                            {
                                RegistryKey rkRoot = Registry.ClassesRoot;
                                string keyref = rkRoot.OpenSubKey(file.Extension).GetValue("").ToString();
                                filetype = rkRoot.OpenSubKey(keyref).GetValue("").ToString();
                                filename = filename.Replace(file.Extension, "");
                            }
                            catch { filetype = "File"; }

                            string dirpath = file.FullName;
                            if (unc == null) dirpath = dirpath.Replace(userhome, "N");
                            else dirpath = dirpath.Replace(string.Format(unc.UNC, Username), unc.Drive);
                            dirpath = dirpath.Replace('\\', '/').Replace("//", "/");
                            if (!string.IsNullOrEmpty(file.Extension))
                                context.Response.Write(string.Format(format, filename, MyComputerItem.ParseForImage(file), parseLength(file.Length), filetype, "/Extranet/Download/" + dirpath.Replace('&', '^'), allowedit));
                            else
                                context.Response.Write(string.Format(format, filename, MyComputerItem.ParseForImage(file), parseLength(file.Length), filetype, "/Extranet/Download/" + dirpath.Replace('&', '^'), allowedit));
                        }
                    }
                    catch
                    {
                        //Response.Redirect("/extranet/unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                    }
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                context.Response.Clear();
                context.Response.Write("ERROR: UNAUTHORISED");
            }

        }

        public string parseLength(object size)
        {
            decimal d = decimal.Parse(size.ToString() + ".00");
            string[] s = { "bytes", "KB", "MB", "GB", "TB", "PB" };
            int x = 0;
            while (d > 1024)
            {
                d = d / 1024;
                x++;
            }
            d = Math.Round(d, 2);
            return d.ToString() + " " + s[x];
        }

        public bool checkext(string extension)
        {
            string[] exc = config.MyComputer.HideExtensions.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in exc)
                if (s.ToLower() == extension.ToLower()) return false;
            return true;
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