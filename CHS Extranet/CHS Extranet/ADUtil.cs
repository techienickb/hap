using System;
using System.DirectoryServices;		// Be sure to set a reference to "System.DirectoryServices.dll"

namespace CHSAd			// Change namespace for your project
{

    // Structures for returning user information
    public struct UserInfo
    {
        public string LoginName;
        public string FirstName;
        public string LastName;
    }

    public struct UserInfoEx
    {
        public string LoginName;
        public string FirstName;
        public string LastName;
        public string EmailAddress;
        public string Company;
        public string Department;
        public string DisplayName;
        public string Description;
        public string Notes;
    }

    // Static class containing all the supported user property names
    public class UserProperty
    {
        public static string CommonName = "cn";
        public static string UserName = "sAMAccountName";
        public static string Company = "company";
        public static string Department = "department";
        public static string Description = "description";
        public static string DisplayName = "displayName";
        public static string FirstName = "givenName";
        public static string Email = "mail";
        public static string LastName = "sn";
        public static string Notes = "info";
    }

    // Active Directory Utility Class
    public class ADUtil
    {
        public ADUtil()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Constants
        // *** SECURE CONSTANTS ***
        // Reality Check: In production, these would be stored in a secure are of the registry
        // or another secure location. In production, instead of "Administrator", an account
        // would be created which has ONLY the privileges it needs for the AD operations
        // and no more.

        // Domain Settings:
        const string usersLdapPath = "LDAP://chs01.crickhowell.internal:389/OU=Establishments,DC=crickhowell,DC=internal";
        const string adLoginName = "CRICKHOWELL\\Administrator";
        const string adLoginPassword = "colorado";
        #endregion

