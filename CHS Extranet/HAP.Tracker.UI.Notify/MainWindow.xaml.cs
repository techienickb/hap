using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HAP.Tracker.UI.Notify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((System.Windows.Media.Animation.Storyboard)FindResource("closeStoryBoard")).Begin();
            e.Handled = true;
        }

        private DateTime lastDT;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = System.Windows.SystemParameters.PrimaryScreenWidth - Width;
            poller = new Thread(new ThreadStart(Poll));
            poll = true;
            lastDT = DateTime.Now;
            poller.Start();
        }

        Thread poller;
        private bool poll = false;
        void Poll()
        {
            //start a loop until the program closes
            while (poll)
            {
                if (lastDT.AddMinutes(2) < DateTime.Now)
                {
                    WebClient c = new WebClient();
                    c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    c.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    c.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    this.lastDT = DateTime.Now;
                    c.UploadStringCompleted += c_UploadStringCompleted;
                    c.UploadStringAsync(new Uri(App.BaseUrl, "./api/tracker/poll"), "POST", "{ \"Username\": \"" + Environment.UserName + "\", \"Computer\": \"" + Dns.GetHostName() + "\", \"DomainName\":\"" + Environment.UserDomainName + "\" }");
                }
                Thread.Sleep(100);
            }
        }

        void c_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                LogonsList list = JsonConvert.DeserializeObject<LogonsList>(e.Result);
                if (list.Logons.Count(l => DateTime.Parse(l.LogOnDateTime) > lastDT) > 0)
                    Dispatcher.BeginInvoke(new Action(ShowMe));
            }
        }

        void ShowMe()
        {
            if (Visibility != System.Windows.Visibility.Visible)
            {
                Visibility = System.Windows.Visibility.Visible;
                ((System.Windows.Media.Animation.Storyboard)FindResource("openStoryBoard")).Begin();
            }
        }

        Point start;
        bool started = false;
        private void Window_TouchDown(object sender, TouchEventArgs e)
        {
            start = e.GetTouchPoint(this).Position;
        }

        private void Window_TouchUp(object sender, TouchEventArgs e)
        {
            if (Width < 400) ((System.Windows.Media.Animation.Storyboard)FindResource("closeStoryBoard")).Begin();
            else if (start == e.GetTouchPoint(this).Position)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "HAP Logon Tracker.exe";
                proc.StartInfo.Arguments = "poll " + App.BaseUrl.ToString();
                proc.Start();
                ((System.Windows.Media.Animation.Storyboard)FindResource("closeStoryBoard")).Begin();
            }
        }

        private void Window_TouchMove(object sender, TouchEventArgs e)
        {
            double newpos = e.GetTouchPoint(this).Position.X - start.X;
            if (newpos > 0) { Left += newpos; Width -= newpos; }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(this);
            started = true;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (started)
            {
                double newpos = e.GetPosition(this).X - start.X;
                if (newpos > 0) { Left += newpos; Width -= newpos; }
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            started = false;
            if (Width < 400) ((System.Windows.Media.Animation.Storyboard)FindResource("closeStoryBoard")).Begin();
            else if (start == e.GetPosition(this))
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "HAP Logon Tracker.exe";
                proc.StartInfo.Arguments = "poll " + App.BaseUrl.ToString();
                proc.Start();
                ((System.Windows.Media.Animation.Storyboard)FindResource("closeStoryBoard")).Begin();
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            started = false;
            Width = 400;
            Left = System.Windows.SystemParameters.PrimaryScreenWidth - Width;
        }


        private void closeStoryBoard_Completed(object sender, EventArgs e)
        {
            Visibility = System.Windows.Visibility.Collapsed;
            Width = 400;
            Left = System.Windows.SystemParameters.PrimaryScreenWidth - Width;
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            poll = false;
            Thread.Sleep(100);
            poller.Abort();
        }
    }
}
