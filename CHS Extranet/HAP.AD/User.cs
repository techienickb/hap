using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.DirectoryServices;


namespace HAP.AD
{
    /// <summary>
    /// Summary description for HAP
    /// </summary>
    public class User : ActiveDirectoryMembershipUser, IComparable
    {
        public User()
        {
        }

        public User(string username)
        {
            this.UserName = username;
            this.DomainName = HAP.Web.Configuration.hapConfig.Current.AD.UPN;
            UserPrincipal userp = UserPa;
            this.UserName = userp.SamAccountName;
            this.Comment = userp.Description;
            this.DisplayName = userp.DisplayName;
            this.EmployeeID = userp.EmployeeId;
            this.Email = userp.EmailAddress;
            this.LastLoginDate = userp.LastLogon.HasValue ? userp.LastLogon.Value : DateTime.Now;
            this.IsLockedOut = userp.IsAccountLockedOut();
            this.IsApproved = userp.Enabled.HasValue ? userp.Enabled.Value : false;
            this.HomeDirectory = userp.HomeDirectory;
            this.FirstName = userp.GivenName;
            this.LastName = userp.Surname;
            this.MiddleNames = userp.MiddleName;
        }
        
        public void Authenticate(string username, string password)
        {
            this.UserName = username;
            this.Password = password;
            this.DomainName = HAP.Web.Configuration.hapConfig.Current.AD.UPN;
            UserPrincipal userp = HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms ? UserP : UserPa;
            this.UserName = userp.SamAccountName;
            this.Comment = userp.Description;
            this.DisplayName = userp.DisplayName;
            this.EmployeeID = userp.EmployeeId;
            this.Email = userp.EmailAddress;
            this.LastLoginDate = userp.LastLogon.HasValue ? userp.LastLogon.Value : DateTime.Now;
            this.IsLockedOut = userp.IsAccountLockedOut();
            this.IsApproved = userp.Enabled.HasValue ? userp.Enabled.Value : false;
            this.HomeDirectory = userp.HomeDirectory;
            this.FirstName = userp.GivenName;
            this.LastName = userp.Surname;
            this.MiddleNames = userp.MiddleName;
        }

        public void Save()
        {
            UserPrincipal userp = UserPa;
            userp.MiddleName = this.MiddleNames;
            userp.GivenName = this.FirstName;
            userp.Surname = this.LastName;
            userp.DisplayName = this.DisplayName;
            userp.Description = this.Comment;
            userp.Save();
        }

        
        private UserPrincipal UserP
        {
            get
            {
                PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, this.DomainName, this.UserName, this.Password);
                UserPrincipal userp = UserPrincipal.FindByIdentity(pcontext, this.UserName);
                return userp;
            }
        }

        private UserPrincipal UserPa
        {
            get
            {
                PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, this.DomainName, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
                UserPrincipal userp = UserPrincipal.FindByIdentity(pcontext, this.UserName);
                return userp;
            }
        }

        public bool IsMemberOf(GroupPrincipal gp)
        {
            return UserPa.IsMemberOf(gp);
        }

