using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Storage;
using Windows.Foundation;
using Windows.Security.Credentials;

namespace HAP.Win.MyFiles
{
    public class HAPSettings
    {
        public void InitHandlers()
        {
            Windows.Storage.ApplicationData.Current.DataChanged += new TypedEventHandler<ApplicationData, object>(DataChangeHandler);
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.DeleteContainer("hapSites");
            Windows.Storage.ApplicationDataContainer container = roamingSettings.CreateContainer("hapSites", Windows.Storage.ApplicationDataCreateDisposition.Always);
            if (roamingSettings.Containers.ContainsKey("hapSites") && roamingSettings.Containers["hapSites"].Values.Count == 0)
            {
                roamingSettings.Containers["hapSites"].Values["site0"] = new string[] { "site0", "", "", "" };
                roamingSettings.Values["sites"] = new string[] { "site0" };
            }
        }

        public static HAPSetting CurrentSite
        {
            get { return HAPSetting.Parse((string[])ApplicationData.Current.LocalSettings.Values["currentsite"]); }
            set { ApplicationData.Current.LocalSettings.Values["currentsite"] = value.ToString(); }
        }

        public static string[] CurrentToken
        {
            get { return (string[])ApplicationData.Current.LocalSettings.Values["currenttoken"]; }
            set { ApplicationData.Current.LocalSettings.Values["currenttoken"] = value; }
        }

        public Dictionary<string, HAPSetting> Settings
        {
            get
            {
                Dictionary<string, HAPSetting> settings = new Dictionary<string, HAPSetting>();
                Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                Windows.Storage.ApplicationDataContainer container = roamingSettings.CreateContainer("hapSites", Windows.Storage.ApplicationDataCreateDisposition.Always);
                foreach (string s in (string[])roamingSettings.Values["sites"])
                    settings.Add(s, HAPSetting.Parse((string[])roamingSettings.Containers["hapSites"].Values[s]));
                return settings;
            }
        }

        public ApplicationDataContainer SiteContainer
        {
            get
            {
                Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                return roamingSettings.CreateContainer("hapSites", Windows.Storage.ApplicationDataCreateDisposition.Always);
            }
        }

        public void UpdateSite(string key, HAPSetting value)
        {
            RemoveSite(key);
            AddSite(value);
        }
        public void AddSite(HAPSetting value)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            Windows.Storage.ApplicationDataContainer container = roamingSettings.CreateContainer("hapSites", Windows.Storage.ApplicationDataCreateDisposition.Always);
            if (roamingSettings.Containers["hapSites"].Values.ContainsKey(value.Name)) return;
            roamingSettings.Containers["hapSites"].Values[value.Name] = value.ToString();
            List<string> s = new List<string>();
            s.AddRange((string[])roamingSettings.Values["sites"]);
            s.Add(value.Name);
            roamingSettings.Values["sites"] = s.ToArray();
        }
        public void RemoveSite(string key)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            Windows.Storage.ApplicationDataContainer container = roamingSettings.CreateContainer("hapSites", Windows.Storage.ApplicationDataCreateDisposition.Always);
            roamingSettings.Containers["hapSites"].Values.Remove(key);
            List<string> s = new List<string>();
            s.AddRange((string[])roamingSettings.Values["sites"]);
            s.Remove(key);
            if (s.Count == 0)
            {
                roamingSettings.Containers["hapSites"].Values["site0"] = new string[] { "site0", "", "", "" };
                roamingSettings.Values["sites"] = new string[] { "site0" };
            } else roamingSettings.Values["sites"] = s.ToArray();
        }

        public void DataChangeHandler(Windows.Storage.ApplicationData appData, object o)
        {
            // TODO: Refresh your data
        }
    }

    public class HAPSetting
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Uri Address { get; set; }
        public string Name { get; set; }
        public static HAPSetting Parse(string[] s)
        {
            HAPSetting set = new HAPSetting() { Name = s[0], Address = new Uri(s[1]), Username = "", Password = "" };
            if (s[0] != "site0")
            {
                PasswordVault vault = new PasswordVault();
                if (vault.FindAllByResource("HAP+ Credentials").Count(c => c.UserName.EndsWith(set.Address.DnsSafeHost)) == 0)
                {

                    //ask for new credentails - possible change of computer so credentails will not be in the vault.
                }
                else
                {
                    PasswordCredential cred = vault.FindAllByResource("HAP+ Credentials").Single(c => c.UserName.EndsWith(set.Address.DnsSafeHost));
                    set.Username = cred.UserName.Substring(0, cred.UserName.IndexOf('@'));
                    cred.RetrievePassword();
                    set.Password = cred.Password;
                }
            }
            return set;
        }
        public string[] ToString()
        {
            PasswordVault vault = new PasswordVault();
            vault.Add(new PasswordCredential("HAP+ Credentials", Username + "@" + Address.DnsSafeHost, Password));
            return new string[] { Name, Address.ToString() };
        }
    }
}