using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Xml;
using HAP.Web.Configuration;

namespace HAP.Web.API
{
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LiveTiles
    {
        [OperationContract]
        [WebGet(UriTemplate="Exchange/Unread")]
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
    }
}