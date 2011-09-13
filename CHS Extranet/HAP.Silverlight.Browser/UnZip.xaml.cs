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
    public partial class UnZip : ChildWindow
    {
        public UnZip(BItem Item, BItem ParentItem, RoutedEventHandler CompletedEv)
        {
            InitializeComponent();
            this.Item = Item;
            this.ParentItem = ParentItem;
            foldername.Text = Item.Name;
            Completed += CompletedEv;
            soap = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
            soap.UnzipCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(soap_UnzipCompleted);
        }

        public apiSoapClient soap { get; set; }

        public BItem Item { get; set; }
        public BItem ParentItem { get; set; }

        private void SpecifcFolder_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).BorderBrush = Resources["LBB"] as SolidColorBrush;
            ((Border)sender).Background = Resources["hover"] as LinearGradientBrush;
        }

        private void SpecifcFolder_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).BorderBrush = ((Border)sender).Background = new SolidColorBrush(Colors.Transparent);
        }

        private void SpecifcFolder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Border)sender).Background = Resources["ActiveBG"] as LinearGradientBrush;
        }

        public event RoutedEventHandler Completed;

        private void SpecifcFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            busyindicator.IsBusy = true;
            string _d = Common.GetPath(ParentItem) + "/" + foldername.Text;
            soap.UnzipAsync(Item.Path, _d, App.Current.Resources["token"].ToString());
        }

        void soap_UnzipCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(soap_UnzipCompleted2), sender, e);
        }

        void soap_UnzipCompleted2(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            busyindicator.IsBusy = false;
            if (e.Error == null)
            {
                if (Completed != null) Completed(this, new RoutedEventArgs());
                this.DialogResult = true;
            }
            else MessageBox.Show(e.Error.ToString());
        }

        private void CurrentFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string _d = Common.GetPath(ParentItem);
            soap.UnzipAsync(Item.Path, _d, App.Current.Resources["token"].ToString());
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

