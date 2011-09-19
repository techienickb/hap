using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using HAP.Data.ComputerBrowser;
using HAP.Web.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using Microsoft.Win32;
using ICSharpCode.SharpZipLib.Zip;
using System.Web.Security;
using System.Web.SessionState;
using HAP.AD;
using HAP.Data;

namespace HAP.Web
{
    /// <summary>
    /// Home Access Plus+ - Primary API
    /// </summary>
    [WebService(Namespace = "http://hap.codeplex.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class api : System.Web.Services.WebService
    {
        //method for checking the upload the file is ok. This will be called from Silverlight code behind.
        [WebMethod]
        public FileCheckResponse CheckFile(string FileName, string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            UNCPath unc; string path = Converter.DriveToUNC(FileName, out unc, user);
            FileCheckResponse fcr = new FileCheckResponse(path, unc, user);
            user.EndContainedImpersonate();
            return fcr;
        }

        //method for listing
        [WebMethod]
        public PrimaryResponse ListDrives(string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            PrimaryResponse res = new PrimaryResponse();
            res.HAPVerion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            res.HAPName = Assembly.GetExecutingAssembly().GetName().Name;

            hapConfig config = hapConfig.Current; 

            long freeBytesForUser, totalBytes, freeBytes;

            string userhome = user.HomeDirectory;
            List<ComputerBrowserAPIItem> items = new List<ComputerBrowserAPIItem>();
            foreach (DriveMapping p in config.MySchoolComputerBrowser.Mappings.Values)
            {
                UNCPath path = new UNCPath();
                path.Drive = p.Drive.ToString();
                path.EnableMove = p.EnableMove;
                path.EnableReadTo = p.EnableReadTo;
                path.EnableWriteTo = p.EnableWriteTo;
                path.Name = p.Name;
                path.UNC = p.UNC;
                path.Usage = p.UsageMode;
                decimal space = -1;
                bool showspace = isWriteAuth(path);
                if (showspace)
                {
                    if (path.Usage == MappingUsageMode.DriveSpace)
                    {
                        if (Win32.GetDiskFreeSpaceEx(Converter.FormatMapping(path.UNC, user), out freeBytesForUser, out totalBytes, out freeBytes))
                            space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                    }
                    else
                    {
                        try
                        {
                            HAP.Data.Quota.QuotaInfo qi = HAP.Data.ComputerBrowser.Quota.GetQuota(user.UserName, Converter.FormatMapping(path.UNC, user));
                            space = Math.Round((Convert.ToDecimal(qi.Used) / Convert.ToDecimal(qi.Total)) * 100, 2);
                            if (qi.Total == -1)
                                if (Win32.GetDiskFreeSpaceEx(Converter.FormatMapping(path.UNC, user), out freeBytesForUser, out totalBytes, out freeBytes))
                                    space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                        }
                        catch { }
                    }
                }
                if (isAuth(path)) items.Add(new ComputerBrowserAPIItem(path.Name, "images/icons/netdrive.png", space.ToString(), "Drive", BType.Drive, path.Drive + ":", isWriteAuth(path) ? AccessControlActions.Change : AccessControlActions.View));
            }
            res.Items = items.ToArray();
            user.EndContainedImpersonate();
            List<UploadFilter> filters = new List<UploadFilter>();
            foreach (Filter filter in config.MySchoolComputerBrowser.Filters)
                if (isAuth(filter))
                {
                    UploadFilter u = new UploadFilter();
                    u.EnableFor = filter.EnableFor;
                    u.Filter = filter.Expression;
                    u.Name = filter.Name;
                    filters.Add(u);
                }
            res.Filters = filters.ToArray();

            return res;
        }

        //method for listing
        [WebMethod]
        public ComputerBrowserAPIItem[] List(string path, string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            List<ComputerBrowserAPIItem> items = new List<ComputerBrowserAPIItem>();
            UNCPath unc;
            path = Converter.DriveToUNC(path, out unc, user);
            AccessControlActions allowactions = isWriteAuth(unc) ? AccessControlActions.Change : AccessControlActions.View;
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (DirectoryInfo subdir in dir.GetDirectories())
                try
                {
                    if (!subdir.Name.ToLower().Contains("recycle") && subdir.Attributes != FileAttributes.Hidden && subdir.Attributes != FileAttributes.System && !subdir.Name.ToLower().Contains("system volume info"))
                    {
                        AccessControlActions actions = allowactions;
                        if (actions == AccessControlActions.Change)
                        {
                            try { File.Create(Path.Combine(subdir.FullName, "temp.ini")).Close(); File.Delete(Path.Combine(subdir.FullName, "temp.ini")); }
                            catch { actions = AccessControlActions.View; }
                        }
                        try { subdir.GetDirectories(); }
                        catch { actions = AccessControlActions.None; }

                        string dirpath = Converter.UNCtoDrive(subdir.FullName, unc, user);
                        items.Add(new ComputerBrowserAPIItem(subdir, unc, user, actions));
                    }
                }
                catch { }

            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    if (!file.Name.ToLower().Contains("thumbs") && checkext(file.Extension) && file.Attributes != FileAttributes.Hidden && file.Attributes != FileAttributes.System)
                    {
                        string dirpath = Converter.UNCtoDrive2(file.FullName, unc, user);
                        items.Add(new ComputerBrowserAPIItem(file, unc, user, allowactions, "Download/" + dirpath.Replace('&', '^')));
                    }
                }
                catch
                {
                    //Response.Redirect("unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                }
            }
            user.EndContainedImpersonate();
            return items.ToArray();
        }

        //method for searching
        [WebMethod]
        public ComputerBrowserAPIItem[] Search(string path, string searchterm, string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            List<ComputerBrowserAPIItem> items = new List<ComputerBrowserAPIItem>();
            UNCPath unc;
            path = Converter.DriveToUNC(path, out unc);
            AccessControlActions allowactions = isWriteAuth(unc) ? AccessControlActions.Change : AccessControlActions.View;
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (DirectoryInfo subdir in dir.GetDirectories())
                try
                {
                    if (!subdir.Name.ToLower().Contains("recycle") && subdir.Attributes != FileAttributes.Hidden && subdir.Attributes != FileAttributes.System && !subdir.Name.ToLower().Contains("system volume info") && subdir.Name.ToLower().Contains(searchterm.ToLower()))
                    {
                        AccessControlActions actions = allowactions;
                        if (actions == AccessControlActions.Change)
                        {
                            try { File.Create(Path.Combine(subdir.FullName, "temp.ini")).Close(); File.Delete(Path.Combine(subdir.FullName, "temp.ini")); }
                            catch { actions = AccessControlActions.View; }
                        }
                        try { subdir.GetDirectories(); }
                        catch { actions = AccessControlActions.None; }

                        string dirpath = Converter.UNCtoDrive(subdir.FullName, unc, user);
                        items.Add(new ComputerBrowserAPIItem(subdir, unc, user, actions));
                    }
                }
                catch { }

            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    if (!file.Name.ToLower().Contains("thumbs") && checkext(file.Extension) && file.Attributes != FileAttributes.Hidden && file.Attributes != FileAttributes.System && file.Name.ToLower().Contains(searchterm.ToLower()))
                    {
                        string dirpath = Converter.UNCtoDrive2(file.FullName, unc, user);
                        items.Add(new ComputerBrowserAPIItem(file, unc, user, allowactions, "Download/" + dirpath.Replace('&', '^')));
                    }
                }
                catch
                {
                    //Response.Redirect("unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                }
            }
            user.EndContainedImpersonate();
            return items.ToArray();
        }

        //method for renaming/saving
        [WebMethod]
        public CBFile[] Save(CBFile Current, string newpath, bool overwrite, string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            UNCPath unc;
            string path = Converter.DriveToUNC(Current.Path, out unc, user);
            newpath = Converter.DriveToUNC(newpath, out unc, user);

            if (File.Exists(path))
            {
                FileInfo file = new FileInfo(path);
                string fname = file.Name;
                if (fname.EndsWith(file.Extension) && !string.IsNullOrWhiteSpace(file.Extension)) fname = fname.Remove(fname.IndexOf(file.Extension));
                if (!newpath.EndsWith(file.Extension)) newpath += file.Extension;
                if (!overwrite)
                {
                    if (File.Exists(newpath)) return new CBFile[] { new CBFile(new FileInfo(newpath), unc, user) };
                }
                else File.Delete(newpath);
                file.MoveTo(newpath);
            }
            else
            {
                DirectoryInfo file = new DirectoryInfo(path);

                if (!overwrite && Directory.Exists(newpath)) return new CBFile[] { new CBFile(new DirectoryInfo(newpath), unc, user) };
                else if (Directory.Exists(newpath)) Directory.Delete(newpath);
                file.MoveTo(newpath);
            }
            user.EndContainedImpersonate();
            return new CBFile[] { };
        }

        //method for deleting
        [WebMethod]
        public void Delete(string path, string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            UNCPath unc;
            path = Converter.DriveToUNC(path, out unc, user);
            if (File.Exists(path)) File.Delete(path);
            else Directory.Delete(path, true);
            user.EndContainedImpersonate();
        }

        //method for Creating a New Folder
        [WebMethod]
        public void NewFolder(string basepath, string foldername, string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            UNCPath unc;
            basepath = Converter.DriveToUNC(basepath, out unc, user);
            Directory.CreateDirectory(Path.Combine(basepath, foldername));
            user.EndContainedImpersonate();
        }

        //method for ZIPPING
        [WebMethod]
        public void ZIP(string basepath, string filename, string[] filepaths, string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            UNCPath unc;
            basepath = Converter.DriveToUNC(basepath, out unc, user);
            string path = Path.Combine(basepath, filename);
            ZipFile zf;
            if (File.Exists(Path.Combine(basepath, filename)))
                zf = new ZipFile(path);
            else zf = ZipFile.Create(path);
            zf.BeginUpdate();
            foreach (string s in filepaths)
            {
                string p = Converter.DriveToUNC(s, out unc, user);
                if (File.Exists(p))
                    zf.Add(p);
                else if (Directory.Exists(p))
                    zf.AddDirectory(p);
            }
            zf.CommitUpdate();
            zf.Close();
            user.EndContainedImpersonate();
        }

        //method for Unzipping
        [WebMethod]
        public void Unzip(string zipfile, string extractfolder, string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            UNCPath unc;
            string path = Converter.DriveToUNC(zipfile, out unc, user);
            string c = Converter.DriveToUNC(extractfolder, out unc, user);
            if (!Directory.Exists(c)) Directory.CreateDirectory(c);
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(path, c, "");
            user.EndContainedImpersonate();
        }

        //method for upload the files. This will be called from Silverlight code behind.
        [WebMethod]
        public void UploadFile(string FileName, long StartByte, byte[] Data, bool Complete, string token)
        {
            User user = new User();
            user.Authenticate(TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[0], TokenGenerator.ConvertToPlain(token).Split(new char[] { '@' })[1]);
            user.ImpersonateContained();
            UNCPath unc;
            string path = Converter.DriveToUNC(FileName, out unc, user);
            FileStream fs = (StartByte > 0 && File.Exists(path)) ? File.Open(path, FileMode.Append) : File.Create(path);
            using (fs)
            {
                SaveFile(new MemoryStream(Data), fs);
                fs.Close();
                fs.Dispose();
            }
            user.EndContainedImpersonate();
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
                    if (!vis) vis = Context.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        private bool isAuth(UNCPath path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Context.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        private bool isWriteAuth(UNCPath path)
        {
            if (path == null) return true;
            if (path.EnableWriteTo == "All") return true;
            else if (path.EnableWriteTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableWriteTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Context.User.IsInRole(s.Trim());
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
