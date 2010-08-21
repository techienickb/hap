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
using System.Windows.Browser;

namespace HAP.Silverlight.Browser
{
    public partial class ZipQuestion : ChildWindow
    {
        public ZipQuestion(BItem[] items, BItem Parent)
        {
            InitializeComponent();
            this.Items = items;
            this.ParentItem = Parent;
            this.namebox.Text = items[0].Name + ".zip";
            this.namebox.Focus();
            this.namebox.Select(0, items[0].Name.Length);
        }

        public BItem[] Items { get; set; }
        public BItem ParentItem { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = true;
            busyindicator.IsBusy = true;
            List<string> ds = new List<string>();
            foreach (BItem item in Items)
                ds.Add(Common.GetPath(item));
            string _d = string.Join("\n", ds.ToArray());
            WebClient zipclient = new WebClient();
            zipclient.UploadStringCompleted += new UploadStringCompletedEventHandler(zipclient_UploadStringCompleted);
            zipclient.UploadStringAsync(new Uri(HtmlPage.Document.DocumentUri, "api/mycomputer/zip/" + Common.GetPath(ParentItem) + "/" + namebox.Text), "POST", _d);
        }

        private void zipclient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new UploadStringCompletedEventHandler(zipclient_UploadStringCompleted2), sender, e);
        }

        private void zipclient_UploadStringCompleted2(object sender, UploadStringCompletedEventArgs e)
        {
            busyindicator.IsBusy = false;
            try
            {
                if (e.Result == "DONE")
                {
                    if (ZipQuestionComplete != null) ZipQuestionComplete(this, new RoutedEventArgs());
                    this.DialogResult = true;
                }
                else MessageBox.Show(e.Result);
            }
            catch (Exception ex) { MessageBox.Show("EX:" + ex.ToString()); }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public event RoutedEventHandler ZipQuestionComplete;
    }
}

