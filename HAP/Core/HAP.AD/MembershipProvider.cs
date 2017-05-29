using HAP.Web.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;


namespace HAP.AD
{
    /// <summary>
    /// Summary description for MembershipProvider
    /// </summary>
    public class MembershipProvider : System.Web.Security.MembershipProvider
    {
        public MembershipProvider()
        {
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (HttpContext.Current.Cache["usercache-" + username] == null) HttpContext.Current.Cache.Insert("usercache-" + username, new User(username), null, DateTime.Now.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
            return HttpContext.Current.Cache["usercache-" + username] as User;
        }

        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Web.Security.MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(System.Web.Security.MembershipUser user)
        {
            ((HAP.AD.User)user).Save();
        }

        public override bool ValidateUser(string username, string password)
        {
            try
            {
                User u = new User();
                u.Authenticate(username, password);
                var config = System.Web.Configuration.WebConfigurationManager.GetSection("system.web/authorization") as AuthorizationSection;
                foreach (AuthorizationRule rule in config.Rules)
                    if (rule.Action == AuthorizationRuleAction.Deny)
                    {
                        if (rule.Roles != null) foreach (string s in rule.Roles) if (s != "?" && new RoleProvider().IsUserInRole(u.UserName, s)) throw new UnauthorizedAccessException();
                        if (rule.Users != null) foreach (string s in rule.Users) if (s != "*" && u.UserName.ToLower().Equals(s)) throw new UnauthorizedAccessException();
                    }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ApplicationName { get; set; }
    }
}