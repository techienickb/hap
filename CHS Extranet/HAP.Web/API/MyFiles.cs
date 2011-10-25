using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using HAP.Data.MyFiles;
using HAP.AD;
using HAP.Data;
using HAP.Web.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using HAP.Data.ComputerBrowser;

namespace HAP.Web.API
{
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MyFiles
    {
        [OperationContract]
        [WebGet(UriTemplate="{Drive}/{*Path}")]
        public HAP.Data.MyFiles.File[] List(string Drive, string Path)
        {
            DateTime d1 = DateTime.Now;
            Path = "/" + Path;
            hapConfig config = hapConfig.Current;
            List<HAP.Data.MyFiles.File> Items = new List<Data.MyFiles.File>();
            User user = new User();
            HttpCookie token = HttpContext.Current.Request.Cookies["token"];
            if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
            user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            user.ImpersonateContained();
            try
            {
                DriveMapping mapping;
                string path = Converter.DriveToUNC(Path, Drive, out mapping, user);
                HAP.Data.MyFiles.AccessControlActions allowactions = isWriteAuth(mapping) ? HAP.Data.MyFiles.AccessControlActions.Change : HAP.Data.MyFiles.AccessControlActions.View;
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                    try
                    {
                        bool isHidden = (subdir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                        bool isSystem = (subdir.Attributes & FileAttributes.System) == FileAttributes.System;
                        if (!subdir.Name.ToLower().Contains("recycle") && !isSystem && !isHidden && !subdir.Name.ToLower().Contains("system volume info"))
                        {
                            HAP.Data.MyFiles.AccessControlActions actions = allowactions;
                            if (actions == HAP.Data.MyFiles.AccessControlActions.Change)
                            {
                                try { System.IO.File.Create(System.IO.Path.Combine(subdir.FullName, "temp.ini")).Close(); System.IO.File.Delete(System.IO.Path.Combine(subdir.FullName, "temp.ini")); }
                                catch { actions = HAP.Data.MyFiles.AccessControlActions.View; }
                            }
                            try { subdir.GetDirectories(); }
                            catch { actions = HAP.Data.MyFiles.AccessControlActions.None; }
                            Items.Add(new Data.MyFiles.File(subdir, mapping, user));
                        }
                    }
                    catch { }
                foreach (FileInfo file in dir.GetFiles())
                {
                    try
                    {
                        bool isHidden = (file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                        bool isSystem = (file.Attributes & FileAttributes.System) == FileAttributes.System;
                        if (!file.Name.ToLower().Contains("thumbs") && checkext(file.Extension) && !isHidden && !isSystem)
                            Items.Add(new Data.MyFiles.File(file, mapping, user, allowactions));
                    }
                    catch
                    {
                        //Response.Redirect("unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                    }
                }
            }
            finally { user.EndContainedImpersonate(); }
            return Items.ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate="Drives")]
        public Drive[] Drives()
        {
            List<Drive> drives = new List<Drive>();
            User user = new User();
            HttpCookie token = HttpContext.Current.Request.Cookies["token"];
            if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
            user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            user.ImpersonateContained();

            hapConfig config = hapConfig.Current;

            long freeBytesForUser, totalBytes, freeBytes;

            string userhome = user.HomeDirectory;
            foreach (DriveMapping p in config.MySchoolComputerBrowser.Mappings.Values.OrderBy(m => m.Drive))
            {
                decimal space = -1;
                bool showspace = isWriteAuth(p);
                if (showspace)
                {
                    if (p.UsageMode == MappingUsageMode.DriveSpace)
                    {
                        if (Win32.GetDiskFreeSpaceEx(Converter.FormatMapping(p.UNC, user), out freeBytesForUser, out totalBytes, out freeBytes))
                            space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                    }
                    else
                    {
                        try
                        {
                            HAP.Data.Quota.QuotaInfo qi = HAP.Data.ComputerBrowser.Quota.GetQuota(user.UserName, Converter.FormatMapping(p.UNC, user));
                            space = Math.Round((Convert.ToDecimal(qi.Used) / Convert.ToDecimal(qi.Total)) * 100, 2);
                            if (qi.Total == -1)
                                if (Win32.GetDiskFreeSpaceEx(Converter.FormatMapping(p.UNC, user), out freeBytesForUser, out totalBytes, out freeBytes))
                                    space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                        }
                        catch { }
                    }
                }
                if (isAuth(p)) drives.Add(new Drive(p.Name, "../images/icons/netdrive.png", space, p.Drive.ToString() + "\\", isWriteAuth(p) ? HAP.Data.MyFiles.AccessControlActions.Change : HAP.Data.MyFiles.AccessControlActions.View));
            }
            user.EndContainedImpersonate();
            return drives.ToArray();
        }

        #region Private Stuff
        internal static class Win32
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);
        }

        private void SaveFile(Stream stream, FileStream fs)
        {
            stream.CopyTo(fs);
        }

        private bool isAuth(Filter filter)
        {
            if (filter.EnableFor == "All") return true;
            else if (filter.EnableFor != "None")
            {
                bool vis = false;
                foreach (string s in filter.EnableFor.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        private bool isAuth(DriveMapping path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        private bool isWriteAuth(DriveMapping path)
        {
            if (path == null) return true;
            if (path.EnableWriteTo == "All") return true;
            else if (path.EnableWriteTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableWriteTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
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

        public bool checkext(string extension)
        {
            string[] exc = hapConfig.Current.MySchoolComputerBrowser.HideExtensions.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in exc)
                if (s.Trim().ToLower() == extension.ToLower()) return false;
            return true;
        }

        #endregion
    }
}