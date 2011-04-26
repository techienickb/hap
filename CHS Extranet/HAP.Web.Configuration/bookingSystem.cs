using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class bookingSystem : ConfigurationElement
    {
        [ConfigurationProperty("maxbookingsperweek", DefaultValue = 3, IsRequired = true)]
        public int MaxBookingsPerWeek
        {
            get { return (int)this["maxbookingsperweek"]; }
            set { this["maxbookingsperweek"] = value.ToString(); }
        }

        [ConfigurationProperty("keepxmlclean", DefaultValue = true, IsRequired = false)]
        public bool KeepXmlClean
        {
            get { return (bool)this["keepxmlclean"]; }
            set { this["keepxmlclean"] = value; }
        }

        [ConfigurationProperty("maxdays", DefaultValue = 14, IsRequired = true)]
        public int MaxDays
        {
            get { return (int)this["maxdays"]; }
            set { this["maxdays"] = value.ToString(); }
        }

        [ConfigurationProperty("admingroups", DefaultValue = "s", IsRequired = false)]
        public string AdminGroups
        {
            get { return (string)this["admingroups"]; }
            set { this["admingroups"] = value.ToString(); }
        }

        [ConfigurationProperty("twoweektimetable", DefaultValue = true, IsRequired = true)]
        public bool TwoWeekTimetable
        {
            get { return (bool)this["twoweektimetable"]; }
            set { this["twoweektimetable"] = value.ToString(); }
        }

        [ConfigurationProperty("resources", IsDefaultCollection = false)]
        public bookingresources Resources
        {
            get { return (bookingresources)base["resources"]; }
        }

        [ConfigurationProperty("lessons", IsDefaultCollection = false)]
        public lessons Lessons
        {
            get { return (lessons)base["lessons"]; }
        }

        [ConfigurationProperty("subjects", IsDefaultCollection = false)]
        public subjects Subjects
        {
            get { return (subjects)base["subjects"]; }
        }
    }
}