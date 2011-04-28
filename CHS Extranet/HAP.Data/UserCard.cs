using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using HAP.Data;
using System.DirectoryServices;

namespace HAP.Data.UserCard
{
    public class Init
    {
        public Init()
        {
        }
        public Init(string username)
        {
            string ConnStringName = ConfigurationManager.ConnectionStrings[hapConfig.Current.ADSettings.ADConnectionString].ConnectionString;
            DirectoryEntry DirectoryRoot = new DirectoryEntry(ConnStringName, hapConfig.Current.ADSettings.ADUsername, hapConfig.Current.ADSettings.ADPassword);
            string DomainName = HAP.AD.ActiveDirectoryHelper.GetDomainName(ConnStringName);
            string _DomainDN = "";
            if (string.IsNullOrEmpty(ConnStringName))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (ConnStringName.StartsWith("LDAP://"))
                _DomainDN = ConnStringName.Remove(0, ConnStringName.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, hapConfig.Current.ADSettings.ADUsername, hapConfig.Current.ADSettings.ADPassword);
            UserPrincipal up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, username);
            if (HAP.AD.ActiveDirectoryHelper.IsUserInRole(DirectoryRoot, DomainName, username, "Domain Admins", true)) UserLevel = UserCard.UserLevel.Admin;
            else if (HAP.AD.ActiveDirectoryHelper.IsUserInRole(DirectoryRoot, DomainName, username, hapConfig.Current.ADSettings.StudentsGroupName, true)) UserLevel = UserCard.UserLevel.Student;
            else UserLevel = UserCard.UserLevel.Teacher;
            Username = username;
            DisplayName = up.DisplayName;
            EmailAddress = up.EmailAddress;
            try
            {
                HomeDirectory = up.HomeDirectory;
                HomeDrive = up.HomeDrive;
            }
            catch { }
            if (!string.IsNullOrEmpty(up.EmployeeId)) EmployeeID = up.EmployeeId;
            DirectoryEntry usersDE = new DirectoryEntry(ConnStringName, hapConfig.Current.ADSettings.ADUsername, hapConfig.Current.ADSettings.ADPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(sAMAccountName=" + username + ")";
            //ds.Filter = "(sAMAccountName=rmstaff)";
            if (UserLevel == UserCard.UserLevel.Student) ds.PropertiesToLoad.Add("rmCom2000-UsrMgr-uPN");
            ds.PropertiesToLoad.Add("department");
            SearchResult r = ds.FindOne();
            try
            {
                Department = r.Properties["department"][0].ToString();
                if (UserLevel == UserCard.UserLevel.Student && string.IsNullOrEmpty(EmployeeID))
                    EmployeeID = r.Properties["rmCom2000-UsrMgr-uPN"][0].ToString();
            }
            catch { Department = "n/a"; }
        }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string HomeDirectory { get; set; }
        public string HomeDrive { get; set; }
        public string EmailAddress { get; set; }
        public UserLevel UserLevel { get; set; }
        public string EmployeeID { get; set; }
    }

    public enum UserLevel { Admin, Teacher, Student }
}
