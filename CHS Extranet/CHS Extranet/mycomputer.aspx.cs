using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using CHS_Extranet.Configuration;

namespace CHS_Extranet
{
    public partial class mycomputer : System.Web.UI.Page
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
            this.Title = string.Format("{0} - Home Access Plus+ - My Computer", config.BaseSettings.EstablishmentName);
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
            List<MyComputerItem> items = new List<MyComputerItem>();
            if (string.IsNullOrEmpty(Request.PathInfo))
            {
                items.Add(new MyComputerItem("My Documents", string.Format("{0} on {1}", Username, config.BaseSettings.EstablishmentCode), "/Extranet/MyComputer.aspx/N\\", "netdrive.png", false));
                foreach (uncpath path in config.UNCPaths)
                    if (isAuth(path)) items.Add(new MyComputerItem(path.Name, string.Format("{0} on {1}", path.Name, config.BaseSettings.EstablishmentCode), string.Format("/Extranet/MyComputer.aspx/{0}\\", path.Drive), "netdrive.png", false));
                if (config.HomePageLinks["Access Learning Resources"] != null)
                {
                    if (config.HomePageLinks["Access Learning Resources"].ShowTo == "All") items.Add(new MyComputerItem("Learning Resources", string.Format("{0} on {1}", "Learning Resources", config.BaseSettings.EstablishmentCode), config.HomePageLinks["Access Learning Resources"].LinkLocation, config.HomePageLinks["Access Learning Resources"].Icon.Remove(0, config.HomePageLinks["Access Learning Resources"].Icon.LastIndexOf('/') + 1), false));
                    else if (config.HomePageLinks["Access Learning Resources"].ShowTo != "None")
                    {
                        bool vis = false;
                        foreach (string s in config.HomePageLinks["Access Learning Resources"].ShowTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, s);
                            if (!vis) vis = up.IsMemberOf(gp);
                        }
                        if (vis) items.Add(new MyComputerItem("Learning Resources", string.Format("{0} on {1}", "Learning Resources", config.BaseSettings.EstablishmentCode), config.HomePageLinks["Access Learning Resources"].LinkLocation, config.HomePageLinks["Access Learning Resources"].Icon.Remove(0, config.HomePageLinks["Access Learning Resources"].Icon.LastIndexOf('/') + 1), false));
                    }
                    
                }
            }
            else
            {
                string userhome = up.HomeDirectory;
                if (!userhome.EndsWith("\\")) userhome += "\\";
                string p = Request.PathInfo.Substring(1, 1);
                string path = Request.PathInfo.Remove(0, 2);
                uncpath unc = null;
                if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
                else
                {
                    unc = config.UNCPaths[p];
                    if (unc == null || !isAuth(unc)) Response.Redirect("/Extranet/unauthorised.aspx", true);
                    else
                    {
                        path = string.Format(unc.UNC, Username) + path.Replace('/', '\\');
                    }
                }

                Response.Write("<!--" + path + "-->\n");
                if (unc != null) Response.Write("<!--" + unc.UNC + "-->");

                DirectoryInfo dir = new DirectoryInfo(path);
                if (Request.PathInfo.Length <= 3)
                    items.Add(new MyComputerItem("My Computer", "Back to My Computer", "/Extranet/MyComputer.aspx", "school.png", false));
                else items.Add(new MyComputerItem("..", "Up a Directory", "/Extranet/MyComputer.aspx" + Request.PathInfo.Remove(Request.PathInfo.LastIndexOf('/')), "folder.png", false));

                bool allowedit = isWriteAuth(config.UNCPaths[p]);
                newfolderlink.Visible = fileuploadlink.Visible = allowedit;
                if (p != "N" && p != "H") rckmove.Style.Add("display", "none");

                newfolderlink.NavigateUrl = "/Extranet/NewFolder.aspx?path=" + Request.PathInfo.Remove(0, 1);
                fileuploadlink.NavigateUrl = "/Extranet/Upload.aspx?path=" + Request.PathInfo.Remove(0, 1);
                try
                {
                    foreach (DirectoryInfo subdir in dir.GetDirectories())
                    {
                        if (!subdir.Name.ToLower().Contains("recycle"))
                        {
                            string dirpath = subdir.FullName;
                            if (unc == null) dirpath = dirpath.Replace(userhome, "N/");
                            else dirpath = dirpath.Replace(string.Format(unc.UNC, Username), unc.Drive);
                            items.Add(new MyComputerItem(subdir.Name, "Last Modified: " + subdir.LastWriteTime.ToString("dd/MM/yy hh:mm tt"), "/Extranet/MyComputer.aspx/" + dirpath, MyComputerItem.ParseForImage(subdir.Name), allowedit));
                        }
                    }
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        if (!file.Name.ToLower().Contains("thumbs"))
                        {
                            string dirpath = file.FullName;
                            if (unc == null) dirpath = dirpath.Replace(userhome, "N/");
                            else dirpath = dirpath.Replace(string.Format(unc.UNC, Username), unc.Drive);
                            if (!string.IsNullOrEmpty(file.Extension))
                                items.Add(new MyComputerItem(file.Name.Replace(file.Extension, ""), "Last Modified: " + file.LastWriteTime.ToString("dd/MM/yy hh:mm tt"), "/Extranet/f.ashx/" + dirpath.Replace("&", "&amp;"), MyComputerItem.ParseForImage(file.Extension.ToLower()), allowedit));
                            else
                                items.Add(new MyComputerItem(file.Name, "Last Modified: " + file.LastWriteTime.ToString("dd/MM/yy hh:mm tt"), "/Extranet/f.ashx/" + dirpath.Replace("&", "&amp;"), MyComputerItem.ParseForImage(file.Extension.ToLower()), allowedit));
                        }
                    }
                }
                catch (UnauthorizedAccessException uae)
                {
                    Response.Redirect("/extranet/unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                }
            }
            browserrepeater.DataSource = items.ToArray();
            browserrepeater.DataBind();
        }
    }
}