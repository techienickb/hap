using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using CHS_Extranet.Configuration;
using System.Configuration;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace CHS_Extranet
{
    public partial class Unzip : System.Web.UI.Page
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
                string p = Request.PathInfo.Substring(1, 1);
                string path = Request.PathInfo.Remove(0, 2).Replace('^', '&');
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

                FileInfo file = new FileInfo(path);

                item.Text = file.Name;
                unziptox.Text = string.Format(unziptox.Text, file.Name);
                fullname.Value = path;
            }
        }

        protected void ok_Click(object sender, EventArgs e)
        {
            FileInfo file = new FileInfo(fullname.Value);
            using (ZipInputStream s = new ZipInputStream(file.OpenRead()))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    DirectoryInfo dir;
                    if (unziphere.Checked) dir = file.Directory;
                    else dir  = file.Directory.CreateSubdirectory(file.Name.Replace(file.Extension, ""));

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        if (!Directory.Exists(Path.Combine(dir.FullName, directoryName))) dir.CreateSubdirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(Path.Combine(dir.FullName, theEntry.Name)))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            closeandrefresh.Visible = true;
        }
    }
}