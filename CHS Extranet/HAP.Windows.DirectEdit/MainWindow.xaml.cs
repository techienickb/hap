using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HAP.Win.DirectEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        string[] allowed = { ".docx", ".doc", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
        string filename = "";
        string[] parts = null;
        string url = "";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string p = Environment.GetCommandLineArgs()[1];
            if (p.ToLower().StartsWith("hap://")) p = p.Remove(0, 6);
            else if (p.ToLower().StartsWith("hap:/")) p = p.Remove(0, 5);
            else if (p.ToLower().StartsWith("hap:")) p = p.Remove(0, 4);
            if (p == "" || p == "/")
            {
                info.Text = "Initializing...";
                Hide();
                ShowInTaskbar = false;
                Thread.Sleep(3000);
                this.cancelclose = false;
                Close();
            } 
            else
            {
                p = Decrypt.ConvertToPlain(p);
                parts = p.Split(new char[] { '|' });
                url = p.Substring(p.IndexOf(parts[1]) + parts[1].Length + 1);
                if (url.ToLower().EndsWith("/api/myfiles/destatus/confirm"))
                {
                    this.cancelclose = false;
                    Hide();
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "HAPDirectEdit/1.0 (Windows)");
                    wc.Headers.Add(HttpRequestHeader.Cookie, ".ASPXAUTH=" + parts[1] + "; token=" + parts[0]);
                    wc.Headers.Add("Content-Type", "application/json");
                    wc.UploadString(new Uri(url), "POST", "");
                    Close();
                }
                else
                {
                    string file = Uri.UnescapeDataString(url.Replace('|', '%')).Replace('^', '&');
                    filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(file));
                    if (allowed.Contains(System.IO.Path.GetExtension(file).ToLower()))
                    {
                        info.Text = "Downloading " + System.IO.Path.GetFileName(file);
                        ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors){ return true; };
                        WebClient wc = new WebClient();
                        wc.Headers.Add(HttpRequestHeader.UserAgent, "HAPDirectEdit/1.0 (Windows)");
                        wc.Headers.Add(HttpRequestHeader.Cookie, ".ASPXAUTH=" + parts[1] + "; token=" + parts[0]);
                        wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                        wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                        wc.DownloadFileAsync(new Uri(url), filename);
                    }
                    else
                    {
                        this.cancelclose = false;
                        Hide();
                        MessageBox.Show(this, "Invalid File Type for Secure Download\n\n" + System.IO.Path.GetExtension(file).ToLower(), "Invalid File Type", MessageBoxButton.OK, MessageBoxImage.Error);
                        Close();
                    }
                }
            }
        }

        void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new UpdateProgress((o) =>
            {
                progress.Value = (int)o;
                info.Text = "Downloaded " + System.IO.Path.GetFileName(filename) + " Waiting to upload";
            }), 100);
            string ext = System.IO.Path.GetExtension(filename).ToLower();
            ProcessStartInfo psi = new ProcessStartInfo(filename);
            psi.UseShellExecute = true;
            Process p = Process.Start(psi);
            p.WaitForExit();

            info.Text = "Uploading " + System.IO.Path.GetFileName(filename);
            progress.Value = 0;
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            WebClient wc = new WebClient();
            wc.Headers.Clear();
            wc.Headers.Add(HttpRequestHeader.UserAgent, "HAPSecureEdit/1.0 (Windows)");
            wc.Headers.Add(HttpRequestHeader.Cookie, ".ASPXAUTH=" + parts[1] + "; HAPSecure=1; token=" + parts[0]);
            wc.Headers.Add("X_FILENAME", System.IO.Path.GetFileName(filename));
            wc.UploadFileCompleted += wc_UploadFileCompleted;
            wc.UploadProgressChanged += wc_UploadProgressChanged;
            string url1 = url.Remove(url.LastIndexOf('/') + 1).ToLower().Replace("download/", "api/myfiles-upload/");
            wc.UploadFileAsync(new Uri(url1), "POST", filename);
        }

        void wc_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new UpdateProgress((o) =>
            {
                progress.Value = (int)o;
            }), e.ProgressPercentage);
        }

        void wc_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            cancelclose = false;
            Close();
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new UpdateProgress((o) => {
                progress.Value = (int)o;
            }), e.ProgressPercentage);
        }

        delegate void UpdateProgress(object o);

        bool cancelclose = true;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cancelclose && Environment.GetCommandLineArgs().Length > 1) MessageBox.Show(this, "You still have the document open\nYou Cannot close the HAP+ DirectEdit Program", "Still Open", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Cancel = cancelclose;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (filename != "") System.IO.File.Delete(filename);
        }
    }
}
