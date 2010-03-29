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
using CHS_Extranet.Configuration;

namespace CHS_Extranet
{
    public partial class move : System.Web.UI.Page
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
            if (!Page.IsPostBack)
            {
                string userhome = up.HomeDirectory;
                if (!userhome.EndsWith("\\")) userhome += "\\";
                string path, p;
                if (Request.QueryString["path"].Substring(0, 1) == "f")
                {
                    path = Request.QueryString["path"].Remove(0, 3).Replace('^', '&');
                    p = Request.QueryString["path"].Substring(2, 1);
                }
                else
                {
                    path = Request.QueryString["path"].Remove(0, 1).Replace('^', '&');
                    p = Request.QueryString["path"].Substring(0, 1);
                }

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
                if (Request.QueryString["path"].Substring(1, 1) == "f")
                {
                    FileInfo file = new FileInfo(path);
                    moveitem.Text = file.Name;
                    fullname.Value = file.FullName;
                    populatetree(file.Directory, p, p == "H");
                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    moveitem.Text = dir.Name;
                    fullname.Value = dir.FullName;
                    populatetree(dir, p, p == "H");
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
                TreeNode h = new TreeNode("My Admin Documents", string.Format(config.UNCPaths["H"].UNC, Username));
                populatenode(h, ignoredir);
                TreeView1.Nodes.Add(h);
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