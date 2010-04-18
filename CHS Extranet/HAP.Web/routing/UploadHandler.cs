using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.DirectoryServices.AccountManagement;
using HAP.Web.Configuration;
using System.IO;
using System.Configuration;

namespace HAP.Web.routing
{
    public class UploadHandler : IRouteHandler
    {
        private string mode;
        public UploadHandler(string mode) { this.mode = mode; }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            if (mode == "check")
            {
                string drive = requestContext.RouteData.Values["drive"] as string;
                return new UploadCheckerHander(path, drive);
            }
            return new UploadTransferHander(path);
        }
    }

}