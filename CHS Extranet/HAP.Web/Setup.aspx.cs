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
using System.Diagnostics;

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
                Cache.Insert("tempConfig", Config, null, DateTime.MaxValue, TimeSpan.FromMinutes(10));
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
                mscbFilters.DataSource = Config.MyFiles.Filters.ToArray();
                mscbFilters.DataBind();
                mscbMappings.DataSource = Config.MyFiles.Mappings.Values;
                mscbMappings.DataBind();
                mscbQuotaServers.DataSource = Config.MyFiles.QuotaServers.ToArray();
                mscbQuotaServers.DataBind();
                mscbExt.Text = Config.MyFiles.HideExtensions;
                mscbWrite.Checked = Config.MyFiles.WriteChecks;
                liveid.Text = Config.MyFiles.LiveAppId;
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            error.Visible = false;
            Config = Cache["tempConfig"] as hapConfig;
            try
            {
                Config.School.Name = name.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the School Name"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.School.WebSite = schoolurl.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the School Url"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.AD.UPN = upn.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the AD UPN"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.AD.User = un.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the AD Username"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            if (up.Text.Length > 0) Config.AD.Password = up.Text;
            try
            {
                Config.AD.StudentsGroup = sg.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the AD Students Group"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.SMTP.Server = smtpaddress.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the SMTP Server Address"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.SMTP.Enabled = smtpenabled.Checked;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the SMTP Enabled Checkbox"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.SMTP.FromEmail = smtpfromemail.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the SMTP From Address"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.SMTP.FromUser = smtpfromname.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the SMTP From User"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.SMTP.Port = int.Parse(smtpport.Text);
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the SMTP Port"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.SMTP.SSL = smtpssl.Checked;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the SMTP Server SSL Checkbox"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.SMTP.User = smtpuser.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the SMTP User"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.SMTP.Password = smtppassword.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the SMTP Password"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                adorgs.DataSource = Config.AD.OUs.Values;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the AD OUs"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.Homepage.AnnouncementBox.ShowTo = hp_ab_st.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Home Page Announcement Box Show To "; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.Homepage.AnnouncementBox.EnableEditTo = hp_ab_adt.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Home Page Announcement Box Enable Edit To"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.ProxyServer.Address = proxyaddress.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Proxy Server Address"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.ProxyServer.Enabled = proxyenabled.Checked;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Proxy Server Enable Box"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.ProxyServer.Port = int.Parse(proxyport.Text);
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Proxy Server Port"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.BookingSystem.KeepXmlClean = bsclean.Checked;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Booking System Keep XML Clean Option"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.BookingSystem.Admins = bsadmins.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Booking System Admins"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.BookingSystem.MaxDays = int.Parse(bsdays.Text);
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Booking System Max Days"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.BookingSystem.MaxBookingsPerWeek = int.Parse(bsmax.Text);
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Booking System Max Bookings Per Week"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.BookingSystem.TwoWeekTimetable = bstwoweek.Checked;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Booking System Two Week Timetable Option"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.MyFiles.HideExtensions = mscbExt.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the My Files Hidden Extensions"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.MyFiles.WriteChecks = mscbWrite.Checked;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the My Files Write Checks Option"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.Tracker.OverrideCode = trackercode.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Logon Tracker Override Code"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.Tracker.Provider = trackerprovider.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Logon Tracker Provider"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.Tracker.MaxStaffLogons = int.Parse(trackerstafflogs.Text);
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Logon Tracker Max Staff Logons"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.Tracker.MaxStudentLogons = int.Parse(trackerstudentlogs.Text);
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Logon Tracker Max Student Logons"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.MyFiles.LiveAppId = liveid.Text;
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error with the Live App Id"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
            try
            {
                Config.Save();
                if (!EventLog.SourceExists("Home Access Plus+"))
                {
                    HAP.AD.User _user = new HAP.AD.User();
                    _user.Authenticate(Config.AD.User, Config.AD.Password);
                    try
                    {
                        _user.ImpersonateContained();
                        EventLog.CreateEventSource("Home Access Plus+", "Application");
                    }
                    catch { }
                    finally { _user.EndContainedImpersonate(); }
                }
                Response.Redirect("~/Setup.aspx?Saved=1");
            }
            catch (Exception ex) { error.Visible = true; errormessage.Text = "Error Saving the Configuration"; errormessagemore.Text = ex.Message + "<br /><br />" + ex.StackTrace; }
        }
    }
}