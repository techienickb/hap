using HAP.Win.MyFiles.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace HAP.Win.MyFiles
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class Details : HAP.Win.MyFiles.Common.LayoutAwarePage
    {
        public Details()
        {
            this.InitializeComponent();
        }

        private string Path;
        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["path"];
            }
            loading.IsIndeterminate = true;
            Path = navigationParameter as string;
            if (Path == "")
            {
                Frame.GoBack();
                return;
            }
            HttpWebRequest req = WebRequest.CreateHttp(new Uri(HAPSettings.CurrentSite.Address, "./api/myfiles/Properties/" + Path.Replace('\\', '/').Replace("../Download/", "")));
            req.Method = "GET";
            req.ContentType = "application/json";

            req.CookieContainer = new CookieContainer();
            bool tokengood = false;
            try
            {
                req.CookieContainer.Add(HAPSettings.CurrentSite.Address, new Cookie("token", HAPSettings.CurrentToken[0]));
                req.CookieContainer.Add(HAPSettings.CurrentSite.Address, new Cookie(HAPSettings.CurrentToken[2], HAPSettings.CurrentToken[1]));
                tokengood = true;
            }
            catch
            {
                MessageDialog mes = new MessageDialog("Your Logon Token has Expired");
                mes.Commands.Add(new UICommand("OK"));
                mes.DefaultCommandIndex = 0;
                mes.ShowAsync();
                Frame.Navigate(typeof(MainPage));
            }
            if (tokengood)
            {
                HttpWebResponse x1 = null;
                try
                {
                    WebResponse x = await req.GetResponseAsync();
                    x1 = (HttpWebResponse)x;
                    JSONProperties prop = JsonConvert.DeserializeObject<JSONProperties>(new StreamReader(x1.GetResponseStream()).ReadToEnd());
                    DataContext = prop;
                    pageTitle.Text = prop.Name;
                }
                catch (Exception ex)
                {
                    MessageDialog mes = new MessageDialog(ex.ToString(), "An error has occured processing this request");
                    mes.Commands.Add(new UICommand("OK"));
                    mes.DefaultCommandIndex = 0;
                    mes.ShowAsync();
                    Frame.GoBack();
                }
            }
            loading.IsIndeterminate = false;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            pageState.Add("path", Path);
        }


        async void makefile(MemoryStream ms, JSONProperties jfile)
        {
            Windows.Storage.StorageFolder temporaryFolder = ApplicationData.Current.TemporaryFolder;
            StorageFile sampleFile = await temporaryFolder.CreateFileAsync(jfile.Name + jfile.Extension, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(sampleFile, ms.ToArray());

            if (EnsureUnsnapped())
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add(jfile.Type, new List<string>() { jfile.Extension });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = jfile.Name;

                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);
                    // write to file
                    await FileIO.WriteBytesAsync(file, ms.ToArray());
                    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    MessageDialog mes;
                    if (status == FileUpdateStatus.Complete)
                    {
                        mes = new MessageDialog("File " + file.Name + " was saved.", "File Operation");
                    }
                    else
                    {
                        mes = new MessageDialog("File " + file.Name + " couldn't be saved.", "File Operation");
                    }
                    mes.Commands.Add(new UICommand("Ok"));
                    mes.DefaultCommandIndex = 0;
                    var ignored1 = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { pro.Visibility = Windows.UI.Xaml.Visibility.Collapsed; mes.ShowAsync(); });
                }
                else
                {
                }
            }
            await sampleFile.DeleteAsync();
        }

        internal bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            //if (!unsnapped)
            //{
            //    NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage);
            //}

            return unsnapped;
        }

        private void downloadbutton_Click(object sender, RoutedEventArgs e)
        {
            loading.IsIndeterminate = true;
            pro.Visibility = Windows.UI.Xaml.Visibility.Visible;
            pro.Value = 0;
            HttpWebRequest req = WebRequest.CreateHttp(new Uri(HAPSettings.CurrentSite.Address, Path.Substring(1)));
            req.Method = "GET";
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(HAPSettings.CurrentSite.Address, new Cookie("token", HAPSettings.CurrentToken[0]));
            req.CookieContainer.Add(HAPSettings.CurrentSite.Address, new Cookie(HAPSettings.CurrentToken[2], HAPSettings.CurrentToken[1]));
            req.AllowReadStreamBuffering = false;
            req.BeginGetResponse(a =>
            {
                using (WebResponse response = req.EndGetResponse(a))
                {
                    int expected = (int)response.ContentLength;
                    using (System.IO.BinaryReader reader = new BinaryReader(response.GetResponseStream()))
                    {
                        MemoryStream ms = new MemoryStream(expected);
                        bool canwrite = true;
                        int count = 0, x = 0;
                        while (canwrite)
                        {
                            try
                            {
                                ms.WriteByte(reader.ReadByte());
                            }
                            catch { canwrite = false; }
                            count++;
                            x++;
                            if (x == 100) { var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { pro.Value = count * 100.0 / expected; }); x = 0; }

                        }
                        var ignored1 = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { makefile(ms, (JSONProperties)a.AsyncState); });
                        var ignored2 = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { pro.Value = 100.0; loading.IsIndeterminate = false; });
                    }
                }
            }
            , (JSONProperties)DataContext);
        }
    }
}
