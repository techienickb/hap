using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using CHS_Extranet.Configuration;

namespace CHS_Extranet
{
    public partial class UploadH : System.Web.UI.Page
    {
        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private extranetConfig config;

        private bool isAuth(uncpath path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, s);
                    if (!vis) vis = up.IsMemberOf(gp);
                }
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
                {
                    GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, s);
                    if (!vis) vis = up.IsMemberOf(gp);
                }
                return vis;
            }
            return false;
        }

        protected override void OnInitComplete(EventArgs e)
        {
            config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);
        }

        public string Username
        {
            get
            {
                if (User.Identity.Name.Contains('\\'))
                    return User.Identity.Name.Remove(0, User.Identity.Name.IndexOf('\\') + 1);
                else return User.Identity.Name;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !IsCallback && !IsAsync)
            {
                string userhome = up.HomeDirectory;
                if (!userhome.EndsWith("\\")) userhome += "\\";
                string path = Request.QueryString["path"].Remove(0, 1);
                string p = Request.QueryString["path"].Substring(0, 1);
                uncpath unc = null;
                if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
                else
                {
                    unc = config.UNCPaths[p];
                    if (unc == null || !isWriteAuth(unc)) Response.Redirect("/Extranet/unauthorised.aspx", true);
                    else
                    {
                        path = string.Format(unc.UNC, Username) + path.Replace('/', '\\');
                    }
                }
            }
        }

        protected void uploadbtn_Click(object sender, EventArgs e)
        {
            string userhome = up.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            string path = Request.QueryString["path"].Remove(0, 1);
            string p = Request.QueryString["path"].Substring(0, 1);
            uncpath unc = null;
            if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
            else
            {
                unc = config.UNCPaths[p];
                if (unc == null || !isWriteAuth(unc)) Response.Redirect("/Extranet/unauthorised.aspx", true);
                else
                {
                    path = string.Format(unc.UNC, Username) + path.Replace('/', '\\');
                }
            }
            if (FileUpload1.HasFile)
                FileUpload1.SaveAs(Path.Combine(path, FileUpload1.FileName));
            if (FileUpload2.HasFile)
                FileUpload2.SaveAs(Path.Combine(path, FileUpload2.FileName));
            if (FileUpload3.HasFile)
                FileUpload3.SaveAs(Path.Combine(path, FileUpload3.FileName));
            if (FileUpload4.HasFile)
                FileUpload4.SaveAs(Path.Combine(path, FileUpload4.FileName));
            if (FileUpload5.HasFile)
                    FileUpload5.SaveAs(Path.Combine(path, FileUpload5.FileName));
            closeb.Visible = (((Button)sender).ID == "uploadbtnClose");

        }

    }
}