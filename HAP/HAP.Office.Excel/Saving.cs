using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace HAP.Office.Excel
{
    public partial class Saving : Form
    {
        public Saving(string filepath, string uploadpath, string[] parts)
        {
            InitializeComponent();
            FilePath = filepath;
            UploadPath = uploadpath;
            Parts = parts;
        }

        private string[] Parts { get; set; }
        private string FilePath { get; set; }
        private string UploadPath { get; set; }

        private void Saving_Load(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            WebClient wc = new WebClient();
            wc.Headers.Clear();
            wc.Headers.Add(HttpRequestHeader.UserAgent, "HAPOfficeAdding/1.0 (Windows)");
            wc.Headers.Add(HttpRequestHeader.Cookie, Parts[2] + "=" + Parts[1] + "; token=" + Parts[0]);
            wc.Headers.Add("X_FILENAME", System.IO.Path.GetFileName(FilePath));
            wc.UploadDataCompleted += wc_UploadDataCompleted;
            wc.UploadProgressChanged += wc_UploadProgressChanged;
            using (FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                MemoryStream ms = new MemoryStream();
                fileStream.CopyTo(ms);
                wc.UploadDataAsync(new Uri(new Uri(Properties.Settings.Default.Url), UploadPath.Remove(UploadPath.LastIndexOf('/')).Replace("Download/", "api/myfiles-upload/")), "POST", ms.ToArray());
            }
        }

        void wc_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Invoke(new UpdateProgress((o) =>
            {
                Close();
            }), 100);
        }


        void wc_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            this.Invoke(new UpdateProgress((o) =>
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = (int)o;
            }), e.ProgressPercentage);
        }

        void wc_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Invoke(new UpdateProgress((o) =>
            {
                Close();
            }), 100);
        }

        delegate void UpdateProgress(object o);

    }
}
