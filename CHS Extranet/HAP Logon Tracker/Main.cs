using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace HAP.Tracker.UI
{
    public partial class Main : Form
    {
        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        public Main()
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            InitializeComponent();
            KeepOpen = true;
            Override = false;
            Done.Enabled = false;
        }

        private bool KeepOpen;
        private bool Override;
        private string code;
        private UT Usertype;
        public int MaxLogons { get; set; }
        public string baseurl { get; set; }
        public void SetGrid(LogonsList data)
        {
            code = data.OverrideCode;
            MaxLogons = data.MaxLogons;
            Usertype = data.UserType;
            label2.Text = (Usertype == UT.Student) ? string.Format(label2.Text, MaxLogons) : "Check you logged on to these computers";
            foreach (trackerlogentrysmall entry in data.Logons)
                dataGridView1.Rows.Add(entry.ComputerName, entry.DomainName, entry.LogOnDateTime, "Logoff");
            CheckCount();
        }

        private void CheckCount()
        {
            Done.Enabled = false;
            if (MaxLogons == 0) Done.Enabled = true;
            else if (dataGridView1.Rows.Count < MaxLogons) Done.Enabled = true;
            KeepOpen = !Done.Enabled;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                WebClient c = new WebClient();
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.Accept, "application/json");
                c.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                c.UploadStringCompleted += c_RemoteLogoff;
                c.UploadStringAsync(new Uri(new Uri(baseurl), "./api/tracker/RemoteLogoff"), "POST", "{ \"Computer\": \"" + dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString() + "\", \"DomainName\":\"" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() + "\" }", e.RowIndex);
                this.Enabled = false;
                this.Cursor = Cursors.AppStarting;
            }
        }

        void c_RemoteLogoff(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null) MessageBox.Show(this, e.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else dataGridView1.Rows.RemoveAt((int)e.UserState);
            }
            catch { }
            this.Enabled = true;
            this.Cursor = Cursors.Default;
            CheckCount();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Override)
            {
                if (new OverrideCode(code).ShowDialog(this) == System.Windows.Forms.DialogResult.OK) { this.KeepOpen = false; Close(); return; }
                else this.KeepOpen = true;
            }
            else if (MessageBox.Show(this, "Clicking this button will result in the system logging you off.", "Logoff?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                WebClient c = new WebClient();
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.Accept, "application/json");
                c.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                c.UploadStringCompleted += c_RemoteLogoff;
                c.UploadStringAsync(new Uri(new Uri(baseurl), "./api/tracker/RemoteLogoff"), "POST", "{ \"Computer\": \"" + Dns.GetHostName() + "\", \"DomainName\":\"" + Environment.UserDomainName + "\" }");
                this.Hide();
            }
            Override = false;
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = this.KeepOpen;
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            Override = e.Control;
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            Override = false;
        }
    }
}
