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
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using Microsoft.Ajax.Utilities;

namespace HAP.Web.API
{
    public class JSHandler : IRouteHandler
    {
        public JSHandler(JSType type)
        {
            JSType = type;
        }
        public JSType JSType { get; set; }
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new JS(JSType);
        }
    }

    public enum JSType { CSS, Start, BeforeHAP, HAP, AfterHAP, End }

    public class JS : IHttpHandler, IRequiresSessionState
    {
        public JS(JSType type)
        {
            JSType = type;
        }

        public JSType JSType { get; set; }

        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "text/plain";
            Minifier minifier = new Minifier();
            if (JSType == API.JSType.HAP)
            {
                string s = "";
                StreamReader sr = File.OpenText(context.Server.MapPath("~/Scripts/hap.web.js.js"));
                while (!sr.EndOfStream)
                {
                    s += sr.ReadLine();
                }
                sr.Close();
                s = s.Replace("root: \"/hap/\",", "root: \"" + VirtualPathUtility.ToAbsolute("~/") + "\",");
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    s = s.Replace("user: \"\",", "user: \"" + ((HAP.AD.User)Membership.GetUser()).UserName + "\",");
                    s = s.Replace("admin: false,", "admin: " + HttpContext.Current.User.IsInRole("Domain Admins").ToString().ToLower() + ",");
                    s = s.Replace("bsadmin: false,", "bsadmin: " + isBSAdmin.ToString().ToLower() + ",");
                    s = s.Replace("hdadmin: false,", "hdadmin: " + isHDAdmin.ToString().ToLower() + ",");
                }
                s = s.Replace("localization: []", "localization: [" + string.Join(", ", BuildLocalization(_locals.SelectSingleNode("/hapStrings"), "")) + "]");
#if !DEBUG
                s = minifier.MinifyJavaScript(s);
#endif
                context.Response.Write(s);
            }
            else 
            {
                if (JSType == API.JSType.CSS) context.Response.ContentType = "text/css";
                RegistrationPath[] paths = GetPaths(context.Request.QueryString.Keys[0]);
                int status = 200;
                if (paths.Length > 0)
                {
                    DateTime lastModified = File.GetLastWriteTimeUtc(context.Server.MapPath(paths[0].Path));

                    foreach (RegistrationPath s in paths)
                    {
                        DateTime l = File.GetLastWriteTimeUtc(context.Server.MapPath(s.Path));
                        if (lastModified == null || lastModified < l) lastModified = l;
                        context.Response.AddFileDependency(context.Server.MapPath(s.Path));
                    }
                    lastModified = new DateTime(lastModified.Year, lastModified.Month, lastModified.Day, lastModified.Hour, lastModified.Minute, lastModified.Second, 0, DateTimeKind.Utc);

                    context.Response.Cache.SetETagFromFileDependencies();
                    context.Response.Cache.SetLastModifiedFromFileDependencies();
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);


                    if (context.Request.Headers["If-Modified-Since"] != null)
                    {
                        status = 304;
                        DateTime modifiedSinceDate = DateTime.UtcNow;
                        if (DateTime.TryParse(context.Request.Headers["If-Modified-Since"], out modifiedSinceDate))
                        {
                            if (lastModified != modifiedSinceDate)
                                status = 200;
                        }
                    }
                }
                context.Response.StatusCode = status;
                if (status == 200)
                {

                    foreach (RegistrationPath s in paths)
                    {
                        //context.Response.Write("\n/* " + s.Path + " */\n\n");
                        StreamReader sr = File.OpenText(context.Server.MapPath(s.Path));
                        string f = sr.ReadToEnd();
#if !DEBUG
                        if (s.Minify) {
                            if (JSType != API.JSType.CSS)
                                f = minifier.MinifyJavaScript(f);
                            else f = minifier.MinifyStyleSheet(f);
                        }
#endif
                        sr.Close();
                        context.Response.Write(f);
                    }

                }

            }
        }


        public RegistrationPath[] GetPaths(string Referrer)
        {
            List<RegistrationPath> s = new List<RegistrationPath>();
            foreach (RegistrationPath path in GetPaths())
            {
                bool load = false;
                foreach (string s1 in path.LoadOn)
                {
                    if (s1.ToLower() == "all") { load = true; break; }
                    Regex r = new Regex(s1, RegexOptions.IgnoreCase);
                    if (r.IsMatch(Referrer)) { load = true; break; }
                }
                if (load) s.Add(path);
            }
            return s.ToArray();
        }

        public RegistrationPath[] GetPaths()
        {
            List<RegistrationPath> paths = new List<RegistrationPath>();
            foreach (IRegister reg in GetPlugins())
                try
                {
                    if (JSType == API.JSType.CSS)
                        paths.AddRange(reg.RegisterCSS());
                    else if (JSType == API.JSType.Start)
                        paths.AddRange(reg.RegisterJSStart());
                    else if (JSType == API.JSType.BeforeHAP)
                        paths.AddRange(reg.RegisterJSBeforeHAP());
                    else if (JSType == API.JSType.AfterHAP)
                        paths.AddRange(reg.RegisterJSAfterHAP());
                    else if (JSType == API.JSType.End)
                        paths.AddRange(reg.RegisterJSEnd());
                }
                catch { }
            return paths.ToArray();
        }

        public IRegister[] GetPlugins()
        {
            List<IRegister> plugins = new List<IRegister>();
            //load apis in the bin folder
            foreach (FileInfo assembly in new DirectoryInfo(HttpContext.Current.Server.MapPath("~/bin/")).GetFiles("*.dll").Where(fi => fi.Name != "HAP.Web.dll" && fi.Name != "HAP.Web.Configuration.dll" && !fi.Name.StartsWith("Microsoft")))
            {
                Assembly a = Assembly.LoadFrom(assembly.FullName);
                foreach (Type type in a.GetTypes())
                {
                    if (!type.IsClass || type.IsNotPublic) continue;
                    Type[] interfaces = type.GetInterfaces();
                    if (((IList)interfaces).Contains(typeof(IRegister)))
                    {
                        object obj = Activator.CreateInstance(type);
                        IRegister t = (IRegister)obj;
                        plugins.Add(t);
                    }
                }
            }
            return plugins.ToArray();
        }

        public bool isBSAdmin
        {
            get
            {
                foreach (string s in hapConfig.Current.BookingSystem.Admins.Split(new char[] { ',' }))
                    if (s.Trim().ToLower().Equals(((HAP.AD.User)Membership.GetUser()).UserName.ToLower())) return true;
                    else if (HttpContext.Current.User.IsInRole(s.Trim())) return true;
                return false;
            }
        }

        public bool isHDAdmin
        {
            get
            {
                foreach (string s in hapConfig.Current.HelpDesk.Admins.Split(new char[] { ',' }))
                    if (s.Trim().ToLower().Equals(((HAP.AD.User)Membership.GetUser()).UserName.ToLower())) return true;
                    else if (HttpContext.Current.User.IsInRole(s.Trim())) return true;
                return false;
            }
        }

        private string[] BuildLocalization(XmlNode node, string salt)
        {
            List<string> s = new List<string>();
            string _salt = salt;
            if (node.Name != "hapStrings") _salt += "/" + node.Name;
            if (_salt.StartsWith("/")) _salt = _salt.Remove(0, 1);
            if (node.HasChildNodes && node.ChildNodes[0].Name != "#text") foreach (XmlNode n in node.ChildNodes) s.AddRange(BuildLocalization(n, _salt));
            else s.Add("{ name: '" + _salt + "', value: '" + node.InnerText.Replace("'", "\'").Replace("\n", "\\n").Replace("\r", "\\r") + "' }");
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