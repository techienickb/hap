using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Reflection;

namespace HAP.Web.Configuration
{
    public class hapConfig
    {
        public static string ConfigPath { get { return HttpContext.Current.Server.MapPath("~/app_data/hapconfig.xml"); } }
        private XmlDocument doc;

        public static hapConfig Current { 
            get 
            {
                if (HttpContext.Current.Cache.Get("hapConfig") == null)
                    HttpContext.Current.Cache.Insert("hapConfig", new hapConfig(), new System.Web.Caching.CacheDependency(HttpContext.Current.Server.MapPath("~/app_data/hapconfig.xml")));
                return (hapConfig)HttpContext.Current.Cache.Get("hapConfig");
            }
        }
        public hapConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                StreamWriter sw = File.CreateText(ConfigPath);
                sw.WriteLine("<?xml version=\"1.0\"?>");
                sw.WriteLine("<hapConfig version=\"" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\" firstrun=\"True\" />");
                sw.Close();
                sw.Dispose();
            }
            doc = new XmlDocument();
            doc.Load(ConfigPath);
            if (Version.Parse(doc.SelectSingleNode("/hapConfig").Attributes["version"].Value).CompareTo(Assembly.GetExecutingAssembly().GetName().Version) == -1) doUpgrade(Version.Parse(doc.SelectSingleNode("/hapConfig").Attributes["version"].Value));
            AD = new AD(ref doc);
            Homepage = new Homepage(ref doc);
            ProxyServer = new ProxyServer(ref doc);
            SMTP = new SMTP(ref doc);
            Tracker = new Tracker(ref doc);
            School = new School(ref doc);
            BookingSystem = new BookingSystem(ref doc);
            MySchoolComputerBrowser = new MySchoolComputerBrowser(ref doc);
        }

        public bool FirstRun
        {
            get { return bool.Parse(doc.SelectSingleNode("/hapConfig").Attributes["firstrun"].Value); }
            set { doc.SelectSingleNode("/hapConfig").Attributes["firstrun"].Value = value.ToString(); }
        }

        public AD AD { get; private set; }
        public Homepage Homepage { get; private set; }
        public ProxyServer ProxyServer { get; private set; }
        public SMTP SMTP { get; private set; }
        public School School { get; private set; }
        public Tracker Tracker { get; private set; }
        public BookingSystem BookingSystem { get; private set; }
        public MySchoolComputerBrowser MySchoolComputerBrowser { get; private set; }

        private void doUpgrade(Version version) {
            if (version.CompareTo(Version.Parse("7.1")) == -1)
            {//Perform v7.0 to v7.1 upgrade
                doc.SelectSingleNode("/hapConfig/mscb").Attributes["hideextensions"].Value = doc.SelectSingleNode("/hapConfig/mscb").Attributes["hideextensions"].Value.Replace(';', ',');
            }
            if (version.CompareTo(Version.Parse("7.3")) == -1)
            {//Perform v7.3 upgrade
                if (doc.SelectSingleNode("/hapConfig/Homepage/Links/Group/Link[@name='Browse My School Computer']") != null)
                    doc.SelectSingleNode("/hapConfig/Homepage/Links/Group/Link[@name='Browse My School Computer']").Attributes["name"].Value = "My School Files";
                if (doc.SelectSingleNode("/hapConfig/Homepage/Links/Group/Link[@name='Access a School Computer']") != null)
                    doc.SelectSingleNode("/hapConfig/Homepage/Links/Group/Link[@name='Access a School Computer']").Attributes["name"].Value = "Remote Apps";
                if (doc.SelectSingleNode("/hapConfig/Homepage/Links/Group/Link[@name='RM Management Console']") != null)
                    doc.SelectSingleNode("/hapConfig/Homepage/Links/Group/Link[@name='RM Management Console']").Attributes["name"].Value = "RM Console";
            } 
            if (version.CompareTo(Version.Parse("7.4")) == -1)
            {//Perform v7.4 upgrade
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/Homepage/Links/Group"))
                {
                    XmlAttribute a = doc.CreateAttribute("subtitle");
                    a.Value = "";
                    n.Attributes.Append(a);
                }
            }
            if (version.CompareTo(Version.Parse("7.7")) == -1)
            {//Perform v7.7 Upgrade
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/AD/OUs/OU")) {
                    XmlAttribute a = doc.CreateAttribute("visibility");
                    a.Value = bool.Parse(n.Attributes["ignore"].Value) ? OUVisibility.None.ToString() : OUVisibility.Both.ToString();
                    n.Attributes.Append(a);
                    n.Attributes.Remove(n.Attributes["ignore"]);
                }
            }
            doc.SelectSingleNode("hapConfig").Attributes["version"].Value = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            doc.Save(ConfigPath);
        }

        public void Save()
        {
            FirstRun = false;
            doc.Save(ConfigPath);
        }
    }
}
