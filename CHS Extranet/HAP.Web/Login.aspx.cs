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

namespace HAP.Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (hapConfig.Current.FirstRun) Response.Redirect("setup.aspx");
            Title = string.Format(Title, HAP.Web.Configuration.hapConfig.Current.School.Name);
        }

        protected void login_Click(object sender, EventArgs e)
        {
            if (Membership.ValidateUser(username.Text.Trim(), password.Text.Trim()))
            {
                Session.Add("password", password.Text);
                FormsAuthentication.SetAuthCookie(username.Text, false);
                HttpCookie tokenCookie = new HttpCookie("token", TokenGenerator.ConvertToToken(password.Text));
                if (Request.Cookies["token"] == null) Response.AppendCookie(tokenCookie);
                else Response.SetCookie(tokenCookie);
                FormsAuthentication.RedirectFromLoginPage(username.Text, false);
            }
            else // Fail to login
            {
                string invalidLogin = "Invalid Login.";
                message.Text = invalidLogin;
            }
        }
    }
}