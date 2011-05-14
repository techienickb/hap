using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Security.Authentication;
using System.IO;
using Excel;
using HAP.Web.Configuration;
using HAP.Web.routing;

namespace HAP.Web
{
    public partial class xls : Page, IMyComputerDisplay
    {
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private hapConfig config;

        private bool isAuth(uncpath path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = User.IsInRole(s);
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
                    if (!vis) vis = User.IsInRole(s);
                return vis;
            }
            return false;
        }

        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            pcontext = HAP.AD.ADUtil.PContext;
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, HAP.AD.ADUtil.Username);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string userhome = up.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            string path = RoutingPath.Replace('^', '&');
            uncpath unc = null;
                unc = config.MyComputer.UNCPaths[RoutingDrive];
                if (unc == null || !isWriteAuth(unc)) Response.Redirect(Request.ApplicationPath + "/unauthorised.aspx", true);
                else path = Path.Combine(string.Format(unc.UNC.Replace("%homepath%", up.HomeDirectory), HAP.AD.ADUtil.Username), path.Replace('/', '\\'));

            FileInfo file = new FileInfo(path);

            IExcelDataReader excelReader;
            if (file.Extension.ToLower().Contains("xlsx"))
                excelReader= ExcelReaderFactory.CreateOpenXmlReader(file.OpenRead());
            else excelReader = ExcelReaderFactory.CreateBinaryReader(file.OpenRead());
            excelReader.IsFirstRowAsColumnNames = false;
            GridView1.DataSource = excelReader.AsDataSet();
            GridView1.DataBind();
        }

        public string RoutingPath { get; set; }

        public string RoutingDrive { get; set; }
    }
}