using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace HAP.Logon.Tracker
{
    public partial class Loading : Form
    {
        private Action action;
        private api.api api;
        public Loading(Action action, string baseurl)
        {
            InitializeComponent();
            this.api = new api.api();
            this.api.Url = new Uri(new Uri(baseurl), "tracker/api.asmx").ToString();
            this.api.LogonCompleted += new Tracker.api.LogonCompletedEventHandler(api_LogonCompleted);
            this.api.ClearCompleted += new Tracker.api.ClearCompletedEventHandler(api_ClearCompleted);
            this.action = action;
            if (action == Action.Clear) label1.Text = "Refreshing the Tracker...";
            else label1.Text = "Registering your Logon...";
            this.Text = "Logon Tracker - " + label1.Text;
        }

        void  api_ClearCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }

        void  api_LogonCompleted(object sender, api.LogonCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
