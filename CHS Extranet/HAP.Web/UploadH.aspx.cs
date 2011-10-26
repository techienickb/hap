using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using HAP.Web.Configuration;
using HAP.Data.ComputerBrowser;

namespace HAP.Web
{
    public partial class UploadH : HAP.Web.Controls.Page
    {
        public UploadH()
        {
            this.SectionTitle = "My School Computer Browser - Upload HTML";
            Context.Server.ScriptTimeout = 2400;
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


        private bool isAuth(string extension)
        {
            foreach (Filter filter in config.MySchoolComputerBrowser.Filters)
                if (filter.Expression.Contains(extension)) return true;
            return isAuth(config.MySchoolComputerBrowser.Filters.Single(fil => fil.Name == "All Files"));
        }

        private bool isAuth(Filter filter)
        {
            if (filter.EnableFor == "All") return true;
            else if (filter.EnableFor != "None")
            {
                bool vis = false;
                foreach (string s in filter.EnableFor.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = User.IsInRole(s.Trim());
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
                foreach (string s in path.EnableWriteTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !IsCallback && !IsAsync)
            {
                ADUser.Impersonate();
                string path = Request.QueryString["path"].Remove(0, 1).Replace('^', '&');
                string p = Request.QueryString["path"].Substring(0, 1);
                DriveMapping unc = null;
                unc = config.MySchoolComputerBrowser.Mappings[p.ToCharArray()[0]];
                if (unc == null || !isWriteAuth(unc)) Response.Redirect(Request.ApplicationPath + "/unauthorised.aspx", true);
                else path = Converter.FormatMapping(unc.UNC, ADUser) + path.Replace('/', '\\');
                ADUser.EndImpersonate();
            }
        }

        protected void uploadbtn_Click(object sender, EventArgs e)
        {
            ADUser.Impersonate();
            message.Text = "";
            string path = Request.QueryString["path"].Remove(0, 1);
            string p = Request.QueryString["path"].Substring(0, 1);
            DriveMapping unc = null;
            unc = config.MySchoolComputerBrowser.Mappings[p.ToCharArray()[0]];
            if (unc == null || !isWriteAuth(unc)) Response.Redirect(Request.ApplicationPath + "/unauthorised.aspx", true);
            else path = Converter.FormatMapping(unc.UNC, ADUser) + path.Replace('/', '\\');
            if (FileUpload1.HasFile && isAuth(Path.GetExtension(FileUpload1.FileName))) FileUpload1.SaveAs(Path.Combine(path, FileUpload1.FileName));
            else if (FileUpload1.HasFile) message.Text += "Error: " + FileUpload1.FileName + " is a restricted file type<br/>";
            if (FileUpload2.HasFile && isAuth(Path.GetExtension(FileUpload2.FileName))) FileUpload2.SaveAs(Path.Combine(path, FileUpload2.FileName));
            else if (FileUpload2.HasFile) message.Text += "Error: " + FileUpload2.FileName + " is a restricted file type<br/>";
            if (FileUpload3.HasFile && isAuth(Path.GetExtension(FileUpload3.FileName))) FileUpload3.SaveAs(Path.Combine(path, FileUpload3.FileName));
            else if (FileUpload3.HasFile) message.Text += "Error: " + FileUpload3.FileName + " is a restricted file type<br/>";
            if (FileUpload4.HasFile && isAuth(Path.GetExtension(FileUpload4.FileName))) FileUpload4.SaveAs(Path.Combine(path, FileUpload4.FileName));
            else if (FileUpload4.HasFile) message.Text += "Error: " + FileUpload4.FileName + " is a restricted file type<br/>";
            if (FileUpload5.HasFile && isAuth(Path.GetExtension(FileUpload5.FileName))) FileUpload5.SaveAs(Path.Combine(path, FileUpload5.FileName));
            else if (FileUpload5.HasFile) message.Text += "Error: " + FileUpload5.FileName + " is a restricted file type<br/>";
            if (!string.IsNullOrEmpty(message.Text)) message.Text = "<div style=\"padding: 4px; color: red;\">" + message.Text + "</div>";
            closeb.Visible = (((Button)sender).ID == "uploadbtnClose");
            ADUser.EndImpersonate();
        }

    }
}