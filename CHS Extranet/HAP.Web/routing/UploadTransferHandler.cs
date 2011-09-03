using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.IO;
using HAP.Data.ComputerBrowser;
using System.Web.Security;

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
                    ADUser.Impersonate();
                    string userhome = ADUser.HomeDirectory;
                    if (!userhome.EndsWith("\\")) userhome += "\\";
                    string path = RoutingPath.Replace('^', '&');
                    DriveMapping unc = null;
                    unc = config.MySchoolComputerBrowser.Mappings[RoutingDrive.ToCharArray()[0]];
                    path = Converter.FormatMapping(unc.UNC, ADUser) + '\\' + path.Replace('/', '\\');
                    UploadProcess fileUpload = new UploadProcess();
                    fileUpload.FileUploadCompleted += new FileUploadCompletedEvent(fileUpload_FileUploadCompleted);
                    fileUpload.ProcessRequest(context, path);
                    ADUser.EndImpersonate();
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
                ADUser.EndImpersonate();
                FileInfo file = new FileInfo(context.Server.MapPath("~/App_Data/log.log"));
                if (!file.Exists) file.Create();
                StreamWriter sw = file.AppendText();
                sw.WriteLine(e.Message);
                sw.Close();
            }
            finally
            {
                ADUser.EndImpersonate();
            }
        }
        private HAP.AD.User _ADUser = null;
        public HAP.AD.User ADUser
        {
            get
            {
                if (_ADUser == null) _ADUser = ((HAP.AD.User)Membership.GetUser());
                return _ADUser;
            }
        }

        private hapConfig config;


        void fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
        {
        }
    }
}