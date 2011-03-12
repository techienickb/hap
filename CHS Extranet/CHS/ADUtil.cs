using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP.Web.Configuration;
using System.DirectoryServices;
using System.Configuration;

namespace HAP.AD
{

    public class ADUtil
    {
        public static UserInfo[] FindUsers()
        {
            config = hapConfig.Current;
            System.Collections.Generic.List<UserInfo> users = new System.Collections.Generic.List<UserInfo>();
            foreach (ouobject ob in hapConfig.Current.ADSettings.OUObjects)
                if (!ob.Ignore)
                    foreach (UserInfo info in FindUsers(ob, ""))
                        if (!users.Contains(info))
                            users.Add(info);
            users.Sort();
            return users.ToArray();
        }

        static hapConfig config;

        public static UserInfo[] FindUsers(ouobject ou, string subou)
        {
            List<UserInfo> results = new List<UserInfo>();

            string cs = string.Format(ou.Path, subou, config.BaseSettings.EstablishmentCode);
            DirectoryEntry usersDE = new DirectoryEntry(cs, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            //throw new Exception(usersDE1.Path);
            //DirectoryEntry usersDE = new DirectoryEntry(ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(&(objectClass=user)(mail=*)(sAMAccountName=*))";
            ds.PropertiesToLoad.Add(UserProperty.UserName);
            ds.PropertiesToLoad.Add(UserProperty.DisplayName);
            ds.PropertiesToLoad.Add(UserProperty.Email);
            ds.PropertiesToLoad.Add(UserProperty.Notes);

            try
            {
                SearchResultCollection sr = ds.FindAll();

                for (int i = 0; i < sr.Count; i++)
                {
                    int z = 0;

                    if (!int.TryParse(sr[i].Properties[UserProperty.UserName][0].ToString().ToCharArray()[0].ToString(), out z))
                    {
                        UserInfo info = new UserInfo();
                        info.LoginName = sr[i].Properties[UserProperty.UserName][0].ToString();
                        if (sr[i].Properties[UserProperty.DisplayName].Count == 0)
                            info.DisplayName = "";
                        else if (sr[i].Properties[UserProperty.DisplayName] != null)
                            info.DisplayName = sr[i].Properties[UserProperty.DisplayName][0].ToString();
                        else
                            info.DisplayName = "";
                        if (sr[i].Properties[UserProperty.Notes].Count == 0)
                            info.Notes = "";
                        else if (sr[i].Properties[UserProperty.Notes] != null)
                            info.Notes = sr[i].Properties[UserProperty.Notes][0].ToString();
                        else
                            info.Notes = "";
                        if (sr[i].Properties[UserProperty.Email].Count == 0)
                            info.EmailAddress = "";
                        else if (sr[i].Properties[UserProperty.Email] != null)
                            info.EmailAddress = sr[i].Properties[UserProperty.Email][0].ToString();
                        else
                            info.EmailAddress = "";
                        results.Add(info);
                    }

                }
            }
            catch (Exception e) { throw new Exception(usersDE.Path, e); }
            results.Sort();
            return results.ToArray();
        }

        public static UserInfo GetUserInfo(string username)
        {
            if (config == null) config = hapConfig.Current;
            try
            {
                DirectoryEntry usersDE = new DirectoryEntry(ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
                DirectorySearcher ds = new DirectorySearcher(usersDE);
                ds.Filter = "(sAMAccountName=" + username + ")";
                ds.PropertiesToLoad.Add("cn");
                ds.PropertiesToLoad.Add(UserProperty.UserName);
                ds.PropertiesToLoad.Add(UserProperty.DisplayName);
                ds.PropertiesToLoad.Add(UserProperty.Notes);
                ds.PropertiesToLoad.Add(UserProperty.Email);
                SearchResult r = ds.FindOne();

                UserInfo info = new UserInfo();
                info.LoginName = r.Properties[UserProperty.UserName][0].ToString();
                if (r.Properties[UserProperty.DisplayName].Count == 0)
                    info.DisplayName = info.LoginName;
                else if (r.Properties[UserProperty.DisplayName] != null)
                    info.DisplayName = r.Properties[UserProperty.DisplayName][0].ToString();
                else
                    info.DisplayName = info.LoginName;
                if (r.Properties[UserProperty.Notes].Count == 0)
                    info.Notes = info.DisplayName;
                else if (r.Properties[UserProperty.Notes] != null)
                    info.Notes = r.Properties[UserProperty.Notes][0].ToString();
                else
                    info.Notes = info.DisplayName;
                if (r.Properties[UserProperty.Email].Count == 0)
                    info.EmailAddress = "";
                else if (r.Properties[UserProperty.Email] != null)
                    info.EmailAddress = r.Properties[UserProperty.Email][0].ToString();
                else
                    info.EmailAddress = "";

                return info;
            }
            catch
            {
                UserInfo io = new UserInfo();
                io.DisplayName = "Unknown Username";
                io.Notes = "Unknown Username";
                io.LoginName = "N/A";
                io.EmailAddress = hapConfig.Current.BaseSettings.AdminEmailAddress;
                return io;
            }
        }
    }
}
