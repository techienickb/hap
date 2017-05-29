using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using System.Xml;
using HAP.Web.Configuration;

namespace HAP.Web.API
{
    [ServiceAPI("api/announcement")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Announcement
    {
        [WebInvoke(UriTemplate = "Save", ResponseFormat=WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public bool Save(string content, bool show)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Announcement.xml"));
            XmlNode node = doc.SelectSingleNode("/announcement");
            node.Attributes[0].Value = show.ToString();
            node.InnerXml = string.Format("<![CDATA[ {0} ]]>", HttpUtility.UrlDecode(content, System.Text.Encoding.Default));
            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/Announcement.xml"));
            return true;
        }
    }
}