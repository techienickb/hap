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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;

namespace HAP.WP.MyFiles
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
        }

        string path = "";
        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.TryGetValue("Path", out path))
            {
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                wc.Headers[HttpRequestHeader.Cookie] = string.Format("{0}={1}; token={2}", ((App)App.Current).JSONUser.Token2Name, ((App)App.Current).JSONUser.Token2, ((App)App.Current).JSONUser.Token1);
                wc.DownloadStringAsync(new Uri(((App)App.Current).RootUrl, "./api/myfiles/Properties/" + path.Replace('\\', '/').Replace("../Download/", "")));
            }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            else
            {
                DataContext = JsonConvert.DeserializeObject<JSON.JSONProperties>(e.Result);
                Dispatcher.BeginInvoke(() => { loading.IsIndeterminate = false; });
            }
        }
    }
}