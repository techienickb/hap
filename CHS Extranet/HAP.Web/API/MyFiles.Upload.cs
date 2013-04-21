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
using System.Xml;
using HAP.AD;

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
                if (_ADUser == null)
                {
                    hapConfig config = hapConfig.Current;
                    _ADUser = new User();
                    if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
                    {
                        HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                        if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                        _ADUser.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
                    }
                    else
                    {
                        _ADUser = new User(HttpContext.Current.User.Identity.Name);
                    }
                }
                return _ADUser;
            }
        }

        private bool isAuth(string extension)
        {
            foreach (Filter filter in hapConfig.Current.MyFiles.Filters)
                if (filter.Expression.ToLower().Contains(extension.ToLower())) return true;
            return isAuth(hapConfig.Current.MyFiles.Filters.Single(fil => fil.Name == "All Files"));
        }

        private bool isAuth(Filter filter)
        {
            if (filter.EnableFor == "All") return true;
            else if (filter.EnableFor != "None")
            {
                bool vis = false;
                foreach (string s in filter.EnableFor.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        protected XmlDocument _doc
        {
            get
            {
                if (HttpContext.Current.Cache.Get("hapLocal") == null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~/App_LocalResources/" + hapConfig.Current.Local + "/Strings.xml"));
                    HttpContext.Current.Cache.Insert("hapLocal", doc, new System.Web.Caching.CacheDependency(HttpContext.Current.Server.MapPath("~/App_LocalResources/" + hapConfig.Current.Local + "/Strings.xml")));
                }
                return (XmlDocument)HttpContext.Current.Cache.Get("hapLocal");
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request.Headers["X_FILENAME"]))
            {

                if (!isAuth(Path.GetExtension(context.Request.Headers["X_FILENAME"]))) throw new UnauthorizedAccessException(_doc.SelectSingleNode("/hapStrings/myfiles/upload/filetypeerror").InnerText);
                DriveMapping m;
                string path = Path.Combine(Converter.DriveToUNC('\\' + RoutingPath, RoutingDrive, out m, ADUser), context.Request.Headers["X_FILENAME"]);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Upload", ADUser.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Uploading of: " + context.Request.Headers["X_FILENAME"] + " to: " + path);
                try
                {
                    ADUser.ImpersonateContained();
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