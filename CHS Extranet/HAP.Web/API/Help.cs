using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Xml;
using HAP.Web.Configuration;
using HAP.AD;

namespace HAP.Web.API
{
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Help
    {
        [OperationContract]
        [WebGet(UriTemplate = "{*Path}", ResponseFormat = WebMessageFormat.Json)]
        public string Get(string Path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_LocalResources/" + hapConfig.Current.Local + "/help.xml"));
            string am = "Forms";
            try { TokenGenerator.ConvertToPlain(HttpContext.Current.Request.Cookies["token"].Value); }
            catch { am = HttpContext.Current.Request.Cookies["token"].Value; }
            return doc.SelectSingleNode("/resources/" + Path.ToLower()).InnerText.Replace("%am", am).Replace("%l", "~/login.aspx?ReturnUrl=~/&From=" + am).Replace("~/", VirtualPathUtility.ToAbsolute("~/"));
        }
    }
}