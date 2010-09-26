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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            KeepOpen = true;
            Override = false;
        }

        public Uri BaseUri { get; set; }
        private bool KeepOpen;
        private bool Override;
        private string code;

        public int MaxLogons { get; set; }
        public void SetGrid(string data)
        {
            data = data.Remove(0, 7);
            string[] datas = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int x = 0; x < datas.Length; x++)
            {
                string s = datas[x];
                if (x == 0)
                {
                    if ((s.StartsWith("Student") || s.StartsWith("Staff")) && s.Contains(":"))
                    {
                        MaxLogons = int.Parse(s.Remove(s.IndexOf('!')).Remove(0, s.IndexOf(':') + 1));
                        label2.Text = string.Format(label2.Text, MaxLogons);
                    }
                    else
                    {
                        label2.Text = "Check you logged on to these computers";
                        MaxLogons = 0;
                    }
                    code = s.Remove(0, s.IndexOf('!') + 1);
                }
                else dataGridView1.Rows.Add(s.Remove(s.LastIndexOf('|')), DateTime.Parse(s.Remove(0, s.IndexOf('|') + 1)).ToString("f"), "Logoff");

            }
            CheckCount();
        }


        private void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (InvokeRequired) BeginInvoke(new UploadStringCompletedEventHandler(client_UploadStringCompleted), sender, e);
            else
            {
                try
                {
                    if (e.Result == "Done") dataGridView1.Rows.RemoveAt((int)e.UserState);
                    else MessageBox.Show(this, e.Result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch { }
                this.Enabled = true;
                this.Cursor = Cursors.Default;
                CheckCount();
            }
        }

        private void CheckCount()
        {
            if (MaxLogons == 0) Done.Enabled = true;
            else if (dataGridView1.Rows.Count < (MaxLogons - 1)) Done.Enabled = true;
            else Done.Enabled = false;
            KeepOpen = !Done.Enabled;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                WebClient client = new WebClient();
                client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_UploadStringCompleted);
                client.UploadStringAsync(new Uri(BaseUri, "tracker/api.ashx?op=" + Action.RemoteLogoff.ToString().ToLower()), "POST", dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), e.RowIndex);
                this.Enabled = false;
                this.Cursor = Cursors.AppStarting;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Override)
            {
                if (new OverrideCode(code).ShowDialog(this) == System.Windows.Forms.DialogResult.OK) { this.KeepOpen = false; this.Close(); }
            }
            else if (MessageBox.Show(this, "Clicking this button will result in the system logging you off.", "Logoff?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                WebClient client = new WebClient();
                client.UploadStringCompleted += new UploadStringCompletedEventHandler(client2_UploadStringCompleted);
                client.UploadStringAsync(new Uri(BaseUri, "tracker/api.ashx?op=" + Action.RemoteLogoff.ToString().ToLower()), "POST", Dns.GetHostName(), -1);
                this.Hide();
            }
            Override = false;
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }

        private void client2_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (InvokeRequired) BeginInvoke(new UploadStringCompletedEventHandler(client2_UploadStringCompleted), sender, e);
            else
            {
                try
                {
                    if (e.Result != "Done") MessageBox.Show(this, e.Result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch { }
                this.KeepOpen = false;
                this.Close();
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = KeepOpen;
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
