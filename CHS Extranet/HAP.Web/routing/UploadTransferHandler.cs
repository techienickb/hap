using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.IO;

namespace HAP.Web.routing
{
    public class UploadTransferHander : IHttpHandler, IMyComputerDisplay
    {
        public UploadTransferHander(string path)
        {
            if (path.Length > 2)
            {
                RoutingPath = path.Remove(0, 2);
                RoutingDrive = path.Substring(0, 1);
            }
            else { RoutingPath = ""; RoutingDrive = path; }
        }

        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (context.Request.HttpMethod == "POST")
                {
                    config = hapConfig.Current;
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
                    string path = RoutingPath.Replace('^', '&');
                    uncpath unc = null;
                    unc = config.MyComputer.UNCPaths[RoutingDrive];
                    path = string.Format(unc.UNC.Replace("%homepath%", up.HomeDirectory), Username) + '\\' + path.Replace('/', '\\');
                    UploadProcess fileUpload = new UploadProcess();
                    fileUpload.FileUploadCompleted += new FileUploadCompletedEvent(fileUpload_FileUploadCompleted);
                    fileUpload.ProcessRequest(context, path);
                }
                else
                {
                    context.Response.Write("You have reached this page via GET, this is NOT supported!\n");
                    context.Response.Write("DEBUG INFO:\n");
                    context.Response.Write(RoutingDrive + "\n");
                    context.Response.Write(RoutingPath + "\n");
                }
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

        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private hapConfig config;

        public string Username
        {
            get
            {
                if (HttpContext.Current.User.Identity.Name.Contains('\\'))
                    return HttpContext.Current.User.Identity.Name.Remove(0, HttpContext.Current.User.Identity.Name.IndexOf('\\') + 1);
                else return HttpContext.Current.User.Identity.Name;
            }
        }

        void fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
        {
        }
    }
}