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
            try
            {
                System.Net.HttpWebRequest req = WebRequest.CreateHttp(new Uri(new Uri(s), "./api/ad"));
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Proxy = WebRequest.DefaultWebProxy;
                HttpWebResponse x1 = null;
                try
                {
                    StreamWriter sw = new StreamWriter(await req.GetRequestStreamAsync());
                    sw.Write("{ \"username\": \"" + username.Text + "\", \"password\": \"" + password.Password + "\" }");
                    sw.Flush();
                    WebResponse x = await req.GetResponseAsync();
                    x1 = (HttpWebResponse)x;
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
                if (x1 != null)
                {
                    try
                    {
                        JSON.JSONUser user = JsonConvert.DeserializeObject<JSON.JSONUser>(new StreamReader(x1.GetResponseStream()).ReadToEnd());
                        if (user.isValid)
                        {
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
            catch
            {
                MessageDialog mes = new MessageDialog("That Address doesn't seem to be valid");
                mes.Commands.Add(new UICommand("OK"));
                mes.DefaultCommandIndex = 0;
                mes.ShowAsync();
                loading.IsIndeterminate = false;
                login.IsEnabled = true;
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
    }
}
