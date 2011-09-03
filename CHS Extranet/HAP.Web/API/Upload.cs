using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.DirectoryServices.AccountManagement;
using HAP.Web.Configuration;
using HAP.Web.routing;
using System.IO;
using System.Configuration;
using HAP.Data.ComputerBrowser;
using System.Web.Security;

namespace HAP.Web.API
{
    public class UploadHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new Upload(path, drive);
        }
    }

    public class Upload : IHttpHandler
    {

        public Upload(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }
        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            ((HAP.AD.User)Membership.GetUser()).Impersonate();
            context.Response.ExpiresAbsolute = DateTime.Now;
            try
            {
                if (context.Request.HttpMethod == "POST")
                {
                    UploadProcess fileUpload = new UploadProcess();
                    fileUpload.FileUploadCompleted += new FileUploadCompletedEvent(fileUpload_FileUploadCompleted);
                    fileUpload.ProcessRequest(context, Converter.DriveToUNC(RoutingPath, RoutingDrive));
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
            ((HAP.AD.User)Membership.GetUser()).EndImpersonate();
        }

        void fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
        {
        }
    }
}