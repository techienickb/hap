﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using HAP.AD;
using HAP.Web.Configuration;

namespace HAP.Web.LiveTiles
{
    [HAP.Web.Configuration.ServiceAPI("api/livetiles")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class API
    {
        [OperationContract]
        [WebGet(UriTemplate = "Exchange/Unread")]
        public int ExchangeUnread()
        {
            return HAP.Web.LiveTiles.ExchangeConnector.Unread();
        }

        [OperationContract]
        [WebGet(UriTemplate = "Exchange/Appointments")]
        public string[] ExchangeAppointments()
        {
            return HAP.Web.LiveTiles.ExchangeConnector.Appointments();
        }

        [OperationContract]
        [WebGet(UriTemplate = "Me")]
        public HAP.Web.LiveTiles.Me Me()
        {
            return HAP.Web.LiveTiles.Me.GetMe;
        }

        [OperationContract]
        [WebGet(UriTemplate = "Uptime/{Server}")]
        public string Uptime(string Server)
        {
            TimeSpan t = HAP.Web.LiveTiles.ServerUptime.Uptime(Server);
            if (t == new TimeSpan(0))
                return "The Server " + Server + " is DOWN";
            else return t.Days + " days<br/>" + t.Hours + "hours<br/>" + t.Minutes + "mins<br/>" + t.Seconds + " secs";
        }

        [OperationContract]
        [WebInvoke(Method="POST", RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json, UriTemplate="Me/Password", BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        public void SetPassword(string oldpassword, string newpassword) 
        {
            User u = new User();
            u.Authenticate(HttpContext.Current.User.Identity.Name, oldpassword);
            u.ChangePassword(oldpassword, newpassword);
            HttpCookie tokenCookie = new HttpCookie("token", TokenGenerator.ConvertToToken(newpassword));
            if (HttpContext.Current.Request.Cookies["token"] == null) HttpContext.Current.Response.AppendCookie(tokenCookie);
            else HttpContext.Current.Response.SetCookie(tokenCookie);
        }
    }
}