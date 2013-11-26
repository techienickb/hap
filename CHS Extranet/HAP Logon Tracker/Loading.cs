using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using Newtonsoft.Json;

namespace HAP.Tracker.UI
{
    public partial class Loading : Form
    {
        private Action action;
        private bool silent;
        private string baseurl;

        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        public Loading(Action action, string baseurl, bool silent)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            InitializeComponent();
            this.silent = silent;
            this.action = action;
            this.baseurl = baseurl;
            if (action == Action.Clear) label1.Text = "Refreshing the Tracker...";
            else label1.Text = "Registering your Logon...";
            this.Text = "Logon Tracker - " + label1.Text;
        }

        private void Loading_Load(object sender, EventArgs e)
        {
            if (this.silent) this.Hide();
            WebClient c = new WebClient();
            c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            c.Headers.Add(HttpRequestHeader.Accept, "application/json");
            c.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            if (action == Action.Clear)
            {
                c.UploadStringCompleted += c_ClearCompleted;
                c.UploadStringAsync(new Uri(new Uri(baseurl), "./api/tracker/clear"), "POST", "{ \"Computer\": \"" + Dns.GetHostName() + "\", \"DomainName\":\"" + Environment.UserDomainName + "\" }");
            }
            else if (action == Action.Poll)
            {
                string ip1 = "";
                foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    if (ip.ToString().Contains(".")) { ip1 = ip.ToString(); break; }

                c.UploadStringCompleted += c_UploadStringCompleted;
                c.UploadStringAsync(new Uri(new Uri(baseurl), "./api/tracker/poll"), "POST", "{ \"Username\": \"" + Environment.UserName + "\", \"Computer\": \"" + Dns.GetHostName() + "\", \"DomainName\":\"" + Environment.UserDomainName + "\" }");
            }
            else
            {
                string ip1 = "";
                foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    if (ip.ToString().Contains(".")) { ip1 = ip.ToString(); break; }

                c.UploadStringCompleted += c_UploadStringCompleted;
                c.UploadStringAsync(new Uri(new Uri(baseurl), "./api/tracker/logon"), "POST", "{ \"Username\": \"" + Environment.UserName + "\", \"Computer\": \"" + Dns.GetHostName() + "\", \"DomainName\":\"" + Environment.UserDomainName + "\", \"IP\": \"" + ip1 + "\", \"LogonServer\": \"" + Environment.GetEnvironmentVariable("logonserver") + "\", \"os\": \"" + Environment.OSVersion.VersionString + "\" }");
            }
            
        }

        void c_ClearCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            Close();
        }

        void c_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                LogonsList list = JsonConvert.DeserializeObject<LogonsList>(e.Result);
                if (list.Logons.Length > 0)
                {
                    Background bg = new Background();
                    bg.Main.SetGrid(list);
                    bg.Main.baseurl = baseurl;
                    bg.ShowDialog(this);
                }
            }
            if (action == Action.Logon)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "HAP Logon Tracker Notifier.exe");
                proc.StartInfo.Arguments = baseurl;
                proc.Start();
            }
            Close();
        }
    }

    public enum Action { Logon, Clear, RemoteLogoff, Poll }
}
