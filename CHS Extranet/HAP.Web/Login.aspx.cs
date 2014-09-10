using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using HAP.Web.Configuration;
using HAP.Data;
using HAP.AD;
using System.Net;
using System.Net.NetworkInformation;
using System.Web.Configuration;

namespace HAP.Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            oneusecodes.Visible = hapConfig.Current.AD.AllowOneUseCodes;
            HAP.AD.OneUse.Current.RemoveExpiredCodes();
            if (User.Identity.IsAuthenticated && !Page.IsPostBack && Request.QueryString.Count < 2) Response.Redirect("unauthorised.aspx");
            if (!User.Identity.IsAuthenticated && Request.Cookies["token"] != null)
            {
                Response.Cookies.Remove("token");
                HttpCookie t = new HttpCookie("token") { Expires = DateTime.UtcNow.AddHours(-10), Secure = true, Domain = ((AuthenticationSection)WebConfigurationManager.GetWebApplicationSection("system.web/authentication")).Forms.Domain };
                Response.AppendCookie(t);
            }
            if (!Page.IsPostBack)
            {
                if (!Request.Browser.Browser.Contains("Chrome"))
                {
                    try
                    {
                        foreach (string ip in hapConfig.Current.AD.InternalIP)
                        {
                            if (new IPSubnet(ip).Contains(Request.UserHostAddress))
                            {
                                if (Dns.GetHostEntry(Request.UserHostAddress).HostName.ToLower().EndsWith(hapConfig.Current.AD.UPN.ToLower()) && Request.QueryString.Count < 2)
                                    Response.Redirect("~/kerberos.aspx?ReturnUrl=" + Request.QueryString[0], true);
                                else if (Dns.GetHostEntry(Request.UserHostAddress).HostName.ToLower().EndsWith(hapConfig.Current.AD.UPN.ToLower()) && Request.QueryString.Count == 2)
                                    username.Text = User.Identity.Name.Contains('\\') ? User.Identity.Name.Substring(User.Identity.Name.IndexOf('\\') + 1) : User.Identity.Name;
                            }
                        }
                    }
                    catch { }
                }
                if (Cache.Get("hapBannedIps") == null) HttpContext.Current.Cache.Insert("hapBannedIps", new List<Banned>());
                List<Banned> bans = Cache.Get("hapBannedIps") as List<Banned>;
                if (bans.Count(b => b.Computer == Request.UserHostName && b.IPAddress == Request.UserHostAddress && b.UserAgent == Request.UserAgent) > 0)
                {
                    Banned ban = bans.Single(b => b.Computer == Request.UserHostName && b.IPAddress == Request.UserHostAddress && b.UserAgent == Request.UserAgent);
                    if (ban.IsBanned)
                    {
                        if (ban.BannedUntil.Value < DateTime.Now) { ban.IsBanned = false; ban.BannedUntil = null; ban.Attempts = 0; login.Visible = true; }
                        else
                        {
                            message.Text = "<div class=\"ui-state-error ui-corner-all\" style=\" padding: 5px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px;\"></span>Your IP Addresss has been banned from logging on until " + ban.BannedUntil.Value.ToShortTimeString() + "</div>";
                            login.Visible = false;
                            return;
                        }
                    }
                    else login.Visible = true;
                }
                else login.Visible = true;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (hapConfig.Current.FirstRun) Response.Redirect(new Uri(new Uri((Request.Url.Scheme == Uri.UriSchemeHttp ? Request.Url.ToString().Replace("http", "https") : Request.Url.ToString())), "setup.aspx").ToString());
            Title = string.Format(Title, HAP.Web.Configuration.hapConfig.Current.School.Name);
        }

        protected bool IsValidCode(out string code)
        {
            if (HAP.AD.OneUse.Current.ContainsKey(oneusecode.Text) && HAP.AD.OneUse.Current[oneusecode.Text].Username.ToLower() == username.Text.ToLower())
            {
                code = HAP.AD.OneUse.Current[oneusecode.Text].Token;
                HAP.AD.OneUse.Current.RemoveCode(oneusecode.Text);
                return true;
            }
            code = "";
            return false;
        }

        protected void login_Click(object sender, EventArgs e)
        {
            if (Cache.Get("hapBannedIps") == null) HttpContext.Current.Cache.Insert("hapBannedIps", new List<Banned>());
            List<Banned> bans = Cache.Get("hapBannedIps") as List<Banned>;
            Cache.Remove("hapBannedIps");
            if (bans.Count(b => b.Computer == Request.UserHostName && b.IPAddress == Request.UserHostAddress && b.UserAgent == Request.UserAgent) == 0)
                bans.Add(new Banned { Attempts = 0, Computer = Request.UserHostName, IPAddress = Request.UserHostAddress, IsBanned = false, UserAgent = Request.UserAgent });
            Banned ban = bans.Single(b => b.Computer == Request.UserHostName && b.IPAddress == Request.UserHostAddress && b.UserAgent == Request.UserAgent);
            if (ban.IsBanned)
            {
                if (ban.BannedUntil.Value < DateTime.Now) { ban.IsBanned = false; ban.BannedUntil = null; ban.Attempts = 0; login.Visible = true; }
                else
                {
                    message.Text = "<div class=\"ui-state-error ui-corner-all\" style=\" padding: 5px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px;\"></span>Your IP Addresss has been banned from logging on until " + ban.BannedUntil.Value.ToShortTimeString() + "</div>";
                    login.Visible = false;
                    return;
                }
            }
            string code;
            ban.Attempts++;
            try
            {
                UserAccountControl uac = HAP.AD.User.UserAccountControl(username.Text);
                if ((uac & UserAccountControl.AccountDisabled) == UserAccountControl.AccountDisabled)
                {
                    HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Logon\n\nUsername: " + username.Text + "\nState: Disabled", System.Diagnostics.EventLogEntryType.Information, true);
                    HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Disabled Logon", username.Text, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
                    message.Text = "<div class=\"ui-state-error ui-corner-all\" style=\" padding: 5px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px;\"></span>" + Localizable.Localize("ad/disabled") + "</div>";
                    return;
                }
                else if ((uac & UserAccountControl.PasswordExpired) == UserAccountControl.PasswordExpired)
                {
                    HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Logon\n\nUsername: " + username.Text + "\nState: Password Expired", System.Diagnostics.EventLogEntryType.Information, true);
                    HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Expired Logon", username.Text, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
                    message.Text = "<div class=\"ui-state-error ui-corner-all\" style=\" padding: 5px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px;\"></span>" + Localizable.Localize("ad/passexpired") + "</div>";
                    return;
                }
                else if ((uac & UserAccountControl.Lockout) == UserAccountControl.Lockout)
                {
                    HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Logon\n\nUsername: " + username.Text + "\nState: Locked Out", System.Diagnostics.EventLogEntryType.Information, true);
                    HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Lockedout Logon", username.Text, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
                    message.Text = "<div class=\"ui-state-error ui-corner-all\" style=\" padding: 5px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px;\"></span>" + Localizable.Localize("ad/lockedout") + "</div>";
                    return;
                }
            }
            catch
            {
                HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Logon\n\nUsername: " + username.Text + "\nState: Invalid", System.Diagnostics.EventLogEntryType.Error, true);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Invalid User", username.Text, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
            }
            if (oneusecode.Text.Length == 4 && IsValidCode(out code) && !ban.IsBanned && Membership.ValidateUser(username.Text.Trim(), HAP.AD.TokenGenerator.ConvertToPlain(code)))
            {
                HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Logon\n\nUsername: " + username.Text, System.Diagnostics.EventLogEntryType.Information, true);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Logon", username.Text, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
                FormsAuthentication.SetAuthCookie(username.Text, false);
                HttpCookie tokenCookie = new HttpCookie("token", code);
                tokenCookie.Domain = ((AuthenticationSection)WebConfigurationManager.GetWebApplicationSection("system.web/authentication")).Forms.Domain;
                tokenCookie.Secure = true;
                if (Request.Cookies["token"] == null) Response.AppendCookie(tokenCookie);
                else Response.SetCookie(tokenCookie);
                bans.Remove(ban);
                Cache.Insert("hapBannedIps", bans);
                FormsAuthentication.RedirectFromLoginPage(username.Text, false);
            }
            else if (Membership.ValidateUser(username.Text.Trim(), password.Text.Trim()) && !ban.IsBanned)
            {
                HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Logon\n\nUsername: " + username.Text, System.Diagnostics.EventLogEntryType.Information, true);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Logon", username.Text, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
                FormsAuthentication.SetAuthCookie(username.Text, false);
                HttpCookie tokenCookie = new HttpCookie("token", TokenGenerator.ConvertToToken(password.Text));
                tokenCookie.Secure = true;
                tokenCookie.Domain = ((AuthenticationSection)WebConfigurationManager.GetWebApplicationSection("system.web/authentication")).Forms.Domain;
                if (Request.Cookies["token"] == null) Response.AppendCookie(tokenCookie);
                else Response.SetCookie(tokenCookie);
                bans.Remove(ban);
                Cache.Insert("hapBannedIps", bans);
                if (Request.QueryString["ReturnUrl"] == "OneUseCodes.aspx") Response.Redirect("OneUseCodes.aspx?gencodes=1");
                else FormsAuthentication.RedirectFromLoginPage(username.Text, false);
            }
            else
            {
                if (ban.Attempts > (hapConfig.Current.AD.MaxLogonAttemps - 1))
                {
                    ban.IsBanned = true;
                    ban.BannedUntil = DateTime.Now.AddMinutes(30);
                    message.Text = "<div class=\"ui-state-error ui-corner-all\" style=\" padding: 5px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px;\"></span>Your IP Addresss has been banned from logging on until " + ban.BannedUntil.Value.ToShortTimeString() + "</div>";
                    login.Visible = false;
                    HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Logon\n\nBanned logon Username: " + username.Text, System.Diagnostics.EventLogEntryType.Information, true);
                    HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Logon.Banned", username.Text, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
                }
                else
                {
                    login.Visible = true;
                    message.Text = "<div class=\"ui-state-error ui-corner-all\" style=\" padding: 5px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px;\"></span>Either your Username or Password was Incorrect or you do not have permission to access this site.</div>";
                }
                Cache.Insert("hapBannedIps", bans);
            }
        }
    }

    public class Banned
    {
        public bool IsBanned { get; set; }
        public DateTime? BannedUntil { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public string Computer { get; set; }
        public int Attempts { get; set; }

    }

    public class IPSubnet
    {
        private readonly byte[] _address;
        private readonly int _prefixLength;

        public IPSubnet(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            string[] parts = value.Split('/');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid CIDR notation.", "value");

            _address = IPAddress.Parse(parts[0]).GetAddressBytes();
            _prefixLength = Convert.ToInt32(parts[1], 10);
        }

        public bool Contains(string address)
        {
            return this.Contains(IPAddress.Parse(address).GetAddressBytes());
        }

        public bool Contains(byte[] address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            if (address.Length != _address.Length)
                return false; // IPv4/IPv6 mismatch

            int index = 0;
            int bits = _prefixLength;

            for (; bits >= 8; bits -= 8)
            {
                if (address[index] != _address[index])
                    return false;
                ++index;
            }

            if (bits > 0)
            {
                int mask = (byte)~(255 >> bits);
                if ((address[index] & mask) != (_address[index] & mask))
                    return false;
            }

            return true;
        }
    }
}