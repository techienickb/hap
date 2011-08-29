using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.DirectoryServices;
using HAP.Web.Configuration;
using System.Xml;
using System.Runtime.InteropServices;
using HAP.Web.UserCard;

namespace HAP.Web
{
    public partial class Default : HAP.Web.Controls.Page
    {
        private PrincipalContext pcontext;
        protected UserPrincipal up { get; set; }
        private hapConfig config;

        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            if (config.FirstRun) Response.Redirect("~/Setup.aspx", true);
            this.Title = string.Format("{0} - Home Access Plus+", config.School.Name);
        }

        protected string Department { get; set; }
        protected decimal space { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<TabType, Tab> tabs = config.Homepage.Tabs.FilteredTabs;
            tabheader_repeater.DataSource = config.Homepage.Tabs;
            tabheader_repeater.DataBind();
            tab_Me.Visible = tabs.ContainsKey(TabType.Me);
            if (tab_Me.Visible)
            {
                updatemydetails.Visible = false;
                if (tabs[TabType.Me].AllowUpdateTo == "All") updatemydetails.Visible = true;
                else if (tabs[TabType.Me].ShowTo != "None")
                {
                    bool vis = false;
                    foreach (string s in tabs[TabType.Me].AllowUpdateTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                        if (!vis) vis = User.IsInRole(s);
                    if (vis) updatemydetails.Visible = true;
                }
                
                DirectoryEntry usersDE = new DirectoryEntry(AD.ADUtils.FriendlyDomainToLdapDomain(config.AD.UPN), config.AD.User, config.AD.Password);
                DirectorySearcher ds = new DirectorySearcher(usersDE);
                ds.Filter = "(sAMAccountName=" + ADUser.UserName + ")";
                ds.PropertiesToLoad.Add("rmCom2000-UsrMgr-uPN");
                ds.PropertiesToLoad.Add("department");
                SearchResult r = ds.FindOne();
                try
                {
                    Department = r.Properties["department"][0].ToString();
                }
                catch { Department = "n/a"; }
                //rmCom2000-UsrMgr-uPN

                if (string.IsNullOrEmpty(config.School.PhotoHandler))
                    userimage.Visible = false;
                else
                {
                    if (string.IsNullOrWhiteSpace(up.EmployeeId))
                    {
                        try
                        {
                            userimage.ImageUrl = string.Format("{0}?UPN={1}", config.School.PhotoHandler, r.Properties["rmCom2000-UsrMgr-uPN"][0].ToString());
                        }
                        catch { }
                    }
                    else userimage.ImageUrl = string.Format("{0}?UPN={1}", config.School.PhotoHandler, up.EmployeeId);
                }
                if (tabs[TabType.Me].ShowSpace.Value)
                {
                    DriveMapping mapping = null;
                    foreach (DriveMapping m in config.MySchoolComputerBrowser.Mappings.Values)
                        if (m.UNC.Contains("%homepath%")) mapping = m;
                    space = -1;
                    if (mapping != null)
                    {
                        try
                        {
                            long freeBytesForUser, totalBytes, freeBytes;
                            if (mapping.UsageMode == MappingUsageMode.DriveSpace)
                            {
                                if (Win32.GetDiskFreeSpaceEx(string.Format(mapping.UNC.Replace("%homepath%", up.HomeDirectory), ADUser.UserName), out freeBytesForUser, out totalBytes, out freeBytes))
                                    space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                            }
                            else
                            {

                                HAP.Data.Quota.QuotaInfo qi = HAP.Data.ComputerBrowser.Quota.GetQuota(ADUser.UserName, string.Format(mapping.UNC.Replace("%homepath%", up.HomeDirectory)));
                                space = Math.Round((Convert.ToDecimal(qi.Used) / Convert.ToDecimal(qi.Total)) * 100, 2);
                                if (qi.Total == -1)
                                    if (Win32.GetDiskFreeSpaceEx(string.Format(mapping.UNC.Replace("%homepath%", up.HomeDirectory), ADUser.UserName), out freeBytesForUser, out totalBytes, out freeBytes))
                                        space = Math.Round(100 - ((Convert.ToDecimal(freeBytes.ToString() + ".00") / Convert.ToDecimal(totalBytes.ToString() + ".00")) * 100), 2);
                            }
                        }
                        catch { }
                    }
                }
            }
            tab_Password.Visible = tabs.ContainsKey(TabType.Password);
            tab_Bookings.Visible = tabs.ContainsKey(TabType.Bookings);
            if (tab_Bookings.Visible)
            {
                
                List<HAP.Data.BookingSystem.Booking> mybookings = new List<Data.BookingSystem.Booking>();
                foreach (XmlNode n in HAP.Data.BookingSystem.BookingSystem.BookingsDoc.SelectNodes("/Bookings/Booking[@username='" + ADUser.UserName.ToLower() + "']"))
                    mybookings.Add(new HAP.Data.BookingSystem.Booking(n));
                var list = from element in mybookings orderby element.Date ascending, element.Lesson select element;
                mybookings = new List<Data.BookingSystem.Booking>();
                if (list.ToArray().Length > 6) for (int x = 0; x < 6; x++) mybookings.Add(list.ToArray()[x]);
                else foreach (HAP.Data.BookingSystem.Booking booking in list) mybookings.Add(booking);
                bookingslist.DataSource = mybookings.ToArray();
                bookingslist.DataBind();
            }
            tab_Tickets.Visible = tabs.ContainsKey(TabType.Tickets);
            if (tab_Tickets.Visible)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Server.MapPath("~/App_Data/Tickets.xml"));
                string xpath = string.Format("/Tickets/Ticket[@status!='Fixed']");
                int x = 0;
                if (User.IsInRole("Domain Admins"))
                {
                    List<Ticket> tickets = new List<Ticket>();
                    foreach (XmlNode node in doc.SelectNodes(xpath))
                        if (x < 4)
                        {
                            tickets.Add(Ticket.Parse(node));
                            x++;
                        }
                    ticketslist.DataSource = tickets.ToArray();
                }
                else
                {
                    List<Ticket> tickets = new List<Ticket>();
                    foreach (XmlNode node in doc.SelectNodes(xpath))
                        if (node.SelectNodes("Note")[0].Attributes["username"].Value.ToLower() == ADUser.UserName.ToLower() && x < 4)
                        {
                            tickets.Add(Ticket.Parse(node));
                            x++;
                        } 
                    ticketslist.DataSource = tickets.ToArray();
                }
                ticketslist.DataBind();
            }
            List<LinkGroup> groups = new List<LinkGroup>();
            foreach (LinkGroup group in config.Homepage.Groups)
                if (group.ShowTo == "All") groups.Add(group);
                else if (group.ShowTo != "None")
                {
                    bool vis = false;
                    foreach (string s in group.ShowTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                        if (!vis) vis = User.IsInRole(s);
                    if (vis) groups.Add(group);
                }
            if (!Page.IsPostBack)
            {
                txtfname.Text = up.GivenName;
                txtlname.Text = up.Surname;
                try
                {
                    txtform.Text = Department;
                }
                catch { txtform.Text = ""; }
                if (User.IsInRole(config.AD.StudentsGroup)) formlabel.Text = "<b>Form: </b>";
                else formlabel.Text = "<b>Department: </b>";
            }
            homepagelinks.DataSource = groups.ToArray();
            homepagelinks.DataBind();
        }

