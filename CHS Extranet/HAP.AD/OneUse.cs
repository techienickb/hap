using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace HAP.AD
{
    public class OneUseCode
    {
        public string Code { get; set; }
        public string Username { get; set; }
        public DateTime Expires { get; set; }
        public string Token { get; set; }
    }

    public class OneUse : Dictionary<string, OneUseCode>
    {
        public OneUse() : base()
        {
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml")))
            {
                StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
                sw.WriteLine("<?xml version=\"1.0\"?>");
                sw.WriteLine("<OneUseCodes />");
                sw.Close();
                sw.Dispose();
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
            foreach (XmlNode n in doc.SelectNodes("/OneUseCodes/Code"))
                this.Add(n.Attributes["code"].Value, new OneUseCode { Code = n.Attributes["code"].Value, Token = n.Attributes["token"].Value, Username = n.Attributes["username"].Value, Expires = DateTime.Parse(n.Attributes["expires"].Value) });
        }


        public static OneUse Current
        {
            get
            {
                if (HttpContext.Current.Cache.Get("oneuse") == null)
                    HttpContext.Current.Cache.Insert("oneuse", new OneUse(), new System.Web.Caching.CacheDependency(HttpContext.Current.Server.MapPath("~/app_data/OneUseCodes.xml")));
                return (OneUse)HttpContext.Current.Cache.Get("oneuse");
            }
        }

        public void AddCode(string code, string token, string username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
            XmlElement e = doc.CreateElement("Code");
            e.SetAttribute("code", code);
            e.SetAttribute("token", token);
            e.SetAttribute("username", username);
            e.SetAttribute("expires", DateTime.Now.AddDays(7).ToString("u"));
            doc.SelectSingleNode("/OneUseCodes").AppendChild(e);
            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
        }

        public void RemoveCode(string code)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
            doc.SelectSingleNode("/OneUseCodes").RemoveChild(doc.SelectSingleNode("/OneUseCodes/Code[@code='" + code + "']"));
            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
        }

        public void RemoveCodes(string username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
            foreach (XmlNode n in doc.SelectNodes("/OneUseCodes/Code[@username='" + username + "']"))
                doc.SelectSingleNode("/OneUseCodes").RemoveChild(n);
            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
        }

        public void RemoveCodes()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
            doc.SelectSingleNode("/OneUseCodes").RemoveAll();
            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
        }

        public void RemoveExpiredCodes()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
            foreach (XmlNode n in doc.SelectNodes("/OneUseCodes"))
                if (DateTime.Parse(n.Attributes["expires"].Value) < DateTime.Now) doc.SelectSingleNode("/OneUseCodes").RemoveChild(n);
            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/OneUseCodes.xml"));
        }

    }
}