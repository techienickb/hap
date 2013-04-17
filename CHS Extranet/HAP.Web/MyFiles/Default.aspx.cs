using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Web.Configuration;
using System.Configuration;
using System.IO;
using System.Xml;

namespace HAP.Web.MyFiles
{
    public partial class Default : HAP.Web.Controls.Page
    {
        public Default()
        {
            this.SectionTitle = Localize("myfiles/myfiles");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FirstTime = true;
            W8AppCap = Request.UserAgent.ToLower().Contains("windows nt 6.2");
            if (!File.Exists(Server.MapPath("~/app_data/myfiles-appusers.txt"))) File.Create(Server.MapPath("~/app_data/myfiles-appusers.txt")).Close();
            StreamReader sr = File.OpenText(Server.MapPath("~/app_data/myfiles-users.txt"));
            string s;
            while ((s = sr.ReadLine()) != null) if (s.ToLower() == HttpContext.Current.User.Identity.Name.ToLower()) FirstTime = false;
            sr.Close();
            sr.Dispose();
            if (FirstTime)
            {
                StreamWriter sw = File.AppendText(Server.MapPath("~/app_data/myfiles-users.txt"));
                sw.WriteLine(HttpContext.Current.User.Identity.Name);
                sw.Close();
                sw.Dispose();
            }
            sr = File.OpenText(Server.MapPath("~/app_data/myfiles-appusers.txt"));
            while ((s = sr.ReadLine()) != null) if (s.ToLower() == HttpContext.Current.User.Identity.Name.ToLower()) W8AppCap = false;
            sr.Close();
            sr.Dispose();
            if (W8AppCap)
            {
                StreamWriter sw = File.AppendText(Server.MapPath("~/app_data/myfiles-appusers.txt"));
                sw.WriteLine(HttpContext.Current.User.Identity.Name);
                sw.Close();
                sw.Dispose();
            }
        }

        protected bool FirstTime { get; set; }
        protected bool W8AppCap { get; set; }

        /// <summary>
        /// The max file size in bytes
        /// </summary>
        protected int maxRequestLength
        {
            get
            {
                HttpRuntimeSection section = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;

                if (section != null)
                    return section.MaxRequestLength * 1024; // Cofig Value
                else
                    return 4096 * 1024; // Default Value
            }
        }

        protected string AcceptedExtensions
        {
            get
            {
                List<string> filters = new List<string>();
                foreach (Filter f in config.MyFiles.Filters)
                    if (isAuth(f) && f.Expression == "*.*") return "";
                    else if (isAuth(f))
                        filters.Add(f.Name + " - " + f.Expression.Trim());
                return string.Join("\\n ", filters.ToArray());
            }
        }

        protected string DropZoneAccepted
        {
            get
            {
                List<string> filters = new List<string>();
                foreach (Filter f in config.MyFiles.Filters)
                    if (isAuth(f) && f.Expression == "*.*") return "";
                    else if (isAuth(f))
                        foreach (string s in f.Expression.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            filters.Add("f:" + HAP.MyFiles.File.GetMimeType(s.Trim().ToLower().Remove(0, 1)));
                return " " + string.Join(" ", filters.ToArray());
            }
        }


        private bool isAuth(Filter filter)
        {
            if (filter.EnableFor == "All") return true;
            else if (filter.EnableFor != "None")
            {
                bool vis = false;
                foreach (string s in filter.EnableFor.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Page.User.IsInRole(s.Trim());
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


    }
}