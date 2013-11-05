using HAP.Win.MyFiles.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

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
            cts = new CancellationTokenSource();
        }
        private CancellationTokenSource cts;
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
            HttpClient c = new HttpClient(HAPSettings.certfilter);
            bool tokengood = false;
            try
            {
                c.DefaultRequestHeaders.Cookie.Add(new HttpCookiePairHeaderValue("token", HAPSettings.CurrentToken[0]));
                c.DefaultRequestHeaders.Cookie.Add(new HttpCookiePairHeaderValue(HAPSettings.CurrentToken[2], HAPSettings.CurrentToken[1]));
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
                c.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    JSONProperties prop = JsonConvert.DeserializeObject<JSONProperties>(await c.GetStringAsync(new Uri(HAPSettings.CurrentSite.Address, "./api/myfiles/Properties/" + Path.Replace('\\', '/').Replace("../Download/", "") + "?" + DateTime.Now.Ticks)));
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
            if (EnsureUnsnapped())
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add(string.IsNullOrEmpty(jfile.Type) ? jfile.Extension : jfile.Type, new List<string>() { jfile.Extension });
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
                    XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                    XmlNodeList texts = toastXML.GetElementsByTagName("text");
                    if (status == FileUpdateStatus.Complete)
                    {
                        texts[0].AppendChild(toastXML.CreateTextNode("HAP+ - Download Complete"));
                        texts[1].AppendChild(toastXML.CreateTextNode(file.Name + " has been downloaded and is ready to use."));
                    }
                    else
                    {
                        texts[0].AppendChild(toastXML.CreateTextNode("HAP+ - Download Canceled"));
                        texts[1].AppendChild(toastXML.CreateTextNode("You have canceled this download"));
                    }
                    ((XmlElement)toastXML.SelectSingleNode("/toast")).SetAttribute("launch", "{\"type\":\"toast\"}");
                    ToastNotification toast = new ToastNotification(toastXML);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                    var ignored1 = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { pro.Visibility = Windows.UI.Xaml.Visibility.Collapsed; });
                }
                else
                {
                }
            }
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

        private async void downloadbutton_Click(object sender, RoutedEventArgs e)
        {
            loading.IsIndeterminate = true;
            pro.Visibility = Windows.UI.Xaml.Visibility.Visible;
            pro.Value = 0;
            if (((JSONProperties)DataContext).Size.ToLower().Contains("mb") || ((JSONProperties)DataContext).Size.ToLower().Contains("gb"))
            {
                if (EnsureUnsnapped())
                {
                    FileSavePicker savePicker = new FileSavePicker();
                    savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    // Dropdown of file types the user can save the file as
                    savePicker.FileTypeChoices.Add(string.IsNullOrEmpty(((JSONProperties)DataContext).Type) ? ((JSONProperties)DataContext).Extension : ((JSONProperties)DataContext).Type, new List<string>() { ((JSONProperties)DataContext).Extension });
                    // Default file name if the user does not type one in or select a file to replace
                    savePicker.SuggestedFileName = ((JSONProperties)DataContext).Name;

                    StorageFile file = await savePicker.PickSaveFileAsync();
                    Uri uri = new Uri(HAPSettings.CurrentSite.Address, Path.Substring(1));
                    BackgroundDownloader downloader = new BackgroundDownloader();
                    downloader.Method = "GET";
                    downloader.SetRequestHeader("Cookie", string.Format("{0}={1}; token={2}", HAPSettings.CurrentToken[2], HAPSettings.CurrentToken[1], HAPSettings.CurrentToken[0]));
                    DownloadOperation download = downloader.CreateDownload(uri, file);
                    // Attach progress and completion handlers.
                    HandleDownloadAsync(download, true);
                    MessageDialog mes = new MessageDialog("The download of " + ((JSONProperties)DataContext).Name + " has started, you will get notified when it's done", "Downloading");
                    mes.Commands.Add(new UICommand("OK"));
                    mes.DefaultCommandIndex = 0;
                    mes.ShowAsync();
                    var ignored3 = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { pro.Visibility = Windows.UI.Xaml.Visibility.Collapsed; loading.IsIndeterminate = false; });
                }
            }
            else
            {
                HttpWebRequest req = WebRequest.CreateHttp(new Uri(HAPSettings.CurrentSite.Address, Path.Substring(1)));
                req.Method = "GET";
                req.Headers = new WebHeaderCollection();
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
                            long count = 0; int x = 0;
                            while (canwrite)
                            {
                                try
                                {
                                    ms.WriteByte(reader.ReadByte());
                                }
                                catch { canwrite = false; }
                                count++;
                                x++;
                                if (x == 100)
                                {
                                    x = 0;
                                    try
                                    {
                                        var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { try { pro.Value = count * 100.0 / expected; } catch { } });
                                    }
                                    catch { }
                                }

                            }
                            var ignored1 = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { makefile(ms, (JSONProperties)a.AsyncState); });
                        }
                    }
                }
                , (JSONProperties)DataContext);
            }
        }
        private async Task HandleDownloadAsync(DownloadOperation download, bool start)
        {
            try
            {

                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
                if (start)
                {
                    // Start the upload and attach a progress handler.
                    await download.StartAsync().AsTask(cts.Token, progressCallback);
                }
                else
                {
                    // The upload was already running when the application started, re-attach the progress handler.
                    await download.AttachAsync().AsTask(cts.Token, progressCallback);
                }

                ResponseInformation response = download.GetResponseInformation();
            }
            catch (TaskCanceledException)
            {
                XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                XmlNodeList texts = toastXML.GetElementsByTagName("text");
                texts[0].AppendChild(toastXML.CreateTextNode("HAP+ - Download Canceled"));
                texts[1].AppendChild(toastXML.CreateTextNode("The download of " + download.ResultFile.Name + " has been Canceled."));
                ((XmlElement)toastXML.SelectSingleNode("/toast")).SetAttribute("launch", "{\"type\":\"toast\"}");
                ToastNotification toast = new ToastNotification(toastXML);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
        }

        private void DownloadProgress(DownloadOperation download)
        {
            BackgroundDownloadProgress progress = download.Progress;

            double percentSent = 100;
            if (progress.TotalBytesToReceive > 0)
            {
                percentSent = progress.BytesReceived * 100 / progress.TotalBytesToReceive;
            }
            if (progress.TotalBytesToReceive > 2097152 && Math.Round(percentSent, 0) == 50)
            {
                XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                XmlNodeList texts = toastXML.GetElementsByTagName("text");
                texts[0].AppendChild(toastXML.CreateTextNode("HAP+ - 50% Downloaded"));
                texts[1].AppendChild(toastXML.CreateTextNode("The download of " + download.ResultFile.Name + " has reached 50%."));
                ((XmlElement)toastXML.SelectSingleNode("/toast")).SetAttribute("launch", "{\"type\":\"toast\"}");
                ToastNotification toast = new ToastNotification(toastXML);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            if (progress.Status == BackgroundTransferStatus.Completed || percentSent == 100)
            {
                XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                XmlNodeList texts = toastXML.GetElementsByTagName("text");
                texts[0].AppendChild(toastXML.CreateTextNode("HAP+ - Download Complete"));
                texts[1].AppendChild(toastXML.CreateTextNode("The download of " + download.ResultFile.Name + " has completed."));
                ((XmlElement)toastXML.SelectSingleNode("/toast")).SetAttribute("launch", "{\"type\":\"toast\"}");
                ToastNotification toast = new ToastNotification(toastXML);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
        }
    }
}
