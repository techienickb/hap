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
        static HttpContext Context { get { return HttpContext.Current; } }

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
            hapConfig config = hapConfig.Current;
            PrincipalContext pcontext; UserPrincipal up;
            pcontext = HAP.AD.ADUtil.PContext;
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, HAP.AD.ADUtil.Username);

            userhome = "";
            string u = "";
            if (!string.IsNullOrEmpty(up.HomeDirectory))
            {
                u = userhome = up.HomeDirectory;
                if (userhome.EndsWith("\\")) userhome = userhome.Remove(userhome.LastIndexOf('\\'));
            }
            string path = "";
            unc = config.MyComputer.UNCPaths[RoutingDrive];
            path = string.Format(unc.UNC.Replace("%homepath%", u), HAP.AD.ADUtil.Username) + RoutingPath;

            path = path.TrimEnd(new char[] { '\\' }).Replace('^', '&').Replace('/', '\\');
            return path;
        }

        public static string DriveToUNC(string RoutingPath, string RoutingDrive, out UNCPath unc, out string userhome)
        {
            uncpath u;
            string s = DriveToUNC(RoutingPath, RoutingDrive, out u, out userhome);
            UNCPath u1 = new UNCPath();
            u1.Drive = u.Drive;
            u1.EnableMove = u.EnableMove;
            u1.EnableReadTo = u.EnableReadTo;
            u1.EnableWriteTo = u.EnableWriteTo;
            u1.Name = u.Name;
            u1.UNC = u.UNC;
            u1.Usage = u.Usage;
            unc = u1;
            return s;
        }

        public static string DriveToUNC(string Path)
        {
            return DriveToUNC(Path.Remove(0, 1), Path.Substring(0, 1));
        }

        public static string DriveToUNC(string Path, out uncpath unc, out string userhome)
        {
            return DriveToUNC(Path.Remove(0, 2), Path.Substring(0, 1), out unc, out userhome);
        }

        public static string DriveToUNC(string Path, out UNCPath unc, out string userhome)
        {
            return DriveToUNC(Path.Remove(0, 2), Path.Substring(0, 1), out unc, out userhome);
        }

        public static string UNCtoDrive(string dirpath, uncpath unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", userhome), HAP.AD.ADUtil.Username), unc.Drive + ":");
            dirpath = dirpath.Replace("\\\\", "\\");
            return dirpath;
        }

        public static string UNCtoDrive(string dirpath, UNCPath unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", userhome), HAP.AD.ADUtil.Username), unc.Drive + ":");
            dirpath = dirpath.Replace("\\\\", "\\");
            return dirpath;
        }

        public static string UNCtoDrive2(string dirpath, uncpath unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", userhome), HAP.AD.ADUtil.Username), unc.Drive);
            dirpath = dirpath.Replace('\\', '/').Replace("//", "/");
            return dirpath;
        }

        public static string UNCtoDrive2(string dirpath, UNCPath unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", userhome), HAP.AD.ADUtil.Username), unc.Drive);
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

        static bool isWriteAuth(UNCPath path)
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

        public static UNCPath ToUNCPath(uncpath unc)
        {
            UNCPath Unc = new UNCPath();
            Unc.Drive = unc.Drive;
            Unc.EnableMove = unc.EnableMove;
            Unc.EnableReadTo = unc.EnableReadTo;
            Unc.EnableWriteTo = unc.EnableWriteTo;
            Unc.Name = unc.Name;
            Unc.UNC = unc.UNC;
            Unc.Usage = unc.Usage;
            return Unc;
        }
    }
}