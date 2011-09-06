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

        public Tabs Tabs { get { return new Tabs(ref doc); } }
        public LinkGroups Groups { get { return new LinkGroups(ref doc); } }
        public AnnouncementBox AnnouncementBox { get { return new AnnouncementBox(ref doc); } }

        public void Initialize()
        {
            XmlElement e = doc.CreateElement("Homepage");
            e.AppendChild(doc.CreateElement("Links"));
            e.AppendChild(doc.CreateElement("Tabs"));
            doc.SelectSingleNode("/hapConfig").AppendChild(e);

            Tabs.Add("Me", "All", TabType.Me, "Domain Admins", true);
            Tabs.Add("Password", "All", TabType.Password);
            Tabs.Add("Bookings", "CHS Teaching Staff, CHS Non-Teach Staff, Domain Admins", TabType.Bookings);
            Tabs.Add("Tickets", "CHS Teaching Staff, CHS Non-Teach Staff, Domain Admins", TabType.Tickets);
            Groups.Add("Resources", "All");
            Groups["Resources"].Add("Browse My School Computer", "Inherit", "Access your School My Documents", "~/mycomputer.aspx", "~/images/icons/net.png", "");
            Groups["Resources"].Add("Access a School Computer", "Inherit", "Run School Applications at Home", "/rdweb/", "~/images/icons/remotedesktop.png", "");
            Groups["Resources"].Add("Access My Webmail", "Domain Admins, CHS Teaching Staff, CHS Non-Teach Staff", "Access Student Mail", "https://schoolmail.crickhowell-hs.powys.sch.uk/webmail/", "~/images/icons/email.png", "");
            Groups["Resources"].Add("Access My Emails", "CHS Students", "Access Outlook Web App", "https://schoolmail.crickhowell-hs.powys.sch.uk/owa/", "~/images/icons/email.png", "");
            Groups.Add("Management", "CHS Teaching Staff, CHS Non-Teach Staff, Domain Admins");
            Groups["Management"].Add("Help Desk", "Inherit", "Log/View a Support Ticket", "~/helpdesk/", "~/images/icons/helpdesk.png", "");
            Groups["Management"].Add("Booking System", "Inherit", "Book an IT Resource", "~/bookingsystem/", "~/images/icons/bookingsystem.png", "");
            Groups["Management"].Add("RM Management Console", "Domain Admins", "RM Management Console", "/authorise/", "~/images/icons/rm.png", "");
            Groups["Management"].Add("Logon Tracker", "Domain Admins", "View the Logon History", "~/tracker/", "~/images/icons/tracker.png", "");
        }
    }
}
