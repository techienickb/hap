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

        [ConfigurationProperty("homepage", IsDefaultCollection = false)]
        public homepage HomePage
        {
            get { return (homepage)base["homepage"]; }
        }

        [ConfigurationProperty("tracker")]
        public tracker Tracker
        {
            get { return (tracker)this["tracker"]; }
        }

        [ConfigurationProperty("announcementbox")]
        public announcementBox AnnouncementBox
        {
            get { return (announcementBox)this["announcementbox"]; }
        }

        [ConfigurationProperty("mycomputer", IsRequired = false)]
        public mycomputer MyComputer
        {
            get { return (mycomputer)this["mycomputer"]; }
        }

        [ConfigurationProperty("bookingsystem", IsRequired = false)]
        public bookingSystem BookingSystem
        {
            get { return (bookingSystem)this["bookingsystem"]; }
        }

        public static hapConfig Current { get { return ConfigurationManager.GetSection("hapConfig") as hapConfig; } }

    }
}