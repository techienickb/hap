﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.DirectoryServices;
using System.Collections.Specialized;
using System.DirectoryServices.AccountManagement;

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
            if (!HAP.Web.Configuration.hapConfig.Current.AD.UseNestedLookups) return wtrp.IsUserInRole(HAP.Web.Configuration.hapConfig.Current.AD.UPN.Remove(HAP.Web.Configuration.hapConfig.Current.AD.UPN.IndexOf('.')) + '\\' + username, roleName);
            try
            {
                PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, HAP.Web.Configuration.hapConfig.Current.AD.UPN, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, roleName);
                if (gp == null) return false;

                foreach (Principal p in gp.GetMembers(true))
                    if (p.Name.ToLower() == username.ToLower()) return true;
            }
            catch { }
            return false;
        }

        public override string[] GetRolesForUser(string username)
        {
            if (!HAP.Web.Configuration.hapConfig.Current.AD.UseNestedLookups) return wtrp.GetRolesForUser(username);
            else if (HttpContext.Current.Cache["userrolecache-" + username] == null)
            {
                List<string> roles = new List<string>();
                try
                {
                    PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, HAP.Web.Configuration.hapConfig.Current.AD.UPN, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
                    UserPrincipal userp = UserPrincipal.FindByIdentity(pcontext, username);
                    foreach (Principal p in userp.GetGroups())
                    {
                        roles.Add(p.SamAccountName);
                        foreach (Principal p1 in (((GroupPrincipal)p).GetGroups()))
                            Recurse(p1, ref roles, 0);
                    }
                }
                catch { }
                HttpContext.Current.Cache.Insert("userrolecache-" + username, roles.ToArray(), null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
            }
            return HttpContext.Current.Cache["userrolecache-" + username] as string[];
        }

        public void Recurse(Principal p, ref List<string> roles, int loop)
        {
            roles.Add(p.Name);
            if (loop < HAP.Web.Configuration.hapConfig.Current.AD.MaxRecursions)
                try
                {
                    foreach (Principal p1 in (((GroupPrincipal)p).GetGroups()))
                        Recurse(p1, ref roles, loop + 1);
                }
                catch { }
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
            return wtrp.RoleExists(roleName);
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
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, HAP.Web.Configuration.hapConfig.Current.AD.UPN, HAP.Web.Configuration.hapConfig.Current.AD.User, HAP.Web.Configuration.hapConfig.Current.AD.Password);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, roleName);
            if (gp == null) return new string[] { };
            List<string> users = new List<string>();
            foreach (Principal p in gp.GetMembers(true))
                users.Add(p.Name);
            return users.ToArray();//wtrp.GetUsersInRole(roleName);
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