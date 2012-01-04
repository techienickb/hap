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
    public class Help
    {
        [OperationContract]
        [WebGet(UriTemplate="{*Path}")]
        public string Get(string Path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_LocalResources/" + hapConfig.Current.Local + "/help.xml"));
            return doc.SelectSingleNode("/resources/" + Path.ToLower()).InnerText.Replace("~/", VirtualPathUtility.ToAbsolute("~/"));
        }
    }
}