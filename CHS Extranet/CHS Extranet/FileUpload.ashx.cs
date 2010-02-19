using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DC.SilverlightFileUpload;
using System.IO;
using System.Configuration;
using System.DirectoryServices.AccountManagement;

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

        private HttpContext ctx;
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings["ADConnectionString"];
                if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
                if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                    throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
                if (_ActiveDirectoryConnectionString.StartsWith("LDAP:/"))
                    _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
                else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
                pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, ConfigurationManager.AppSettings["ADUsername"], ConfigurationManager.AppSettings["ADPassword"]);
                studentgp = GroupPrincipal.FindByIdentity(pcontext, ConfigurationManager.AppSettings["StudentGroup"]);
                if (ConfigurationManager.AppSettings["EnableAdmin"] == "True")
                {
                    admindrivegp = GroupPrincipal.FindByIdentity(pcontext, ConfigurationManager.AppSettings["AdminStaffGroup"]);
                    smt = GroupPrincipal.FindByIdentity(pcontext, ConfigurationManager.AppSettings["SMTGroup"]);
                }
                up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);
                string userhome = up.HomeDirectory;
                if (!userhome.EndsWith("\\")) userhome += "\\";
                string path = context.Request.PathInfo.Remove(0, 2);
                string p = context.Request.PathInfo.Substring(1, 1);
                if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
                else if (p == "W") path = ConfigurationManager.AppSettings["SharedDocsUNC"] + path.Replace('/', '\\');
                else if (p == "T") path = ConfigurationManager.AppSettings["RMStaffUNC"] + path.Replace('/', '\\');
                else if (p == "H") path = string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username) + path.Replace('/', '\\');

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