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

namespace HAP.Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated && !Page.IsPostBack) Response.Redirect("unauthorised.aspx");
            if (!Page.IsPostBack)
            {
                List<Banned> bans = Cache.Get("hapBannedIps") as List<Banned>;
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
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (hapConfig.Current.FirstRun) Response.Redirect(new Uri(new Uri((Request.Url.Scheme == Uri.UriSchemeHttp ? Request.Url.ToString().Replace("http", "https") : Request.Url.ToString())), "setup.aspx").ToString());
            Title = string.Format(Title, HAP.Web.Configuration.hapConfig.Current.School.Name);
        }

        protected void login_Click(object sender, EventArgs e)
        {
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
            ban.Attempts++;
            if (Membership.ValidateUser(username.Text.Trim(), password.Text.Trim()) && !ban.IsBanned)
            {
                HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Logon\n\nUsername: " + username.Text, System.Diagnostics.EventLogEntryType.SuccessAudit, true);
                HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Logon", username.Text, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
                FormsAuthentication.SetAuthCookie(username.Text, false);
                HttpCookie tokenCookie = new HttpCookie("token", TokenGenerator.ConvertToToken(password.Text));
                if (Request.Cookies["token"] == null) Response.AppendCookie(tokenCookie);
                else Response.SetCookie(tokenCookie);
                Cache.Insert("hapBannedIps", bans);
                FormsAuthentication.RedirectFromLoginPage(username.Text, false);
            }
            else
            {
                if (ban.Attempts > 3)
                {
                    ban.IsBanned = true;
                    ban.BannedUntil = DateTime.Now.AddMinutes(30);
                    message.Text = "<div class=\"ui-state-error ui-corner-all\" style=\" padding: 5px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px;\"></span>Your IP Addresss has been banned from logging on until " + ban.BannedUntil.Value.ToShortTimeString() + "</div>";
                    login.Visible = false;
                    HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Logon\n\nBanned logon Username: " + username.Text, System.Diagnostics.EventLogEntryType.FailureAudit, true);
                    HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Logon.Banned", username.Text, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
                }
                else
                {
                    login.Visible = true;
                    message.Text = "<div class=\"ui-state-error ui-corner-all\" style=\" padding: 5px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px;\"></span>Either your Username or Password was Incorrect.</div>";
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
}