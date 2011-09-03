using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using HAP.Data.ComputerBrowser;
using System.Web.Security;

namespace HAP.Web.routing
{
    public class UploadCheckerHander : IHttpHandler, IMyComputerDisplay
    {
        public UploadCheckerHander(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }

        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public bool IsReusable { get { return true; } }

        private HAP.AD.User _ADUser = null;
        public HAP.AD.User ADUser
        {
            get
            {
                if (_ADUser == null) _ADUser = ((HAP.AD.User)Membership.GetUser());
                return _ADUser;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            config = hapConfig.Current;
            ADUser.Impersonate();
            string userhome = ADUser.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            string path = RoutingPath.Replace('^', '&');
            DriveMapping unc = null;
            unc = config.MySchoolComputerBrowser.Mappings[RoutingDrive.ToCharArray()[0]];
            path = Converter.FormatMapping(unc.UNC, ADUser) + '\\' + path.Replace('/', '\\');
            FileInfo file = new FileInfo(path);
            context.Response.Clear();
            context.Response.ContentType = "text/plain";
            context.Response.Write(file.Exists.ToString());
            context.Response.Write(",");
            context.Response.Write(MyComputerItem.ParseForImage(file));
            ADUser.EndImpersonate();
        }

        private hapConfig config;

    }
}