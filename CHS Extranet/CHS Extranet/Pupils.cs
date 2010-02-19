using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Xml;
using CHS.Base64;

namespace CHS_Extranet
{
    public class Pupils
    {
        public static string getPhoto(string upn)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/report.xml"));
            foreach (XmlNode node in doc.SelectNodes("/SuperStarReport/Record"))
            {
                string upn1 = node.SelectSingleNode("UPN").InnerText;
                if (upn1 == upn)
                {
                    string photo = "";
                    if (node.SelectSingleNode("Photo") != null) photo = node.SelectSingleNode("Photo").InnerText;
                    return photo;
                }
            }
            return "";
        }

    }
}
