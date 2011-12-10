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
using System.Web.SessionState;

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

    public class Upload : IHttpHandler, IRequiresSessionState 
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
            HAP.AD.User user = Membership.GetUser() as HAP.AD.User;
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MSCB.Upload", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Uploading of: " + HttpContext.Current.Request.QueryString["filename"] + " to Drive: " + RoutingDrive + " Path: " + RoutingPath);
            user.ImpersonateContained();
            try
            {
                context.Response.ExpiresAbsolute = DateTime.Now;
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
            finally
            {
                user.EndContainedImpersonate();
            }
        }

        void fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
        {
        }
    }
}