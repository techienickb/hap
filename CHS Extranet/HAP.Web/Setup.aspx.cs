using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using HAP.Web.Configuration;

namespace HAP.Web
{
    public partial class Setup : System.Web.UI.Page
    {
        public hapConfig Config { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Config = hapConfig.Current;
                Cache.Insert("tempConfig", Config, null, DateTime.MaxValue, TimeSpan.FromMinutes(4));
                name.Text = Config.School.Name;
                schoolurl.Text = Config.School.WebSite;
                upn.Text = Config.AD.UPN;
                un.Text = Config.AD.User;
                up.Text = Config.AD.Password;
                sg.Text = Config.AD.StudentsGroup;
                if (sg.Text.Length > 0) adbasestate.ImageUrl = "~/images/setup/267.png";
                else adstate.ImageUrl = "~/images/setup/266.png";
                hp_ab_st.Text = Config.Homepage.AnnouncementBox.ShowTo;
                hp_ab_adt.Text = Config.Homepage.AnnouncementBox.EnableEditTo;
                proxyaddress.Text = Config.ProxyServer.Address;
                proxyenabled.Checked = Config.ProxyServer.Enabled;
                proxyport.Text = Config.ProxyServer.Port.ToString();
                smtpaddress.Text = Config.SMTP.Server;
                smtpenabled.Checked = Config.SMTP.Enabled;
                smtpfromemail.Text = Config.SMTP.FromEmail;
                smtpfromname.Text = Config.SMTP.FromUser;
                smtpport.Text = Config.SMTP.Port.ToString();
                smtpssl.Checked = Config.SMTP.SSL;
                trackercode.Text = Config.Tracker.OverrideCode;
                trackerprovider.Text = Config.Tracker.Provider;
                trackerstafflogs.Text = Config.Tracker.MaxStaffLogons.ToString();
                trackerstudentlogs.Text = Config.Tracker.MaxStudentLogons.ToString();
                adorgs.DataSource = Config.AD.OUs.Values;
                adorgs.DataBind();
                homepageTabs.DataSource = homepageTabs2.DataSource = Config.Homepage.Tabs.Values;
                homepageTabs.DataBind(); homepageTabs2.DataBind();
                homepageLinkGroups.DataSource = Config.Homepage.Groups.Values;
                homepageLinkGroups.DataBind();
                bsclean.Checked = Config.BookingSystem.KeepXmlClean;
                bstwoweek.Checked = Config.BookingSystem.TwoWeekTimetable;
                bsmax.Text = Config.BookingSystem.MaxBookingsPerWeek.ToString();
                bsdays.Text = Config.BookingSystem.MaxDays.ToString();
                bssubjects.DataSource = Config.BookingSystem.Subjects;
                bssubjects.DataBind();
                bslessons.DataSource = Config.BookingSystem.Lessons.Values;
                bslessons.DataBind();
                bsresources.DataSource = Config.BookingSystem.Resources.Values;
                bsresources.DataBind();
                bsclean.Checked = Config.BookingSystem.KeepXmlClean;
                bsdays.Text = Config.BookingSystem.MaxDays.ToString();
                bsmax.Text = Config.BookingSystem.MaxBookingsPerWeek.ToString();
                bstwoweek.Checked = Config.BookingSystem.TwoWeekTimetable;
                mscbFilters.DataSource = Config.MySchoolComputerBrowser.Filters.ToArray();
                mscbFilters.DataBind();
                mscbMappings.DataSource = Config.MySchoolComputerBrowser.Mappings.Values;
                mscbMappings.DataBind();
                mscbQuotaServers.DataSource = Config.MySchoolComputerBrowser.QuotaServers.ToArray();
                mscbQuotaServers.DataBind();
                mscbExt.Text = Config.MySchoolComputerBrowser.HideExtensions;
            }
        }

        protected void sg_TextChanged(object sender, EventArgs e)
        {
            if (sg.Text.Length > 0) adbasestate.ImageUrl = "~/images/setup/267.png";
            else adstate.ImageUrl = "~/images/setup/266.png";
        }

        protected void adorgs_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                if (Config == null) Config = Cache["tempConfig"] as hapConfig;
                Config.AD.OUs.Remove((string)e.CommandArgument);
                adorgs.DataSource = Config.AD.OUs.Values;
                adorgs.DataBind();
                Cache.Remove("tempConfig");
                Cache.Insert("tempConfig", Config, null, DateTime.MaxValue, TimeSpan.FromMinutes(4));
            }
        }

        protected void newou_Click(object sender, EventArgs e)
        {
            if (Config == null) Config = Cache["tempConfig"] as hapConfig;
            Config.AD.OUs.Add(ouname.Text, oupath.Text, ouignore.Checked);
            ouname.Text = oupath.Text = "";
            ouignore.Checked = false;
            adorgs.DataSource = Config.AD.OUs.Values;
            adorgs.DataBind();
            Cache.Remove("tempConfig");
            Cache.Insert("tempConfig", Config, null, DateTime.MaxValue, TimeSpan.FromMinutes(4));
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            Config = Cache["tempConfig"] as hapConfig;
            Config.School.Name = name.Text;
            Config.School.WebSite = schoolurl.Text;
            Config.AD.UPN = upn.Text;
            Config.AD.User = un.Text;
            Config.AD.Password = up.Text;
            Config.AD.StudentsGroup = sg.Text;
            Config.SMTP.Server=smtpaddress.Text;
            Config.SMTP.Enabled = smtpenabled.Checked;
            Config.SMTP.FromEmail = smtpfromemail.Text;
            Config.SMTP.FromUser = smtpfromname.Text;
            Config.SMTP.Port = int.Parse(smtpport.Text);
            Config.SMTP.SSL = smtpssl.Checked;
            adorgs.DataSource = Config.AD.OUs.Values;
            Config.Homepage.AnnouncementBox.ShowTo = hp_ab_st.Text;
            Config.Homepage.AnnouncementBox.EnableEditTo = hp_ab_adt.Text;
            Config.ProxyServer.Address = proxyaddress.Text;
            Config.ProxyServer.Enabled = proxyenabled.Checked;
            Config.ProxyServer.Port = int.Parse(proxyport.Text);
            Config.BookingSystem.KeepXmlClean = bsclean.Checked;
            Config.BookingSystem.MaxDays =  int.Parse(bsdays.Text);
            Config.BookingSystem.MaxBookingsPerWeek = int.Parse(bsmax.Text);
            Config.BookingSystem.TwoWeekTimetable = bstwoweek.Checked;
            Config.MySchoolComputerBrowser.HideExtensions = mscbExt.Text;
            Config.Tracker.OverrideCode = trackercode.Text;
            Config.Tracker.Provider = trackerprovider.Text;
            Config.Tracker.MaxStaffLogons = int.Parse(trackerstafflogs.Text);
            Config.Tracker.MaxStudentLogons = int.Parse(trackerstudentlogs.Text);
            Config.BookingSystem.KeepXmlClean = bsclean.Checked;
            Config.BookingSystem.TwoWeekTimetable = bstwoweek.Checked;
            Config.BookingSystem.MaxBookingsPerWeek = int.Parse(bsmax.Text);
            Config.BookingSystem.MaxDays = int.Parse(bsdays.Text);
            Config.Save();
            Response.Redirect("~/Setup.aspx?Saved=1");
        }
    }
}