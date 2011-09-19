using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.Web.Configuration
{
    public class MySchoolComputerBrowser
    {
        private XmlDocument doc;
        public MySchoolComputerBrowser(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/mscb") == null) Initialize();
            KnownIcons = new XmlDocument();
            KnownIcons.Load(HttpContext.Current.Server.MapPath("~/Images/Icons/KnownIcons.xml"));
        }

        public Filters Filters { get { return new Filters(ref doc); } }
        public DriveMappings Mappings { get { return new DriveMappings(ref doc); } }
        public QuotaServers QuotaServers { get { return new QuotaServers(ref doc); } }
        public XmlDocument KnownIcons { get; private set; }
        public string HideExtensions 
        { 
            get { return doc.SelectSingleNode("/hapConfig/mscb").Attributes["hideextensions"].Value; } 
            set { doc.SelectSingleNode("/hapConfig/mscb").Attributes["hideextensions"].Value = value; } 
        }

        public void Initialize()
        {
            XmlElement e = doc.CreateElement("mscb");
            e.SetAttribute("hideextensions", ".lnk,.ini");
            e.AppendChild(doc.CreateElement("mappings"));
            e.AppendChild(doc.CreateElement("filters"));
            e.AppendChild(doc.CreateElement("quotaservers"));
            doc.SelectSingleNode("/hapConfig").AppendChild(e);
            Filters.Add("Access Database", "*.mdb;*.accdb", "All");
            Filters.Add("Excel Documents", "*.xls;*.xlsx;*.xlt;*.xltx", "All");
            Filters.Add("HTML Files", "*.html;*.htm", "All");
            Filters.Add("Images", "*.jpg;*.gif;*.png;*.bmp;*.jpeg", "All");
            Filters.Add("Word Documents", "*.doc;*.docx;*.dotx;*.dot;*.txt;*.rft;*.pdf", "All");
            Filters.Add("ZIP Files", "*.zip", "All");
            Filters.Add("All Files", "*.*", "Domain Admins");

        }
    }
}
