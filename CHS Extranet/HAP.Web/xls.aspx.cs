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
using HAP.Data.ComputerBrowser;

namespace HAP.Web
{
    public partial class xls : HAP.Web.Controls.Page, IMyComputerDisplay
    {
        public xls()
        {
            this.SectionTitle = "My School Computer Browser - Excel Viewer";
        }
        private bool isAuth(DriveMapping path)
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

        private bool isWriteAuth(DriveMapping path)
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

        protected void Page_Load(object sender, EventArgs e)
        {
            ADUser.Impersonate();
            string userhome = ADUser.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            string path = RoutingPath.Replace('^', '&');
            DriveMapping unc = null;
            unc = config.MySchoolComputerBrowser.Mappings[RoutingDrive.ToCharArray()[0]];
            if (unc == null || !isWriteAuth(unc)) Response.Redirect(Request.ApplicationPath + "/unauthorised.aspx", true);
            else path = Path.Combine(Converter.FormatMapping(unc.UNC, ADUser), path.Replace('/', '\\'));

            FileInfo file = new FileInfo(path);

            IExcelDataReader excelReader;
            if (file.Extension.ToLower().Contains("xlsx"))
                excelReader= ExcelReaderFactory.CreateOpenXmlReader(file.OpenRead());
            else excelReader = ExcelReaderFactory.CreateBinaryReader(file.OpenRead());
            excelReader.IsFirstRowAsColumnNames = false;
            GridView1.DataSource = excelReader.AsDataSet();
            GridView1.DataBind();
            ADUser.EndImpersonate();
        }

        public string RoutingPath { get; set; }

        public string RoutingDrive { get; set; }
    }
}