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
using HAP.Silverlight.Browser.service;
using System.ServiceModel;

namespace HAP.Silverlight.Browser
{
    public partial class UploadItem : UserControl
    {
        public UploadItem()
        {
            InitializeComponent();
            Progress.Value = 0;
            BytesUploaded = 0;
        }

        public UploadItem(FileInfo file, BItem item, ref StackPanel Queue, RoutedEventHandler Uploaded) : this()
        {
            File = file;
            ParentData = item;
            queue = Queue;
            State = UploadItemState.Checking;
            Check();
            this.Uploaded += Uploaded;
        }

        public UploadItem(string name) : this()
        {
            this.name.Text = name;
            State = UploadItemState.Debug;
            Progress.Value = 30;
        }

        public apiSoapClient soap { get; set; }
        private StackPanel queue;
        public event RoutedEventHandler Uploaded;
        public BItem ParentData { get; set; }
        delegate void UpdateUIDelegate(string s);
        public UploadItemState State { get; set; }
        public long BytesUploaded { get; set; }
        public double Value { get { return Progress.Value; } }
        public double Value1 { get; set; }
        private FileInfo _file;

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
            soap = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
            soap.UploadFileCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(soap_UploadFileCompleted);
            soap.CheckFileCompleted += new EventHandler<CheckFileCompletedEventArgs>(soap_CheckFileCompleted);
            soap.CheckFileAsync(ParentData.Path + '\\' + File.Name, App.Current.Resources["token"].ToString());
        }

        void soap_CheckFileCompleted(object sender, CheckFileCompletedEventArgs e)
        {
           Dispatcher.BeginInvoke(new EventHandler<CheckFileCompletedEventArgs>(soap_CheckFileCompleted2), sender, e);
        }

        void soap_CheckFileCompleted2(object sender, CheckFileCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            else
            {
                if (e.Result.Code == FileCheckResponseCode.Exists && MessageBox.Show(File.Name + " already exists\nDo you want to overwrite?", "Overwrite", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;
                State = UploadItemState.Ready;
                queue.Children.Add(this);
                image1.Source = new BitmapImage(new Uri(HtmlPage.Document.DocumentUri, e.Result.Thumb));
                BytesUploaded = 0;
                if (queue.Children.IndexOf(this) == 0) Dispatcher.BeginInvoke(() => { Upload(); }); //Upload();

            }
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
            soap = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
            soap.UploadFileCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(soap_UploadFileCompleted);
            long temp = File.Length - BytesUploaded;
            int buffersize = 4096;
            if (temp < 4096) buffersize = Convert.ToInt32(temp) + 1;
            byte[] buffer = new Byte[buffersize];
            Stream fileStream = File.OpenRead();
            fileStream.Position = BytesUploaded;
            int a = buffer.Length;
            if (temp < a) a = Convert.ToInt32(temp);
            int bytesRead = fileStream.Read(buffer, 0, a);
            soap.UploadFileAsync(ParentData.Path + '\\' + File.Name, BytesUploaded, buffer, false);
            BytesUploaded += bytesRead;
            Value1 = (((double)BytesUploaded / (double)File.Length) * 100);
            fileStream.Close();
            fileStream.Dispose();
        }

        void soap_UploadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            if (BytesUploaded < File.Length) Dispatcher.BeginInvoke(() => { Upload(); });
            else State = UploadItemState.Done;
            Dispatcher.BeginInvoke(new UpdateUIDelegate(UpdateUI), "");
        }
    }

    public enum UploadItemState { Checking, Ready, Uploading, Done, Debug }
}
