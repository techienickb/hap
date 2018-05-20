using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.DirectoryServices;
using System.Collections.Specialized;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Threading;

namespace HAP.AD
{
    /// <summary>
    /// Summary description for RoleProvider
    /// </summary>
    public sealed class RoleProvider : System.Web.Security.RoleProvider
    {
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            ApplicationName = config["applicationName"];
        }

        System.Web.Security.WindowsTokenRoleProvider wtrp = new System.Web.Security.WindowsTokenRoleProvider();

        public override bool IsUserInRole(string username, string roleName)
        {
            foreach (string s in GetRolesForUser(username)) if (s.ToLower().Trim().Contains(roleName.Trim().ToLower())) return true;
            return false;
        }

        public override string[] GetRolesForUser(string username)
        {
            if (!HAP.Web.Configuration.hapConfig.Current.AD.UseNestedLookups) return wtrp.GetRolesForUser(username);
            else if (HttpContext.Current.Cache["userrolecache-" + username.ToLower()] == null)
            {
                List<string> roles = new List<string> { "Authenticated Users" };
                //try
                //{
                PrincipalContext pcontext;
                if (HAP.Web.Configuration.hapConfig.Current.AD.SecureLDAP) pcontext = new PrincipalContext(ContextType.Domain, HAP.Web.Configuration.hapConfig.Current.AD.UPN, null, ContextOptions.Negotiate, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
                else pcontext = new PrincipalContext(ContextType.Domain, HAP.Web.Configuration.hapConfig.Current.AD.UPN, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
                UserPrincipal userp = UserPrincipal.FindByIdentity(pcontext, username);
                foreach (Principal p in userp.GetAuthorizationGroups())
                {
                    try
                    {
                        if (!roles.Contains(p.Name))
                        {
                            roles.Add(p.Name);
                            foreach (Principal p1 in (((GroupPrincipal)p).GetGroups()))
                                Recurse(p1, ref roles, 0);
                        }
                    }
                    catch { }
                }
                //}
                //catch { }
                HttpContext.Current.Cache.Insert("userrolecache-" + username.ToLower(), roles.ToArray(), null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
            }
            return HttpContext.Current.Cache["userrolecache-" + username.ToLower()] as string[];
        }

        public void Recurse(Principal p, ref List<string> roles, int loop)
        {
            if (!roles.Contains(p.Name))
            {
                roles.Add(p.Name);
                if (loop < HAP.Web.Configuration.hapConfig.Current.AD.MaxRecursions)
                    foreach (Principal p1 in (((GroupPrincipal)p).GetGroups()))
                        Recurse(p1, ref roles, loop + 1);
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            PrincipalContext pcontext;
            if (HAP.Web.Configuration.hapConfig.Current.AD.SecureLDAP) pcontext = new PrincipalContext(ContextType.Domain, HAP.Web.Configuration.hapConfig.Current.AD.UPN, null, ContextOptions.Negotiate, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
            else pcontext = new PrincipalContext(ContextType.Domain, HAP.Web.Configuration.hapConfig.Current.AD.UPN, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
            GroupPrincipal p = GroupPrincipal.FindByIdentity(pcontext, roleName);
            try { var s = p.StructuralObjectClass; }
            catch { return false; }
            return true;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (!HAP.Web.Configuration.hapConfig.Current.AD.UseNestedLookups) return wtrp.GetUsersInRole(roleName);
            else if (HttpContext.Current.Cache["rolecache-" + roleName.ToLower()] == null)
            {
                PrincipalContext pcontext;
                if (HAP.Web.Configuration.hapConfig.Current.AD.SecureLDAP) pcontext = new PrincipalContext(ContextType.Domain, HAP.Web.Configuration.hapConfig.Current.AD.UPN, null, ContextOptions.Negotiate, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
                else pcontext = new PrincipalContext(ContextType.Domain, HAP.Web.Configuration.hapConfig.Current.AD.UPN, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, roleName);
                if (gp == null) return new string[] { };
                List<string> users = new List<string>();
                foreach (Principal p in gp.GetMembers(true))
                    users.Add(p.Name);
                HttpContext.Current.Cache.Insert("rolecache-" + roleName.ToLower(), users.ToArray(), null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
            }
            return HttpContext.Current.Cache["rolecache-" + roleName.ToLower()] as string[];
        }

        public override string[] GetAllRoles()
        {
            return wtrp.GetAllRoles();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return wtrp.FindUsersInRole(roleName, usernameToMatch);
        }

        public override string ApplicationName { get; set; }
    }
}