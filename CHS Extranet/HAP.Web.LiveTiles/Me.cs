using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using HAP.Web.Configuration;

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
            if (string.IsNullOrEmpty(config.School.PhotoHandler) || string.IsNullOrEmpty(User.EmployeeID)) Photo = null;
            else Photo = string.Format("{0}?UPN={1}", config.School.PhotoHandler, User.EmployeeID);
            Email = User.Email;
        }

        public string Name { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }

        private HAP.AD.User User;
    }
}
