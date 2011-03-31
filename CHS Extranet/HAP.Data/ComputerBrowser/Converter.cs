using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using HAP.Web.Configuration;
using System.DirectoryServices.AccountManagement;
using Microsoft.Win32;

namespace HAP.Data.ComputerBrowser
{
    public class Converter
    {
        static HttpContext Context;

        public static string DriveToUNC(string RoutingPath, string RoutingDrive)
        {
            uncpath unc;
            return DriveToUNC(RoutingPath, RoutingDrive, out unc);
        }

        public static string DriveToUNC(string RoutingPath, string RoutingDrive, out uncpath unc)
        {
            string userhome;
            return DriveToUNC(RoutingPath, RoutingDrive, out unc, out userhome);
        }

        public static string DriveToUNC(string RoutingPath, string RoutingDrive, out uncpath unc, out string userhome)
        {
            Context = HttpContext.Current;
            hapConfig config = hapConfig.Current;
            string _ActiveDirectoryConnectionString = "";
            string _DomainDN = "";
            PrincipalContext pcontext; UserPrincipal up;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);

            userhome = "";
            string u = "";
            if (!string.IsNullOrEmpty(up.HomeDirectory))
            {
                u = userhome = up.HomeDirectory;
                if (userhome.EndsWith("\\")) userhome = userhome.Remove(userhome.LastIndexOf('\\'));
            }
            string path = "";
            unc = config.MyComputer.UNCPaths[RoutingDrive];
            path = string.Format(unc.UNC.Replace("%homepath%", u), Username) + RoutingPath;

            path = path.TrimEnd(new char[] { '\\' }).Replace('^', '&').Replace('/', '\\');
            return path;
        }

        public static string DriveToUNC(string Path)
        {
            return DriveToUNC(Path.Remove(0, 1), Path.Substring(0, 1));
        }

        public static string DriveToUNC(string Path, out uncpath unc, out string userhome)
        {
            return DriveToUNC(Path.Remove(0, 2), Path.Substring(0, 1), out unc, out userhome);
        }

        public static string UNCtoDrive(string dirpath, uncpath unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", userhome), Username), unc.Drive + ":");
            dirpath = dirpath.Replace("\\\\", "\\");
            return dirpath;
        }

        public static string UNCtoDrive2(string dirpath, uncpath unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", userhome), Username), unc.Drive);
            dirpath = dirpath.Replace('\\', '/').Replace("//", "/");
            return dirpath;
        }

        static bool isWriteAuth(uncpath path)
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

        static string Username
        {
            get
            {
                if (Context.User.Identity.Name.Contains('\\'))
                    return Context.User.Identity.Name.Remove(0, Context.User.Identity.Name.IndexOf('\\') + 1);
                else return Context.User.Identity.Name;
            }
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

        public static string parseLength(object size)
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
    }
}