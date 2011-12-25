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
using Excel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using System.Net;
using System.Collections.Specialized;

namespace HAP.Web.API
{
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MyFiles
    {

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SendTo/Google/{Drive}/{*Path}", BodyStyle = WebMessageBodyStyle.WrappedRequest,  RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string GoogleUpload(string Drive, string Path, string username, string password)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            HAP.Web.SendTo.Google.Client client = new SendTo.Google.Client();
            client.Login(username, password);
            user.ImpersonateContained();
            try 
            {
                DriveMapping mapping;
                string p = Converter.DriveToUNC('/' + Path, Drive, out mapping, user);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.SendTo.Google", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Sending to Google Docs: " + p);
                return client.Upload(p);
            } 
            finally 
            { 
                user.EndContainedImpersonate(); 
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Copy", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Copy(string OldPath, string NewPath)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();
            try
            {
                DriveMapping mapping;
                string p = Converter.DriveToUNC(OldPath.Remove(0, 1), OldPath.Substring(0, 1), out mapping, user);
                string p2 = Converter.DriveToUNC(NewPath.Remove(0, 1), NewPath.Substring(0, 1), out mapping, user);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Copy", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Copying: " + p);
                FileAttributes attr = System.IO.File.GetAttributes(p);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) copyDirectory(p, p2);
                else System.IO.File.Copy(p, p2);
            }
            finally
            {
                user.EndContainedImpersonate();
            }
        }

        public static void copyDirectory(string Src,string Dst)
        {
            string[] Files;

            if( Dst[Dst.Length-1] != Path.DirectorySeparatorChar) Dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
            Files = Directory.GetFileSystemEntries(Src);
            foreach (string Element in Files) {
                if (Directory.Exists(Element)) copyDirectory(Element, Dst + Path.GetFileName(Element));
                else System.IO.File.Copy(Element, Dst + Path.GetFileName(Element), true);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Move", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Move(string OldPath, string NewPath)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();
            try
            {
                DriveMapping mapping;
                string p = Converter.DriveToUNC(OldPath.Remove(0, 1), OldPath.Substring(0, 1), out mapping, user);
                string p2 = Converter.DriveToUNC(NewPath.Remove(0, 1), NewPath.Substring(0, 1), out mapping, user);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Move", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Moving: " + p);
                FileAttributes attr = System.IO.File.GetAttributes(p);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) Directory.Move(p, p2);
                else System.IO.File.Move(p, p2);
            }
            finally
            {
                user.EndContainedImpersonate();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "Delete", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string[] Delete(string[] Paths)
        {
            List<string> ret = new List<string>();
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();
            try
            {
                foreach (string path in Paths) 
                {
                    try
                    {
                        DriveMapping mapping;
                        string p = Converter.DriveToUNC(path.Remove(0, 1), path.Substring(0, 1), out mapping, user);
                        HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Delete", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Deleting: " + p);
                        FileAttributes attr = System.IO.File.GetAttributes(p);
                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory) Directory.Delete(p, true);
                        else System.IO.File.Delete(p);
                        ret.Add("Deleted " + path.Remove(0, path.LastIndexOf('/') + 1));
                    }
                    catch { ret.Add("I could not delete :" + path.Remove(0, path.LastIndexOf('/') + 1)); }
                }
            }
            finally
            {
                user.EndContainedImpersonate();
            }
            return ret.ToArray();
        }

        [OperationContract]
        [WebInvoke(Method="POST", UriTemplate="New/{Drive}/{*Path}", RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
        public void NewFolder(string Drive, string Path)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();
            try
            {
                DriveMapping mapping;
                string path = Converter.DriveToUNC("/" + Path, Drive, out mapping, user);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.NewFolder", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Creating new folder: " + path);
                Directory.CreateDirectory(path);
            }
            finally
            {
                user.EndContainedImpersonate();
            }
        }

        [OperationContract]
        [WebGet(UriTemplate="Preview/{Drive}/{*Path}", ResponseFormat=WebMessageFormat.Json, BodyStyle=WebMessageBodyStyle.Bare)]
        public string Preview(string Drive, string Path)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();
            DriveMapping mapping;
            string path = Converter.DriveToUNC("/" + Path, Drive, out mapping, user);
            FileInfo file = new FileInfo(path);

            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Preview", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Requesting preview of: " + file.FullName);

            string s = "";

            try
            {
                if (Path.ToLower().EndsWith(".docx"))
                {
                    s = HAP.Data.MyFiles.DocXPreview.Render(file.FullName);
                }
                else if (Path.ToLower().EndsWith(".xls") || Path.EndsWith(".xlsx"))
                {
                    IExcelDataReader excelReader;
                    if (file.Extension.ToLower().Contains("xlsx"))
                        excelReader = ExcelReaderFactory.CreateOpenXmlReader(file.OpenRead());
                    else excelReader = ExcelReaderFactory.CreateBinaryReader(file.OpenRead());
                    excelReader.IsFirstRowAsColumnNames = false;
                    StringWriter writer = new StringWriter();
                    HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);
                    GridView GridView1 = new GridView();
                    GridView1.DataSource = excelReader.AsDataSet();
                    GridView1.DataBind();
                    GridView1.RenderControl(htmlWriter);

                    s = writer.ToString();
                }
                else if (Path.ToLower().EndsWith(".csv"))
                {
                    StreamReader sr = file.OpenText();
                    s += "<table border=\"1\" style=\"border-collapse: collapse;\">";
                    while(!sr.EndOfStream) {
                        string[] r = sr.ReadLine().Split(new char[] { ',' });
                        s += "<tr>";
                        foreach (string s1 in r)
                        {
                            s += "<td>";
                            s += s1.Trim().Replace("\"", "");
                            s += "</td>";
                        }
                        s += "</tr>";
                    }
                    sr.Close();
                    sr.Dispose();
                    sr = null;
                    user.EndContainedImpersonate();

                    s += "</table>";
                }
                else if (HAP.Data.MyFiles.File.GetMimeType(file.Extension.ToLower()).StartsWith("image"))
                {
                    s = "<img src=\"../Download/" + Drive + "/" + Path + "\" alt=\"" + file.Name + "\" />";
                }
                else if (Path.ToLower().EndsWith(".txt"))
                {
                    StreamReader sr = file.OpenText();
                    s = sr.ReadToEnd().Replace("\n", "<br />");
                    sr.Close();
                    sr.Dispose();
                    sr = null;
                }
            }
            finally
            {
                user.EndContainedImpersonate();
            }
            return s;
        }

        [OperationContract]
        [WebGet(UriTemplate = "info/{Drive}/{*Path}")]
        public Properties info(string Drive, string Path)
        {
            Properties ret = new Data.MyFiles.Properties();
            Path = "/" + Path;
            hapConfig config = hapConfig.Current;
            List<HAP.Data.MyFiles.File> Items = new List<Data.MyFiles.File>();
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();
            try
            {
                DriveMapping mapping;
                string path = Converter.DriveToUNC(Path, Drive, out mapping, user);
                FileAttributes attr = System.IO.File.GetAttributes(path);
                //detect whether its a directory or file
                ret = new Properties(new DirectoryInfo(path), user, mapping);
            }
            finally { user.EndContainedImpersonate(); }

            return ret;
        }

        [OperationContract]
        [WebGet(UriTemplate = "Exists/{Drive}/{*Path}")]
        public Properties Exists(string Drive, string Path)
        {
            try
            {
                return Properties(Drive, Path);
            }
            catch
            {
                return new Properties();
            }
        }

        [OperationContract]
        [WebGet(UriTemplate="Properties/{Drive}/{*Path}")]
        public Properties Properties(string Drive, string Path)
        {
            Properties ret = new Data.MyFiles.Properties();
            Path = "/" + Path;
            hapConfig config = hapConfig.Current;
            List<HAP.Data.MyFiles.File> Items = new List<Data.MyFiles.File>();
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();
            try
            {
                DriveMapping mapping;
                string path = Converter.DriveToUNC(Path, Drive, out mapping, user);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Properties", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Requesting properties of: " + path);
                FileAttributes attr = System.IO.File.GetAttributes(path);
                //detect whether its a directory or file
                ret = ((attr & FileAttributes.Directory) == FileAttributes.Directory) ? new Properties(new DirectoryInfo(path), mapping, user) : new Properties(new FileInfo(path), mapping, user);
            }
            finally { user.EndContainedImpersonate(); }
            
            return ret;
        }

        [OperationContract]
        [WebGet(UriTemplate="{Drive}/{*Path}")]
        public HAP.Data.MyFiles.File[] List(string Drive, string Path)
        {
            DateTime d1 = DateTime.Now;
            Path = "/" + Path;
            hapConfig config = hapConfig.Current;
            List<HAP.Data.MyFiles.File> Items = new List<Data.MyFiles.File>();
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();
            try
            {
                DriveMapping mapping;
                string path = Converter.DriveToUNC(Path, Drive, out mapping, user);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.List", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Requesting list of: " + path);
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
                            Items.Add(new Data.MyFiles.File(subdir, mapping, user, actions));
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
            hapConfig config = hapConfig.Current;
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();

            long freeBytesForUser, totalBytes, freeBytes;

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