        #region Obsolete Properties
        [Obsolete("Not Implemented", true)]
        public override bool ChangePasswordQuestionAndAnswer(string password, string newPasswordQuestion, string newPasswordAnswer) { throw new NotImplementedException(); }
        [Obsolete("Not Implemented", true)]
        public override string GetPassword() { throw new NotImplementedException(); }
        [Obsolete("Not Implemented", true)]
        public override string GetPassword(string passwordAnswer) { throw new NotImplementedException(); }
        [Obsolete("Not Implemented", true)]
        public override bool IsOnline { get { throw new NotImplementedException(); } }
        [Obsolete("Not Implemented", true)]
        public override string ProviderName { get { throw new NotImplementedException(); } }
        [Obsolete("Not Implemented", true)]
        public override DateTime LastActivityDate { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        [Obsolete("Not Implemented", true)]
        public override DateTime LastLockoutDate { get { throw new NotImplementedException(); } }
        [Obsolete("Not Implemented", true)]
        public override string PasswordQuestion { get { throw new NotImplementedException(); } }
        [Obsolete("Not Implemented", true)]
        public override DateTime LastPasswordChangedDate { get { throw new NotImplementedException(); } }
        #endregion

        public override string ResetPassword() { return ResetPassword("password"); }
        public override string ResetPassword(string passwordAnswer) { UserPa.SetPassword(passwordAnswer); UserPa.ExpirePasswordNow(); UserPa.Save(); return passwordAnswer; }
        public override bool UnlockUser() { UserPa.UnlockAccount(); return true; }

        private WindowsImpersonationContext impersonationContext
        {
            get
            {
                if (HttpContext.Current.Session["impersonationContext"] != null) return HttpContext.Current.Session["impersonationContext"] as WindowsImpersonationContext;
                return null;
            }
            set
            {
                if (HttpContext.Current.Session["impersonationContext"] != null) HttpContext.Current.Session.Remove("impersonationContext");
                if (value != null) HttpContext.Current.Session.Add("impersonationContext", value);
            }
        }
        public string Password { private get; set; }
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_LOGON_NETWORK = 3;
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        public new string UserName { get; private set; }
        public new string Email { get; private set; }
        /// <summary>
        /// Booking System Notes
        /// </summary>
        public new string Comment { get; set;}
        public new DateTime CreationDate { get; private set; }
        public new bool IsApproved { get; private set; }
        public new bool IsLockedOut { get; private set; }
        public string DomainName { get; private set; }
        public string DisplayName { get; set; }
        public new DateTime LastLoginDate { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleNames { get; set; }
        public string HomeDirectory { get; private set; }
        public string EmployeeID { get; private set; }
        public string Notes
        {
            get
            {
                DirectorySearcher ds = new DirectorySearcher(ADUtils.DirectoryRoot);
                ds.Filter = "(&(objectClass=user)(sAMAccountName=*" + this.UserName + "*))";
                ds.PropertiesToLoad.Add("info");
                try
                {
                    SearchResultCollection sr = ds.FindAll();
                    if (sr[0].Properties["info"].Count == 0)
                        return "";
                    else if (sr[0].Properties["info"] != null)
                        return sr[0].Properties["info"][0].ToString();
                    else
                        return "";
                }
                catch { return ""; }
            }
        }

        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            UserPa.ChangePassword(oldPassword, newPassword);
            UserPa.Save();
            return true;
        }

        public bool Impersonate()
        {
            if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == Web.Configuration.AuthMode.Windows)
            {
                if (ADUtils.RevertToSelf())
                {
                    ContainedImpersonationContext = ((WindowsIdentity)HttpContext.Current.User.Identity).Impersonate();
                    return true;
                }
                return false;
            }

            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (ADUtils.RevertToSelf())
            {
                if (string.IsNullOrEmpty(this.Password) && HttpContext.Current.Request.Cookies["token"] == null)
                { 
                    FormsAuthentication.SignOut(); 
                    FormsAuthentication.RedirectToLoginPage("error=timeout"); 
                }
                else
                {
                    if (string.IsNullOrEmpty(this.Password) && HttpContext.Current.Request.Cookies["token"] != null) this.Password = TokenGenerator.ConvertToPlain(HttpContext.Current.Request.Cookies["token"].Value);
                    else if (string.IsNullOrEmpty(this.Password) && HttpContext.Current.Session != null) this.Password = HttpContext.Current.Session["password"].ToString();
                    if (ADUtils.LogonUserA(this.UserName, this.DomainName, this.Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                    {
                        if (ADUtils.DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            impersonationContext = tempWindowsIdentity.Impersonate();
                            if (impersonationContext != null)
                            {
                                ADUtils.CloseHandle(token);
                                ADUtils.CloseHandle(tokenDuplicate);
                                return true;
                            }
                        }
                    } else throw new Exception("I cannot impersonate " + this.UserName + " due to an issue logging onto the domain " + this.DomainName + " using an Interactive Login.  HAP+ Requires Interactive Login Rights on the Server it is running on");
                }
            }
            if (token != IntPtr.Zero)
                ADUtils.CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                ADUtils.CloseHandle(tokenDuplicate);
            return false;
        }

        public bool ImpersonateContained()
        {
            if (HAP.Web.Configuration.hapConfig.Current.AD.AuthenticationMode == Web.Configuration.AuthMode.Windows)
            {
                if (ADUtils.RevertToSelf())
                {
                    ContainedImpersonationContext = ((WindowsIdentity)HttpContext.Current.User.Identity).Impersonate();
                    return true;
                }
                return false;
            }
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (ADUtils.RevertToSelf())
            {
                if (string.IsNullOrEmpty(this.Password) && HttpContext.Current.Request.Cookies["token"] != null) this.Password = TokenGenerator.ConvertToPlain(HttpContext.Current.Request.Cookies["token"].Value);
                if (ADUtils.LogonUserA(this.UserName, this.DomainName, this.Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                {
                    if (ADUtils.DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        ContainedImpersonationContext = tempWindowsIdentity.Impersonate();
                        if (ContainedImpersonationContext != null)
                        {
                            ADUtils.CloseHandle(token);
                            ADUtils.CloseHandle(tokenDuplicate);
                            return true;
                        }
                    }
                }
                else throw new Exception("Something is wrong");
            }
            if (token != IntPtr.Zero)
                ADUtils.CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                ADUtils.CloseHandle(tokenDuplicate);
            return false;
        }

        public WindowsImpersonationContext ContainedImpersonationContext { get; set; }

        public void EndContainedImpersonate()
        {
            if (ContainedImpersonationContext != null) ContainedImpersonationContext.Undo();
            ContainedImpersonationContext = null;
        }

        public void EndImpersonate()
        {
            if (impersonationContext != null) impersonationContext.Undo();
            impersonationContext = null;
        }

        public int CompareTo(object obj)
        {
            return this.UserName.CompareTo(((User)obj).UserName);
        }
    }
}