        // GetUserCN - given the CMS user string, returns a friendly name for the user
        static public string GetUserCN(string username)
        {
            DirectoryEntry usersDE =
                new DirectoryEntry(usersLdapPath, adLoginName, adLoginPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(sAMAccountName=" + username + ")";
            ds.PropertiesToLoad.Add(UserProperty.FirstName);
            ds.PropertiesToLoad.Add(UserProperty.LastName);
            SearchResult r = ds.FindOne();

            return (r.Properties[UserProperty.FirstName][0].ToString()
                    + " "
                    + r.Properties[UserProperty.LastName][0].ToString());
        }

        // GetUserInfo - given the CMS user string, returns user information
        static public UserInfo GetUserInfo(string username)
        {
            DirectoryEntry usersDE =
                new DirectoryEntry(usersLdapPath, adLoginName, adLoginPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(sAMAccountName=*" + username + ")";
            ds.PropertiesToLoad.Add("cn");
            ds.PropertiesToLoad.Add(UserProperty.UserName);
            ds.PropertiesToLoad.Add(UserProperty.FirstName);
            ds.PropertiesToLoad.Add(UserProperty.LastName);
            SearchResult r = ds.FindOne();

            UserInfo result = new UserInfo();

            result.FirstName = r.Properties[UserProperty.FirstName][0].ToString();
            result.LastName = r.Properties[UserProperty.LastName][0].ToString();
            result.LoginName = r.Properties[UserProperty.UserName][0].ToString();

            return (result);
        }

        // GetUserInfoEx - given the CMS user string, returns user information
        static public UserInfoEx GetUserInfoEx(string username)
        {
            DirectoryEntry usersDE =
                new DirectoryEntry(usersLdapPath, adLoginName, adLoginPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(&(objectClass=user)(anr=" + username + "))";
            ds.Filter = "(sAMAccountName=" + username + ")";
            ds.PropertiesToLoad.Add("cn");
            ds.PropertiesToLoad.Add(UserProperty.UserName);
            ds.PropertiesToLoad.Add(UserProperty.FirstName);
            ds.PropertiesToLoad.Add(UserProperty.LastName);
            ds.PropertiesToLoad.Add(UserProperty.Email);
            ds.PropertiesToLoad.Add(UserProperty.Company);
            ds.PropertiesToLoad.Add(UserProperty.Department);
            ds.PropertiesToLoad.Add(UserProperty.DisplayName);
            ds.PropertiesToLoad.Add(UserProperty.Description);
            ds.PropertiesToLoad.Add(UserProperty.Notes);
            SearchResult r = ds.FindOne();

            UserInfoEx result = new UserInfoEx();

            result.LoginName = r.Properties[UserProperty.UserName][0].ToString();
            if (r.Properties[UserProperty.FirstName].Count == 0)
                result.FirstName = "";
            else if (r.Properties[UserProperty.FirstName] != null)
                result.FirstName = r.Properties[UserProperty.FirstName][0].ToString();
            else
                result.FirstName = "";

            if (r.Properties[UserProperty.LastName].Count == 0)
                result.LastName = "";
            else if (r.Properties[UserProperty.LastName] != null)
                result.LastName = r.Properties[UserProperty.LastName][0].ToString();
            else
                result.LastName = "";

            if (r.Properties[UserProperty.Email].Count == 0)
                result.EmailAddress = "";
            else if (r.Properties[UserProperty.Email] != null)
                result.EmailAddress = r.Properties[UserProperty.Email][0].ToString();
            else
                result.EmailAddress = "";

            if (r.Properties[UserProperty.Company].Count == 0)
                result.Company = ""; 
            else if (r.Properties[UserProperty.Company] != null)
                result.Company = r.Properties[UserProperty.Company][0].ToString();
            else
                result.Company = "";

            if (r.Properties[UserProperty.Department].Count == 0)
                result.Department = ""; 
            else if (r.Properties[UserProperty.Department] != null)
                result.Department = r.Properties[UserProperty.Department][0].ToString();
            else
                result.Department = "";

            if (r.Properties[UserProperty.Description].Count == 0)
                result.Description = ""; 
            else if (r.Properties[UserProperty.Description] != null)
                result.Description = r.Properties[UserProperty.Description][0].ToString();
            else
                result.Description = "";

            if (r.Properties[UserProperty.DisplayName].Count == 0)
                result.DisplayName = ""; 
            else if (r.Properties[UserProperty.DisplayName] != null)
                result.DisplayName = r.Properties[UserProperty.DisplayName][0].ToString();
            else
                result.DisplayName = "";

            if (r.Properties[UserProperty.Notes].Count == 0)
                result.Notes = "";
            else if (r.Properties[UserProperty.Notes] != null)
                result.Notes = r.Properties[UserProperty.Notes][0].ToString();
            else
                result.Notes = "";

            return (result);
        }

        // UpdateUserProperty - Updates a property for the AD User
        static public void UpdateUserProperty(string username, string propertyName, string propertyValue)
        {
            // First, get a DE for the user
            DirectoryEntry userContainerDE =
                new DirectoryEntry(usersLdapPath, adLoginName, adLoginPassword);
            DirectorySearcher ds = new DirectorySearcher(userContainerDE);
            ds.Filter = "(sAMAccountName=" + username + ")";
            ds.PropertiesToLoad.Add("cn");
            SearchResult r = ds.FindOne();
            DirectoryEntry theUserDE = new DirectoryEntry(r.Path, adLoginName, adLoginPassword);

            // Now update the property setting
            if (theUserDE.Properties[propertyName].Count == 0)
            {
                theUserDE.Properties[propertyName].Add(propertyValue);
            }
            else
            {
                theUserDE.Properties[propertyName][0] = propertyValue;
            }
            theUserDE.CommitChanges();
        }

        // FindUsers - Returns all users matching a pattern
        static public UserInfo[] FindUsers(string username)
        {
            UserInfo[] results;

            DirectoryEntry usersDE =
                new DirectoryEntry(usersLdapPath, adLoginName, adLoginPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(&(objectClass=user)(sAMAccountName=" + username + "*))";
            ds.PropertiesToLoad.Add(UserProperty.UserName);
            ds.PropertiesToLoad.Add(UserProperty.FirstName);
            ds.PropertiesToLoad.Add(UserProperty.LastName);

            SearchResultCollection sr = ds.FindAll();

            results = new UserInfo[sr.Count];

            for (int i = 0; i < sr.Count; i++)
            {
                results[i].LoginName = sr[i].Properties[UserProperty.UserName][0].ToString();
                if (sr[i].Properties[UserProperty.FirstName] != null)
                {
                    results[i].FirstName = sr[i].Properties[UserProperty.FirstName][0].ToString();
                }
                else
                {
                    results[i].FirstName = "";
                }
                if (sr[i].Properties[UserProperty.LastName] != null)
                {
                    results[i].LastName = sr[i].Properties[UserProperty.LastName][0].ToString();
                }
                else
                {
                    results[i].LastName = "";
                }
            }

            return (results);
        }

    }
}
