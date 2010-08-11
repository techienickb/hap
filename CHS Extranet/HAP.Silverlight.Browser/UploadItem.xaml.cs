using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Browser;
using System.Windows.Media.Imaging;

namespace HAP.Silverlight.Browser
{
    public partial class UploadItem : UserControl
    {
        public UploadItem()
        {
            InitializeComponent();
            Progress.Value = 0;
        }

        public UploadItem(FileInfo file, BItem item, ref StackPanel Queue, RoutedEventHandler Uploaded) : this()
        {
            File = file;
            ParentData = item;
            queue = Queue;
            State = UploadItemState.Checking;
            Check();
            this.Uploaded += Uploaded;
            BaseUri = new Uri(HtmlPage.Document.DocumentUri, ParentData.Path.Replace("api/mycomputer/list/", "api/mycomputer/upload/")).ToString();
        }

        public UploadItem(string name) : this()
        {
            this.name.Text = name;
            State = UploadItemState.Debug;
            Progress.Value = 30;
        }

        private StackPanel queue;
        public event RoutedEventHandler Uploaded;
        public BItem ParentData { get; set; }
        delegate void UpdateUIDelegate(string s);
        public UploadItemState State { get; set; }
        public long BytesUploaded { get; set; }
        public double Value { get { return Progress.Value; } }
        public double Value1 { get; set; }
        private FileInfo _file;
        public string BaseUri { get; set; }

        public FileInfo File {

            get { return _file; }
            set
            {
                _file = value;
                name.Text = _file.Name.Replace(File.Extension, "");
                FileSize.Text = parseLength(_file.Length);
            }
        }

        public static string parseLength(object size)
        {
            decimal d = decimal.Parse(size.ToString() + ".00");
            string[] s = { "bytes", "KB", "MB", "GB", "TB", "PB" };
            int x = 0;
            while (d > 1024)
            {
                d = d / 1024;
                x++;
            }
            d = Math.Round(d, 2);
            return d.ToString() + " " + s[x];
        }

        public void Check()
        {
            WebClient checkclient = new WebClient();
            checkclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(checkclient_DownloadStringCompleted);
            checkclient.DownloadStringAsync(new Uri(HtmlPage.Document.DocumentUri, ParentData.Path.Replace("api/mycomputer/list/", "api/mycomputer/check/") + "/" + File.Name), false);
        }

        void checkclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new DownloadStringCompletedEventHandler(checkclient_DownloadStringCompleted2), sender, e);
        }

        void checkclient_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            string[] s = e.Result.Split(new char[] { '\n' });
            if (s[0] == "EXISTS" && MessageBox.Show(File.Name + " already exists\nDo you want to overwrite?", "Overwrite", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;
            State = UploadItemState.Ready;
            queue.Children.Add(this);
            image1.Source = new BitmapImage(new Uri(HtmlPage.Document.DocumentUri, s[1]));
            if (queue.Children.IndexOf(this) == 0) Dispatcher.BeginInvoke(() => { Upload(); }); //Upload();
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            if (this.State == UploadItemState.Uploading) { MessageBox.Show("This file is currently uploading and can't be removed!"); return; }
            this.queue.Children.Remove(this);
        }

        public void UpdateUI(string s)
        {
            Progress.Value = Value1;
            if (State == UploadItemState.Done) Dispatcher.BeginInvoke(new RoutedEventHandler(Uploaded), this, new RoutedEventArgs());
        }

        public void Upload()
        {
            long temp = File.Length - BytesUploaded;

            context.IsEnabled = false;
            UriBuilder ub = new UriBuilder(BaseUri);
            bool complete = temp <= 20480;
            ub.Query = string.Format("{3}filename={0}&StartByte={1}&Complete={2}", File.Name, BytesUploaded, complete, string.IsNullOrEmpty(ub.Query) ? "" : ub.Query.Remove(0, 1) + "&");
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(ub.Uri);
            webrequest.Method = "POST";
            webrequest.BeginGetRequestStream(new AsyncCallback(WriteCallback), webrequest);
        }

        private void WriteCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webrequest = (HttpWebRequest)asynchronousResult.AsyncState;
            // End the operation.
            Stream requestStream = webrequest.EndGetRequestStream(asynchronousResult);

            byte[] buffer = new Byte[4096];
            int bytesRead = 0;
            int tempTotal = 0;
            Stream fileStream = File.OpenRead();
            fileStream.Position = BytesUploaded;

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0 && tempTotal + bytesRead < 20480)
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();
                BytesUploaded += bytesRead;
                tempTotal += bytesRead;
                Value1 = (((double)BytesUploaded / (double)File.Length) * 100);
                Dispatcher.BeginInvoke(new UpdateUIDelegate(UpdateUI), "");
            }
            fileStream.Close();
            requestStream.Close();
            webrequest.BeginGetResponse(new AsyncCallback(ReadCallback), webrequest);
        }

        private void ReadCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webrequest = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)webrequest.EndGetResponse(asynchronousResult);
            StreamReader reader = new StreamReader(response.GetResponseStream());

            string responsestring = reader.ReadToEnd();
            reader.Close();

            if (BytesUploaded < File.Length) Dispatcher.BeginInvoke(() => { Upload(); });
            else State = UploadItemState.Done; 
            Dispatcher.BeginInvoke(new UpdateUIDelegate(UpdateUI), "");
        }
    }

    public enum UploadItemState { Checking, Ready, Uploading, Done, Debug }
}
