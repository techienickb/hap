using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Web.Routing;
using HAP.Data.ComputerBrowser;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml;

namespace HAP.Web.API
{
    public class JSHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new JS();
        }
    }

    public class JS : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "text/plain";
            string s = "";
            StreamReader sr = File.OpenText(context.Server.MapPath("~/Scripts/hap.web.js.js"));
            while (!sr.EndOfStream)
            {
                s += sr.ReadLine();
            }
            sr.Close();
            s = s.Replace("root: \"/hap/\",", "root: \"" + VirtualPathUtility.ToAbsolute("~/") + "\",");
            s = s.Replace("\t", "").Replace("  ", " ").Replace("  ", " ");
            s = s.Replace("localization: []", "localization: [" + string.Join(", ", BuildLocalization(_locals.SelectSingleNode("/hapStrings"), "")) + "]");
            context.Response.Write(s);
        }

        private string[] BuildLocalization(XmlNode node, string salt)
        {
            List<string> s = new List<string>();
            string _salt = salt;
            if (node.Name != "hapStrings") _salt += "/" + node.Name;
            if (_salt.StartsWith("/")) _salt = _salt.Remove(0, 1);
            if (node.HasChildNodes && node.ChildNodes[0].Name != "#text") foreach (XmlNode n in node.ChildNodes) s.AddRange(BuildLocalization(n, _salt));
            else s.Add("{ name: '" + _salt + "', value: '" + node.InnerText.Replace("'", "\'").Replace("\\", "\\\\") + "' }");
            return s.ToArray();
        }

        protected XmlDocument _locals
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
    }
}