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
using HAP.Web.Configuration;

namespace HAP.Web
{
    public partial class move : System.Web.UI.Page
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
                unc = config.MyComputer.UNCPaths[p];
                if (unc == null || !isWriteAuth(unc)) Response.Redirect(Request.ApplicationPath + "/unauthorised.aspx", true);
                else path = string.Format(unc.UNC.Replace("%homepath%", up.HomeDirectory), HAP.AD.ADUtil.Username) + path.Replace('/', '\\');
                if (Request.QueryString["path"].Substring(0, 1) == "f")
                {
                    FileInfo file = new FileInfo(path);
                    moveitem.Text = file.Name;
                    fullname.Value = file.FullName;
                    populatetree(file.Directory, p);
                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    moveitem.Text = dir.Name;
                    fullname.Value = dir.FullName;
                    populatetree(dir, p);
                }
            }
        }

        private void populatetree(DirectoryInfo ignoredir, string p)
        {
            TreeNode h = new TreeNode(hapConfig.Current.MyComputer.UNCPaths[p].Name, string.Format(config.MyComputer.UNCPaths[p].UNC.Replace("%homepath%", up.HomeDirectory), HAP.AD.ADUtil.Username));
            populatenode(h, ignoredir);
            TreeView1.Nodes.Add(h);
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
                if (Request.QueryString["path"].Substring(0, 1) == "f")
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