using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.MyFiles.Homework
{
    public class UserNode
    {
        public string Method { get; set; }
        public string Mode { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public static UserNode Parse(XmlNode node)
        {
            UserNode n = new UserNode();
            n.Value = node.InnerText;
            n.Method = node.Name;
            n.Mode = node.Attributes["mode"].Value;
            n.Type = node.Attributes["type"].Value;
            return n;
        }
    }
}
