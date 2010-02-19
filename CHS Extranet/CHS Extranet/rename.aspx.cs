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

namespace CHS_Extranet
{
    public partial class Rename : System.Web.UI.Page
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
            if (!Page.IsPostBack)
            {
                string userhome = up.HomeDirectory;
                if (!userhome.EndsWith("\\")) userhome += "\\";
                string path, p;
                if (Request.PathInfo.Substring(1, 1) == "f")
                {
                    path = Request.PathInfo.Remove(0, 4);
                    p = Request.PathInfo.Substring(3, 1);
                }
                else
                {
                    path = Request.PathInfo.Remove(0, 2);
                    p = Request.PathInfo.Substring(1, 1);
                }
                if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
                else if (p == "W") path = ConfigurationManager.AppSettings["SharedDocsUNC"] + path.Replace('/', '\\');
                else if (p == "T") path = ConfigurationManager.AppSettings["RMStaffUNC"] + path.Replace('/', '\\');
                else if (p == "H") path = string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username) + path.Replace('/', '\\');

                if (up.IsMemberOf(studentgp) && (p == "T" || p == "H"))
                {
                    throw new AuthenticationException("Not Authorized to Access this Resource");
                }
                else
                {
                    if (Request.PathInfo.Substring(1, 1) == "f")
                    {
                        FileInfo file = new FileInfo(path);
                        filename.Text = newname.Text = file.Name;
                        fullname.Value = file.FullName;
                    }
                    else
                    {
                        DirectoryInfo dir = new DirectoryInfo(path);
                        filename.Text = newname.Text = dir.Name;
                        fullname.Value = dir.FullName;
                    }
                }
            }
        }

        protected void yesren_Click(object sender, EventArgs e)
        {

            if (Request.PathInfo.Substring(1, 1) == "f")
            {
                FileInfo file = new FileInfo(fullname.Value);
                file.MoveTo(file.FullName.Replace(file.Name, newname.Text));
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(fullname.Value);
                dir.MoveTo(dir.FullName.Replace(dir.Name, newname.Text));
            }
            closeandrefresh.Visible = true;
        }
    }
}