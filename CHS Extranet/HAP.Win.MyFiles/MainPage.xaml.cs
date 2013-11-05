using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

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
                foreach (HAPSetting h in hs.Settings.Values) sites.Items.Add(new ComboBoxItem() { Content = h.Name, DataContext = h });
                sites.SelectedIndex = 1;
            }
            else
            {
                address.Focus(Windows.UI.Xaml.FocusState.Keyboard);
                address.Select(address.Text.Length, 0);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            loading.IsIndeterminate = true;
            login.IsEnabled = false;
            string s = address.Text;
            if (!s.EndsWith("/")) s += "/";
            Uri url = null;
            try
            {
                url = new Uri(new Uri(s), "./api/ad/?" + DateTime.Now.Ticks);
            }
            catch
            {
                MessageDialog mes = new MessageDialog("That Address doesn't seem to be valid");
                mes.Commands.Add(new UICommand("OK"));
                mes.DefaultCommandIndex = 0;
                mes.ShowAsync();
                loading.IsIndeterminate = false;
                login.IsEnabled = true;
            }
            if (url != null)
            {
                HttpClient c = new HttpClient(HAPSettings.certfilter);
                c.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));
                HttpStringContent sc = new HttpStringContent("{ \"username\": \"" + username.Text + "\", \"password\": \"" + password.Password + "\" }", Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                string sc1 = null;
                try
                {
                    var cr = await c.PostAsync(url, sc);
                    sc1 = await cr.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    loading.IsIndeterminate = false;
                    login.IsEnabled = true;
                    MessageDialog mes = new MessageDialog("An Active Internet Connection is Required\n\n" + ex.ToString());
                    mes.Commands.Add(new UICommand("OK"));
                    mes.DefaultCommandIndex = 0;
                    mes.ShowAsync();
                }
                if (sc1 != null)
                {
                    try
                    {
                        JSON.JSONUser user = JsonConvert.DeserializeObject<JSON.JSONUser>(sc1);
                        if (user.isValid)
                        {
                            HAPSettings hs = new HAPSettings();
                            hs.AddSite(new HAPSetting() { Name = user.SiteName, Address = new Uri(s), Password = password.Password, Username = username.Text });
                            if (hs.SiteContainer.Values.ContainsKey("site0")) hs.RemoveSite("site0");

                            HAPSettings.CurrentSite = hs.Settings[user.SiteName];
                            HAPSettings.CurrentToken = user.ToString();

                            MessageDialog mes = new MessageDialog("Hello " + user.FirstName + ", you are now connected via HAP+ to " + user.SiteName + "");
                            mes.Commands.Add(new UICommand("OK"));
                            mes.DefaultCommandIndex = 0;
                            await mes.ShowAsync();
                            Frame.Navigate(typeof(Browser), "");
                        }
                        else
                        {
                            MessageDialog mes = new MessageDialog("Your Username/Password conbination doesn't match a user at this school!", "Error");
                            mes.Commands.Add(new UICommand("OK"));
                            mes.DefaultCommandIndex = 0;
                            await mes.ShowAsync();
                            loading.IsIndeterminate = false;
                            login.IsEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageDialog mes = new MessageDialog((ex.ToString().Contains("404") ? "Your School's Home Access Plus+ Install isn't up-to-date\n\nPlease ask them to upgrade to HAP+ v9 or above" : "Communication Error\n\n" + ex.ToString()));
                        mes.Commands.Add(new UICommand("OK"));
                        mes.DefaultCommandIndex = 0;
                        mes.ShowAsync();
                    }
                }
            }
        }

        private void sites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            var no = box.SelectedIndex;
            try
            {
                if (no == 0)
                {
                    address.Text = "https://";
                    username.Text = "";
                    password.Password = "";
                    removesite.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    address.Focus(Windows.UI.Xaml.FocusState.Keyboard);
                    address.Select(address.Text.Length, 0);
                }
                else if (no > 0)
                {
                    HAPSetting h = (HAPSetting)((ComboBoxItem)box.SelectedItem).DataContext;
                    address.Text = h.Address.ToString();
                    username.Text = h.Username;
                    password.Password = h.Password;
                    removesite.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    login.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
            }
            catch { }
        }

        private void removesite_Click(object sender, RoutedEventArgs e)
        {
            string name = ((HAPSetting)((ComboBoxItem)sites.SelectedItem).DataContext).Name;
            int index = sites.SelectedIndex;
            new HAPSettings().RemoveSite(name);
            sites.SelectedIndex = 0;
            sites.Items.RemoveAt(index);
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewUser));
        }
    }
}
