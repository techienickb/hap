using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DC.SilverlightFileUpload;
using System.IO;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using CHS_Extranet.Configuration;

namespace CHS_Extranet
{
    /// <summary>
    /// Summary description for FileUpload
    /// </summary>
    public class FileUpload : IHttpHandler
    {
        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private GroupPrincipal studentgp, admindrivegp, smt;

        public string Username
        {
            get
            {
                if (HttpContext.Current.User.Identity.Name.Contains('\\'))
                    return HttpContext.Current.User.Identity.Name.Remove(0, HttpContext.Current.User.Identity.Name.IndexOf('\\') + 1);
                else return HttpContext.Current.User.Identity.Name;
            }
        }

        private bool isAuth(uncpath path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, s);
                    if (!vis) vis = up.IsMemberOf(gp);
                }
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
                {
                    GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, s);
                    if (!vis) vis = up.IsMemberOf(gp);
                }
                return vis;
            }
            return false;
        }

        private HttpContext ctx;
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                extranetConfig config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
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
                if (!userhome.EndsWith("\\")) userhome += "\\";
                string path = context.Request.PathInfo.Remove(0, 2);
                string p = context.Request.PathInfo.Substring(1, 1);
                if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
                else path = string.Format(config.UNCPaths[p].UNC, Username) + path.Replace('/', '\\');

                if (!isWriteAuth(config.UNCPaths[p]))
                    context.Response.Redirect("/extranet/unauthorised.aspx", true);

                ctx = context;
                FileUploadProcess fileUpload = new FileUploadProcess();
                fileUpload.FileUploadCompleted += new FileUploadCompletedEvent(fileUpload_FileUploadCompleted);
                fileUpload.ProcessRequest(context, path);
            }
            catch (Exception e)
            {
                FileInfo file = new FileInfo(context.Server.MapPath("~/App_Data/log.log"));
                if (!file.Exists) file.Create();
                StreamWriter sw = file.AppendText();
                sw.WriteLine(e.Message);
                sw.Close();
            }
        }

        void fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
        {
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}