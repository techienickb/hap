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
            if (!FirstRun && Assembly.GetExecutingAssembly().GetName().Version.CompareTo(Version.Parse(doc.SelectSingleNode("/hapConfig").Attributes["version"].Value)) < 0) doUpgrade(Version.Parse(doc.SelectSingleNode("/hapConfig").Attributes["version"].Value));
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

        private void doUpgrade(Version version) { }

        public void Save()
        {
            FirstRun = false;
            doc.Save(ConfigPath);
        }
    }
}
