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
        public FileCheckResponse CheckFile(string FileName)
        {
            string home; uncpath unc; string path = Converter.DriveToUNC(FileName, out unc, out home);
            return new FileCheckResponse(path, unc, home);
        }

        //method for listing
        [WebMethod]
        public PrimaryResponse ListDrives()
        {
            PrimaryResponse res = new PrimaryResponse();
            res.HAPVerion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            res.HAPName = Assembly.GetExecutingAssembly().GetName().Name;

            hapConfig config = hapConfig.Current; 

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
            List<ComputerBrowserAPIItem> items = new List<ComputerBrowserAPIItem>();
            foreach (uncpath path in config.MyComputer.UNCPaths)
            {
                decimal space = -1;
                bool showspace = false;
                if (HttpContext.Current.User.IsInRole("Domain Admins") || !path.UNC.Contains("%homepath%")) showspace = isWriteAuth(path);
                if (showspace)
                {
                    if (path.Usage == UsageMode.DriveSpace)
                    {
                        if (Win32.GetDiskFreeSpaceEx(string.Format(path.UNC.Replace("%homepath%", userhome), Username), out freeBytesForUser, out totalBytes, out freeBytes))
                            space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                    }
                    else
                    {
                        try
                        {
                            HAP.Data.Quota.QuotaInfo qi = HAP.Data.ComputerBrowser.Quota.GetQuota(Username, string.Format(path.UNC.Replace("%homepath%", userhome)));
                            space = Math.Round((Convert.ToDecimal(qi.Used) / Convert.ToDecimal(qi.Total)) * 100, 2);
                            if (qi.Total == -1)
                                if (Win32.GetDiskFreeSpaceEx(string.Format(path.UNC.Replace("%homepath%", userhome), Username), out freeBytesForUser, out totalBytes, out freeBytes))
                                    space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                        }
                        catch { }
                    }
                }
                if (isAuth(path)) items.Add(new ComputerBrowserAPIItem(path.Name, "images/icons/netdrive.png", space.ToString(), "Drive", BType.Drive, path.Drive + ":", isWriteAuth(path) ? AccessControlActions.Change : AccessControlActions.View));
            }
            res.Items = items.ToArray();
            List<uploadfilter> filters = new List<uploadfilter>();
            foreach (uploadfilter filter in config.MyComputer.UploadFilters)
                if (isAuth(filter)) filters.Add(filter);
            res.Filters = filters.ToArray();

            return res;
        }

        //method for listing
        [WebMethod]
        public ComputerBrowserAPIItem[] List(string path)
        {
            List<ComputerBrowserAPIItem> items = new List<ComputerBrowserAPIItem>();
            uncpath unc; string userhome;
            Converter.DriveToUNC(path, out unc, out userhome);
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

                        string dirpath = Converter.UNCtoDrive(subdir.FullName, unc, userhome);
                        items.Add(new ComputerBrowserAPIItem(subdir, unc, userhome, actions));
                    }
                }
                catch { }

            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    if (!file.Name.ToLower().Contains("thumbs") && checkext(file.Extension) && file.Attributes != FileAttributes.Hidden && file.Attributes != FileAttributes.System)
                    {
                        string dirpath = Converter.UNCtoDrive2(file.FullName, unc, userhome);
                        items.Add(new ComputerBrowserAPIItem(file, unc, userhome, allowactions, "Download/" + dirpath.Replace('&', '^')));
                    }
                }
                catch
                {
                    //Response.Redirect("unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                }
            }
            return items.ToArray();
        }

        //method for renaming/saving
        [WebMethod]
        public CBFile[] Save(CBFile Current, string newpath, bool overwrite)
        {
            uncpath unc; string userhome;
            string path = Converter.DriveToUNC(Current.Path, out unc, out userhome);
            newpath = Converter.DriveToUNC(newpath, out unc, out userhome);

            if (File.Exists(path))
            {
                FileInfo file = new FileInfo(path);
                string fname = file.Name;
                if (fname.EndsWith(file.Extension) && !string.IsNullOrWhiteSpace(file.Extension)) fname = fname.Remove(fname.IndexOf(file.Extension));
                if (!overwrite)
                {
                    if (File.Exists(newpath)) return new CBFile[] { new CBFile(new FileInfo(newpath), unc, userhome) };
                }
                else File.Delete(newpath);
                file.MoveTo(newpath);
            }
            else
            {
                DirectoryInfo file = new DirectoryInfo(path);

                if (!overwrite)
                {
                    if (Directory.Exists(newpath)) return new CBFile[] { new CBFile( new DirectoryInfo(newpath), unc, userhome) };
                }
                else Directory.Delete(newpath);
                file.MoveTo(newpath);
            }
            return new CBFile[] { };
        }

        //method for deleting
        [WebMethod]
        public void Delete(string path)
        {
            path = Converter.DriveToUNC(path);
            if (File.Exists(path)) File.Delete(path);
            else Directory.Delete(path, true);
        }

        //method for Creating a New Folder
        [WebMethod]
        public void NewFolder(string basepath, string foldername)
        {
            basepath = Converter.DriveToUNC(basepath);
            Directory.CreateDirectory(Path.Combine(basepath, foldername));
        }

        //method for ZIPPING
        [WebMethod]
        public void ZIP(string basepath, string filename, params string[] filepaths)
        {
            string path = Path.Combine(Converter.DriveToUNC(basepath), filename);
            ZipFile zf;
            if (File.Exists(Path.Combine(basepath, filename)))
                zf = new ZipFile(path);
            else zf = ZipFile.Create(path);
            zf.BeginUpdate();
            foreach (string s in filepaths)
            {
                string p = Converter.DriveToUNC(s);
                if (File.Exists(p))
                    zf.Add(p);
                else if (Directory.Exists(p))
                    zf.AddDirectory(p);
            }
            zf.CommitUpdate();
            zf.Close();
        }

        //method for Unzipping
        [WebMethod]
        public void Unzip(string zipfile, string extractfolder)
        {
            string path = Converter.DriveToUNC(zipfile);
            string c = Converter.DriveToUNC(extractfolder);
            if (!Directory.Exists(c)) Directory.CreateDirectory(c);
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(path, c, "");
        }

        //method for upload the files. This will be called from Silverlight code behind.
        [WebMethod]
        public void UploadFile(string FileName, long StartByte, byte[] Data)
        {
            string home; uncpath unc; string path = Converter.DriveToUNC(FileName, out unc, out home);
            FileStream fs = (StartByte > 0 && File.Exists(path)) ? File.Open(path, FileMode.Append) : File.Create(path);
            using (fs)
            {
                SaveFile(new MemoryStream(Data), fs);
                fs.Close();
                fs.Dispose();
            }
        }

        internal static class Win32
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);

        }

        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
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
            string[] exc = hapConfig.Current.MyComputer.HideExtensions.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in exc)
                if (s.ToLower() == extension.ToLower()) return false;
            return true;
        }

        private string Username
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
