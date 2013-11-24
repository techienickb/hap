using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using System.Xml;
using HAP.AD;
using HAP.Web.Configuration;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web.Security;
using HAP.Data.Tracker;
using System.Management;

namespace HAP.Tracker
{
    [ServiceAPI("api/tracker")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class API
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/Clear", Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public int Clear(string Computer, string DomainName)
        {
            if (hapConfig.Current.Tracker.Provider == "XML") xml.Clear(Computer, DomainName);
            else HAP.Data.SQL.Tracker.Clear(Computer, DomainName);
            return 0;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "/Logon", Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public LogonsList Logon(string Username, string Computer, string DomainName, string IP, string LogonServer, string os)
        {
            LogonsList ll = new LogonsList();
            if (hapConfig.Current.Tracker.Provider == "XML") ll.Logons = trackerlogentrysmall.Convert(xml.Logon(Username, Computer, DomainName, IP, LogonServer, os));
            else ll.Logons = trackerlogentrysmall.Convert(HAP.Data.SQL.Tracker.Logon(Username, Computer, DomainName, IP, LogonServer, os));
            ll.OverrideCode = hapConfig.Current.Tracker.OverrideCode;
            ll.MaxLogons = isAdmin(Username) ? 0 : isStudent(Username) ? hapConfig.Current.Tracker.MaxStudentLogons : hapConfig.Current.Tracker.MaxStaffLogons;
            ll.UserType = isAdmin(Username) ? UT.Admin : isStudent(Username) ? UT.Student : UT.Staff;
            return ll;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "/Poll", Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public LogonsList Poll(string Username, string Computer, string DomainName)
        {
            LogonsList ll = new LogonsList();
            if (hapConfig.Current.Tracker.Provider == "XML") ll.Logons = trackerlogentrysmall.Convert(xml.Poll(Username, Computer, DomainName));
            else ll.Logons = trackerlogentrysmall.Convert(HAP.Data.SQL.Tracker.Poll(Username, Computer, DomainName));
            ll.OverrideCode = hapConfig.Current.Tracker.OverrideCode;
            ll.MaxLogons = isAdmin(Username) ? 0 : isStudent(Username) ? hapConfig.Current.Tracker.MaxStudentLogons : hapConfig.Current.Tracker.MaxStaffLogons;
            ll.UserType = isAdmin(Username) ? UT.Admin : isStudent(Username) ? UT.Student : UT.Staff;
            return ll;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "/RemoteLogoff", Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public int RemoteLogoff(string Computer, string DomainName)
        {
            try
            {
                ConnectionOptions connoptions = new ConnectionOptions();
                connoptions.Username = hapConfig.Current.AD.User;
                connoptions.Password = hapConfig.Current.AD.Password;
                connoptions.EnablePrivileges = true;
                ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", Computer), connoptions);
                scope.Connect();
                ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
                ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
                foreach (ManagementObject o in q.Get())
                    o.InvokeMethod("Win32Shutdown", new object[] { 4 });
            }
            catch { }
            return Clear(Computer, DomainName);
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