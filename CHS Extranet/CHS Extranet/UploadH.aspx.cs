using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.DirectoryServices.AccountManagement;
using System.Configuration;

namespace CHS_Extranet
{
    public partial class UploadH : System.Web.UI.Page
    {
        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private GroupPrincipal studentgp, admindrivegp, smt;

        public string Username
        {
            get
            {
                if (User.Identity.Name.Contains('\\'))
                    return User.Identity.Name.Remove(0, User.Identity.Name.IndexOf('\\') + 1);
                else return User.Identity.Name;
            }
        }

        protected override void OnInitComplete(EventArgs e)
        {
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings["ADConnectionString"];
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP:/"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, ConfigurationManager.AppSettings["ADUsername"], ConfigurationManager.AppSettings["ADPassword"]);
            studentgp = GroupPrincipal.FindByIdentity(pcontext, ConfigurationManager.AppSettings["StudentGroup"]);
            if (ConfigurationManager.AppSettings["EnableAdmin"] == "True")
            {
                admindrivegp = GroupPrincipal.FindByIdentity(pcontext, ConfigurationManager.AppSettings["AdminStaffGroup"]);
                smt = GroupPrincipal.FindByIdentity(pcontext, ConfigurationManager.AppSettings["SMTGroup"]);
            }
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !IsCallback && !IsAsync)
            {
                string userhome = up.HomeDirectory;
                if (!userhome.EndsWith("\\")) userhome += "\\";
                string path = Request.QueryString["path"].Remove(0, 1);
                string p = Request.QueryString["path"].Substring(0, 1);
                if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
                else if (p == "W") path = ConfigurationManager.AppSettings["SharedDocsUNC"] + path.Replace('/', '\\');
                else if (p == "T") path = ConfigurationManager.AppSettings["RMStaffUNC"] + path.Replace('/', '\\');
                else if (p == "R") path = ConfigurationManager.AppSettings["AdminSharedUNC"] + path.Replace('/', '\\');
                else if (p == "H") path = string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username) + path.Replace('/', '\\');
                if (up.IsMemberOf(studentgp) && (p == "T" || p == "H" || p == "R"))
                {
                    closeb.Visible = true;
                }
            }
        }

        protected void uploadbtn_Click(object sender, EventArgs e)
        {
            string userhome = up.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            string path = Request.QueryString["path"].Remove(0, 1);
            string p = Request.QueryString["path"].Substring(0, 1);
            if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
            else if (p == "W") path = ConfigurationManager.AppSettings["SharedDocsUNC"] + path.Replace('/', '\\');
            else if (p == "T") path = ConfigurationManager.AppSettings["RMStaffUNC"] + path.Replace('/', '\\');
            else if (p == "R") path = ConfigurationManager.AppSettings["AdminSharedUNC"] + path.Replace('/', '\\');
            else if (p == "H") path = string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username) + path.Replace('/', '\\');
            if (up.IsMemberOf(studentgp) && (p == "T" || p == "H" || p == "R"))
            {
                closeb.Visible = true;
            }
            else 
            {
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
            }
            closeb.Visible = (((Button)sender).ID == "uploadbtnClose");

        }

    }
}