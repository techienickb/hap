using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.Web.Configuration
{
    public class DriveMappings : Dictionary<char, DriveMapping>
    {
        private XmlDocument doc;
        private XmlNode node;
        public DriveMappings(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/myfiles/mappings");
            foreach (XmlNode n in node.ChildNodes) base.Add(n.Attributes["drive"].Value.ToCharArray()[0], new DriveMapping(n));
        }
        public void Add(char Drive, string Name, string UNC, string EnableReadTo, string EnableWriteTo, bool EnableMove, MappingUsageMode UsageMode)
        {
            XmlElement e = doc.CreateElement("mapping");
            e.SetAttribute("drive", Drive.ToString());
            e.SetAttribute("name", Name);
            e.InnerText = HttpContext.Current.Server.HtmlEncode(UNC);
            e.SetAttribute("enablereadto", EnableReadTo);
            e.SetAttribute("enablewriteto", EnableWriteTo);
            e.SetAttribute("enablemove", EnableMove.ToString());
            e.SetAttribute("usagemode", UsageMode.ToString());
            doc.SelectSingleNode("/hapConfig/myfiles/mappings").AppendChild(e);
            base.Add(Drive, new DriveMapping(e));
        }
        public void Delete(char Drive)
        {
            this.Remove(Drive);
            node.RemoveChild(node.SelectSingleNode("mapping[@drive='" + Drive + "']"));
        }
        public void Update(char Drive, DriveMapping New)
        {
            base.Remove(Drive);
            XmlNode e = node.SelectSingleNode("mapping[@drive='" + Drive + "']");
            e.Attributes["name"].Value = New.Name;
            e.Attributes["drive"].Value = New.Drive.ToString();
            e.InnerText = HttpContext.Current.Server.HtmlEncode(New.UNC);
            e.Attributes["enablereadto"].Value = New.EnableReadTo;
            e.Attributes["enablewriteto"].Value = New.EnableWriteTo;
            e.Attributes["enablemove"].Value = New.EnableMove.ToString();
            e.Attributes["usagemode"].Value = New.UsageMode.ToString();
            base.Add(New.Drive, new DriveMapping(e));
        }
    }
}
