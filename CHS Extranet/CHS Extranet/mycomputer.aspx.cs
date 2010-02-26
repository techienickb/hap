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

namespace CHS_Extranet
{
    public partial class mycomputer : System.Web.UI.Page
    {
        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private GroupPrincipal studentgp, admindrivegp, smt;

        public string FullPath { get; set; }
        public string Drive { get; set; }


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
            this.Title = string.Format("{0} - Home Access Plus+ - My Computer", ConfigurationManager.AppSettings["SchoolName"]);
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
            string est = ConfigurationManager.AppSettings["EstablishmentName"];
            if (string.IsNullOrEmpty(Request.PathInfo))
            {
                items.Add(new MyComputerItem("My Documents", Username + " on " + est, "/Extranet/MyComputer.aspx/N\\", "netdrive.png", false));
                if (ConfigurationManager.AppSettings["EnableAdmin"] == "True") 
                {
                    if (up.IsMemberOf(admindrivegp) || up.IsMemberOf(smt))
                    {
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AdminServerUNC"])) items.Add(new MyComputerItem("My Admin Documents", Username + " on Admin", "/Extranet/MyComputer.aspx/H\\", "netdrive.png", false));
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AdminSharedUNC"])) items.Add(new MyComputerItem("Admin Shared", "Admin Shared", "/Extranet/MyComputer.aspx/R\\", "netdrive.png", false));
                    }
                }
                items.Add(new MyComputerItem("RMShared Documents", "Shared Documents on " + ConfigurationManager.AppSettings["EstablishmentName"], "/Extranet/MyComputer.aspx/W\\", "netdrive.png", false));
                if (!up.IsMemberOf(studentgp)) items.Add(new MyComputerItem("RMStaff", "RMStaff on " + ConfigurationManager.AppSettings["EstablishmentName"], "/Extranet/MyComputer.aspx/T\\", "netdrive.png", false));
                items.Add(new MyComputerItem("Learning Resources", "Learning Resources on " + ConfigurationManager.AppSettings["EstablishmentName"], "/easylink/rf/", "school.png", false));
            }
            else
            {
                string userhome = up.HomeDirectory;
                if (!userhome.EndsWith("\\")) userhome += "\\";
                string p = Request.PathInfo.Substring(1, 1);
                string path = Request.PathInfo.Remove(0, 2);
                if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
                else if (p == "W") path = ConfigurationManager.AppSettings["SharedDocsUNC"] + path.Replace('/', '\\');
                else if (p == "T") path = ConfigurationManager.AppSettings["RMStaffUNC"] + path.Replace('/', '\\');
                else if (p == "R") path = ConfigurationManager.AppSettings["AdminSharedUNC"] + path.Replace('/', '\\');
                else if (p == "H") path = string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username) + path.Replace('/', '\\');

                DirectoryInfo dir = new DirectoryInfo(path);
                if (Request.PathInfo.Length <= 3)
                    items.Add(new MyComputerItem("My Computer", "Back to My Computer", "/Extranet/MyComputer.aspx", "school.png", false));
                else items.Add(new MyComputerItem("..", "Up a Directory", "/Extranet/MyComputer.aspx" + Request.PathInfo.Remove(Request.PathInfo.LastIndexOf('/')), "folder.png", false));

                if (up.IsMemberOf(studentgp) && (p == "T" || p == "H" || p == "R"))
                {
                    items.Add(new MyComputerItem("Not Authorised", "Back to My Computer", "/Extranet/MyComputer.aspx", "school.png", false));
                }
                else
                {
                    newfolderlink.Visible = fileuploadlink.Visible = true;
                    bool allowedit = true;
                    if (up.IsMemberOf(studentgp) && p == "W")
                        newfolderlink.Visible = fileuploadlink.Visible = allowedit = false;

                    if (p == "W" || p == "T" || p == "R") rckmove.Style.Add("display", "none");

                    newfolderlink.NavigateUrl = "/Extranet/NewFolder.aspx?path=" + Request.PathInfo.Remove(0, 1);
                    fileuploadlink.NavigateUrl = "/Extranet/Upload.aspx?path=" + Request.PathInfo.Remove(0, 1);
                    try
                    {
                        foreach (DirectoryInfo subdir in dir.GetDirectories())
                        {
                            if (!subdir.Name.ToLower().Contains("recycle"))
                            {
                                string dirpath = subdir.FullName;
                                dirpath = dirpath.Replace(userhome, "N/");
                                dirpath = dirpath.Replace(ConfigurationManager.AppSettings["SharedDocsUNC"], "W");
                                dirpath = dirpath.Replace(ConfigurationManager.AppSettings["RMStaffUNC"], "T");
                                if (ConfigurationManager.AppSettings["EnableAdmin"] == "True")
                                {
                                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AdminServerUNC"])) dirpath = dirpath.Replace(string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username), "H");
                                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AdminSharedUNC"])) dirpath = dirpath.Replace(ConfigurationManager.AppSettings["AdminSharedUNC"], "R");
                                }
                                items.Add(new MyComputerItem(subdir.Name, "Last Modified: " + subdir.LastWriteTime.ToString("dd/MM/yy hh:mm tt"), "/Extranet/MyComputer.aspx/" + dirpath, MyComputerItem.ParseForImage(subdir.Name), allowedit));
                            }
                        }
                        foreach (FileInfo file in dir.GetFiles())
                        {
                            if (!file.Name.ToLower().Contains("thumbs"))
                            {
                                string dirpath = file.FullName;
                                dirpath = dirpath.Replace(userhome, "N\\");
                                dirpath = dirpath.Replace(ConfigurationManager.AppSettings["SharedDocsUNC"], "W");
                                dirpath = dirpath.Replace(ConfigurationManager.AppSettings["RMStaffUNC"], "T");
                                if (ConfigurationManager.AppSettings["EnableAdmin"] == "True")
                                {
                                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AdminServerUNC"])) dirpath = dirpath.Replace(string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username), "H");
                                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AdminSharedUNC"])) dirpath = dirpath.Replace(ConfigurationManager.AppSettings["AdminSharedUNC"], "R");
                                }
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
            }
            browserrepeater.DataSource = items.ToArray();
            browserrepeater.DataBind();
        }
    }
}