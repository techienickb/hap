using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word2 = Microsoft.Office.Interop.Word;

namespace HAP.Office.Word
{
    public partial class OpenBrowser : Form
    {
        public OpenBrowser(bool Save)
        {
            InitializeComponent();
            SaveMode = panel2.Visible = button2.Visible = Save;
            button2.Enabled = false;
            if (SaveMode)
            {
                filenameInput.Text = "";
                fileType.SelectedIndex = 0;
            }
        }

        private bool SaveMode { get; set; }
        private string[] token;

        private void OpenBrowser_Load(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(Start)).Start();
        }

        private void Start()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            string ret = string.Empty;

            StreamWriter requestWriter;
            var webRequest = System.Net.WebRequest.Create(new Uri(new Uri(Properties.Settings.Default.Url), "./api/ad/?" + DateTime.Now.Ticks)) as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.Method = "POST";

                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 20000;

                webRequest.ContentType = "application/json";
                //POST the data.
                using (requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write("{ \"username\": \"" + Properties.Settings.Default.Username + "\", \"password\": \"" + Properties.Settings.Default.Password + "\" }");
                }
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();

            if (ret != null || ret != string.Empty)
            {
                JSONUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONUser>(ret);
                if (user.isValid)
                {
                    token = user.ToString();
                    LoadDrives();
                }
                else
                {
                    MessageBox.Show("Your Username/Password conbination doesn't match a user at this school!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Error loading", "Error reading response from HAP+ Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private JSONDrive[] drives;
        private string path = "";
        private void LoadDrives()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            string ret = string.Empty;

            var webRequest = System.Net.WebRequest.Create(new Uri(new Uri(Properties.Settings.Default.Url), "./api/myfiles/drives?" + DateTime.Now.Ticks)) as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.Method = "GET";
                webRequest.CookieContainer = new CookieContainer();
                webRequest.CookieContainer.Add(new Cookie("token", token[0], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.CookieContainer.Add(new Cookie(token[2], token[1], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 20000;

                webRequest.ContentType = "application/json";
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();

            if (ret != null || ret != string.Empty)
            {
                drives = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONDrive[]>(ret);
                this.BeginInvoke(new Action(() => {
                    button2.Enabled = false;
                    listView1.Items.Clear();
                    foreach (JSONDrive drive in drives)
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { drive.Name, "", drive.Path, "", "" }));
                    }
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.Value = 100;
                }));
            }
            else
            {
                MessageBox.Show("Error loading", "Error reading response from HAP+ Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

        }
        private JSONFile[] files;
        private void LoadFile()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            string ret = string.Empty;

            var webRequest = System.Net.WebRequest.Create(new Uri(new Uri(Properties.Settings.Default.Url), "./api/myfiles/" + path.Replace('\\', '/') + "?" + DateTime.Now.Ticks)) as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.Method = "GET";
                webRequest.CookieContainer = new CookieContainer();
                webRequest.CookieContainer.Add(new Cookie("token", token[0], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.CookieContainer.Add(new Cookie(token[2], token[1], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 20000;

                webRequest.ContentType = "application/json";
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();

            if (SaveMode)
            {
                new Thread(new ThreadStart(LoadPerms)).Start();
            }

            if (ret != null || ret != string.Empty)
            {
                files = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONFile[]>(ret);
                this.BeginInvoke(new Action(() =>
                {
                    listView1.Items.Clear();
                    listView1.Items.Add(new ListViewItem(new string[] { "...", "", path.Length == 2 ? "" : path.Remove(path.LastIndexOf('\\')) }));
                    foreach (JSONFile file in files)
                    {
                        if (file.Extension == "" || Regex.IsMatch(file.Extension, ".*\\.(doc||docx||dotx||dot)$", RegexOptions.IgnoreCase))
                            listView1.Items.Add(new ListViewItem(new string[] { file.Name, file.Size, file.Path, file.Type, file.Extension }));
                    }
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.Value = 100;
                }));
            }
            else
            {
                MessageBox.Show("Error loading", "Error reading response from HAP+ Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private JSONUploadParams prop;
        private void LoadPerms()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            string ret = string.Empty;
            var webRequest = System.Net.WebRequest.Create(new Uri(new Uri(Properties.Settings.Default.Url), "./api/myfiles/UploadParams/" + path.Replace('\\', '/') + "?" + DateTime.Now.Ticks)) as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.Method = "GET";
                webRequest.CookieContainer = new CookieContainer();
                webRequest.CookieContainer.Add(new Cookie("token", token[0], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.CookieContainer.Add(new Cookie(token[2], token[1], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 20000;

                webRequest.ContentType = "application/json";
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();
            if (ret != null || ret != string.Empty)
            {
                prop = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONUploadParams>(ret);
                this.BeginInvoke(new Action(() =>
                {
                    if (prop.Properties.Permissions.AppendData || prop.Properties.Permissions.Modify || prop.Properties.Permissions.Write) button2.Enabled = true;
                }));
            }
            else
            {
                MessageBox.Show("Error loading", "Error reading response from HAP+ Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        string filename = "";
        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            string temppath = listView1.SelectedItems[0].SubItems[2].Text;
            if (temppath.StartsWith(".."))
            {
                if (SaveMode)
                {
                    filenameInput.Text = listView1.SelectedItems[0].SubItems[0].Text;
                    fileType.SelectedIndex = Regex.IsMatch(listView1.SelectedItems[0].SubItems[4].Text, ".*\\.(doc||docx)") ? 0 : 1;
                    if (button2.Enabled) button2.PerformClick();
                }
                else
                {
                    button1.Enabled = listView1.Enabled = false;
                    progressBar1.Value = 0;
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    new Thread(new ParameterizedThreadStart(Download)).Start(new string[] { filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), temppath.Remove(0, temppath.LastIndexOf('/') + 1)), path = temppath.Remove(0, 1) });
                }
            }
            else
            {
                path = temppath;
                progressBar1.Style = ProgressBarStyle.Marquee;
                if (path.Length == 1) path += "\\";
                if (path == "") new Thread(new ThreadStart(LoadDrives)).Start();
                else new Thread(new ThreadStart(LoadFile)).Start();
            }
        }

        private void Download(object o)
        {
            string[] s1 = o as string[];

            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            var webRequest = System.Net.WebRequest.Create(new Uri(new Uri(Properties.Settings.Default.Url), s1[1])) as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.Method = "GET";
                webRequest.CookieContainer = new CookieContainer();
                webRequest.CookieContainer.Add(new Cookie("token", token[0], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.CookieContainer.Add(new Cookie(token[2], token[1], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.ServicePoint.Expect100Continue = false;
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            FileStream fs = File.Create(s1[0]);
            resStream.CopyTo(fs);
            fs.Close();

            this.Invoke(new Action(() =>
            {
                progressBar1.Value = 100;
                progressBar1.Style = ProgressBarStyle.Blocks;
                try
                {
                    Globals.ThisAddIn.Application.Documents.Open(filename);
                    Globals.ThisAddIn.Application.DocumentBeforeClose +=Application_DocumentBeforeClose;
                    WordSaveHandler wsh = new WordSaveHandler(Globals.ThisAddIn.Application);
                    wsh.AfterSaveEvent += wsh_AfterSaveEvent;
                    wsh.AfterUiSaveEvent += wsh_AfterUiSaveEvent;
                    Globals.ThisAddIn.Shutdown += ThisAddIn_Shutdown;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Close();
            }));
        }

        bool SaveAsd = false;

        void wsh_AfterUiSaveEvent(Word2.Document doc, bool isClosed)
        {
            SaveAsd = true;
            if (SaveMode)
            {
                SaveAsd = false;
                new Saving(filename, path, token).ShowDialog();
                if (isClosed) File.Delete(filename);

            } else if (File.Exists(filename)) File.Delete(filename);
        }

        void Application_DocumentBeforeClose(Word2.Document Doc, ref bool Cancel)
        {
            if (!SaveAsd && !Doc.Saved && MessageBox.Show("If you wish to Save the changes and upload them back to HAP+, please use the File > Save function first before closing.\nDo you still wish to close?", "Question", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) Cancel = true;
        }

        void wsh_AfterSaveEvent(Word2.Document doc, bool isClosed)
        {
            if (!SaveAsd)
            {
                new Saving(filename, path, token).ShowDialog();
                if (isClosed) File.Delete(filename);
            }
        }

        void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            if (File.Exists(filename)) File.Delete(filename);
        }

        private void CheckFile(object o)
        {
            string[] os = o as string[];
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            string ret = string.Empty;
            path = path.Replace('\\', '/');
            string p2 = path + (path.EndsWith("/") ? "" : "/") + os[0] + "." + (os[1] == "0" ? "docx" : "dotx");
            var webRequest = System.Net.WebRequest.Create(new Uri(new Uri(Properties.Settings.Default.Url), "./api/myfiles/exists/" + p2 + "?" + DateTime.Now.Ticks)) as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.Method = "GET";
                webRequest.CookieContainer = new CookieContainer();
                webRequest.CookieContainer.Add(new Cookie("token", token[0], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.CookieContainer.Add(new Cookie(token[2], token[1], "/", new Uri(Properties.Settings.Default.Url).Host));
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 20000;

                webRequest.ContentType = "application/json";
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();
            if (ret != null || ret != string.Empty)
            {
                JSONProperties p = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONProperties>(ret);
                if (p.Name == null || MessageBox.Show("Do you want to overwrite this file?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    this.BeginInvoke(new Action(() =>
                    {
                        filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), os[0] + "." + (os[1] == "0" ? "docx" : "dotx"));
                        path = "./api/myfiles-upload/" + path + (path.EndsWith("/") ? "" : "/") + os[0] + "." + (os[1] == "0" ? "docx" : "dotx");
                        Globals.ThisAddIn.Application.DocumentBeforeClose += Application_DocumentBeforeClose;
                        WordSaveHandler wsh = new WordSaveHandler(Globals.ThisAddIn.Application);
                        wsh.AfterSaveEvent += wsh_AfterSaveEvent;
                        wsh.AfterUiSaveEvent += wsh_AfterUiSaveEvent;
                        Globals.ThisAddIn.Shutdown += ThisAddIn_Shutdown;
                        Globals.ThisAddIn.Application.ActiveDocument.SaveAs2(filename, AddToRecentFiles: false);
                        SaveAsd = false;
                        Close();
                    }));
                else
                {
                    this.BeginInvoke(new Action(() =>
                        {
                            button1.Enabled = button2.Enabled = listView1.Enabled = true;
                            progressBar1.Value = 0;
                            progressBar1.Style = ProgressBarStyle.Blocks;
                        }));
                }
            }
            else
            {
                MessageBox.Show("Error loading", "Error reading response from HAP+ Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        delegate void UpdateProgress(object o);

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = button2.Enabled = listView1.Enabled = false;
            progressBar1.Value = 0;
            progressBar1.Style = ProgressBarStyle.Marquee;
            new Thread(new ParameterizedThreadStart(CheckFile)).Start(new string[] { filenameInput.Text, fileType.SelectedIndex.ToString() });
        }
    }
}
