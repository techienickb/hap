using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class ou
    {
        public ou(XmlNode node)
        {
            Name = node.Attributes["name"].Value;
            Path = node.Attributes["path"].Value;
            OUVisibility vis;
            if (Enum.TryParse<OUVisibility>(node.Attributes["visibility"].Value, out vis)) Visibility = vis; else Visibility = OUVisibility.None;
        }

        public string Name { get; private set; }
        public string Path { get; private set; }
        public OUVisibility Visibility { get; private set; }
    }
    public enum OUVisibility { HelpDesk, BookingSystem, Both, None }
}
