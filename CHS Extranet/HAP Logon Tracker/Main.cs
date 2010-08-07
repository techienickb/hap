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
        }

        public Uri BaseUri { get; set; }

        public int MaxLogons { get; set; }
        public void SetGrid(string data)
        {
            data = data.Remove(0, 7);
            foreach (string s in data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if ((s.StartsWith("Student") || s.StartsWith("Staff")) && s.Contains(":"))
                {
                    MaxLogons = int.Parse(s.Remove(0, s.IndexOf(':')).TrimStart(new char[] { ':' }));
                    label2.Text = string.Format(label2.Text, MaxLogons);
                }
                else
                    dataGridView1.Rows.Add(s.Remove(s.LastIndexOf('|')).TrimEnd(new char[] { '|' }), DateTime.Parse(s.Remove(0, s.IndexOf('|')).TrimStart(new char[] { '|' })).ToString("f"), "Logoff");
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
                this.Enabled = true;
                this.Cursor = Cursors.Default;
                CheckCount();
            }
        }

        private void CheckCount()
        {
            if (MaxLogons == 0) button1.Enabled = true;
            else if (dataGridView1.Rows.Count <= MaxLogons) button1.Enabled = true;
            else button1.Enabled = false;
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
    }
}
