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
                string space = "";
                bool showspace = false;
                if (HttpContext.Current.User.IsInRole("Domain Admins") || !path.UNC.Contains("%homepath%")) showspace = isWriteAuth(path);
                if (showspace)
                {
                    if (Win32.GetDiskFreeSpaceEx(string.Format(path.UNC.Replace("%homepath%", userhome), Username), out freeBytesForUser, out totalBytes, out freeBytes))
                        space = "|" + Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                    else space = "";
                }
                if (isAuth(path)) items.Add(new ComputerBrowserAPIItem(path.Name, "images/icons/netdrive.png", space, "Drive", BType.Drive, path.Drive + ":", isWriteAuth(path) ? AccessControlActions.Change : AccessControlActions.View));
            }
            res.Items = items.ToArray();
            List<uploadfilter> filters = new List<uploadfilter>();
            foreach (uploadfilter filter in config.MyComputer.UploadFilters)
                if (isAuth(filter)) filters.Add(filter);
            res.Filters = filters.ToArray();

            return res;
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
