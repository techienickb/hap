using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP.Web.Configuration;
using System.DirectoryServices;

namespace HAP.AD
{
    public class UserInfo : IComparable
    {
        public string UserName { get; private set; }
        public string Notes { get; private set; }
        public string DisplayName { get; private set;}
        public string Email { get; private set; }

        public UserInfo(string username, string notes, string displayname, string email)
        {
            this.UserName = username;
            this.Notes = notes;
            this.DisplayName = displayname;
            this.Email = email;
            if (string.IsNullOrEmpty(this.DisplayName)) this.DisplayName = this.UserName;
            if (string.IsNullOrEmpty(this.Notes)) this.Notes = this.DisplayName;
        }

        public int CompareTo(object obj)
        {
            return this.UserName.CompareTo(((UserInfo)obj).UserName);
        }
    }
}
