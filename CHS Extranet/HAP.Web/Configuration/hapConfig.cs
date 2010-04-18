using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HAP.Web.Configuration
{
    public class hapConfig : ConfigurationSection
    {
        [ConfigurationProperty("adsettings")]
        public adSettings ADSettings
        {
            get { return (adSettings)this["adsettings"]; }
        }

        [ConfigurationProperty("basesettings")]
        public baseSettings BaseSettings
        {
            get { return (baseSettings)this["basesettings"]; }
        }

        [ConfigurationProperty("uncpaths", IsDefaultCollection = false)]
        public uncpaths UNCPaths
        {
            get { return (uncpaths)base["uncpaths"]; }
        }

        [ConfigurationProperty("homepagelinks", IsDefaultCollection = false)]
        public homepagelinks HomePageLinks
        {
            get { return (homepagelinks)base["homepagelinks"]; }
        }

        [ConfigurationProperty("uploadfilters", IsDefaultCollection = false)]
        public uploadfilters UploadFilters
        {
            get { return (uploadfilters)base["uploadfilters"]; }
        }

        [ConfigurationProperty("announcementbox")]
        public announcementBox AnnouncementBox
        {
            get { return (announcementBox)this["announcementbox"]; }
        }

        [ConfigurationProperty("bookingsystem", IsRequired = false)]
        public bookingSystem BookingSystem
        {
            get { return (bookingSystem)this["bookingsystem"]; }
        }

        public static hapConfig Current { get { return ConfigurationManager.GetSection("hapConfig") as hapConfig; } }

    }
}