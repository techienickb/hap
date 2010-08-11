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
using HAP.Web.Configuration;
using HAP.Web.routing;

namespace HAP.Web
{
    public partial class mycomputer : Page, IMyComputerDisplay
    {
        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
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
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);
            this.Title = string.Format("{0} - Home Access Plus+ - My School Computer", config.BaseSettings.EstablishmentName);
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
            DataBind();
            postbackmove.Visible = Page.IsPostBack;
        }

        public override void DataBind()
        {
            base.DataBind();
            List<MyComputerItem> items = new List<MyComputerItem>();
            if (string.IsNullOrEmpty(RoutingDrive))
            {
                breadcrumbrepeater.Visible = false;
                foreach (uncpath path in config.MyComputer.UNCPaths)
                    if (isAuth(path)) items.Add(new MyComputerItem(path.Name, string.Format("{0} on {1}", path.Name, config.BaseSettings.EstablishmentCode), string.Format("{1}/MyComputer/{0}", path.Drive, Request.ApplicationPath), "netdrive.png", false));
                if (config.HomePageLinks["Access Learning Resources"] != null)
                {
                    if (config.HomePageLinks["Access Learning Resources"].ShowTo == "All") items.Add(new MyComputerItem("Learning Resources", string.Format("{0} on {1}", "Learning Resources", config.BaseSettings.EstablishmentCode), config.HomePageLinks["Access Learning Resources"].LinkLocation, config.HomePageLinks["Access Learning Resources"].Icon.Remove(0, config.HomePageLinks["Access Learning Resources"].Icon.LastIndexOf('/') + 1), false));
                    else if (config.HomePageLinks["Access Learning Resources"].ShowTo != "None")
                    {
                        bool vis = false;
                        foreach (string s in config.HomePageLinks["Access Learning Resources"].ShowTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                            if (!vis) vis = User.IsInRole(s);
                        if (vis) items.Add(new MyComputerItem("Learning Resources", string.Format("{0} on {1}", "Learning Resources", config.BaseSettings.EstablishmentCode), config.HomePageLinks["Access Learning Resources"].LinkLocation, config.HomePageLinks["Access Learning Resources"].Icon.Remove(0, config.HomePageLinks["Access Learning Resources"].Icon.LastIndexOf('/') + 1), false));
                    }

                }
            }
            else
            {
                string userhome = up.HomeDirectory;
                if (!userhome.EndsWith("\\")) userhome += "\\";
                string path = "";
                uncpath unc = null;
                //if (RoutingDrive == "N") path = up.HomeDirectory + "\\" + RoutingPath;
                //else
                //{
                    unc = config.MyComputer.UNCPaths[RoutingDrive];
                    if (unc == null || !isAuth(unc)) Response.Redirect(Request.ApplicationPath + "/unauthorised.aspx", true);
                    else if (unc.UNC.Contains("%homepath%")) path = unc.UNC.Replace("%homepath%", up.HomeDirectory) + "\\" + RoutingPath;
                    else path = string.Format(unc.UNC, Username) + "\\" + RoutingPath;
                //}

                List<MyComputerItem> breadcrumbs = new List<MyComputerItem>();

                path = path.TrimEnd(new char[] { '\\' }).Replace('^', '&').Replace('/', '\\');
                DirectoryInfo dir = new DirectoryInfo(path);
                newfolderlink.Directory = DeleteBox.Dir = RenameBox.Dir = UnzipBox.Dir = ZipBox.Dir = dir;
                newfolderlink.DataBind();
                DirectoryInfo subdir1 = dir;
                string uncroot = string.Format(unc.UNC.Replace("%homepath%", up.HomeDirectory), Username);
                uncroot = uncroot.TrimEnd(new char[] { '\\' });
                DirectoryInfo rootdir = new DirectoryInfo(uncroot);
                while (subdir1.FullName != rootdir.FullName && subdir1 != null)
                {
                    string sdirpath = subdir1.FullName;
                    sdirpath = sdirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", up.HomeDirectory), Username), unc.Drive);
                    breadcrumbs.Add(new MyComputerItem(subdir1.Name, "", Request.ApplicationPath + "/MyComputer/" + sdirpath.Replace('&', '^').Replace('\\', '/'), "", false));
                    try
                    {
                        subdir1 = subdir1.Parent;
                    }
                    catch { subdir1 = null; }
                }
                breadcrumbs.Add(new MyComputerItem(unc.Name, "", Request.ApplicationPath + "/MyComputer/" + unc.Drive, "", false));
                breadcrumbs.Add(new MyComputerItem("My School Computer", "", Request.ApplicationPath + "/MyComputer.aspx", "", false));
                breadcrumbs.Reverse();
                breadcrumbrepeater.Visible = true;
                breadcrumbrepeater.DataSource = breadcrumbs.ToArray();
                breadcrumbrepeater.DataBind();

                if (!string.IsNullOrEmpty(RoutingPath)) items.Add(new MyComputerItem("..", "Up a Directory", Request.ApplicationPath + "/MyComputer/" + (RoutingDrive + "/" + RoutingPath).Remove((RoutingDrive + "/" + RoutingPath).LastIndexOf('/')), "folder.png", false));
                //    items.Add(new MyComputerItem("My Computer", "Back to My Computer", "/MyComputer.aspx", "school.png", false));
                //else 

                bool allowedit = isWriteAuth(config.MyComputer.UNCPaths[RoutingDrive]);
                newfolderlink.Visible = newfileuploadlink.Visible = allowedit;
                if (!unc.EnableMove) rckmove.Style.Add("display", "none");
                try {
                    foreach (DirectoryInfo subdir in dir.GetDirectories())
                        try
                        {
                            if (!subdir.Name.ToLower().Contains("recycle"))
                            {
                                string dirpath = subdir.FullName;
                                dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", up.HomeDirectory), Username), unc.Drive);
                                dirpath = dirpath.Replace('\\', '/');
                                items.Add(new MyComputerItem(subdir.Name, "Last Modified: " + subdir.LastWriteTime.ToString("dd/MM/yy hh:mm tt"), Request.ApplicationPath + "/MyComputer/" + dirpath.Replace('&', '^'), MyComputerItem.ParseForImage(subdir), allowedit));
                            }
                        }
                        catch { }

                    foreach (FileInfo file in dir.GetFiles())
                    {
                        try
                        {
                            if (!file.Name.ToLower().Contains("thumbs") && checkext(file.Extension))
                            {
                                string dirpath = file.FullName;
                                dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", up.HomeDirectory), Username), unc.Drive);
                                dirpath = dirpath.Replace('\\', '/');
                                if (!string.IsNullOrEmpty(file.Extension))
                                    items.Add(new MyComputerItem(file.Name.Replace(file.Extension, ""), "Last Modified: " + file.LastWriteTime.ToString("dd/MM/yy hh:mm tt"), Request.ApplicationPath + "/Download/" + dirpath.Replace('&', '^'), MyComputerItem.ParseForImage(file), allowedit));
                                else
                                    items.Add(new MyComputerItem(file.Name, "Last Modified: " + file.LastWriteTime.ToString("dd/MM/yy hh:mm tt"), Request.ApplicationPath + "/Download/" + dirpath.Replace('&', '^'), MyComputerItem.ParseForImage(file), allowedit));
                            }
                        }
                        catch
                        {
                            //Response.Redirect("/unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                        }
                    }
                }
                catch (UnauthorizedAccessException uae)
                {
                    Response.Redirect(Request.ApplicationPath + "/unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                }
            }
            browserrepeater.DataSource = items.ToArray();
            browserrepeater.DataBind();
        }

        public bool checkext(string extension)
        {
            string[] exc = config.MyComputer.HideExtensions.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in exc)
                if (s.ToLower() == extension.ToLower()) return false;
            return true;
        }

        public string RoutingPath { get; set; }

        public string RoutingDrive { get; set; }
    }
}