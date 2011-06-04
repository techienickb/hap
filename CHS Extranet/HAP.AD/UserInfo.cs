using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.AD
{
    // Structures for returning user information
    public struct UserInfo : IComparable
    {
        public string LoginName;
        public string DisplayName;
        public string Notes;
        public string EmailAddress;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return LoginName.CompareTo(((UserInfo)obj).LoginName);
        }

        #endregion
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
}
