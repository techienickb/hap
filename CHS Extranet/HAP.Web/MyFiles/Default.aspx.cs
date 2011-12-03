using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;

namespace HAP.Web.MyFiles
{
    public partial class Default : HAP.Web.Controls.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string DropZoneAccepted
        {
            get
            {
                List<string> filters = new List<string>();
                foreach (Filter f in config.MySchoolComputerBrowser.Filters)
                    if (isAuth(f) && f.Expression == "*.*") return "";
                    else if (isAuth(f))
                        foreach (string s in f.Expression.Split(new char[] { ';' }))
                            filters.Add("f:" + HAP.Data.MyFiles.File.GetMimeType(s.Trim().ToLower().Remove(0, 1)));
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