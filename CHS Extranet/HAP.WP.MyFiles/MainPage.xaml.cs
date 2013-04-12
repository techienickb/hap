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
using Microsoft.Phone.Controls;
using HAP.WP.MyFiles.JSON;
using Newtonsoft.Json;

namespace HAP.WP.MyFiles
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }
        Uri url = null;
        private void login_Click(object sender, RoutedEventArgs e)
        {
            loading.IsIndeterminate = true;
            login.IsEnabled = false;
            string s = address.Text;
            if (!s.EndsWith("/")) s += "/";

            try
            {
                ((App)App.Current).RootUrl = new Uri(s);
                url = new Uri(((App)App.Current).RootUrl, "./api/ad/");
            }
            catch
            {
                MessageBox.Show("That Address doesn't seem to be valid");
                loading.IsIndeterminate = false;
                login.IsEnabled = true;
            }
            if (url != null)
            {
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                wc.DownloadStringAsync(new Uri(url, "./" + username.Text + "/" + password.Password), url.ToString());
            }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show("Something went wrong while trying to authenticate you.\nYou can try again, or check your data connection");
            else
            {
                ((App)App.Current).JSONUser = JsonConvert.DeserializeObject<JSON.JSONUser>(e.Result);
                if (((App)App.Current).JSONUser.isValid)
                {
                    MessageBox.Show("Hello " + ((App)App.Current).JSONUser.FirstName + ", you are now connected via HAP+ to " + ((App)App.Current).JSONUser.SiteName + ",\n\nThis app is currently limited to browsing files from your School");
                    NavigationService.Navigate(new Uri("/Browser.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Your Username/Password conbination doesn't match a user at this school!", "Error", MessageBoxButton.OK);
                }
            }
            Dispatcher.BeginInvoke(() => { loading.IsIndeterminate = false; login.IsEnabled = true; });
        }
    }
}