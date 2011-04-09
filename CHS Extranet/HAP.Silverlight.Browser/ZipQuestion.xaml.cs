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
using HAP.Silverlight.Browser.service;
using System.ServiceModel;

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
            ArrayOfString aos = new ArrayOfString();
            foreach (BItem item in Items)
                aos.Add(Common.GetPath(item));
            apiSoapClient soap = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
            soap.ZIPCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(soap_ZIPCompleted);
            soap.ZIPAsync(Common.GetPath(ParentItem), namebox.Text, aos);
        }

        void soap_ZIPCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new UploadStringCompletedEventHandler(soap_ZIPCompleted2), sender, e);
        }

        private void soap_ZIPCompleted2(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            busyindicator.IsBusy = false;
            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            else
            {
                if (ZipQuestionComplete != null) ZipQuestionComplete(this, new RoutedEventArgs());
                this.DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public event RoutedEventHandler ZipQuestionComplete;
    }
}