        internal static class Win32
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);
        }

        protected void editmydetails_Click(object sender, EventArgs e)
        {
            up.Surname = txtlname.Text;
            up.GivenName = txtfname.Text;
            up.Description = string.Format("{0} {1} in {2}", txtfname.Text, txtlname.Text, txtform.Text);
            up.DisplayName = string.Format("{0} {1}", txtfname.Text, txtlname.Text);
            up.Save();

            // First, get a DE for the user
            DirectoryEntry usersDE = new DirectoryEntry(AD.ADUtils.FriendlyDomainToLdapDomain(config.AD.UPN), config.AD.User, config.AD.Password);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(sAMAccountName=" + ADUser.UserName + ")";
            ds.PropertiesToLoad.Add("cn");
            SearchResult r = ds.FindOne();
            DirectoryEntry theUserDE = new DirectoryEntry(r.Path, config.AD.User, config.AD.Password);

            // Now update the property setting
            if (theUserDE.Properties["Department"].Count == 0)
                theUserDE.Properties["Department"].Add(txtform.Text);
            else
                theUserDE.Properties["Department"][0] = txtform.Text;
            theUserDE.CommitChanges();
            Response.Redirect("./");
        }

        protected void ChangePass_Click(object sender, EventArgs e)
        {
            hapConfig config = hapConfig.Current;
            try
            {
                DirectoryEntry usersDE = new DirectoryEntry(AD.ADUtils.FriendlyDomainToLdapDomain(config.AD.UPN), ADUser.UserName, currentpass.Text);
                DirectorySearcher ds = new DirectorySearcher(usersDE);
                ds.Filter = "(sAMAccountName=" + ADUser.UserName + ")";
                SearchResult r = ds.FindOne();
                DirectoryEntry user = r.GetDirectoryEntry();
                if (user == null) throw new Exception("I can't find your username");
                user.Invoke("ChangePassword", currentpass.Text, newpass.Text);
                user.CommitChanges();
                errormess.Text = "Password Changed";
            }
            catch (Exception ex) { errormess.Text = ex.Message; }
            savepass.Enabled = true;
            currentpass.Text = newpass.Text = confpass.Text = "";
        }
    }
}