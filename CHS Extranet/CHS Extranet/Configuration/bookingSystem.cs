using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace CHS_Extranet.Configuration
{
    public class bookingSystem : ConfigurationElement
    {
        [ConfigurationProperty("lessonsperday", DefaultValue = 5, IsRequired = true)]
        public int LessonsPerDay
        {
            get { return (int)this["lessonsperday"]; }
            set { this["lessonsperday"] = value.ToString(); }
        }

        [ConfigurationProperty("maxbookingsperweek", DefaultValue = 3, IsRequired = true)]
        public int MaxBookingsPerWeek
        {
            get { return (int)this["maxbookingsperweek"]; }
            set { this["maxbookingsperweek"] = value.ToString(); }
        }


        [ConfigurationProperty("maxdays", DefaultValue = 14, IsRequired = true)]
        public int MaxDays
        {
            get { return (int)this["maxdays"]; }
            set { this["maxdays"] = value.ToString(); }
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

        [ConfigurationProperty("lessontimes", DefaultValue = "9:00, 10:00, 11:25, 12:25, 14:30", IsRequired = true)]
        public string LessonTimes
        {
            get { return (string)this["lessontimes"]; }
            set { this["lessontimes"] = value; }
        }

        [ConfigurationProperty("lessonlength", DefaultValue = "1:00", IsRequired = true)]
        public string LessonLength
        {
            get { return (string)this["lessonlength"]; }
            set { this["lessonlength"] = value; }
        }


        public string[] LessonTimesArray
        {
            get { return this.LessonTimes.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries); }
            set { this.LessonTimes = string.Join(", ", value); }
        }
    }
}