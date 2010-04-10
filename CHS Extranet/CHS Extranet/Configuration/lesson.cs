using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace CHS_Extranet.Configuration
{
    public class lesson : ConfigurationElement
    {
        public lesson(string name, string starttime)
        {
            this.Name = name;
            this.StartTime = starttime;
        }

        public lesson() { }
        public lesson(string elementName) { this.Name = elementName; }

        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("starttime", DefaultValue = "9:00", IsRequired = true)]
        public string StartTime
        {
            get { return (string)this["starttime"]; }
            set { this["starttime"] = value; }
        }

        [ConfigurationProperty("endtime", DefaultValue = "10:00", IsRequired = true)]
        public string EndTime
        {
            get { return (string)this["endtime"]; }
            set { this["endtime"] = value; }
        }

        [ConfigurationProperty("type", DefaultValue = "Lesson", IsRequired = false)]
        public lessontype Type
        {
            get { return (lessontype)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("oldid", DefaultValue = -1, IsRequired = false)]
        public int OldID
        {
            get { return (int)this["oldid"]; }
            set { this["oldid"] = value; }
        }
    }

    public enum lessontype { Lesson, Break, Lunch }
}