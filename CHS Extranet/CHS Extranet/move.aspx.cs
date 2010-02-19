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
    public partial class move : System.Web.UI.Page
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
                bool admin = false;
                if (Request.QueryString["path"].Substring(1, 1) == "f")
                {
                    path = Request.QueryString["path"].Remove(0, 4);
                    p = Request.QueryString["path"].Substring(3, 1);
                }
                else
                {
                    path = Request.QueryString["path"].Remove(0, 1);
                    p = Request.QueryString["path"].Substring(0, 1);
                }
                if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
                else if (p == "H")
                {
                    path = string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username) + path.Replace('/', '\\');
                    admin = true;
                }

                if (up.IsMemberOf(studentgp) && (p == "T" || p == "H" || p == "W"))
                {
                    throw new AuthenticationException("Not Authorized to Access this Resource");
                }
                else
                {
                    if (Request.QueryString["path"].Substring(1, 1) == "f")
                    {
                        FileInfo file = new FileInfo(path);
                        moveitem.Text = file.Name;
                        fullname.Value = file.FullName;
                        populatetree(file.Directory, p, admin);
                    }
                    else
                    {
                        DirectoryInfo dir = new DirectoryInfo(path);
                        moveitem.Text = dir.Name;
                        fullname.Value = dir.FullName;
                        populatetree(dir, p, admin);
                    }
                }
            }
        }

        private void populatetree(DirectoryInfo ignoredir, string p, bool admin)
        {
            if (!admin)
            {
                TreeNode h = new TreeNode("My Documents", up.HomeDirectory);
                populatenode(h, ignoredir);
                TreeView1.Nodes.Add(h);
            } else {
                if (up.IsMemberOf(admindrivegp) || up.IsMemberOf(smt))
                {
                    TreeNode h = new TreeNode("My Admin Documents", string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username));
                    populatenode(h, ignoredir);
                    TreeView1.Nodes.Add(h);
                }
            }
        }

        private void populatenode(TreeNode node, DirectoryInfo ignoredir)
        {
            foreach (DirectoryInfo d in new DirectoryInfo(node.Value).GetDirectories())
                if (ignoredir != null && d != ignoredir && !d.FullName.ToLower().Contains("recycle")) {
                    TreeNode child = new TreeNode(d.Name, d.FullName);
                    populatenode(child, ignoredir);
                    child.Collapse();
                    node.ChildNodes.Add(child);
                }
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TreeView1.SelectedValue))
            {
                if (Request.QueryString["path"].Substring(1, 1) == "f")
                {
                    FileInfo file = new FileInfo(fullname.Value);
                    file.MoveTo(Path.Combine(TreeView1.SelectedValue, file.Name));
                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(fullname.Value);
                    dir.MoveTo(Path.Combine(TreeView1.SelectedValue, dir.Name));
                }
                closeandrefresh.Visible = true;
            }

        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            ok.Enabled = true;
        }

    }
}