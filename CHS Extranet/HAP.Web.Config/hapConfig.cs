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
                sw.WriteLine("<hapConfig version=\"" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\" local=\"en-GB\" firstrun=\"True\" />");
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
            MyFiles = new MyFiles(ref doc);
            HelpDesk = new HelpDesk(ref doc);
        }

        public bool FirstRun
        {
            get { return bool.Parse(doc.SelectSingleNode("/hapConfig").Attributes["firstrun"].Value); }
            set { doc.SelectSingleNode("/hapConfig").Attributes["firstrun"].Value = value.ToString(); }
        }

        public string Local
        {
            get { return doc.SelectSingleNode("/hapConfig").Attributes["local"].Value; }
            set { doc.SelectSingleNode("/hapConfig").Attributes["local"].Value = value.ToString(); }
        }

        public AD AD { get; private set; }
        public Homepage Homepage { get; private set; }
        public ProxyServer ProxyServer { get; private set; }
        public SMTP SMTP { get; private set; }
        public School School { get; private set; }
        public Tracker Tracker { get; private set; }
        public BookingSystem BookingSystem { get; private set; }
        public HelpDesk HelpDesk { get; private set; }
        public MyFiles MyFiles { get; private set; }

        private void doUpgrade(Version version) {
            if (version.CompareTo(Version.Parse("7.1")) == -1)
            {//Perform v7.0 to v7.1 upgrade
                doc.SelectSingleNode("/hapConfig/myfiles").Attributes["hideextensions"].Value = doc.SelectSingleNode("/hapConfig/myfiles").Attributes["hideextensions"].Value.Replace(';', ',');
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
            if (version.CompareTo(Version.Parse("7.7")) < 0)
            {//Perform v7.7 Upgrade
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/AD/OUs/OU")) {
                    XmlAttribute a = doc.CreateAttribute("visibility");
                    a.Value = bool.Parse(n.Attributes["ignore"].Value) ? OUVisibility.None.ToString() : OUVisibility.Both.ToString();
                    n.Attributes.Append(a);
                    n.Attributes.Remove(n.Attributes["ignore"]);
                }
            }
            if (version.CompareTo(Version.Parse("7.7.1128.2200")) < 0)
            {//Perform v7.7 Upgrade
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/bookingsystem/resources/resource"))
                {
                    XmlAttribute a = doc.CreateAttribute("showto");
                    a.Value = "All";
                    n.Attributes.Append(a);
                }
            }
            if (version.CompareTo(Version.Parse("7.9.0103.1500")) < 0)
            {//Perform v7.9 Upgrade
                XmlAttribute a = doc.CreateAttribute("local");
                a.Value = "en-gb";
                doc.SelectSingleNode("/hapConfig").Attributes.Append(a);
            }
            if (version.CompareTo(Version.Parse("8.0.0527.1300")) < 0)
            {//Perform v8 Upgrade
                XmlAttribute a = doc.CreateAttribute("local");
                a.Value = "en-gb";
                doc.SelectSingleNode("/hapConfig").Attributes.Append(a);
                doc.SelectSingleNode("/hapConfig/Homepage").RemoveChild(doc.SelectSingleNode("/hapConfig/Homepage/Tabs"));
                if (doc.SelectNodes("/hapConfig/Homepage/Group[@name='Me']").Count > 0) doc.SelectSingleNode("/hapConfig/Homepage").RemoveChild(doc.SelectSingleNode("/hapConfig/Homepage/Group[@name='Me']"));
                XmlElement e = doc.CreateElement("Group");
                e.SetAttribute("showto", "All");
                e.SetAttribute("name", "Me");
                e.SetAttribute("subtitle", "#me");
                XmlElement e1 = doc.CreateElement("Link");
                e1.SetAttribute("name", "Me");
                e1.SetAttribute("showto", "Inherit");
                e1.SetAttribute("description", "");
                e1.SetAttribute("url", "");
                e1.SetAttribute("icon", "");
                e1.SetAttribute("target", "");
                e.AppendChild(e1);
                e1 = doc.CreateElement("Link");
                e1.SetAttribute("name", "Password");
                e1.SetAttribute("showto", "Inerhit");
                e1.SetAttribute("description", "");
                e1.SetAttribute("url", "");
                e1.SetAttribute("icon", "");
                e1.SetAttribute("target", "");
                e.AppendChild(e1);
                doc.SelectSingleNode("/hapConfig/Homepage/Links").AppendChild(e);
            }
            if (version.CompareTo(Version.Parse("8.0.0714.1945")) < 0)
            {//Perform v8.0714 Update
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/bookingsystem/resources/resource"))
                {
                    n.Attributes.Append(doc.CreateAttribute("hidefrom"));
                    n.Attributes.Append(doc.CreateAttribute("years"));
                    n.Attributes.Append(doc.CreateAttribute("quantities"));
                    n.Attributes["hidefrom"].Value = "";
                    n.Attributes["quantities"].Value = "";
                    n.Attributes["years"].Value = "Inherit";
                }
            }
            if (version.CompareTo(Version.Parse("8.0.0714.2010")) < 0)
            {//Perform v8.0714.2010 Update
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/bookingsystem/resources/resource"))
                {
                    n.Attributes.Append(doc.CreateAttribute("readonlyto"));
                    n.Attributes.Append(doc.CreateAttribute("readwriteto"));
                    n.Attributes["readwriteto"].Value = n.Attributes["readonlyto"].Value = "";
                }
            }
            if (version.CompareTo(Version.Parse("8.0.0801.2359")) < 0)
            {//Perform v8.0.0801.2359 Update
                doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes.Append(doc.CreateAttribute("enablemultilesson"));
                doc.SelectSingleNode("/hapConfig/bookingsystem").Attributes["enablemultilesson"].Value = "false";
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/bookingsystem/resources/resource"))
                {
                    n.Attributes.Append(doc.CreateAttribute("multilessonto"));
                    n.Attributes["multilessonto"].Value = "None";
                }
            }
            if (version.CompareTo(Version.Parse("8.0.0802.2000")) < 0)
            {//Perform v8.0.0802.2000 Update
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/Homepage/Links/Group/Link[@url='#me']"))
                {
                    n.Attributes["icon"].Value = "~/images/icons/metro/folders-os/UserNo-Frame.png";
                    XmlAttribute a = doc.CreateAttribute("type");
                    a.Value = "me";
                    n.Attributes.Append(a);
                }
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/Homepage/Links/Group/Link[@url='~/myfiles/']"))
                {
                    n.Attributes["icon"].Value = "~/images/icons/metro/folders-os/DocumentsFolder.png";
                    XmlAttribute a = doc.CreateAttribute("type");
                    a.Value = "myfiles";
                    n.Attributes.Append(a);
                }
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/Homepage/Links/Group/Link[@url='~/helpdesk/']"))
                {
                    n.Attributes["icon"].Value = "~/images/icons/metro/folders-os/help.png";
                    XmlAttribute a = doc.CreateAttribute("type");
                    a.Value = "helpdesk";
                    n.Attributes.Append(a);
                }
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/Homepage/Links/Group/Link[@url='~/bookingsystem/']"))
                {
                    n.Attributes["icon"].Value = "~/images/icons/metro/applications/calendar.png";
                    XmlAttribute a = doc.CreateAttribute("type");
                    a.Value = "bookings";
                    n.Attributes.Append(a);
                }
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/Homepage/Links/Group/Link[@url='~/tracker/']"))
                    n.Attributes["icon"].Value = "~/images/icons/metro/other/History.png";
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/Homepage/Links/Group/Link[@url='~/setup.aspx']"))
                    n.Attributes["icon"].Value = "~/images/icons/metro/folders-os/Configurealt1.png";
            }
            if (version.CompareTo(Version.Parse("8.5.1121.1800")) < 0)//Perform v8.5 upgrade, rename mscb to myfiles
            {
                XmlElement oldElement = (XmlElement)doc.SelectSingleNode("/hapConfig/mscb");
                if (oldElement != null)
                {
                    XmlElement newElement = doc.CreateElement("myfiles");
                    while (oldElement.HasAttributes) newElement.SetAttributeNode(oldElement.RemoveAttributeNode(oldElement.Attributes[0]));
                    while (oldElement.HasChildNodes) newElement.AppendChild(oldElement.FirstChild);
                    if (oldElement.ParentNode != null) oldElement.ParentNode.ReplaceChild(newElement, oldElement);
                }
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/Homepage/Links/Group"))
                {
                    XmlElement en = n as XmlElement;
                    en.SetAttribute("hidehomepage", "False");
                    en.SetAttribute("hidetopmenu", "False");
                    en.SetAttribute("hidehomepagelink", "False");
                }
            }
            if (version.CompareTo(Version.Parse("8.5.1202.0000")) < 0)//Perform v8.5 upgrade, add dfstarget
            {
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/myfiles/quotaservers")) ((XmlElement)n).SetAttribute("dfstarget", "");
            }
            if (version.CompareTo(Version.Parse("9.0.0215.1930")) < 0) //Perform v9 upgrade
            {
                XmlElement el = doc.SelectSingleNode("/hapConfig/SMTP") as XmlElement;
                if (el.Attributes["impersonationuser"] == null)
                {
                    el.SetAttribute("impersonationuser", "");
                    el.SetAttribute("impersonationpassword", "");
                    el.SetAttribute("impersonationdomain", "");
                }

                foreach (XmlNode n in doc.SelectNodes("/hapConfig/Homepage/Links/Group/Link"))
                {
                    XmlElement en = n as XmlElement;
                    en.SetAttribute("width", "1");
                    en.SetAttribute("height", "1");
                    if (en.Attributes["type"] != null && (en.GetAttribute("type") == "exchange.appointments" || en.GetAttribute("type").StartsWith("exchange.calendar") || en.GetAttribute("type") == "helpdesk" || en.GetAttribute("type") == "bookings"))
                        en.SetAttribute("width", "2");
                }
            }
            if (version.CompareTo(Version.Parse("9.0.0315.1900")) < 0) //Perform v9 upgrade
            {
                ((XmlElement)doc.SelectSingleNode("/hapConfig/AD")).SetAttribute("usenestedlookups", "True");
                ((XmlElement)doc.SelectSingleNode("/hapConfig/AD")).SetAttribute("maxlogonattempts", "4");
            }
            if (version.CompareTo(Version.Parse("9.0.0328.1900")) < 0) //Perform v9 upgrade
            {
                ((XmlElement)doc.SelectSingleNode("/hapConfig/AD")).SetAttribute("maxrecursions", "10");
                ((XmlElement)doc.SelectSingleNode("/hapConfig/HelpDesk")).SetAttribute("firstlineemails", doc.SelectSingleNode("/hapConfig/SMTP").Attributes["fromaddress"].Value);
            }
            if (version.CompareTo(Version.Parse("9.2.0527.0000")) < 0) //Perform v9.2 upgrade
            {
                foreach (XmlNode n in doc.SelectNodes("/hapConfig/bookingsystem/resources/resource"))
                {
                    n.Attributes.Append(doc.CreateAttribute("canshare"));
                    n.Attributes["canshare"].Value = "False";
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
