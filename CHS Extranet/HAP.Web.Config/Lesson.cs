using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class Lesson
    {
        private XmlNode node;
        public Lesson(XmlNode node)
        {
            this.node = node;
            Name = node.Attributes["name"].Value;
            Type = (LessonType)Enum.Parse(typeof(LessonType), node.Attributes["type"].Value);
            StartTime = DateTime.Parse(node.Attributes["starttime"].Value);
            EndTime = DateTime.Parse(node.Attributes["endtime"].Value);
        }

        public string Name { get; set; }
        public LessonType Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public enum LessonType { Lesson, Break, Lunch }
}
