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
using HAP.Data.ComputerBrowser;

namespace HAP.Web
{
    public partial class mycomputer : Page, IMyComputerDisplay
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
            this.Title = string.Format("{0} - Home Access Plus+ - My School Computer", config.BaseSettings.EstablishmentName);
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
                long freeBytesForUser, totalBytes, freeBytes;
                breadcrumbrepeater.Visible = false;
                foreach (uncpath path in config.MyComputer.UNCPaths)
                    if (isAuth(path)) {
                        decimal space = -1;
                        bool showspace = isWriteAuth(path);
                        if (showspace)
                        {
                            if (path.Usage == UsageMode.DriveSpace)
                            {
                                if (HAP.Web.api.Win32.GetDiskFreeSpaceEx(string.Format(path.UNC.Replace("%homepath%", up.HomeDirectory), HAP.AD.ADUtil.Username), out freeBytesForUser, out totalBytes, out freeBytes))
                                    space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                            }
                            else
                            {
                                try
                                {
                                    HAP.Data.Quota.QuotaInfo qi = HAP.Data.ComputerBrowser.Quota.GetQuota(HAP.AD.ADUtil.Username, string.Format(path.UNC.Replace("%homepath%", up.HomeDirectory)));
                                    space = Math.Round((Convert.ToDecimal(qi.Used) / Convert.ToDecimal(qi.Total)) * 100, 2);
                                    if (qi.Total == -1)
                                        if (HAP.Web.api.Win32.GetDiskFreeSpaceEx(string.Format(path.UNC.Replace("%homepath%", up.HomeDirectory), HAP.AD.ADUtil.Username), out freeBytesForUser, out totalBytes, out freeBytes))
                                            space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                                }
                                catch { }
                            }
                        }
                        items.Add(new MyComputerItem(path.Name, showspace ? "<u><span style=\"width: " + Math.Round(space, 0) + "%;\"></span></u>" : "", string.Format("{1}/MyComputer/{0}", path.Drive, Request.ApplicationPath), "images/icons/netdrive.png", false));
                    }
            }
            else
            {
                string u = "";
                string userhome = u;
                if (!string.IsNullOrEmpty(up.HomeDirectory))
                {
                    u = userhome = up.HomeDirectory;
                    if (!userhome.EndsWith("\\")) userhome += "\\";
                }
                string path = "";
                uncpath unc = null;
                //if (RoutingDrive == "N") path = up.HomeDirectory + "\\" + RoutingPath;
                //else
                //{
                    unc = config.MyComputer.UNCPaths[RoutingDrive];
                    if (unc == null || !isAuth(unc)) Response.Redirect(Request.ApplicationPath + "/unauthorised.aspx", true);
                    else if (unc.UNC.Contains("%homepath%")) path = unc.UNC.Replace("%homepath%", u) + "\\" + RoutingPath;
                    else path = string.Format(unc.UNC, HAP.AD.ADUtil.Username) + "\\" + RoutingPath;
                //}
                List<MyComputerItem> breadcrumbs = new List<MyComputerItem>();

                path = path.TrimEnd(new char[] { '\\' }).Replace('^', '&').Replace('/', '\\');
                DirectoryInfo dir = new DirectoryInfo(path);
                newfolderlink.Directory = DeleteBox.Dir = RenameBox.Dir = UnzipBox.Dir = ZipBox.Dir = dir;
                newfolderlink.DataBind();
                DirectoryInfo subdir1 = dir;
                string uncroot = string.Format(unc.UNC.Replace("%homepath%", u), HAP.AD.ADUtil.Username);
                uncroot = uncroot.TrimEnd(new char[] { '\\' });
                DirectoryInfo rootdir = new DirectoryInfo(uncroot);
                while (subdir1.FullName != rootdir.FullName && subdir1 != null)
                {
                    string sdirpath = subdir1.FullName;
                    sdirpath = sdirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", u), HAP.AD.ADUtil.Username), unc.Drive);
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

                if (!string.IsNullOrEmpty(RoutingPath)) items.Add(new MyComputerItem("..", "Up a Directory", Request.ApplicationPath + "/MyComputer/" + (RoutingDrive + "/" + RoutingPath).Remove((RoutingDrive + "/" + RoutingPath).LastIndexOf('/')), "images/icons/folder.png", false));
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
                                dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", u), HAP.AD.ADUtil.Username), unc.Drive);
                                dirpath = dirpath.Replace('\\', '/');

                                CBFile cb = new CBFile(subdir, Converter.ToUNCPath(unc), u);
                                items.Add(new MyComputerItem(cb.Name, "<i>" + cb.Type + "</i>", Request.ApplicationPath + "/MyComputer/" + dirpath.Replace('&', '^'), cb.Icon, allowedit));
                            }
                        }
                        catch { }

                    foreach (FileInfo file in dir.GetFiles())
                    {
                        //try
                        //{
                            if (!file.Name.ToLower().Contains("thumbs") && checkext(file.Extension))
                            {
                                string dirpath = file.FullName;
                                dirpath = dirpath.Replace(string.Format(unc.UNC.Replace("%homepath%", u), HAP.AD.ADUtil.Username), unc.Drive);
                                dirpath = dirpath.Replace('\\', '/');

                                CBFile cb = new CBFile(file, Converter.ToUNCPath(unc), u);
                                items.Add(new MyComputerItem(cb.Name, "<i>" + cb.Type + "</i>", Request.ApplicationPath + "/Download/" + dirpath.Replace('&', '^'), cb.Icon, allowedit));
                            }
                        //}
                        //catch
                        //{
                        //    continue;
                        //    //Response.Redirect("/unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                        //}
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