using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
                parts = Decrypt.ConvertToPlain(p).Split(new char[] { '|' });
                Uri uri = new Uri(parts[2]);
                filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(uri.LocalPath));
                if (allowed.Contains(System.IO.Path.GetExtension(filename).ToLower()))
                {
                    info.Text = "Downloading " + System.IO.Path.GetFileName(uri.LocalPath);
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "HAPDirectEdit/1.0 (Windows)");
                    wc.Headers.Add(HttpRequestHeader.Cookie, ".ASPXAUTH=" + parts[1] + "; token=" + parts[0]);
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(uri, filename);
                }
                else
                {
                    this.cancelclose = false;
                    Hide();
                    MessageBox.Show(this, "Invalid File Type for Secure Download", "Invalid File Type", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
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
            WebClient wc = new WebClient();
            wc.Headers.Clear();
            wc.Headers.Add(HttpRequestHeader.UserAgent, "HAPSecureEdit/1.0 (Windows)");
            wc.Headers.Add(HttpRequestHeader.Cookie, ".ASPXAUTH=" + parts[1] + "; HAPSecure=1; token=" + parts[0]);
            wc.Headers.Add("X_FILENAME", System.IO.Path.GetFileName(filename));
            wc.UploadFileCompleted += wc_UploadFileCompleted;
            wc.UploadProgressChanged += wc_UploadProgressChanged;
            string url = parts[2].Remove(parts[2].IndexOf(System.IO.Path.GetFileName(filename))).Replace("Download/", "api/myfiles-upload/");
            wc.UploadFileAsync(new Uri(url), filename);
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
