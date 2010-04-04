using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.DirectoryServices;
using CHS_Extranet.Configuration;
using System.Xml;

namespace CHS_Extranet
{
    public partial class Default : System.Web.UI.Page
    {
        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private extranetConfig config;

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
            config = extranetConfig.Current;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);
            this.Title = string.Format("{0} - Home Access Plus+", config.BaseSettings.EstablishmentName);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, config.ADSettings.StudentsGroupName);
            //rmCom2000-UsrMgr-uPN
            DirectoryEntry usersDE = new DirectoryEntry(_ActiveDirectoryConnectionString, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(sAMAccountName=" + Username + ")";
            ds.PropertiesToLoad.Add("rmCom2000-UsrMgr-uPN");
            ds.PropertiesToLoad.Add("department");
            SearchResult r = ds.FindOne();
            if (string.IsNullOrEmpty(config.BaseSettings.StudentPhotoHandler))
                userimage.Visible = false;
            else
            {
                try
                {
                    userimage.ImageUrl = string.Format("{0}?UPN={1}", config.BaseSettings.StudentPhotoHandler, r.Properties["rmCom2000-UsrMgr-uPN"][0].ToString());
                }
                catch { userimage.Visible = false; }
            }
            welcomename.Text = string.IsNullOrEmpty(up.GivenName) ? up.DisplayName : up.GivenName;
            name.Text = userimage.AlternateText = string.Format("{0} {1}", up.GivenName, up.Surname);
            try
            {
                form.Text = r.Properties["department"][0].ToString();
            }
            catch { form.Text = "n/a"; }
            if (up.IsMemberOf(gp)) form.Text = string.Format("<b>Form: </b>{0}", form.Text);
            else form.Text = string.Format("<b>Department: </b>{0}", form.Text);
            email.Text = up.EmailAddress;
            string aet = config.HomePageLinks["Update My Details"].ShowTo;
            if (aet == "None") updatemydetails.Visible = false;
            else if (aet != "All")
            {
                bool vis = false;
                foreach (string s in aet.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    gp = GroupPrincipal.FindByIdentity(pcontext, s);
                    if (!vis) vis = up.IsMemberOf(gp);
                }
                updatemydetails.Visible = vis;
            }
            List<homepagelink> links = new List<homepagelink>();
            foreach (homepagelink link in config.HomePageLinks)
                if (link.Name != "Update My Details")
                {
                    if (link.ShowTo == "All") links.Add(link);
                    else if (link.ShowTo != "None")
                    {
                        bool vis = false;
                        foreach (string s in link.ShowTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            gp = GroupPrincipal.FindByIdentity(pcontext, s);
                            if (!vis) vis = up.IsMemberOf(gp);
                        }
                        if (vis) links.Add(link);
                    }
                }
            homepagelinks.DataSource = links.ToArray();
            homepagelinks.DataBind();
        }

        protected void updatemydetails_Click(object sender, EventArgs e)
        {
            viewmode.Visible = false;
            editmode.Visible = true;
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, config.ADSettings.StudentsGroupName);
            //rmCom2000-UsrMgr-uPN
            DirectoryEntry usersDE = new DirectoryEntry(_ActiveDirectoryConnectionString, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(sAMAccountName=" + Username + ")";
            ds.PropertiesToLoad.Add("department");
            SearchResult r = ds.FindOne();
            txtfname.Text = up.GivenName;
            txtlname.Text = up.Surname;
            try
            {
                txtform.Text = r.Properties["department"][0].ToString();
            }
            catch { txtform.Text = ""; }
            if (up.IsMemberOf(gp)) formlabel.Text = "<b>Form: </b>";
            else
            {
                formlabel.Text = "<b>Department: </b>";
                txtform.Columns = 14;
            }
            email.Text = up.EmailAddress;
        }

        protected void editmydetails_Click(object sender, EventArgs e)
        {
            viewmode.Visible = true;
            editmode.Visible = false;
            up.Surname = txtlname.Text;
            up.GivenName = txtfname.Text;
            up.Description = string.Format("{0} {1} in {2}", txtfname.Text, txtlname.Text, txtform.Text);
            up.DisplayName = string.Format("{0} {1}", txtfname.Text, txtlname.Text);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, config.ADSettings.StudentsGroupName);
            if (up.IsMemberOf(gp)) up.EmailAddress = string.Format(config.BaseSettings.StudentEmailFormat, Username, up.GivenName, up.Surname);
            up.Save();

            // First, get a DE for the user
            DirectoryEntry usersDE = new DirectoryEntry(_ActiveDirectoryConnectionString, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(sAMAccountName=" + Username + ")";
            ds.PropertiesToLoad.Add("cn");
            SearchResult r = ds.FindOne();
            DirectoryEntry theUserDE = new DirectoryEntry(r.Path, config.ADSettings.ADUsername, config.ADSettings.ADPassword);

            // Now update the property setting
            if (theUserDE.Properties["Department"].Count == 0)
                theUserDE.Properties["Department"].Add(txtform.Text);
            else
                theUserDE.Properties["Department"][0] = txtform.Text;
            theUserDE.CommitChanges();

            //rmCom2000-UsrMgr-uPN
            
            ds = new DirectorySearcher(usersDE);
            ds.Filter = "(sAMAccountName=" + Username + ")";
            ds.PropertiesToLoad.Add("rmCom2000-UsrMgr-uPN");
            ds.PropertiesToLoad.Add("department");
            r = ds.FindOne();
            if (string.IsNullOrEmpty(config.BaseSettings.StudentPhotoHandler))
                userimage.Visible = false;
            else
            {
                try
                {
                    userimage.ImageUrl = string.Format("{0}?UPN={1}", config.BaseSettings.StudentPhotoHandler, r.Properties["rmCom2000-UsrMgr-uPN"][0].ToString());
                }
                catch { userimage.Visible = false; }
            }
            welcomename.Text = up.GivenName;
            name.Text = userimage.AlternateText = string.Format("{0} {1}", up.GivenName, up.Surname);
            try
            {
                form.Text = r.Properties["department"][0].ToString();
            }
            catch { form.Text = "n/a"; }
            if (up.IsMemberOf(gp)) form.Text = string.Format("<b>Form: </b>{0}", form.Text);
            else form.Text = string.Format("<b>Department: </b>{0}", form.Text);
            email.Text = up.EmailAddress;
        }
    }
}