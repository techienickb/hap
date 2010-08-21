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
    public partial class UnZip : ChildWindow
    {
        public UnZip(BItem Item, BItem ParentItem, RoutedEventHandler CompletedEv)
        {
            InitializeComponent();
            this.Item = Item;
            this.ParentItem = ParentItem;
            foldername.Text = Item.Name;
            Completed += CompletedEv;
        }

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
            WebClient zipclient = new WebClient();
            zipclient.UploadStringCompleted += new UploadStringCompletedEventHandler(zipclient_UploadStringCompleted);
            zipclient.UploadStringAsync(Common.GetUri(Item, UriType.Unzip), "POST", _d);
        }

        private void zipclient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new UploadStringCompletedEventHandler(zipclient_UploadStringCompleted2), sender, e);
        }

        private void zipclient_UploadStringCompleted2(object sender, UploadStringCompletedEventArgs e)
        {
            busyindicator.IsBusy = false;
            if (e.Result == "DONE")
            {
                if (Completed != null) Completed(this, new RoutedEventArgs());
                this.DialogResult = true;
            }
            else MessageBox.Show(e.Result);
        }

        private void CurrentFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string _d = Common.GetPath(ParentItem);
            WebClient zipclient = new WebClient();
            zipclient.UploadStringCompleted += new UploadStringCompletedEventHandler(zipclient_UploadStringCompleted);
            zipclient.UploadStringAsync(Common.GetUri(Item, UriType.Unzip), "POST", _d);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

