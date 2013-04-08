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
