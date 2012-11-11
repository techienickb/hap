using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using HAP.Web.Configuration;
using System.DirectoryServices;
using System.IO;

namespace HAP.Web.LiveTiles
{
    public class Me
    {
        public static Me GetMe
        {
            get
            {
                return new Me();
            }
        }

        private hapConfig config;

        public Me()
        {
            config = hapConfig.Current;
            User = ((HAP.AD.User)Membership.GetUser());
            Name = User.DisplayName;

            if (string.IsNullOrEmpty(config.School.PhotoHandler) || string.IsNullOrEmpty(User.EmployeeID)) {
                using (DirectorySearcher dsSearcher = new DirectorySearcher())
                {
                    dsSearcher.Filter = "(&(objectClass=user) (cn=" + User.UserName + "))";
                    SearchResult result = dsSearcher.FindOne();

                    using (DirectoryEntry user = new DirectoryEntry(result.Path))
                    {
                        byte[] data = user.Properties["jpegPhoto"].Value as byte[];
                        if (data != null)
                            Photo = "~/api/mypic";
                        else Photo = null;
                    }
                }
            }
            else Photo = string.Format("{0}?UPN={1}", config.School.PhotoHandler, User.EmployeeID);
            Email = User.Email;
            OtherData = new Dictionary<string, string>();
            OtherData.Add("Comment", User.Comment);
            OtherData.Add("Notes", User.Notes);
            OtherData.Add("LastLoginDate", User.LastLoginDate.ToString());
            OtherData.Add("LastName", User.LastName);
            OtherData.Add("FirstName", User.FirstName);
            OtherData.Add("EmployeeID", User.EmployeeID);
            OtherData.Add("DisplayName", User.DisplayName);
            OtherData.Add("MiddleNames", User.MiddleNames);
        }

        public string Name { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> OtherData { get; set; }

        private HAP.AD.User User;
    }
}
