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
using Newtonsoft.Json;
using HAP.WP.MyFiles.JSON;

namespace HAP.WP.MyFiles
{
    public partial class Browser : PhoneApplicationPage
    {
        // Constructor
        public Browser()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Browser_Loaded);
        }

        // Handle selection changed on ListBox
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
                return;
            JSONFile jfile = ((JSONFile)MainListBox.SelectedValue);
            if (jfile.Type != null && jfile.Type == "Directory")
            {
                path = jfile.Path;
                NavigationService.Navigate(new Uri("/Browser.xaml?Path=" + jfile.Path, UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/DetailsPage.xaml?Path=" + jfile.Path, UriKind.Relative));
            }
        }

        private void DriveListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (DriveListBox.SelectedIndex == -1)
                return;
            // Navigate to the new page
            path = ((JSONDrive)DriveListBox.SelectedValue).Path;
            NavigationService.Navigate(new Uri("/Browser.xaml?Path=" + path, UriKind.Relative));
        }

        private void LoadNewPath()
        {
            if (path == null) path = "";
            DataContext = null;
            MainListBox.Visibility = System.Windows.Visibility.Collapsed;
            DriveListBox.Visibility = System.Windows.Visibility.Collapsed;
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            wc.Headers[HttpRequestHeader.Cookie] = string.Format("{0}={1}; token={2}", ((App)App.Current).JSONUser.Token2Name, ((App)App.Current).JSONUser.Token2, ((App)App.Current).JSONUser.Token1);
            if (path == "")
            {
                DriveListBox.Visibility = System.Windows.Visibility.Visible;
                PageTitle.Text = "Drives";
                wc.DownloadStringAsync(new Uri(((App)App.Current).RootUrl, "./api/myfiles/drives"));
                MainListBox.SelectedIndex = DriveListBox.SelectedIndex = -1;
            }
            else
            {
                MainListBox.Visibility = System.Windows.Visibility.Visible;
                PageTitle.Text = path.Replace("/", "\\").Replace("\\", " \\ ");
                if (PageTitle.Text.IndexOf('\\') != PageTitle.Text.LastIndexOf('\\'))
                {
                    PageTitle.Text = PageTitle.Text.Remove(PageTitle.Text.IndexOf('\\'), PageTitle.Text.LastIndexOf('\\') - PageTitle.Text.IndexOf('\\')).Replace("\\", "...\\");
                }
                wc.DownloadStringAsync(new Uri(((App)App.Current).RootUrl, "./api/myfiles/" + path.Replace('\\', '/')));
                MainListBox.SelectedIndex = DriveListBox.SelectedIndex = -1;
            }
        }

        string path = "";
        // Load data for the ViewModel Items
        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationContext.QueryString.TryGetValue("Path", out path);
            loading.IsIndeterminate = true;
            LoadNewPath();
            
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            else
            {
                if (path == "") DataContext = JsonConvert.DeserializeObject<JSON.JSONDrive[]>(e.Result);
                else DataContext = JsonConvert.DeserializeObject<JSON.JSONFile[]>(e.Result);
                Dispatcher.BeginInvoke(() => { loading.IsIndeterminate = false; });
            }
        }

    }
}