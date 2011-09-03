using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using HAP.Web.routing;

namespace HAP.Web.Controls
{
    public partial class Upload : System.Web.UI.UserControl
    {
        private hapConfig config;

        private bool isAuth(Filter filter)
        {
            if (filter.EnableFor == "All") return true;
            else if (filter.EnableFor != "None")
            {
                bool vis = false;
                foreach (string s in filter.EnableFor.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Page.User.IsInRole(s);
                return vis;
            }
            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            hapConfig config = hapConfig.Current;
            List<string> filters = new List<string>();
            foreach (Filter f in config.MySchoolComputerBrowser.Filters)
                if (isAuth(f)) filters.Add(f.ToString());
            string fs = string.Join("|", filters.ToArray());
            InitParams.Attributes.Add("value", string.Format("Path={0}/{1},Filters={2}", ((IMyComputerDisplay)Page).RoutingDrive, ((IMyComputerDisplay)Page).RoutingPath, fs));
        }
    }
}