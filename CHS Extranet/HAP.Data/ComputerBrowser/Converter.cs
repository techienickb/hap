using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using HAP.Web.Configuration;
using System.DirectoryServices.AccountManagement;
using Microsoft.Win32;
using System.Web.Security;
using HAP.AD;

namespace HAP.Data.ComputerBrowser
{
    public class Converter
    {
        static HttpContext Context { get { return HttpContext.Current; } }

        public static string DriveToUNC(string RoutingPath, string RoutingDrive)
        {
            DriveMapping unc;
            return DriveToUNC(RoutingPath, RoutingDrive, out unc);
        }

        public static string DriveToUNC(string RoutingPath, string RoutingDrive, out DriveMapping unc)
        {
            string userhome;
            return DriveToUNC(RoutingPath, RoutingDrive, out unc, out userhome);
        }

        public static string DriveToUNC(string RoutingPath, string RoutingDrive, out DriveMapping unc, out string userhome)
        {
            hapConfig config = hapConfig.Current;
            User user = (User)Membership.GetUser();

            userhome = "";
            string u = "";
            if (!string.IsNullOrEmpty(user.HomeDirectory))
            {
                u = userhome = user.HomeDirectory;
                if (userhome.EndsWith("\\")) userhome = userhome.Remove(userhome.LastIndexOf('\\'));
            }
            string path = "";
            unc = config.MySchoolComputerBrowser.Mappings[RoutingDrive.ToCharArray()[0]];
            path = string.Format(unc.UNC.Replace("%homedir%", u), user.UserName) + RoutingPath;

            path = path.TrimEnd(new char[] { '\\' }).Replace('^', '&').Replace('/', '\\');
            return path;
        }

        public static string DriveToUNC(string RoutingPath, string RoutingDrive, out UNCPath unc, out string userhome)
        {
            DriveMapping u;
            string s = DriveToUNC(RoutingPath, RoutingDrive, out u, out userhome);
            UNCPath u1 = new UNCPath();
            u1.Drive = u.Drive.ToString();
            u1.EnableMove = u.EnableMove;
            u1.EnableReadTo = u.EnableReadTo;
            u1.EnableWriteTo = u.EnableWriteTo;
            u1.Name = u.Name;
            u1.UNC = u.UNC;
            u1.Usage = u.UsageMode;
            unc = u1;
            return s;
        }

        public static string DriveToUNC(string Path)
        {
            return DriveToUNC(Path.Remove(0, 1), Path.Substring(0, 1));
        }

        public static string DriveToUNC(string Path, out DriveMapping unc, out string userhome)
        {
            return DriveToUNC(Path.Remove(0, 2), Path.Substring(0, 1), out unc, out userhome);
        }

        public static string DriveToUNC(string Path, out UNCPath unc, out string userhome)
        {
            return DriveToUNC(Path.Remove(0, 2), Path.Substring(0, 1), out unc, out userhome);
        }

        public static string UNCtoDrive(string dirpath, DriveMapping unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homedir%", userhome), ((User)Membership.GetUser()).UserName), unc.Drive + ":");
            dirpath = dirpath.Replace("\\\\", "\\");
            return dirpath;
        }

        public static string UNCtoDrive(string dirpath, UNCPath unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homedir%", userhome), ((User)Membership.GetUser()).UserName), unc.Drive + ":");
            dirpath = dirpath.Replace("\\\\", "\\");
            return dirpath;
        }

        public static string UNCtoDrive2(string dirpath, DriveMapping unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homedir%", userhome), ((User)Membership.GetUser()).UserName), unc.Drive.ToString());
            dirpath = dirpath.Replace('\\', '/').Replace("//", "/");
            return dirpath;
        }

        public static string UNCtoDrive2(string dirpath, UNCPath unc, string userhome)
        {
            dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homedir%", userhome), ((User)Membership.GetUser()).UserName), unc.Drive.ToString());
            dirpath = dirpath.Replace('\\', '/').Replace("//", "/");
            return dirpath;
        }

        static bool isWriteAuth(DriveMapping path)
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

        public static UNCPath ToUNCPath(DriveMapping unc)
        {
            UNCPath Unc = new UNCPath();
            Unc.Drive = unc.Drive.ToString();
            Unc.EnableMove = unc.EnableMove;
            Unc.EnableReadTo = unc.EnableReadTo;
            Unc.EnableWriteTo = unc.EnableWriteTo;
            Unc.Name = unc.Name;
            Unc.UNC = unc.UNC;
            Unc.Usage = unc.UsageMode;
            return Unc;
        }
    }
}