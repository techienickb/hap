using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Web;

namespace HAP.AD
{
	public static class ActiveDirectoryHelper
	{
		public const string ERROR_ACTIVEDIRECTORY_QUERY = "AD Query Error";
		public const string ERROR_ROLE_NOT_FOUND = "Role Not Found";
		public const string ERROR_CONFIG_NOT_FOUND = "Config Not Found";
		public const string ERROR_CONNSTR_NOT_FOUND = "Connection String not Found'.";
		public const string ERROR_USER_NOT_FOUND = "'connectionUsername' not Found.";
		public const string ERROR_PASS_NOT_FOUND = "'connectionPassword' not Found.";

		public const string SEARCH_ALL_GROUPS = "(&(objectCategory=group)(|(groupType=-2147483646)(groupType=-2147483644)(groupType=-2147483640)))";
		public const string SEARCH_GROUPS = "(&(objectClass=group)(cn={0}))";
		public const string SEARCH_USERS = "(&(objectClass=user)(sAMAccountName={0}))";

		static ActiveDirectoryHelper() { }

		private static IdentityReferenceCollection ExpandTokenGroups(DirectoryEntry user)
		{
			user.RefreshCache(new[] { "tokenGroups" });
			var irc = new IdentityReferenceCollection();
			foreach (byte[] sidBytes in user.Properties["tokenGroups"])
				irc.Add(new SecurityIdentifier(sidBytes, 0));
			return irc;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connString"></param>
		/// <returns></returns>
		public static string GetDomainName(string connString)
		{
			return connString.Substring(connString.LastIndexOf(@"/") + 1, connString.Length - connString.LastIndexOf(@"/") - 1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connStringName"></param>
		/// <param name="connUsername"></param>
		/// <param name="connPassword"></param>
		/// <returns></returns>
		public static DirectoryEntry GetDirectoryEntry(string connStringName, string connUsername, string connPassword)
		{
			return new DirectoryEntry(connStringName, connUsername, connPassword, AuthenticationTypes.ServerBind);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="filter"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		public static string[] Search(DirectoryEntry root, String filter, String field)
		{
			var res = new List<string>();
			var searcher = new DirectorySearcher { SearchRoot = root, Filter = filter };
			searcher.PropertiesToLoad.Clear();
			searcher.PropertiesToLoad.Add(field);
			searcher.PageSize = 500;
			try
			{
				using (SearchResultCollection results = searcher.FindAll())
				{
					foreach (SearchResult result in results)
					{
						int resultCount = result.Properties[field].Count;
						for (int c = 0; c < resultCount; c++)
							res.Add(result.Properties[field][c].ToString());
					}
				}
			}
			catch (Exception ex)
			{
				throw new ProviderException(ERROR_ACTIVEDIRECTORY_QUERY, ex);
			}
			return res.Count > 0 ? res.ToArray() : new string[0];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public static DirectoryEntry getGroup(DirectoryEntry root, string groupName)
		{
			var ds = new DirectorySearcher(root) { Filter = string.Format(SEARCH_GROUPS, groupName) };
			SearchResultCollection groups = ds.FindAll();
			return groups.Count > 0 ? groups[0].GetDirectoryEntry() : null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static DirectoryEntry getUser(DirectoryEntry root, string userName)
		{
			// Если в имени пользователя присутствует домен, убираем его
			if (userName.IndexOf(@"\") > 0)
				userName = userName.Substring(userName.IndexOf(@"\") + 1, userName.Length - userName.IndexOf(@"\") - 1);

			var ds = new DirectorySearcher(root) { Filter = string.Format(SEARCH_USERS, userName) };
			SearchResultCollection users = ds.FindAll();
			return users.Count > 0 ? users[0].GetDirectoryEntry() : null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public static string[] GetAllRoles(DirectoryEntry root)
		{
			var results = new List<string>();
			string[] roles = Search(root, SEARCH_ALL_GROUPS, "samAccountName");
			foreach (string role in roles)
				results.Add(role);
			return results.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public static bool RoleExists(DirectoryEntry root, string roleName)
		{
			foreach (String role in GetAllRoles(root))
				if (roleName == role) return true;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="domainName"></param>
		/// <param name="username"></param>
		/// <param name="roleName"></param>
		/// <param name="cache">Use Caches</param>
		/// <returns></returns>
		public static bool IsUserInRole(DirectoryEntry root, string domainName, string username, string roleName, bool cache)
		{
			foreach (string user in GetUsersInRole(root, domainName, roleName, cache))
				if (username == user) return true;
			return false;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		public static string[] GetRolesForUser(DirectoryEntry root, string username)
		{
			var user = getUser(root, username);
			var roles = new List<string>();
			IdentityReferenceCollection groupCollection = ExpandTokenGroups(user).Translate(typeof(NTAccount), false);

			if (groupCollection != null)
				foreach (object g in groupCollection)
				{
					try
					{
						NTAccount group = g as NTAccount;
						String roleName = group.ToString().Substring(group.ToString().IndexOf(@"\") + 1);
						if (roleName != String.Empty)
							roles.Add(roleName);
					}
					catch { }
				}
			return roles.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="domainName"></param>
		/// <param name="roleName"></param>
		/// <param name="usecache">Use cached results</param>
		/// <returns></returns>
		public static string[] GetUsersInRole(DirectoryEntry root, string domainName, string roleName, bool usecache)
		{
			if (!RoleExists(root, roleName))
				throw new ProviderException(String.Format(ERROR_ROLE_NOT_FOUND, roleName));

			var results = new List<string>();
			bool reloadres = false;
			if (usecache)
			{
				HttpContext c = HttpContext.Current;
				reloadres = c.Cache["rolecache:" + roleName] == null;
				if (!reloadres) results = (List<string>)c.Cache["rolecache:" + roleName];
			}
			else reloadres = true;
			if (reloadres)
				using (var context = new PrincipalContext(ContextType.Domain, null, domainName))
				{
					try
					{
						var p = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, roleName);
						var users = p.GetMembers(true);
						foreach (UserPrincipal user in users)
							results.Add(user.SamAccountName);
					}
					catch (Exception ex)
					{
						throw new ProviderException(ERROR_ACTIVEDIRECTORY_QUERY, ex);
					}
				}
			if (usecache) HttpContext.Current.Cache.Insert("rolecache:" + roleName, results, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 10, 0));
			return results.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="domainName"></param>
		/// <param name="roleName"></param>
		/// <param name="usecache"></param>
		/// <returns></returns>
		public static string[] GetUserNamesInRole(DirectoryEntry root, string domainName, string roleName, bool usecache)
		{
			if (!RoleExists(root, roleName))
				throw new ProviderException(String.Format(ERROR_ROLE_NOT_FOUND, roleName));

			var results = new List<string>();
			bool reloadres = false;
			if (usecache)
			{
				HttpContext c = HttpContext.Current;
				reloadres = c.Cache["rolenamescache:" + roleName] == null;
				if (!reloadres) results = (List<string>)c.Cache["rolenamescache:" + roleName];
			}
			else reloadres = true;
			if (reloadres)
				using (var context = new PrincipalContext(ContextType.Domain, null, domainName))
				{
					try
					{
						var p = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, roleName);
						var users = p.GetMembers(true);
						foreach (UserPrincipal user in users)
							results.Add(user.Name);
					}
					catch (Exception ex)
					{
						throw new ProviderException(ERROR_ACTIVEDIRECTORY_QUERY, ex);
					}
				}
			if (usecache) HttpContext.Current.Cache.Insert("rolenamescache:" + roleName, results, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 10, 0));
			return results.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="domainName"></param>
		/// <param name="roleName"></param>
		/// <param name="usernameToMatch"></param>
		/// <returns></returns>
		public static string[] FindUsersInRole(DirectoryEntry root, string domainName, string roleName, string usernameToMatch)
		{
			if (!RoleExists(root, roleName))
				throw new ProviderException(String.Format(ERROR_ROLE_NOT_FOUND, roleName));

			var results = new List<string>();
			using (var context = new PrincipalContext(ContextType.Domain, null, domainName))
			{
				try
				{
					var p = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, roleName);
					var users = p.GetMembers(true);
					foreach (UserPrincipal user in users)
						if (user.SamAccountName.IndexOf(usernameToMatch) > 0)
							results.Add(user.SamAccountName);
				}
				catch (Exception ex)
				{
					throw new ProviderException(ERROR_ACTIVEDIRECTORY_QUERY, ex);
				}
			}
			return results.ToArray();
		}



	}
}
