using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HAP.Win.MyFiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            login.IsEnabled = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HAPSettings hs = new HAPSettings();
            if (!hs.SiteContainer.Values.ContainsKey("site0"))
            {
                HAPSetting h1 = hs.Settings[hs.Settings.Keys.First()];
                address.Text = h1.Address.ToString();
                username.Text = h1.Username;
                password.Password = h1.Password;
            }

            address.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            address.Select(address.Text.Length, 0);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            loading.IsIndeterminate = true;
            login.IsEnabled = false;
            string s = address.Text;
            if (!s.EndsWith("/")) s += "/";
            System.Net.HttpWebRequest req = WebRequest.CreateHttp(new Uri(new Uri(s), "./api/ad"));
            req.Method = "POST";
            req.ContentType = "application/json";
            StreamWriter sw = new StreamWriter(await req.GetRequestStreamAsync());
            sw.Write("{ \"username\": \"" + username.Text + "\", \"password\": \"" + password.Password + "\" }");
            sw.Flush();
            WebResponse x = await req.GetResponseAsync();
            HttpWebResponse x1 = (HttpWebResponse)x;
            JSON.JSONUser user = JsonConvert.DeserializeObject<JSON.JSONUser>(new StreamReader(x1.GetResponseStream()).ReadToEnd());

            HAPSettings hs = new HAPSettings();
            hs.AddSite(new HAPSetting() { Name = user.SiteName, Address = new Uri(s), Password = password.Password, Username = username.Text });
            if (hs.SiteContainer.Values.ContainsKey("site0")) hs.RemoveSite("site0");

            HAPSettings.CurrentSite = hs.Settings[user.SiteName];
            HAPSettings.CurrentToken = user.ToString();

            MessageDialog mes = new MessageDialog("Hello " + user.FirstName + ",\n\nThis app is limited to browsing and download files from your School");
            mes.Commands.Add(new UICommand("OK"));
            mes.DefaultCommandIndex = 0;
            await mes.ShowAsync();
            Frame.Navigate(typeof(Browser), "");
        }
    }
}
