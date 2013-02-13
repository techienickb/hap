using HAP.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HAP.Web
{
    public partial class kerberos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "Home Access Plus+ Kerberos Logon\n\nUsername: " + Request.ServerVariables["AUTH_USER"], System.Diagnostics.EventLogEntryType.Information, true);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "Kerberos Logon", Request.ServerVariables["AUTH_USER"], Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
            FormsAuthentication.SetAuthCookie(Request.ServerVariables["AUTH_USER"], false);
            HttpCookie tokenCookie = new HttpCookie("token", Request.ServerVariables["AUTH_TYPE"]);
            if (Request.Cookies["token"] == null) Response.AppendCookie(tokenCookie);
            else Response.SetCookie(tokenCookie);
            FormsAuthentication.RedirectFromLoginPage(Request.ServerVariables["AUTH_USER"], false);
        }
    }
}