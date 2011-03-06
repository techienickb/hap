using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using HAP.Web.Configuration;
using System.Web.Security;

namespace HAP.Web.Tracker
{
    /// <summary>
    /// Home Access Plus+ Logon Tracker API Service
    /// </summary>
    [WebService(Namespace = "http://hap.codeplex.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class api : System.Web.Services.WebService
    {

        [WebMethod]
        public void Clear(string Computer, string DomainName)
        {
            if (hapConfig.Current.Tracker.Provider == "XML") xml.Clear(Computer, DomainName);
        }

        [WebMethod]
        public LogonsList Logon(string Username, string Computer, string DomainName, string IP, string LogonServer, string os)
        {
            LogonsList ll = new LogonsList();
            if (hapConfig.Current.Tracker.Provider == "XML") ll.Logons = xml.Logon(Username, Computer, DomainName, IP, LogonServer, os);
            ll.OverrideCode = hapConfig.Current.Tracker.OverrideCode;
            ll.MaxLogons = isAdmin(Username) ? 0 : isStudent(Username) ? hapConfig.Current.Tracker.MaxStudentLogons : hapConfig.Current.Tracker.MaxStaffLogons;
            ll.UserType = isAdmin(Username) ? UT.Admin : isStudent(Username) ? UT.Student : UT.Staff;
            return ll;
        }

        [WebMethod]
        public void RemoteLogoff(string Computer, string DomainName)
        {
            if (hapConfig.Current.Tracker.Provider == "XML") xml.RemoteLogoff(Computer, DomainName);
        }
        
        bool isAdmin(string username)
        {
            foreach (string s in Roles.GetRolesForUser(username))
                if (s == "Domain Admins") return true;
            return false;
        }

        bool isStudent(string username)
        {
            foreach (string s in Roles.GetRolesForUser(username))
                if (s == hapConfig.Current.ADSettings.StudentsGroupName) return true;
            return false;
        }
    }
}
