using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.Security;
using System.Net;
using HAP.Web.routing;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Xml;

namespace HAP.Web.API
{
    public class ListDrivesHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ListDrives();
        }
    }

    public class ListDrives : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            Context = context;
            config = hapConfig.Current;

            context.Response.Clear();
            context.Response.Headers.Add("HAP:API", "ListDrives");
            context.Response.ContentType = "text/plain";
            string format = "{0},{1},{2},{3}\n";
            context.Response.Write(string.Format(format, "My Documents", "/extranet/images/icons/netdrive.png", "/Extranet/API/MyComputer/List/N", true));
            foreach (uncpath path in config.MyComputer.UNCPaths)
                if (isAuth(path)) context.Response.Write(string.Format(format, path.Name, "/extranet/images/icons/netdrive.png", string.Format("/Extranet/API/MyComputer/list/{0}", path.Drive), isWriteAuth(path)));

            foreach (uploadfilter filter in config.MyComputer.UploadFilters)
                if (isAuth(filter)) context.Response.Write("FILTER" + filter.ToString() + "\n");

            context.Response.ContentType = "text/plain";
        }

        private bool isAuth(uploadfilter filter)
        {
            if (filter.EnableFor == "All") return true;
            else if (filter.EnableFor != "None")
            {
                bool vis = false;
                foreach (string s in filter.EnableFor.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Context.User.IsInRole(s);
                return vis;
            }
            return false;
        }

        private bool isAuth(uncpath path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Context.User.IsInRole(s);
                return vis;
            }
            return false;
        }

        private bool isWriteAuth(uncpath path)
        {
            if (path == null) return true;
            if (path.EnableWriteTo == "All") return true;
            else if (path.EnableWriteTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableWriteTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Context.User.IsInRole(s);
                return vis;
            }
            return false;
        }

        private hapConfig config;
        private HttpContext Context;

        public string Username
        {
            get
            {
                if (Context.User.Identity.Name.Contains('\\'))
                    return Context.User.Identity.Name.Remove(0, Context.User.Identity.Name.IndexOf('\\') + 1);
                else return Context.User.Identity.Name;
            }
        }
    }
}