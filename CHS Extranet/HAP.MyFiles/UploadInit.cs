using HAP.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace HAP.MyFiles
{
    public class UploadInit
    {
        public UploadInit()
        {
            HttpRuntimeSection section = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;

            if (section != null)
                this.maxRequestLength = section.MaxRequestLength * 1024; // Cofig Value
            else
                this.maxRequestLength = 4096 * 1024; // Default Value
            List<string> filters = new List<string>();
            foreach (Filter f in hapConfig.Current.MyFiles.Filters)
                if (isAuth(f) && f.Expression == "*.*") { filters = new List<string>(); filters.Add(f.Expression); break; }
                else if (isAuth(f)) filters.Add(f.Expression.Trim());
            Filters = filters.ToArray();
        }
        public int maxRequestLength { get; private set; }
        public string[] Filters { get; private set; }
        public Properties Properties { get; set; }
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
    }
}
