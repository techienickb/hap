using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.Web.Configuration
{
    public class DriveMappings : Dictionary<MappingKey, DriveMapping>
    {
        private XmlDocument doc;
        private XmlNode node;
        public DriveMappings(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/myfiles/mappings");
            foreach (XmlNode n in node.ChildNodes) base.Add(new MappingKey(n.Attributes["drive"].Value.ToCharArray()[0], HttpContext.Current.Server.HtmlDecode(n.InnerText)), new DriveMapping(n));
        }
        public void Add(char Drive, string Name, string UNC, string EnableReadTo, string EnableWriteTo, MappingUsageMode UsageMode)
        {
            XmlElement e = doc.CreateElement("mapping");
            e.SetAttribute("drive", Drive.ToString());
            e.SetAttribute("name", Name);
            e.InnerText = HttpContext.Current.Server.HtmlEncode(UNC);
            e.SetAttribute("enablereadto", EnableReadTo);
            e.SetAttribute("enablewriteto", EnableWriteTo);
            e.SetAttribute("usagemode", UsageMode.ToString());
            doc.SelectSingleNode("/hapConfig/myfiles/mappings").AppendChild(e);
            base.Add(new MappingKey(Drive, UNC), new DriveMapping(e));
        }
        public void Delete(char Drive, string UNC)
        {
            this.Remove(new MappingKey(Drive, UNC));
            node.RemoveChild(node.SelectSingleNode("mapping[@drive='" + Drive + "']"));
        }
        public void Update(char Drive, string UNC, DriveMapping New)
        {
            base.Remove(new MappingKey(Drive, UNC));
            XmlNode e = node.SelectSingleNode("mapping[@drive='" + Drive + "']");
            e.Attributes["name"].Value = New.Name;
            e.Attributes["drive"].Value = New.Drive.ToString();
            e.InnerText = HttpContext.Current.Server.HtmlEncode(New.UNC);
            e.Attributes["enablereadto"].Value = New.EnableReadTo;
            e.Attributes["enablewriteto"].Value = New.EnableWriteTo;
            e.Attributes["usagemode"].Value = New.UsageMode.ToString();
            base.Add(new MappingKey(New.Drive, New.UNC), new DriveMapping(e));
        }

        public Dictionary<char, DriveMapping> FilteredMappings
        {
            get
            {
                Dictionary<char, DriveMapping> mappings = new Dictionary<char, DriveMapping>();
                foreach (DriveMapping mapping in this.Values)
                    if (mapping.EnableReadTo == "All") mappings.Add(mapping.Drive, mapping);
                    else if (mapping.EnableReadTo != "None")
                    {
                        bool vis = false;
                        foreach (string s in mapping.EnableReadTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                        if (vis) mappings.Add(mapping.Drive, mapping);
                    }
                return mappings;
            }
        }
    }
    public class MappingKey
    {
        public char Drive { get; set; }
        public string UNC { get; set; }
        public MappingKey() { }
        public MappingKey(char Drive, string UNC) { this.Drive = Drive; this.UNC = UNC; }
    }
}
