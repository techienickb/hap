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
    public partial class newfolder : System.Web.UI.Page
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
                string p = Request.QueryString["path"].Substring(0, 1);

                if (up.IsMemberOf(studentgp) && (p == "T" || p == "H" || p == "W"))
                {
                    throw new AuthenticationException("Not Authorized to Access this Resource");
                }
            }
        }

        protected void yes_Click(object sender, EventArgs e)
        {
            string userhome = up.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            string p = Request.QueryString["path"].Substring(0, 1);
            string path = Request.QueryString["path"].Remove(0, 1);
            if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
            else if (p == "W") path = ConfigurationManager.AppSettings["SharedDocsUNC"] + path.Replace('/', '\\');
            else if (p == "T") path = ConfigurationManager.AppSettings["RMStaffUNC"] + path.Replace('/', '\\');
            else if (p == "H") path = string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username) + path.Replace('/', '\\');

            Directory.CreateDirectory(Path.Combine(path, foldername.Text));
            closeandrefresh.Visible = true;
        }
    }
}