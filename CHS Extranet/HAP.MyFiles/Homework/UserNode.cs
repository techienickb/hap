using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.MyFiles.Homework
{
    public class UserNode
    {
        public UserNodeMethod Method { get; set; }
        public UserNodeMode Mode { get; set; }
        public UserNodeType Type { get; set; }
        public string Value { get; set; }
        public static UserNode Parse(XmlNode node)
        {
            UserNode n = new UserNode();
            n.Value = node.InnerText;
            UserNodeMode m; UserNodeType t; UserNodeMethod me;
            if (Enum.TryParse(node.Name, true, out me)) n.Method = me;
            if (node.Attributes["mode"] != null && Enum.TryParse(node.Attributes["mode"].Value, true, out m)) n.Mode = m;
            if (Enum.TryParse(node.Attributes["type"].Value, true, out t)) n.Type = t;
            return n;
        }
    }

    public enum UserNodeMethod { Add, Remove }
    public enum UserNodeMode { Student, Teacher, Admin, None }
    public enum UserNodeType { User, OU, Role }
}
