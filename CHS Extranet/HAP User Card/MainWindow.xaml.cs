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
            pass.Visibility = helpdesk.Visibility = controlled.Visibility = isStudent ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            Web.apiSoapClient c = new Web.apiSoapClient();
            c.getInitCompleted += new EventHandler<Web.getInitCompletedEventArgs>(c_getInitCompleted);
            c.getInitAsync();
        }
        public System.Windows.Forms.NotifyIcon icon { get; private set; }
        void icon_Click(object sender, EventArgs e)
        {
            Show();
        }

        void tabs_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isStudent) return;
            if (e.Delta > 0 && tabs.SelectedIndex < 3) tabs.SelectedIndex++;
            else if (tabs.SelectedIndex > 0 && e.Delta < 0) tabs.SelectedIndex--;

        }

        string path = "";
        bool isStudent = true;
        public UserPrincipal up { get; set; }
        PrincipalContext pcontext;

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
            name.Text = up.DisplayName;
            email.Text = up.EmailAddress;
            homedrive.Text = string.Format("{0} ({1})", up.HomeDrive, up.HomeDirectory);
            pass.Visibility = helpdesk.Visibility = controlled.Visibility = isStudent ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            if (!isStudent)
            {
                controlled.Visibility = string.IsNullOrEmpty(Properties.Settings.Default.ControlledOU) ? System.Windows.Visibility.Hidden : controlled.Visibility;
                helpdesk.Visibility = Properties.Settings.Default.ShowHelpDesk ? controlled.Visibility : System.Windows.Visibility.Hidden;
            }
            departmenttext.Text = dep;
            Cursor = Cursors.Arrow;
        }

        string dep = "";
        string adc = "";
        string sef = "";
        string adun = "";
        string adpw = "";
        string sgn = "";
        string hd = "";
        void doInit(object data)
        {
            //try
            //{
                Web.getInitCompletedEventArgs e = data as Web.getInitCompletedEventArgs;
                string ad = e.Result.ADConString;
                ad = ad.Remove(0, 7);
                ad = ad.Remove(ad.IndexOf('/'));
                sef = e.Result.StudentEmailFormat;
                adun = e.Result.username;
                adpw = e.Result.password;
                sgn = e.Result.StudentGroupName;
                pcontext = new PrincipalContext(ContextType.Domain, ad, adun, adpw);
                up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Environment.UserName);
                //up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, "rmstaff");
                if (!string.IsNullOrEmpty(up.HomeDrive)) hd = up.HomeDirectory;
                isStudent = up.IsMemberOf(GroupPrincipal.FindByIdentity(pcontext, e.Result.StudentGroupName));
                adc = e.Result.ADConString;
                DirectoryEntry usersDE = new DirectoryEntry(adc, adun, adpw);
                DirectorySearcher ds = new DirectorySearcher(usersDE);
                ds.Filter = "(sAMAccountName=" + Environment.UserName + ")";
                //ds.Filter = "(sAMAccountName=rmstaff)";
                ds.PropertiesToLoad.Add("rmCom2000-UsrMgr-uPN");
                ds.PropertiesToLoad.Add("department");
                SearchResult r = ds.FindOne();
                try
                {
                    dep = r.Properties["department"][0].ToString();
                }
                catch { dep = "n/a"; }
                
                path = r.Path.Remove(0, r.Path.IndexOf(',') + 1);

                Dispatcher.BeginInvoke(new Action(UpdateUser));

                Web.apiSoapClient c = new Web.apiSoapClient();
                c.GetFreeSpacePercentageCompleted += new EventHandler<Web.GetFreeSpacePercentageCompletedEventArgs>(c_GetFreeSpacePercentageCompleted);
                if (!isStudent)
                {
                    ThreadStart ts = new ThreadStart(LoadControlled);
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.ControlledOU)) new Thread(ts).Start();
                    c.getMyTicketsCompleted += new EventHandler<Web.getMyTicketsCompletedEventArgs>(c_getMyTicketsCompleted);
                    c.getMyTicketsAsync(Environment.UserName);
                }
                else 
                {
                    try
                    {
                        if (r.Properties["rmCom2000-UsrMgr-uPN"] != null)
                        {
                            c.getPhotoCompleted += new EventHandler<Web.getPhotoCompletedEventArgs>(c_getPhotoCompleted);
                            c.getPhotoAsync(r.Properties["rmCom2000-UsrMgr-uPN"][0].ToString());
                        }
                    }
                    catch { }
                }
                if (!string.IsNullOrEmpty(up.HomeDrive)) c.GetFreeSpacePercentageAsync(up.Name, up.HomeDirectory);
            //}
            //catch { Close();  }
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
            MemoryStream stream = new MemoryStream(Base64Encoder.FromBase64(photo));
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            Photo.Source = image;
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
                c.GetFreeSpacePercentageAsync(up.Name, up.HomeDirectory);
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
                    if (Win32.GetDiskFreeSpaceEx(hd, out freeBytesForUser, out totalBytes, out freeBytes))
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

        private List<OU> OUs;
        void LoadControlled()
        {
            OUs = EnumerateOU(Properties.Settings.Default.ControlledOU);
            Dispatcher.BeginInvoke(new Action(UpdateTree));
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
            foreach (OU o in ou)
            {
                TreeViewItem i = new TreeViewItem();
                i.Header = o.Name;
                i.DataContext = o;
                item.Items.Add(i);
            }
        }

        private List<OU> EnumerateOU(string OuDn)
        {
            List<OU> alObjects = new List<OU>();
            DirectoryEntry directoryObject = new DirectoryEntry(string.Format("LDAP://{0}", OuDn), adun, adpw);
            foreach (DirectoryEntry child in directoryObject.Children)
            {
                string childPath = child.Path.ToString();
                OU ou = new OU(childPath.Remove(0, 7), !childPath.Contains("CN"));
                ou.AddRange(EnumerateOU(ou.OUPath));
                alObjects.Add(ou);
                //remove the LDAP prefix from the path

                child.Close();
                child.Dispose();
            }
            directoryObject.Close();
            directoryObject.Dispose();
            return alObjects;
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            reset.Content = "Resetting...";
            reset.IsEnabled = false;
            try
            {
                new Thread(new ParameterizedThreadStart(doreset)).Start(username.Text);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void doreset(object o)
        {
            try
            {
                UserPrincipal up2 = UserPrincipal.FindByIdentity(pcontext, (string)o);
                if (!up2.IsMemberOf(GroupPrincipal.FindByIdentity(pcontext, sgn)) && !up.IsMemberOf(GroupPrincipal.FindByIdentity(pcontext, "Domain Admins"))) throw new Exception("You can only reset passwords for Students");
                up2.SetPassword("password");
                up2.ExpirePasswordNow();
                up2.Save();
                Dispatcher.BeginInvoke(new donereset(DoneReset), up2.DisplayName);
            }
            catch (Exception ex) { Dispatcher.BeginInvoke(new donereset(ErrorReset), ex.ToString()); }
        }

        delegate void donereset(string displayname);
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
            new Thread(new ParameterizedThreadStart(Enable)).Start(item);
        }

        private void disable_Click(object sender, RoutedEventArgs e)
        {
            controlled.Cursor = Cursors.AppStarting;
            enable.IsEnabled = disable.IsEnabled = false;
            OU item = ((TreeViewItem)treeView1.SelectedItem).DataContext as OU;
            new Thread(new ParameterizedThreadStart(Disable)).Start(item);
            disable.Content = "Disabling...";
        }

        private void Enable(object o)
        {
            OU item = o as OU;
            foreach (OU ou in item)
            {
                try
                {
                    DirectoryEntry user = new DirectoryEntry("LDAP://" + ou.OUPath, adun, adpw);
                    int val = (int)user.Properties["userAccountControl"].Value;
                    user.Properties["userAccountControl"].Value = val & ~0x2;
                    //ADS_UF_NORMAL_ACCOUNT;

                    user.CommitChanges();
                    user.Close();
                }
                catch { continue; }
            }

            Dispatcher.BeginInvoke(new Action<string>(DoneEnabled), item.Name);
        }

        private void Disable(object o)
        {
            OU item = o as OU;
            foreach (OU ou in item)
            {
                try
                {
                    DirectoryEntry user = new DirectoryEntry("LDAP://" + ou.OUPath, adun, adpw);
                    int val = (int)user.Properties["userAccountControl"].Value;
                    user.Properties["userAccountControl"].Value = val | 0x2;
                    //ADS_UF_ACCOUNTDISABLE;

                    user.CommitChanges();
                    user.Close();
                }
                catch { continue; }
            }

            Dispatcher.BeginInvoke(new Action<string>(DoneDisabled), item.Name);
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
