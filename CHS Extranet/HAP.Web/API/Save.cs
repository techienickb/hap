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
    public class SaveHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new Save(path, drive);
        }
    }

    public class Save : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public Save(string path, string drive)
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
            context.Response.Headers.Add("HAP:API", "Save");
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

                StreamReader sr = new StreamReader(context.Request.InputStream);
                string c = sr.ReadToEnd();
                bool folder = c.Contains("..");

                if (File.Exists(path))
                {
                    FileInfo file = new FileInfo(path);
                    bool extension = c.Contains(file.Extension);
                    string name = file.Name;
                    if (!extension) name = name.Replace(file.Extension, "");
                    if (c.StartsWith("SAVETO:"))
                    {
                        c = c.Remove(0, 7);
                        string p2 = path.Replace(name, c);
                        if (folder) p2 = p2.Replace(file.Directory.Name + "\\", "");
                        FileInfo f2 = new FileInfo(p2);
                        if (f2.Exists)
                        {
                            context.Response.Write("EXISTS\n");
                            context.Response.Write(f2.Name.Replace(f2.Extension, "") + "\n");
                            context.Response.Write(UNCtoDrive(file.FullName, unc, userhome) + "\n");
                            context.Response.Write(" \n");
                            context.Response.Write(file.LastWriteTime.ToShortDateString() + " " + file.LastWriteTime.ToShortTimeString() + "\n");
                            context.Response.Write(UNCtoDrive(f2.FullName, unc, userhome) + "\n");
                            context.Response.Write(" \n");
                            context.Response.Write(List.parseLength(f2.LastWriteTime.ToShortDateString() + " " + f2.LastWriteTime.ToShortTimeString() + "\n"));
                            return;
                        }
                    }
                    else
                    {
                        c = c.Remove(0, 10);
                        string p2 = path.Replace(name, c);
                        if (folder) p2 = p2.Replace(file.Directory.Name + "\\", "");
                        File.Delete(p2);
                    }
                    file.MoveTo(file.FullName.Replace(name, c));
                }
                else
                {
                    DirectoryInfo file = new DirectoryInfo(path);

                    if (c.StartsWith("SAVETO:"))
                    {
                        c = c.Remove(0, 7);
                        string p2 = path.Replace(file.Name, c);
                        if (folder) p2 = p2.Replace("..\\", "").Replace(file.Parent.Name + "\\", "");
                        DirectoryInfo f2 = new DirectoryInfo(p2);
                        if (f2.Exists)
                        {
                            context.Response.Write("EXISTS\n");
                            context.Response.Write(f2.Name + "\n");
                            context.Response.Write(UNCtoDrive(file.FullName, unc, userhome) + "\n");
                            context.Response.Write(" \n");
                            context.Response.Write(file.LastWriteTime.ToShortDateString() + " " + file.LastWriteTime.ToShortTimeString() + "\n");
                            context.Response.Write(UNCtoDrive(f2.FullName, unc, userhome) + "\n");
                            context.Response.Write(" \n");
                            context.Response.Write(List.parseLength(f2.LastWriteTime.ToShortDateString() + " " + f2.LastWriteTime.ToShortTimeString() + "\n"));
                            return;
                        }
                    }
                    else
                    {
                        c = c.Remove(0, 10);
                        string p2 = path.Replace(file.Name, c);
                        if (folder) p2 = p2.Replace(file.Parent.Name + "\\", "");
                        Directory.Delete(p2);
                    }
                    file.MoveTo(file.FullName.Replace(file.Name, c));
                }

                context.Response.Write("DONE");
            }
            catch (Exception e)
            {
                context.Response.Write("ERROR: " + e.ToString() + "\\n" + e.Message);
            }
        }

        private string UNCtoDrive(string dirpath, uncpath unc, string userhome)
        {
            if (unc == null) dirpath = dirpath.Replace(userhome, "N");
            else dirpath = dirpath.Replace(string.Format(unc.UNC, Username), unc.Drive);
            dirpath = dirpath.Replace('\\', '/').Replace("//", "/");
            return dirpath;
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