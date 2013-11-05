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

namespace HAP.Logon.Tracker
{
    public partial class Loading : Form
    {
        private Action action;
        private api.api api;
        private bool silent;

        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        public Loading(Action action, string baseurl, bool silent)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            InitializeComponent();
            this.api = new api.api();
            this.silent = silent;
            this.api.Url = new Uri(new Uri(baseurl), "tracker/api.asmx").ToString();
            this.api.LogonCompleted += new Tracker.api.LogonCompletedEventHandler(api_LogonCompleted);
            this.api.ClearCompleted += new Tracker.api.ClearCompletedEventHandler(api_ClearCompleted);
            this.action = action;
            this.api.Timeout = 60000;
            this.Hide();
            if (action == Action.Clear) label1.Text = "Refreshing the Tracker...";
            else label1.Text = "Registering your Logon...";
            this.Text = "Logon Tracker - " + label1.Text;
        }

        void  api_ClearCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                try
                {
                    if (!EventLog.SourceExists("Home Access Plus"))
                        EventLog.CreateEventSource("Home Access Plus+", "Application");
                    EventLog.WriteEntry("Home Access Plus+", "An error occurred in the Logon Tracker\r\n\r\nError: " + e.Error.Message, EventLogEntryType.Error);
                }
                catch { }
            }
            Close();
        }

        void  api_LogonCompleted(object sender, api.LogonCompletedEventArgs e)
        {
            if (e.Error != null) {
                try
                {
                    if (!EventLog.SourceExists("Home Access Plus"))
                        EventLog.CreateEventSource("Home Access Plus+", "Application");
                    EventLog.WriteEntry("Home Access Plus+", "An error occurred in the Logon Tracker\r\n\r\nError: " + e.Error.Message, EventLogEntryType.Error);
                }
                catch { }
            }
            else
            {
                if (e.Result.Logons.Length > 0)
                {
                    Background bg = new Background();
                    bg.Main.SetGrid(e.Result);
                    bg.Main.API = api;
                    bg.ShowDialog(this);
                }
            }
            Close();
        }

        private void Loading_Load(object sender, EventArgs e)
        {
            if (action == Action.Clear) api.ClearAsync(Dns.GetHostName(), Environment.UserDomainName);
            else
            {
                string ip1 = "";
                foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    if (ip.ToString().Contains(".")) { ip1 = ip.ToString(); break; }
                api.LogonAsync(Environment.UserName, Dns.GetHostName(), Environment.UserDomainName, ip1, Environment.GetEnvironmentVariable("logonserver"), Environment.OSVersion.VersionString);
            }
            
        }
    }

    public enum Action { Logon, Clear, RemoteLogoff }
}
