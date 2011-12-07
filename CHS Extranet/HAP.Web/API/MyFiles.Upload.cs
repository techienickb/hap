using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.routing;
using System.Web.Routing;
using System.Web.Compilation;
using System.Web.Security;
using System.IO;
using HAP.Data.ComputerBrowser;
using HAP.Web.Configuration;

namespace HAP.Web.API
{
    public class MyFiles_UploadHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            MyFiles_Upload myu = new MyFiles_Upload();
            if (requestContext.RouteData.Values.ContainsKey("path")) myu.RoutingPath = requestContext.RouteData.Values["path"] as string;
            else myu.RoutingPath = string.Empty;
            myu.RoutingDrive = requestContext.RouteData.Values["drive"] as string;
            myu.RoutingDrive = myu.RoutingDrive.ToUpper();
            return myu;
        }
    }

    public class MyFiles_Upload : IHttpHandler, IMyComputerDisplay
    {
        public bool IsReusable
        {
            get { return false; }
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

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request.Headers["X_FILENAME"]))
            {
                try
                {
                    ADUser.ImpersonateContained();

                    DriveMapping m;
                    string path = Path.Combine(Converter.DriveToUNC('\\' + RoutingPath, RoutingDrive, out m, ADUser), context.Request.Headers["X_FILENAME"]);
                    Stream inputStream = context.Request.InputStream;

                    FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);

                    inputStream.CopyTo(fileStream);
                    fileStream.Close();
                }
                finally { ADUser.EndContainedImpersonate(); }

            }
            else throw new ArgumentNullException("No File Attached!");
        }

        public string RoutingPath { get; set;}

        public string RoutingDrive { get; set; }
    }



}