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

        [OperationContract]
        [WebInvoke(UriTemplate = "/{Year}/{Month}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public DataResult MonthData(string Year, string Month)
        {
            DataResult res = new DataResult();
            Dictionary<DateTime, int> data = new Dictionary<DateTime, int>();
            trackerlog tlog = new trackerlog(int.Parse(Year), int.Parse(Month));
            int dim = dim = DateTime.DaysInMonth(int.Parse(Year), int.Parse(Month));
            for (int x = 0; x < dim; x++)
            {
                for (int y = 0; y < 24; y++)
                    data.Add(new DateTime(int.Parse(Year), int.Parse(Month), x + 1, y, 0, 0), tlog.Count(t => (t.LogOnDateTime.Day == x + 1) && t.LogOnDateTime.Hour == y));
                //int y = 0;
                //y = tlog.Count(t => t.LogOnDateTime.Day == x + 1);
                //DateTime dt = new DateTime(int.Parse(Year), int.Parse(Month), x + 1, 0, 0, 0);
                //data.Add(dt, y);
            }
            List<object> s = new List<object>();
            foreach (DateTime dt2 in data.Keys)
                s.Add(new object[] { dt2.ToString("yyyy-MM-dd h:mmtt"), data[dt2] });
            res.LineData = s.ToArray();
            List<string[]> l = new List<string[]>();
            foreach (HAP.Data.Tracker.trackerlogentry e in tlog) l.Add(new string[] { e.ComputerName, e.IP, e.UserName, e.DomainName, e.LogonServer, e.OS, e.LogOnDateTime.ToString(), e.LogOffDateTime.HasValue ? e.LogOffDateTime.Value.ToString() : "" });
            res.Data = l.ToArray();
            return res;
        }


        [OperationContract]
        [WebInvoke(UriTemplate = "/Live", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public string[][] LiveLogons()
        {
            trackerlog tlog = trackerlog.Current;
            List<string[]> l = new List<string[]>();
            foreach (HAP.Data.Tracker.trackerlogentry e in tlog) l.Add(new string[] { e.ComputerName, e.IP, e.UserName, e.DomainName, e.LogonServer, e.OS, e.LogOnDateTime.ToString(), e.LogOffDateTime.HasValue ? e.LogOffDateTime.Value.ToString() : "" });
            return l.ToArray();
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

    public class DataResult
    {
        public object[] LineData { get; set; }
        public string[][] Data { get; set; }
    }

}