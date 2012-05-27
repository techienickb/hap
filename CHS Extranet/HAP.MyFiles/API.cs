using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using HAP.MyFiles;
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
using System.Data;
using System.Net;
using System.Collections.Specialized;
using ICSharpCode.SharpZipLib.Zip;

namespace HAP.MyFiles
{
    [ServiceAPI("api/myfiles")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class API
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
            HAP.Web.SendTo.Google.Client client = new HAP.Web.SendTo.Google.Client();
            client.Login(username, password);
            DriveMapping mapping;
            string p = Converter.DriveToUNC('/' + Path, Drive, out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.SendTo.Google", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Sending to Google Docs: " + p);
            user.ImpersonateContained();
            try 
            {
                return client.Upload(p);
            } 
            finally 
            { 
                user.EndContainedImpersonate(); 
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SendTo/SkyDrive/{Drive}/{*Path}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void SkyDrive(string Drive, string Path, string accessToken)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            DriveMapping mapping;
            string p = Converter.DriveToUNC('/' + Path, Drive, out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.SendTo.SkyDrive", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Sending to SkyDrive: " + p);
            user.ImpersonateContained();
            try
            {
                HAP.Web.SendTo.SkyDrive.UploadFileToSkydrive(new FileInfo(p), accessToken);
            }
            finally
            {
                user.EndContainedImpersonate();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Zip", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        public void Zip(string Zip, string[] Paths)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            DriveMapping mapping;
            string z = Converter.DriveToUNC(Zip.Remove(0, 1), Zip.Substring(0, 1), out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Zip", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Zip: " + z);
            user.ImpersonateContained();
            try
            {
                ZipFile zip = System.IO.File.Exists(z) ? new ZipFile(z) : ZipFile.Create(z);
                zip.BeginUpdate();
                foreach (string path in Paths)
                {
                    try
                    {
                        string p = Converter.DriveToUNC(path.Remove(0, 1), path.Substring(0, 1), out mapping, user);
                        FileAttributes attr = System.IO.File.GetAttributes(p);
                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory) zip.AddDirectory(p);
                        else zip.Add(p);
                    }
                    catch { }
                }
                zip.CommitUpdate();
                zip.Close();
            }
            finally
            {
                user.EndContainedImpersonate();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Unzip", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Unzip(string ZipFile, bool Overwrite)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            DriveMapping mapping;
            string p = Converter.DriveToUNC(ZipFile.Remove(0, 1), ZipFile.Substring(0, 1), out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.UnZIP", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Unzipping: " + p);
            user.ImpersonateContained();
            string p2 = Path.Combine(new FileInfo(p).Directory.FullName, Path.GetFileNameWithoutExtension(p));
            try
            {
                if (Directory.Exists(p2) && !Overwrite) throw new DuplicateNameException("Destination Folder Exists");
                else Directory.CreateDirectory(p2);
                new FastZip().ExtractZip(p, p2, "");
            }
            finally
            {
                user.EndContainedImpersonate();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Copy", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Copy(string OldPath, string NewPath, bool Overwrite)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            DriveMapping mapping;
            string p = Converter.DriveToUNC(OldPath.Remove(0, 1), OldPath.Substring(0, 1), out mapping, user);
            string p2 = Converter.DriveToUNC(NewPath.Remove(0, 1), NewPath.Substring(0, 1), out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Copy", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Copying: " + p);
            user.ImpersonateContained();
            try
            {
                FileAttributes attr = System.IO.File.GetAttributes(p);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    if (Directory.Exists(p2) && !Overwrite) throw new DuplicateNameException("Destination Folder Exists");
                    else copyDirectory(p, p2);
                }
                else
                {
                    if (System.IO.File.Exists(p2) && !Overwrite) throw new DuplicateNameException("File Exits in Destination");
                    else System.IO.File.Copy(p, p2);
                }
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
                else if (System.IO.File.Exists(Dst + Path.GetFileName(Element)))
                {
                    if (System.IO.File.GetLastWriteTime(Dst + Path.GetFileName(Element)) < System.IO.File.GetLastWriteTime(Src + Path.GetFileName(Element))) System.IO.File.Copy(Element, Dst + Path.GetFileName(Element), true);
                }
                else System.IO.File.Copy(Element, Dst + Path.GetFileName(Element), true);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Move", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Move(string OldPath, string NewPath, bool Overwrite)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            DriveMapping mapping;
            string p = Converter.DriveToUNC(OldPath.Remove(0, 1), OldPath.Substring(0, 1), out mapping, user);
            string p2 = Converter.DriveToUNC(NewPath.Remove(0, 1), NewPath.Substring(0, 1), out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Move", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Moving: " + p);
            user.ImpersonateContained();
            try
            {
                FileAttributes attr = System.IO.File.GetAttributes(p);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    if (Directory.Exists(p2) && !Overwrite) throw new DuplicateNameException("Destination Folder Exists");
                    else Directory.Move(p, p2);
                }
                else
                {
                    if (System.IO.File.Exists(p2) && !Overwrite) throw new DuplicateNameException("File Exits in Destination");
                    else System.IO.File.Move(p, p2);
                }
            }
            finally
            {
                user.EndContainedImpersonate();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Delete", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
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

            foreach (string path in Paths) 
            {
                try
                {
                    DriveMapping mapping;
                    string p = Converter.DriveToUNC(path.Remove(0, 1), path.Substring(0, 1), out mapping, user);
                    HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Delete", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Deleting: " + p);
                    user.ImpersonateContained();
                    FileAttributes attr = System.IO.File.GetAttributes(p);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory) Directory.Delete(p, true);
                    else System.IO.File.Delete(p);
                    ret.Add("Deleted " + path.Remove(0, path.LastIndexOf('/') + 1));
                }
                catch { ret.Add("I could not delete :" + path.Remove(0, path.LastIndexOf('/') + 1)); }
                finally
                {
                    user.EndContainedImpersonate();
                }
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
            DriveMapping mapping;
            string path = Converter.DriveToUNC("/" + Path, Drive, out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.NewFolder", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Creating new folder: " + path);
            user.ImpersonateContained();
            try
            {
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
            DriveMapping mapping;
            string path = Converter.DriveToUNC("/" + Path, Drive, out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Preview", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Requesting preview of: " + path);
            user.ImpersonateContained();
            FileInfo file = new FileInfo(path);

            string s = "";

            try
            {
                if (Path.ToLower().EndsWith(".docx"))
                {
                    s = HAP.MyFiles.DocXPreview.Render(file.FullName);
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
                else if (HAP.MyFiles.File.GetMimeType(file.Extension.ToLower()).StartsWith("image"))
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
            Properties ret = new Properties();
            Path = "/" + Path;
            hapConfig config = hapConfig.Current;
            List<File> Items = new List<File>();
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
                if (path.ToLower().Contains(".zip\\")) path = path.Remove(path.ToLower().IndexOf(".zip\\") + 5);
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
            Properties ret = new Properties();
            Path = "/" + Path;
            hapConfig config = hapConfig.Current;
            List<File> Items = new List<File>();
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            DriveMapping mapping;
            string path = Converter.DriveToUNC(Path, Drive, out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Properties", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Requesting properties of: " + path);
            user.ImpersonateContained();
            try
            {
                FileAttributes attr = System.IO.File.GetAttributes(path);
                //detect whether its a directory or file
                ret = ((attr & FileAttributes.Directory) == FileAttributes.Directory) ? new Properties(new DirectoryInfo(path), mapping, user) : new Properties(new FileInfo(path), mapping, user);
            }
            finally { user.EndContainedImpersonate(); }
            
            return ret;
        }

        private List<HAP.MyFiles.File> ZIPList(string Drive, string Path, User user)
        {
            List<File> Items = new List<File>();
            DriveMapping mapping;
            string path = Converter.DriveToUNC(Path, Drive, out mapping, user);
            string subpath = (path.Length == path.ToLower().IndexOf(".zip") + 4) ? "" : (path.Remove(0, path.ToLower().IndexOf(".zip") + 5) + "\\");
            ICSharpCode.SharpZipLib.Zip.ZipFile zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(subpath == "" ? path : path.Remove(path.ToLower().IndexOf(".zip") + 4));
            try
            {
                foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry ze in zf)
                {
                    if (ze.IsDirectory && ze.Name != "None")
                    {
                        File f = new File();
                        f.Actions = AccessControlActions.ZIP;
                        f.CreationTime = f.ModifiedTime = ze.DateTime.ToShortDateString() + " " + ze.DateTime.ToString("hh:mm");
                        f.Icon = "../images/icons/folder.png";
                        f.Size = "";
                        f.Description = "File Folder";
                        f.Extension = "";
                        f.Name = ze.Name.Replace('/', '\\');
                        if (f.Name.EndsWith("\\")) f.Name = f.Name.Remove(f.Name.Length - 1);
                        f.Type = "Directory";

                        if (subpath == "" && f.Name.Contains('\\')) continue;
                        else if (subpath != "" && !f.Name.StartsWith(subpath)) continue;
                        else if (subpath != "" && f.Name.StartsWith(subpath) && f.Name.Remove(0, subpath.Length).Contains('\\')) continue;
                        if (subpath != "") f.Name = f.Name.Replace(subpath, "");
                        f.Path = Converter.UNCtoDrive(path + "/" + f.Name, mapping, user).Replace(":", "").Replace('/', '\\');
                        Items.Add(f);
                    }
                }
                foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry ze in zf)
                {
                    if (ze.IsFile && ze.Name != "None")
                    {
                        File f = new File();
                        f.Actions = AccessControlActions.ZIP;
                        f.CreationTime = f.ModifiedTime = ze.DateTime.ToShortDateString() + " " + ze.DateTime.ToString("hh:mm");
                        f.Icon = "../images/icons/file.png";
                        f.Name = ze.Name.Replace('/', '\\');

                        if (subpath == "" && f.Name.Contains('\\')) continue;
                        else if (subpath != "" && !f.Name.StartsWith(subpath)) continue;
                        else if (subpath != "" && f.Name.StartsWith(subpath) && f.Name.Remove(0, subpath.Length + 1).Contains('\\')) continue;
                        if (subpath != "") f.Name = f.Name.Replace(subpath, "");
                        f.Type = "File";
                        f.Path = Converter.UNCtoDrive(path + "/" + f.Name, mapping, user).Replace(":", "").Replace('/', '\\');
                        f.Size = File.parseLength(ze.Size);
                        Items.Add(f);
                    }
                }

            }
            finally
            {
                if (zf != null) { zf.IsStreamOwner = true; zf.Close(); }
            }
            return Items;
        }

        [OperationContract]
        [WebGet(UriTemplate="{Drive}/{*Path}")]
        public File[] List(string Drive, string Path)
        {
            Path = "/" + Path;
            hapConfig config = hapConfig.Current;
            List<File> Items = new List<File>();
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            DriveMapping mapping;
            string path = Converter.DriveToUNC(Path, Drive, out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.List", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Requesting list of: " + path);
            user.ImpersonateContained();
            try
            {
                if (path.ToLower().IndexOf(".zip") != -1) Items = ZIPList(Drive, Path, user);
                else
                {
                    HAP.MyFiles.AccessControlActions allowactions = isWriteAuth(mapping) ? HAP.MyFiles.AccessControlActions.Change : HAP.MyFiles.AccessControlActions.View;
                    DirectoryInfo dir = new DirectoryInfo(path);
                    foreach (DirectoryInfo subdir in dir.GetDirectories())
                        try
                        {
                            bool isHidden = (subdir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                            bool isSystem = (subdir.Attributes & FileAttributes.System) == FileAttributes.System;
                            if (!subdir.Name.ToLower().Contains("recycle") && !isSystem && !isHidden && !subdir.Name.ToLower().Contains("system volume info"))
                            {
                                HAP.MyFiles.AccessControlActions actions = allowactions;
                                if (config.MySchoolComputerBrowser.WriteChecks)
                                {
                                    if (actions == HAP.MyFiles.AccessControlActions.Change)
                                    {
                                        try { System.IO.File.Create(System.IO.Path.Combine(subdir.FullName, "temp.ini")).Close(); System.IO.File.Delete(System.IO.Path.Combine(subdir.FullName, "temp.ini")); }
                                        catch { actions = HAP.MyFiles.AccessControlActions.View; }
                                    }
                                }
                                try { subdir.GetDirectories(); }
                                catch { actions = HAP.MyFiles.AccessControlActions.None; }
                                Items.Add(new File(subdir, mapping, user, actions));
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
                                Items.Add(new File(file, mapping, user, allowactions));
                        }
                        catch
                        {
                            //Response.Redirect("unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                        }
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

            long freeBytesForUser, totalBytes, freeBytes;

            foreach (DriveMapping p in config.MySchoolComputerBrowser.Mappings.Values.OrderBy(m => m.Drive))
            {
                decimal space = -1;
                bool showspace = isWriteAuth(p);
                if (showspace)
                {
                    if (p.UsageMode == MappingUsageMode.DriveSpace)
                    {
                        try
                        {
                            user.ImpersonateContained();
                            if (Win32.GetDiskFreeSpaceEx(Converter.FormatMapping(p.UNC, user), out freeBytesForUser, out totalBytes, out freeBytes))
                                space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                        }
                        finally { user.EndContainedImpersonate(); }
                    }
                    else if (p.UsageMode == MappingUsageMode.HAPDriveSpace)
                    {
                        if (Win32.GetDiskFreeSpaceEx(Converter.FormatMapping(p.UNC, user), out freeBytesForUser, out totalBytes, out freeBytes))
                            space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                    }
                    else if (p.UsageMode == MappingUsageMode.Quota)
                    {
                        try
                        {
                            user.ImpersonateContained();
                            HAP.Data.Quota.QuotaInfo qi = new Data.Quota.QuotaInfo();
                            try
                            {
                                qi = HAP.Data.ComputerBrowser.Quota.GetQuota(user.UserName, Converter.FormatMapping(p.UNC, user));
                                space = Math.Round((Convert.ToDecimal(qi.Used) / Convert.ToDecimal(qi.Total)) * 100, 2);
                            }
                            catch (Exception) { }
                            if (qi.Total == -1)
                                if (Win32.GetDiskFreeSpaceEx(Converter.FormatMapping(p.UNC, user), out freeBytesForUser, out totalBytes, out freeBytes))
                                    space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                        }
                        finally { user.EndContainedImpersonate(); }
                    }
                }
                if (isAuth(p)) drives.Add(new Drive(p.Name, "../images/icons/netdrive.png", space, p.Drive.ToString() + "\\", isWriteAuth(p) ? HAP.MyFiles.AccessControlActions.Change : HAP.MyFiles.AccessControlActions.View));
            }
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