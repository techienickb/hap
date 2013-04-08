using HAP.Win.MyFiles.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace HAP.Win.MyFiles
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class Browser : HAP.Win.MyFiles.Common.LayoutAwarePage
    {
        public Browser()
        {
            this.InitializeComponent();
        }

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
            loading.IsIndeterminate = true;
            string path = navigationParameter as string;
            HttpWebRequest req;
            if (path == "")
            {
                pageTitle.Text = "My Drives";
                req = WebRequest.CreateHttp(new Uri(HAPSettings.CurrentSite.Address, "./api/myfiles/drives"));
            }
            else
            {
                pageTitle.Text = path.Replace("/", "\\").Replace("\\", " \\ ");
                req = WebRequest.CreateHttp(new Uri(HAPSettings.CurrentSite.Address, "./api/myfiles/" + path.Replace('\\', '/')));
            }

            req.Method = "GET";
            req.ContentType = "application/json";

            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(HAPSettings.CurrentSite.Address, new Cookie("token", HAPSettings.CurrentToken[0]));
            req.CookieContainer.Add(HAPSettings.CurrentSite.Address, new Cookie(HAPSettings.CurrentToken[2], HAPSettings.CurrentToken[1]));
            WebResponse x = await req.GetResponseAsync();
            HttpWebResponse x1 = (HttpWebResponse)x;
            if (path == "")
            {
                JSONDrive[] drives = JsonConvert.DeserializeObject<JSONDrive[]>(new StreamReader(x1.GetResponseStream()).ReadToEnd());
                itemsViewSource.Source = drives;
                driveGridView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                fileGridView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                driveGridView.SelectedIndex = -1;
            }
            else
            {
                JSONFile[] files = JsonConvert.DeserializeObject<JSONFile[]>(new StreamReader(x1.GetResponseStream()).ReadToEnd());
                itemsViewSource.Source = files;
                driveGridView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                fileGridView.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            loading.IsIndeterminate = false;
        }

        private void driveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(Browser), ((JSONDrive)e.ClickedItem).Path);
        }

        private void fileGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            JSONFile file = ((JSONFile)e.ClickedItem);
            if (file.Extension == "") Frame.Navigate(typeof(Browser), ((JSONFile)e.ClickedItem).Path);
            else
            {
                pro.Visibility = Windows.UI.Xaml.Visibility.Visible;
                pro.Value = 0;
                downloadfile(file);
            }
        }

        void downloadfile(JSONFile file)
        {
            HttpWebRequest req = WebRequest.CreateHttp(new Uri(HAPSettings.CurrentSite.Address, file.Path.Substring(1)));
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
                    using (System.IO.BinaryReader reader  = new BinaryReader(response.GetResponseStream()))
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
                        var ignored1 = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { makefile(ms, (JSONFile)a.AsyncState); });
                        var ignored2 = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {pro.Value = 100.0;});
                    }
                }
            }
            , file);
        }


        async void makefile(MemoryStream ms, JSONFile jfile)
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


    }
}
