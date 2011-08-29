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
using System.Windows.Media.Imaging;
using System.Windows.Browser;

namespace HAP.Silverlight
{
    public partial class File : UserControl
    {
        public File()
        {
            InitializeComponent();
            BytesUploaded = 0;
        }

        public string BaseUri { get; set; }
        public string path { get; set; }
        private FileInfo file;
        public FileInfo Fileinfo
        {
            get { return file; }
            set
            {
                file = value;
                filename.Content = file.Name;
                status.Content = string.Format("{0} - {1}", parseLength(file.Length), Status);
            }
        }

        private string parseLength(object size)
        {
            decimal d = decimal.Parse(size.ToString() + ".00");
            string[] s = { "bytes", "kb", "mb", "gb", "tb", "pb" };
            int x = 0;
            while (d > 1024)
            {
                d = d / 1024;
                x++;
            }
            d = Math.Round(d, 2);
            return d.ToString() + " " + s[x];
        }

        delegate void UpdateUIDelegate(string s);
        public string Status { get; set; }

        public void UpdateUI(string s)
        {
            status.Content = string.Format("{0} - {1}", parseLength(file.Length), Status);
            progress.Value = Progress;
            if (Status == "Done" && Uploaded != null) Uploaded(this, new RoutedEventArgs());
        }

        public event RoutedEventHandler Uploaded;

        public double Progress { get; set; }

        public void Check()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(BaseUri.ToString() + "check/" + (path.TrimEnd(new char[] { '/' }) + "/" + Fileinfo.Name).Replace('&', '^')));            
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string[] res = e.Result.Split(new char[] { ',' });

            bool exists = bool.Parse(res[0]);
            if (exists)
                if (MessageBox.Show(file.Name + " already exists, do you want to overwrite it?", "Overwrite File", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    Status = "Ready (Will Overwrite)";
                else { if (RemoveClick != null) RemoveClick(this, new RoutedEventArgs()); return; }
            else Status = "Ready";
            image.Source = new BitmapImage(new Uri(BaseUri.Replace("upload", "images/icons") + res[1]));
            Dispatcher.BeginInvoke(new UpdateUIDelegate(UpdateUI), "");
        }

        public event RoutedEventHandler RemoveClick;

        private void removebutton_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveClick != null) RemoveClick(this, e);
        }

        public long BytesUploaded { get; set; } 

        public void Upload()
        {
            long temp = Fileinfo.Length - BytesUploaded;

            UriBuilder ub = new UriBuilder(BaseUri + "transfer/" + path.TrimEnd(new char[] { '/' }).Replace('&', '^'));
            bool complete = temp <= 20480;
            ub.Query = string.Format("{3}filename={0}&StartByte={1}&Complete={2}", Fileinfo.Name.Replace('&', '^'), BytesUploaded, complete, string.IsNullOrEmpty(ub.Query) ? "" : ub.Query.Remove(0, 1) + "&");
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
            Stream fileStream = Fileinfo.OpenRead();
            fileStream.Position = BytesUploaded;

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0 && tempTotal + bytesRead < 20480)
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();
                BytesUploaded += bytesRead;
                tempTotal += bytesRead;
                Progress = (((double)BytesUploaded / (double)Fileinfo.Length) * 100);
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

            if (BytesUploaded < Fileinfo.Length) Upload();
            else { Status = "Done"; Dispatcher.BeginInvoke(new RoutedEventHandler(Uploaded), this, new RoutedEventArgs()); }
            Dispatcher.BeginInvoke(new UpdateUIDelegate(UpdateUI), "");
        }
    }
}
