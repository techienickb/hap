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
            DirectoryEntry DirectoryRoot = HAP.AD.ADUtils.GetDirectoryRoot();
            PrincipalContext pcontext = HAP.AD.ADUtils.GetPContext();
            UserPrincipal up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, username);
            UserLevel = UserCard.UserLevel.Teacher;
            try
            {
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, "Domain Admins");
                if (up.IsMemberOf(gp)) UserLevel = UserCard.UserLevel.Admin;
            }
            catch { }
            try
            {
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, hapConfig.Current.AD.StudentsGroup);
                if (up.IsMemberOf(gp)) UserLevel = UserCard.UserLevel.Student;
            }
            catch { }
            
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
            DirectoryEntry usersDE = DirectoryRoot;
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
