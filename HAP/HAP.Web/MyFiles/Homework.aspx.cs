using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;

namespace HAP.Web.MyFiles
{
    public partial class Homework : HAP.Web.Controls.Page
    {
        public Homework()
        {
            this.SectionTitle = "My Files - Homework";
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

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
    }
}