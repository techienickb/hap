using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using HAP.Web.Configuration;

namespace HAP.Web
{
    public partial class upload : System.Web.UI.Page
    {
        private hapConfig config;

        private bool isAuth(uploadfilter filter)
        {
            if (filter.EnableFor == "All") return true;
            else if (filter.EnableFor != "None")
            {
                bool vis = false;
                foreach (string s in filter.EnableFor.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = User.IsInRole(s);
                return vis;
            }
            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            config = hapConfig.Current;
            List<string> filters = new List<string>();
            foreach (uploadfilter f in config.UploadFilters)
                if (isAuth(f)) filters.Add(f.ToString());
            string fs = string.Join("|", filters.ToArray());
            if (!string.IsNullOrEmpty(fs)) fs = ",Filter=" + fs;
            Xaml1.InitParameters = string.Format("UploadPage=FileUpload.ashx/{0}/,UploadChunkSize=131072{1}", Request.QueryString["path"], fs);
        }
    }
}