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
        private Uri baseurl;
        private WebClient client;
        public Loading(Action action, string baseurl)
        {
            InitializeComponent();
            this.action = action;
            this.baseurl = new Uri(baseurl);
            if (action == Action.Clear) label1.Text = "Refreshing the Tracker...";
            else label1.Text = "Registering your Logon...";
            this.Text = "Logon Tracker - " + label1.Text;
        }

        private void Loading_Load(object sender, EventArgs e)
        {
            client = new WebClient();
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_UploadStringCompleted);
            string sb = "";
            if (action == Action.Clear) sb = Dns.GetHostName();
            else
            {
                sb += "computername|" + Dns.GetHostName() + "\n";
                sb += "username|" + Environment.UserName + "\n";
                sb += "domainname|" + Environment.UserDomainName + "\n";
                foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    if (ip.ToString().Contains(".")) { sb += "ip|" + ip.ToString() + "\n"; break; }
                sb += "logonserver|" + Environment.GetEnvironmentVariable("logonserver") + "\n";
                sb += "os|" + Environment.OSVersion.VersionString;
            }
            client.UploadStringAsync(new Uri(baseurl, "tracker/api.ashx?op=" + action.ToString().ToLower()), sb.ToString());
        }

        private void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (InvokeRequired) BeginInvoke(new UploadStringCompletedEventHandler(client_UploadStringCompleted), sender, e);
            else
            {
                try
                {
                    if (e.Result != "Done")
                    {
                        Background bg = new Background();
                        bg.Main.SetGrid(e.Result);
                        bg.Main.BaseUri = baseurl;
                        bg.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    StreamWriter sw;
                    string path = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".log";
                    if (!File.Exists(path))
                        sw = File.CreateText(path);
                    else sw = File.AppendText(path);
                    sw.WriteLine(ex.ToString());
                    sw.Close();
                    sw.Dispose();
                }

                this.Close();
            }
        }
    }

    public enum Action { Logon, Clear, RemoteLogoff }
}
