using HAP.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
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
            HttpWorkerRequest workerRequest = (HttpWorkerRequest)((IServiceProvider)HttpContext.Current).GetService(typeof(HttpWorkerRequest));
            string str2 = workerRequest.GetServerVariable("AUTH_TYPE");
            WindowsIdentity wi = new WindowsIdentity(workerRequest.GetUserToken());
            string username = wi.Name.Contains('\\') ? wi.Name.Substring(wi.Name.IndexOf('\\') + 1) : wi.Name;
            HAP.Web.Logging.EventViewer.Log("HAP+ Logon", "HAP+ " + str2 + " Logon\n\nUsername: " + username, System.Diagnostics.EventLogEntryType.Information, true);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, str2 + " Logon", username, Request.UserHostAddress, Request.Browser.Platform, Request.Browser.Browser + " " + Request.Browser.Version, Request.UserHostName, Request.UserAgent);
            FormsAuthentication.SetAuthCookie(username, false);
            HttpCookie tokenCookie = new HttpCookie("token", str2);
            tokenCookie.Secure = true;
            if (Request.Cookies["token"] == null) Response.AppendCookie(tokenCookie);
            else Response.SetCookie(tokenCookie);
            FormsAuthentication.RedirectFromLoginPage(username, false);
        }
    }
}