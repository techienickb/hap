using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class Homepage
    {
        private XmlDocument doc;
        public Homepage(ref XmlDocument doc)
        {
            this.doc = doc;
            if (doc.SelectSingleNode("/hapConfig/Homepage") == null) Initialize();
        }

        public LinkGroups Groups { get { return new LinkGroups(ref doc); } }
        public AnnouncementBox AnnouncementBox { get { return new AnnouncementBox(ref doc); } }

        public void Initialize()
        {
            XmlElement e = doc.CreateElement("Homepage");
            e.AppendChild(doc.CreateElement("Links"));
            doc.SelectSingleNode("/hapConfig").AppendChild(e);

            Groups.Add("Resources", "All", "");
            Groups["Resources"].Add("Me", "Inherit", "About Me and Change My Password", "#me", "~/images/icons/metro/folders-os/UserNo-Frame.png", "", "me");
            Groups["Resources"].Add("My Files", "Inherit", "Access your School My Files", "~/myfiles/", "~/images/icons/metro/folders-os/DocumentsFolder.png", "", "myfiles");
            Groups["Resources"].Add("Remote Apps", "Inherit", "Run School Applications at Home via School", "/rdweb/", "~/images/icons/remotedesktop.png", "");
            Groups["Resources"].Add("Access My Emails", "Inherit", "Access Email", "https://schoolmail.crickhowell-hs.powys.sch.uk/owa/", "~/images/icons/metro/office/outlook.png", "");
            Groups.Add("Management", "Domain Admins", "");
            Groups["Management"].Add("Help Desk", "Inherit", "Log/View a Support Ticket", "~/helpdesk/", "~/images/icons/metro/folders-os/help.png", "", "helpdesk");
            Groups["Management"].Add("Booking System", "Inherit", "Book an IT Resource", "~/bookingsystem/", "~/images/icons/metro/applications/calendar.png", "", "bookings");
            Groups["Management"].Add("Logon Tracker", "Domain Admins", "View the Logon History", "~/tracker/", "~/images/icons/metro/other/History.png", "");
            Groups["Management"].Add("HAP+ Config", "Domain Admins", "Home Access Plus+ Config", "~/setup.aspx", "~/images/icons/metro/folders-os/Configurealt1.png", "");
            Groups.Add("Me", "All", "#me");
            Groups["Management"].Add("Me", "Inherit", "", "", "", "");
            Groups["Management"].Add("Password", "Inherit", "", "", "", "");
        }
    }
}
