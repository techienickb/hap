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
    public partial class mycomputer : HAP.Web.Controls.Page, IMyComputerDisplay
    {
        public mycomputer()
        {
            this.SectionTitle = "My School Computer Browser";
        }

        private bool isAuth(DriveMapping path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        private bool isWriteAuth(DriveMapping path)
        {
            if (path == null) return true;
            if (path.EnableWriteTo == "All") return true;
            else if (path.EnableWriteTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableWriteTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
            postbackmove.Visible = Page.IsPostBack;
        }

        public override void DataBind()
        {
            base.DataBind();
            ADUser.Impersonate();
            List<MyComputerItem> items = new List<MyComputerItem>();
            if (string.IsNullOrEmpty(RoutingDrive))
            {
                long freeBytesForUser, totalBytes, freeBytes;
                breadcrumbrepeater.Visible = false;
                foreach (DriveMapping path in config.MySchoolComputerBrowser.Mappings.Values)
                    if (isAuth(path)) {
                        decimal space = -1;
                        bool showspace = isWriteAuth(path);
                        if (showspace)
                        {
                            if (path.UsageMode == MappingUsageMode.DriveSpace)
                            {
                                if (HAP.Web.api.Win32.GetDiskFreeSpaceEx(Converter.FormatMapping(path.UNC, ADUser), out freeBytesForUser, out totalBytes, out freeBytes))
                                    space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                            }
                            else
                            {
                                try
                                {
                                    HAP.Data.Quota.QuotaInfo qi = HAP.Data.ComputerBrowser.Quota.GetQuota(ADUser.UserName, Converter.FormatMapping(path.UNC, ADUser));
                                    space = Math.Round((Convert.ToDecimal(qi.Used) / Convert.ToDecimal(qi.Total)) * 100, 2);
                                    if (qi.Total == -1)
                                        if (HAP.Web.api.Win32.GetDiskFreeSpaceEx(Converter.FormatMapping(path.UNC, ADUser), out freeBytesForUser, out totalBytes, out freeBytes))
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
                if (!string.IsNullOrEmpty(ADUser.HomeDirectory))
                {
                    u = userhome = ADUser.HomeDirectory;
                    if (!userhome.EndsWith("\\")) userhome += "\\";
                }
                string path = "";
                DriveMapping unc = null;
                //if (RoutingDrive == "N") path = up.HomeDirectory + "\\" + RoutingPath;
                //else
                //{
                    unc = config.MySchoolComputerBrowser.Mappings[RoutingDrive.ToCharArray()[0]];
                    if (unc == null || !isAuth(unc)) Response.Redirect(ResolveClientUrl("~/unauthorised.aspx"), true);
                    path = Converter.FormatMapping(unc.UNC, ADUser) + "\\" + RoutingPath;
                //}
                List<MyComputerItem> breadcrumbs = new List<MyComputerItem>();
                path = path.TrimEnd(new char[] { '\\' }).Replace('^', '&').Replace('/', '\\');
                DirectoryInfo dir = new DirectoryInfo(path);
                newfolderlink.Directory = DeleteBox.Dir = RenameBox.Dir = UnzipBox.Dir = ZipBox.Dir = dir;
                newfolderlink.DataBind();
                DirectoryInfo subdir1 = dir;
                string uncroot = Converter.FormatMapping(unc.UNC, ADUser);
                uncroot = uncroot.TrimEnd(new char[] { '\\' });
                DirectoryInfo rootdir = new DirectoryInfo(uncroot);
                while (subdir1.FullName != rootdir.FullName && subdir1 != null)
                {
                    string sdirpath = subdir1.FullName;
                    sdirpath = sdirpath.Replace(Converter.FormatMapping(unc.UNC, ADUser), unc.Drive.ToString());
                    breadcrumbs.Add(new MyComputerItem(subdir1.Name, "", ResolveClientUrl("~/MyComputer/" + sdirpath.Replace('&', '^').Replace('\\', '/')), "", false));
                    try
                    {
                        subdir1 = subdir1.Parent;
                    }
                    catch { subdir1 = null; }
                }
                breadcrumbs.Add(new MyComputerItem(unc.Name, "", ResolveClientUrl("~/MyComputer/" + unc.Drive), "", false));
                breadcrumbs.Add(new MyComputerItem("My School Computer", "", ResolveClientUrl("~/MyComputer.aspx"), "", false));
                breadcrumbs.Reverse();
                breadcrumbrepeater.Visible = true;
                breadcrumbrepeater.DataSource = breadcrumbs.ToArray();
                breadcrumbrepeater.DataBind();

                if (!string.IsNullOrEmpty(RoutingPath)) items.Add(new MyComputerItem("..", "Up a Directory", ResolveClientUrl("~/MyComputer/" + (RoutingDrive + "/" + RoutingPath).Remove((RoutingDrive + "/" + RoutingPath).LastIndexOf('/'))), "images/icons/folder.png", false));
                //    items.Add(new MyComputerItem("My Computer", "Back to My Computer", "/MyComputer.aspx", "school.png", false));
                //else 

                bool allowedit = isWriteAuth(config.MySchoolComputerBrowser.Mappings[RoutingDrive.ToCharArray()[0]]);
                newfolderlink.Visible = newfileuploadlink.Visible = allowedit;
                if (!unc.EnableMove) rckmove.Style.Add("display", "none");
                try {
                    foreach (DirectoryInfo subdir in dir.GetDirectories())
                        try
                        {
                            bool isHidden = (subdir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                            bool isSystem = (subdir.Attributes & FileAttributes.System) == FileAttributes.System;
                            if (!subdir.Name.ToLower().Contains("recycle") && !isSystem && !isHidden && !subdir.Name.ToLower().Contains("system volume info"))
                            {
                                string dirpath = subdir.FullName;
                                dirpath = dirpath.Replace(Converter.FormatMapping(unc.UNC, ADUser), unc.Drive.ToString());
                                dirpath = dirpath.Replace('\\', '/');

                                CBFile cb = new CBFile(subdir, Converter.ToUNCPath(unc), ADUser);
                                items.Add(new MyComputerItem(cb.Name, "<i>" + cb.Type + "</i>", ResolveClientUrl("~/MyComputer/" + dirpath.Replace('&', '^')), cb.Icon, allowedit));
                            }
                        }
                        catch { }
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        try
                        {
                            bool isHidden = (file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                            bool isSystem = (file.Attributes & FileAttributes.System) == FileAttributes.System;
                            if (!file.Name.ToLower().Contains("thumbs") && checkext(file.Extension) && !isHidden && !isSystem)
                            {
                                string dirpath = file.FullName;
                                dirpath = dirpath.Replace(Converter.FormatMapping(unc.UNC, ADUser), unc.Drive.ToString());
                                dirpath = dirpath.Replace('\\', '/');

                                CBFile cb = new CBFile(file, Converter.ToUNCPath(unc), ADUser);
                                items.Add(new MyComputerItem(cb.Name, "<i>" + cb.Type + "</i>", ResolveClientUrl("~/Download/" + dirpath.Replace('&', '^')), cb.Icon, allowedit));
                            }
                        }
                        catch
                        {
                            continue;
                            //Response.Redirect("/unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message), true);
                        }
                    }
                }
                catch (UnauthorizedAccessException uae)
                {
                    ADUser.EndImpersonate();
                    Response.Redirect(ResolveClientUrl("~/unauthorised.aspx?path=" + Server.UrlPathEncode(uae.Message)), true);
                }
            }
            ADUser.EndImpersonate();
            browserrepeater.DataSource = items.ToArray();
            browserrepeater.DataBind();
        }

        public bool checkext(string extension)
        {
            string[] exc = config.MySchoolComputerBrowser.HideExtensions.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in exc)
                if (s.Trim().ToLower() == extension.ToLower()) return false;
            return true;
        }

        public string RoutingPath { get; set; }

        public string RoutingDrive { get; set; }
    }
}