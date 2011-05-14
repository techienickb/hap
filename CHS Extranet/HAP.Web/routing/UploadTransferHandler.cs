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
                    pcontext = HAP.AD.ADUtil.PContext;
                    up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, HAP.AD.ADUtil.Username);

                    string userhome = up.HomeDirectory;
                    if (!userhome.EndsWith("\\")) userhome += "\\";
                    string path = RoutingPath.Replace('^', '&');
                    uncpath unc = null;
                    unc = config.MyComputer.UNCPaths[RoutingDrive];
                    path = string.Format(unc.UNC.Replace("%homepath%", up.HomeDirectory), HAP.AD.ADUtil.Username) + '\\' + path.Replace('/', '\\');
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

        private PrincipalContext pcontext;
        private UserPrincipal up;
        private hapConfig config;


        void fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
        {
        }
    }
}