using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using HAP.Web.Configuration;
using System.Web.Security;
using HAP.Data.Tracker;
using System.Management;

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
            else HAP.Data.SQL.Tracker.Clear(Computer, DomainName);
        }

        [WebMethod]
        public LogonsList Logon(string Username, string Computer, string DomainName, string IP, string LogonServer, string os)
        {
            LogonsList ll = new LogonsList();
            if (hapConfig.Current.Tracker.Provider == "XML") ll.Logons = xml.Logon(Username, Computer, DomainName, IP, LogonServer, os);
            else ll.Logons = HAP.Data.SQL.Tracker.Logon(Username, Computer, DomainName, IP, LogonServer, os);
            ll.OverrideCode = hapConfig.Current.Tracker.OverrideCode;
            ll.MaxLogons = isAdmin(Username) ? 0 : isStudent(Username) ? hapConfig.Current.Tracker.MaxStudentLogons : hapConfig.Current.Tracker.MaxStaffLogons;
            ll.UserType = isAdmin(Username) ? UT.Admin : isStudent(Username) ? UT.Student : UT.Staff;
            return ll;
        }

        [WebMethod]
        public void RemoteLogoff(string Computer, string DomainName)
        {
            try
            {
                ConnectionOptions connoptions = new ConnectionOptions();
                connoptions.Username = hapConfig.Current.AD.User;
                connoptions.Password = hapConfig.Current.AD.Password;
                ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", Computer), connoptions);
                scope.Connect();
                ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
                ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
                foreach (ManagementObject o in q.Get())
                    o.InvokeMethod("Win32Shutdown", new object[] { 4 });
            }
            catch { }
            if (hapConfig.Current.Tracker.Provider == "XML") xml.Clear(Computer, DomainName);
            else HAP.Data.SQL.Tracker.Clear(Computer, DomainName);
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
                if (s == hapConfig.Current.AD.StudentsGroup) return true;
            return false;
        }
    }
}
