using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using HAP.Web.Configuration;
using Microsoft.Win32;

namespace HAP.Web
{
    public partial class Setup : System.Web.UI.Page
    {
        public Version GetIisVersion()
        {
            using (RegistryKey componentsKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\InetStp", false))
            {
                if (componentsKey != null)
                {
                    int majorVersion = (int)componentsKey.GetValue("MajorVersion", -1);
                    int minorVersion = (int)componentsKey.GetValue("MinorVersion", -1);
                    if (majorVersion != -1 && minorVersion != -1)
                    {
                        return new Version(majorVersion, minorVersion);
                    }
                }
                return new Version(0, 0);
            }
        }


        public hapConfig Config { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                iisversion.Text = GetIisVersion().ToString();
                iis6wildcardlab.Text = (GetIisVersion().Major < 7) ? "IIS6 Wildcard Mapping: " : "IIS7 Integrated Pipeline: ";
                Config = hapConfig.Current;
                Cache.Insert("tempConfig", Config, null, DateTime.MaxValue, TimeSpan.FromMinutes(4));
                name.Text = Config.School.Name;
                schoolurl.Text = Config.School.WebSite;
                upn.Text = Config.AD.UPN;
                un.Text = Config.AD.User;
                up.Text = "";
                sg.Text = Config.AD.StudentsGroup;
                hp_ab_st.Text = Config.Homepage.AnnouncementBox.ShowTo;
                hp_ab_adt.Text = Config.Homepage.AnnouncementBox.EnableEditTo;
                proxyaddress.Text = Config.ProxyServer.Address;
                proxyenabled.Checked = Config.ProxyServer.Enabled;
                proxyport.Text = Config.ProxyServer.Port.ToString();
                smtpaddress.Text = Config.SMTP.Server;
                smtpenabled.Checked = Config.SMTP.Enabled;
                smtpfromemail.Text = Config.SMTP.FromEmail;
                smtpfromname.Text = Config.SMTP.FromUser;
                smtpuser.Text = Config.SMTP.User;
                smtppassword.Text = Config.SMTP.Password;
                smtpport.Text = Config.SMTP.Port.ToString();
                smtpssl.Checked = Config.SMTP.SSL;
                trackercode.Text = Config.Tracker.OverrideCode;
                trackerprovider.Text = Config.Tracker.Provider;
                trackerstafflogs.Text = Config.Tracker.MaxStaffLogons.ToString();
                trackerstudentlogs.Text = Config.Tracker.MaxStudentLogons.ToString();
                adorgs.DataSource = Config.AD.OUs.Values;
                adorgs.DataBind();
                homepageLinkGroups.DataSource = Config.Homepage.Groups.Values;
                homepageLinkGroups.DataBind();
                bsclean.Checked = Config.BookingSystem.KeepXmlClean;
                bstwoweek.Checked = Config.BookingSystem.TwoWeekTimetable;
                bsmax.Text = Config.BookingSystem.MaxBookingsPerWeek.ToString();
                bsdays.Text = Config.BookingSystem.MaxDays.ToString();
                bssubjects.DataSource = Config.BookingSystem.Subjects;
                bssubjects.DataBind();
                bsadmins.Text = Config.BookingSystem.Admins;
                bslessons.DataSource = Config.BookingSystem.Lessons;
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
                mscbWrite.Checked = Config.MySchoolComputerBrowser.WriteChecks;
                liveid.Text = Config.MySchoolComputerBrowser.LiveAppId;
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            Config = Cache["tempConfig"] as hapConfig;
            Config.School.Name = name.Text;
            Config.School.WebSite = schoolurl.Text;
            Config.AD.UPN = upn.Text;
            Config.AD.User = un.Text;
            if (up.Text.Length > 0) Config.AD.Password = up.Text;
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
            Config.BookingSystem.Admins = bsadmins.Text;
            Config.SMTP.User = smtpuser.Text;
            Config.SMTP.Password = smtppassword.Text;
            Config.BookingSystem.MaxDays =  int.Parse(bsdays.Text);
            Config.BookingSystem.MaxBookingsPerWeek = int.Parse(bsmax.Text);
            Config.BookingSystem.TwoWeekTimetable = bstwoweek.Checked;
            Config.MySchoolComputerBrowser.HideExtensions = mscbExt.Text;
            Config.MySchoolComputerBrowser.WriteChecks = mscbWrite.Checked;
            Config.Tracker.OverrideCode = trackercode.Text;
            Config.Tracker.Provider = trackerprovider.Text;
            Config.Tracker.MaxStaffLogons = int.Parse(trackerstafflogs.Text);
            Config.Tracker.MaxStudentLogons = int.Parse(trackerstudentlogs.Text);
            Config.BookingSystem.KeepXmlClean = bsclean.Checked;
            Config.BookingSystem.TwoWeekTimetable = bstwoweek.Checked;
            Config.BookingSystem.MaxBookingsPerWeek = int.Parse(bsmax.Text);
            Config.BookingSystem.MaxDays = int.Parse(bsdays.Text);
            Config.MySchoolComputerBrowser.LiveAppId = liveid.Text;
            Config.Save();
            Response.Redirect("~/Setup.aspx?Saved=1");
        }
    }
}