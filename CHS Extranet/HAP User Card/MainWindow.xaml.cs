using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Threading;
using System.IO;
using System.Windows.Media.Animation;
using HAP.UserCard.Web;

namespace HAP.UserCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SourceInitialized += new EventHandler(MainWindow_SourceInitialized);
            tabs.MouseWheel += new MouseWheelEventHandler(tabs_MouseWheel);
            Hide();
            icon = new System.Windows.Forms.NotifyIcon();
            icon.Icon = new System.Drawing.Icon("usercardi.ico", new System.Drawing.Size(16, 16));
            icon.Text = this.Title;
            icon.Click += new EventHandler(icon_Click);
            icon.Visible = true;
            Cursor = controlled.Cursor = Cursors.AppStarting;
            pass.Visibility = helpdesk.Visibility = controlled.Visibility = System.Windows.Visibility.Hidden;
            Web.apiSoapClient c = new Web.apiSoapClient();
            c.getInitCompleted += new EventHandler<Web.getInitCompletedEventArgs>(c_getInitCompleted);
            c.getInitAsync(Environment.UserName);
        }
        public System.Windows.Forms.NotifyIcon icon { get; private set; }
        public Init Init { get; set; }
        void icon_Click(object sender, EventArgs e)
        {
            Show();
        }

        void tabs_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Init.UserLevel == UserLevel.Student) return;
            if (e.Delta > 0 && tabs.SelectedIndex < 3) tabs.SelectedIndex++;
            else if (tabs.SelectedIndex > 0 && e.Delta < 0) tabs.SelectedIndex--;

        }

        #region Go to Bottom Right

        const int MONITOR_DEFAULTTONEAREST = 2;

        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor; // Total area
            public RECT rcWork; // Working area
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public char[] szDevice;
        }
        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX monitorInfo);


        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            WindowBehavior.ExtendGlassFrame(this, new Thickness(0, 26, 0, 0));
            WindowInteropHelper wih = new WindowInteropHelper(this);
            IntPtr hMonitor = MonitorFromWindow(wih.Handle, MONITOR_DEFAULTTONEAREST);
            MONITORINFOEX monitorInfo = new MONITORINFOEX();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            GetMonitorInfo(new HandleRef(this, hMonitor), monitorInfo);
            HwndSource source = HwndSource.FromHwnd(wih.Handle);
            if (source == null) return;
            if (source.CompositionTarget == null) return;
            Matrix matrix = source.CompositionTarget.TransformFromDevice;
            RECT workingArea = monitorInfo.rcWork;
            Point dpiIndependentSize =
                matrix.Transform(
                new Point(
                    workingArea.Right - workingArea.Left,
                    workingArea.Bottom - workingArea.Top));
            this.Top = dpiIndependentSize.Y - this.ActualHeight - 10;
            this.Height = this.ActualHeight;
            this.Left = dpiIndependentSize.X - this.ActualWidth - 10;
            this.Width = this.ActualWidth;
            source.AddHook(WndProc);    
        }

        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            switch (msg)
            {
                case WM_SYSCOMMAND:
                    int command = wParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                    {
                        handled = true;
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion

        private void username_KeyUp(object sender, KeyEventArgs e)
        {
            reset.IsEnabled = username.Text.Length > 0;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void c_getInitCompleted(object sender, Web.getInitCompletedEventArgs e)
        {
            new Thread(new ParameterizedThreadStart(doInit)).Start(e);
        }

        void UpdateUser()
        {
            try
            {
                name.Text = Init.DisplayName;
                email.Text = Init.EmailAddress;
                homedrive.Text = string.Format("{0} ({1})", Init.HomeDrive, Init.HomeDirectory);
            }
            catch { }
            pass.Visibility = helpdesk.Visibility = controlled.Visibility = Init.UserLevel == UserLevel.Student ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            if (Init.UserLevel != UserLevel.Student)
            {
                controlled.Visibility = string.IsNullOrEmpty(Properties.Settings.Default.ControlledOU) ? System.Windows.Visibility.Hidden : controlled.Visibility;
                helpdesk.Visibility = Properties.Settings.Default.ShowHelpDesk ? controlled.Visibility : System.Windows.Visibility.Hidden;
            }
            departmenttext.Text = Init.Department;
            Cursor = Cursors.Arrow;
        }
        void doInit(object data)
        {
            try
            {
                Web.getInitCompletedEventArgs e = data as Web.getInitCompletedEventArgs;
                if (e.Error != null)
                {
                    MessageBox.Show("Error:\n" + e.Error.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
                Init = e.Result;
                Dispatcher.BeginInvoke(new Action(UpdateUser));

                Web.apiSoapClient c = new Web.apiSoapClient();
                c.GetFreeSpacePercentageCompleted += new EventHandler<Web.GetFreeSpacePercentageCompletedEventArgs>(c_GetFreeSpacePercentageCompleted);
                if (Init.UserLevel != UserLevel.Student)
                {
                    c.getControlledOUsCompleted += new EventHandler<getControlledOUsCompletedEventArgs>(c_getControlledOUsCompleted);
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.ControlledOU)) c.getControlledOUsAsync(Properties.Settings.Default.ControlledOU);
                    c.getMyTicketsCompleted += new EventHandler<Web.getMyTicketsCompletedEventArgs>(c_getMyTicketsCompleted);
                    c.getMyTicketsAsync(Environment.UserName);
                }
                else
                {
                    c.getPhotoCompleted += new EventHandler<Web.getPhotoCompletedEventArgs>(c_getPhotoCompleted);
                    c.getPhotoAsync(Init.EmployeeID);
                }
                if (!string.IsNullOrEmpty(Init.HomeDrive)) c.GetFreeSpacePercentageAsync(Environment.UserName, Init.HomeDirectory);
            }
            catch (Exception ex) { MessageBox.Show("Init Error:\n" + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error); Close(); }
        }

        void c_getMyTicketsCompleted(object sender, Web.getMyTicketsCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                Dispatcher.BeginInvoke(new EventHandler<Web.getMyTicketsCompletedEventArgs>(c_getMyTicketsCompleted2), sender, e);
            }
        }

        void c_getMyTicketsCompleted2(object sender, Web.getMyTicketsCompletedEventArgs e)
        {
            if (e.Result.Length > 0)
            {
                ticketbox.Visibility = System.Windows.Visibility.Visible;
                notickets.Visibility = System.Windows.Visibility.Hidden;
                ticketbox.ItemsSource = e.Result;
            }
            else
            {
                ticketbox.Visibility = System.Windows.Visibility.Hidden;
                notickets.Visibility = System.Windows.Visibility.Visible;
            }
        }


        void c_getPhotoCompleted(object sender, Web.getPhotoCompletedEventArgs e)
        {
            if (e.Error == null && !string.IsNullOrEmpty(e.Result)) Dispatcher.BeginInvoke(new Action<string>(updatephoto), e.Result);
        }

        void updatephoto(string photo)
        {
            try
            {
                MemoryStream stream = new MemoryStream(Base64Encoder.FromBase64(photo));
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                Photo.Source = image;
            }
            catch
            {
                BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/HAP User Card;component/Images/Imageres18.png"));
                image.BeginInit();
                image.EndInit();
                Photo.Source = image;
            }
        }

        private Thread us = null;
        private bool closing = false;
        public void updatespace()
        {
            while (!closing)
            {
                try
                {
                    Thread.Sleep(new TimeSpan(0, 2, 0));
                }
                catch (ThreadInterruptedException) { break; }

                Web.apiSoapClient c = new Web.apiSoapClient();
                c.GetFreeSpacePercentageCompleted += new EventHandler<Web.GetFreeSpacePercentageCompletedEventArgs>(c_GetFreeSpacePercentageCompleted);
                c.GetFreeSpacePercentageAsync(Environment.UserName, Init.HomeDirectory);
            }
        }

        void c_GetFreeSpacePercentageCompleted(object sender, Web.GetFreeSpacePercentageCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new EventHandler<Web.GetFreeSpacePercentageCompletedEventArgs>(UpdateSpace), sender, e);
        }

        void UpdateSpace(object sender, Web.GetFreeSpacePercentageCompletedEventArgs e)
        {
            if (e.Error != null || e.Result.Total == -1)
            {
                try
                {
                    long freeBytesForUser, totalBytes, freeBytes;
                    if (Win32.GetDiskFreeSpaceEx(Init.HomeDirectory, out freeBytesForUser, out totalBytes, out freeBytes))
                    {
                        drivespaceprog.Maximum = totalBytes;
                        drivespaceprog.Value = drivespaceprog.Maximum - freeBytes;
                        freespace.Text = freespace.Text = "Free Space: " + parseLength(freeBytes);
                    }
                }
                catch { drivespaceprog.Maximum = 0; }
            }
            else
            {
                drivespaceprog.Maximum = e.Result.Total;
                drivespaceprog.Value = e.Result.Used;
                freespace.Text = freespace.Text = "Free Space: " + parseLength(e.Result.Total - e.Result.Used);
            }
            totalspace.Text = "Total Space: " + parseLength(drivespaceprog.Maximum);
            if (drivespaceprog.Maximum > 0)
            {
                DriveSpace.Visibility = System.Windows.Visibility.Visible;
                decimal d = Math.Round(((Convert.ToDecimal(drivespaceprog.Value) / Convert.ToDecimal(drivespaceprog.Maximum)) * 100), 2);
                drivespaceper.Text = d + "%";
                if ((100 - d) < 5 && !hasShownWarning) if (Dialog.ShowMessageDialog("You are running low on Space\nTry deleting some old files to free up space", "Low Space", DialogIcon.Warning).HasValue) hasShownWarning = true;
                if (us == null)
                {
                    us = new Thread(new ThreadStart(updatespace));
                    us.Start();
                }
            }
        }

        private bool hasShownWarning = false;

        internal static class Win32
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);

        }

        public static string parseLength(object size)
        {
            decimal d = decimal.Parse(size.ToString() + ".00");
            string[] s = { "bytes", "KB", "MB", "GB", "TB", "PB" };
            int x = 0;
            while (d > 1024)
            {
                d = d / 1024;
                x++;
            }
            d = Math.Round(d, 2);
            return d.ToString() + " " + s[x];
        }

        private OU[] OUs;
        
        void c_getControlledOUsCompleted(object sender, getControlledOUsCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            else
            {
                OUs = e.Result;
                Dispatcher.BeginInvoke(new Action(UpdateTree));
            }
        }

        void UpdateTree()
        {
            foreach (OU ou in OUs) 
            {
                TreeViewItem i = new TreeViewItem();
                i.Header = ou.Name;
                i.DataContext = ou;
                UpdateTree(i, ou);
                treeView1.Items.Add(i);
            }
            controlled.Cursor = Cursors.Arrow;
        }

        void UpdateTree(TreeViewItem item, OU ou)
        {
            foreach (OU o in ou.OUs)
            {
                TreeViewItem i = new TreeViewItem();
                i.Header = o.Name;
                i.DataContext = o;
                item.Items.Add(i);
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            reset.Content = "Resetting...";
            reset.IsEnabled = false;
            try
            {
                Web.apiSoapClient c1 = new Web.apiSoapClient();
                c1.getInitCompleted += new EventHandler<getInitCompletedEventArgs>(c1_getInitCompleted);
                c1.getInitAsync(username.Text);
            }
            catch (Exception ex) { ErrorReset(ex.ToString()); }
        }

        void c1_getInitCompleted(object sender, getInitCompletedEventArgs e)
        {
            if (e.Error != null) Dispatcher.BeginInvoke(new Action<string>(ErrorReset), e.Error.ToString());
            else if (Init.UserLevel == UserLevel.Admin || e.Result.UserLevel == UserLevel.Student)
            {
                Web.apiSoapClient c = new Web.apiSoapClient();
                c.ResetPasswordCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(c_ResetPasswordCompleted);
                c.ResetPasswordAsync(e.Result.Username, e.Result.DisplayName);
            }
            else Dispatcher.BeginInvoke(new Action<string>(ErrorReset), "I can't reset this user because they are not a student user");
        }

        void c_ResetPasswordCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null) Dispatcher.BeginInvoke(new Action<string>(ErrorReset), e.Error.ToString());
            else Dispatcher.BeginInvoke(new Action<string>(DoneReset), (string)e.UserState);
        }

        private void DoneReset(string displayname)
        {
            Dialog.ShowMessage("I've reset " + displayname + "'s password to 'password'\nThey will be asked to change it when they log on", "Confirmation", DialogIcon.Info);
            reset.Content = "Reset";
            username.Text = "";
            reset.IsEnabled = true;
        }
        private void ErrorReset(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            reset.Content = "Reset";
            reset.IsEnabled = true;
        }

        private void enable_Click(object sender, RoutedEventArgs e)
        {
            controlled.Cursor = Cursors.AppStarting;
            enable.IsEnabled = disable.IsEnabled = false;
            enable.Content = "Enabling...";
            OU item = ((TreeViewItem)treeView1.SelectedItem).DataContext as OU;
            Web.apiSoapClient c = new Web.apiSoapClient();
            c.EnableCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(c_EnableCompleted);
            c.EnableAsync(getPaths(item), item.Name);
        }

        private Web.ArrayOfString getPaths(OU item)
        {
            Web.ArrayOfString s = new Web.ArrayOfString();
            foreach (OU o in item.OUs)
                s.Add(o.OUPath);
            return s;
        }

        void c_EnableCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null) Dispatcher.BeginInvoke(new Action<string>(DoneEnabled), (string)e.UserState);
            else MessageBox.Show(e.Error.ToString());
        }

        private void disable_Click(object sender, RoutedEventArgs e)
        {
            controlled.Cursor = Cursors.AppStarting;
            enable.IsEnabled = disable.IsEnabled = false;
            OU item = ((TreeViewItem)treeView1.SelectedItem).DataContext as OU;
            disable.Content = "Disabling...";
            Web.apiSoapClient c = new Web.apiSoapClient();
            c.DisableCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(c_DisableCompleted);
            c.DisableAsync(getPaths(item), item.Name);
        }

        void c_DisableCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null) Dispatcher.BeginInvoke(new Action<string>(DoneDisabled), (string)e.UserState);
            else MessageBox.Show(e.Error.ToString());
        }

        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reset.IsEnabled = false;
            reset.IsDefault = tabs.SelectedIndex == 1;
            newticket.IsDefault = tabs.SelectedIndex == 3;
            username.Text = "";
        }

        private void DoneEnabled(string name)
        {
            controlled.Cursor = Cursors.Arrow;
            enable.IsEnabled = disable.IsEnabled = true;
            enable.Content = "Enable";
            text.Text = "Users in group " + name + " are now Enabled";
            ((Storyboard)this.Resources["hidemess"]).Begin();
        }

        private void DoneDisabled(string name)
        {
            controlled.Cursor = Cursors.Arrow;
            enable.IsEnabled = disable.IsEnabled = true;
            disable.Content = "Disable";
            text.Text = "Users in group " + name + " are now Disabled";
            ((Storyboard)this.Resources["hidemess"]).Begin();
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            enable.IsEnabled = disable.IsEnabled = (treeView1.SelectedItem != null && !((TreeViewItem)treeView1.SelectedItem).HasItems);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            us.Interrupt();
            us = null;
        }

        private void ticketbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && ticketbox.SelectedIndex > -1)
                new Ticket((Web.Ticket)ticketbox.SelectedItem).Show();

        }

        private void newticket_Click(object sender, RoutedEventArgs e)
        {
            NewTicket nt = new NewTicket();
            nt.Done += new Action(nt_Done);
            nt.Show();
        }

        void nt_Done()
        {
            Web.apiSoapClient c = new Web.apiSoapClient();
            c.getMyTicketsCompleted += new EventHandler<Web.getMyTicketsCompletedEventArgs>(c_getMyTicketsCompleted);
            c.getMyTicketsAsync(Environment.UserName);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            nt_Done();
        }
    }
}
