using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using HAP.AD;
using HAP.Web.Configuration;
using System.Web.Security;
using System.Diagnostics;

namespace HAP.AD
{
    [HAP.Web.Configuration.ServiceAPI("api/ad")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class API
    {
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "{username}/{password}")]
        public JSONUser UserGET(string username, string password)
        {
            JSONUser user = new JSONUser();
            try
            {
                User u = new User();
                u.Authenticate(username, password);
                FormsAuthentication.SetAuthCookie(username, false);
                Log("HAP+ App Logon", "Home Access Plus+ Logon\n\nUsername: " + username, System.Diagnostics.EventLogEntryType.Information);
                user.Token2 = HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Value;
                user.Token1 = TokenGenerator.ConvertToToken(password);
                user.Username = u.UserName;
                user.FirstName = u.FirstName;
                user.isValid = true;
                user.Token2Name = FormsAuthentication.FormsCookieName;
                user.SiteName = hapConfig.Current.School.Name;
            }
            catch (Exception e) { user.Token2 = e.ToString(); user.isValid = false; }
            return user;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "")]
        public JSONUser User(string username, string password)
        {
            return UserGET(username, password);
        }

        public void Log(string source, string message, EventLogEntryType type)
        {
            try
            {
                EventLog myLog = new EventLog("Application", ".", "Home Access Plus+");
                myLog.EnableRaisingEvents = true;
                if (type == EventLogEntryType.Error) myLog.WriteEntry("An error occurred in Home Access Plus+\r\n\r\nPage: " + source + "\r\n\r\n" + message, type);
                else myLog.WriteEntry("Home Access Plus+ Info\r\n\r\nPage: " + source + "\r\n\r\n" + message, type);
                myLog.Close();
            }
            catch { }
        }

    }

    public class JSONUser
    {
        public bool isValid { get; set; }
        public string Username { get;set; }
        public string Token1 { get; set; }
        public string FirstName { get; set; }
        public string Token2 { get; set; }
        public string Token2Name { get; set; }
        public string SiteName { get; set; }
    }
}
