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
            Hide();
            tbi.Visibility = System.Windows.Visibility.Visible;
            Cursor = controlled.Cursor = Cursors.AppStarting;
            pass.Visibility = controlled.Visibility = isStudent ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
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
            WindowBehavior.ExtendGlassFrame(this, new Thickness(0, 27, 0, 0));
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

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            department.Visibility = System.Windows.Visibility.Visible;
            ((TextBlock)sender).Visibility = System.Windows.Visibility.Hidden;
        }

        private void department_MouseLeave(object sender, MouseEventArgs e)
        {
            department.Visibility = System.Windows.Visibility.Hidden; departmenttext.Visibility = System.Windows.Visibility.Visible;
        }

        private void username_KeyUp(object sender, KeyEventArgs e)
        {
            reset.IsEnabled = username.Text.Length > 0;
        }

        private void tbi_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reset.IsEnabled = false;
            save.IsDefault = reset.IsDefault = false;
            if (tabs.SelectedIndex == 0) save.IsDefault = true;
            else if (tabs.SelectedIndex == 1) reset.IsDefault = true;
            username.Text = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Web.apiSoapClient c = new Web.apiSoapClient();
            c.getInitCompleted += new EventHandler<Web.getInitCompletedEventArgs>(c_getInitCompleted);
            c.getInitAsync();
        }

        void c_getInitCompleted(object sender, Web.getInitCompletedEventArgs e)
        {
            new Thread(new ParameterizedThreadStart(doInit)).Start(e);
        }

        void UpdateUser()
        {
            name.Text = up.DisplayName;
            email.Text = up.EmailAddress;
            pass.Visibility = controlled.Visibility = isStudent ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            if (!isStudent) controlled.Visibility = string.IsNullOrEmpty(Properties.Settings.Default.ControlledOU) ? System.Windows.Visibility.Hidden : controlled.Visibility;
            departmenttext.Text = dep;
            save.Visibility = System.Windows.Visibility.Hidden;
            save.IsEnabled = true;
            save.Content = "Save";
            Cursor = Cursors.Arrow;
        }

        string dep = "";
        string adc = "";
        string sef = "";
        string adun = "";
        string adpw = "";
        string sgn = "";
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
                isStudent = up.IsMemberOf(GroupPrincipal.FindByIdentity(pcontext, e.Result.StudentGroupName));
                adc = e.Result.ADConString;
                DirectoryEntry usersDE = new DirectoryEntry(adc, adun, adpw);
                DirectorySearcher ds = new DirectorySearcher(usersDE);
                ds.Filter = "(sAMAccountName=" + Environment.UserName + ")";
                ds.PropertiesToLoad.Add("department");
                SearchResult r = ds.FindOne();
                try
                {
                    dep = r.Properties["department"][0].ToString();
                }
                catch { dep = "n/a"; }
                
                path = r.Path.Remove(0, r.Path.IndexOf(',') + 1);

                Dispatcher.BeginInvoke(new Action(UpdateUser));
                Dispatcher.BeginInvoke(new Action(DoCheck));

                Web.apiSoapClient c = new Web.apiSoapClient();
                c.getDepartmentsCompleted += new EventHandler<Web.getDepartmentsCompletedEventArgs>(c_getDepartmentsCompleted);
                c.getFormsInCompleted += new EventHandler<Web.getFormsInCompletedEventArgs>(c_getFormsInCompleted);
                if (isStudent) c.getFormsInAsync(path.Remove(path.IndexOf(',')).Remove(0, path.IndexOf('=') + 1));
                else c.getDepartmentsAsync();
            //}
            //catch { Close();  }
        }

        private void DoCheck()
        {
            if (!up.LastLogon.HasValue) { MessageBox.Show("Please Update Your " + (isStudent ? "Form" : "Department"), "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation); Show(); }
            else
            {
                if (DateTime.Now.Month == 9 && up.LastLogon.Value.Month != 9) { MessageBox.Show("Please check your " + (isStudent ? "Form" : "Department"), "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation); Show(); }
            }
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

        void c_getFormsInCompleted(object sender, Web.getFormsInCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new EventHandler<Web.getFormsInCompletedEventArgs>(c_getFormsInCompleted1), sender, e);
        }

        void c_getFormsInCompleted1(object sender, Web.getFormsInCompletedEventArgs e)
        {
            department.ItemsSource = e.Result;
            if (e.Result.Count(a => a.Name == departmenttext.Text) > 0) department.SelectedItem = e.Result.Single(a => a.Name == departmenttext.Text);
        }

        void c_getDepartmentsCompleted(object sender, Web.getDepartmentsCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new EventHandler<Web.getDepartmentsCompletedEventArgs>(c_getDepartmentsCompleted1), sender, e);
        }

        void c_getDepartmentsCompleted1(object sender, Web.getDepartmentsCompletedEventArgs e)
        {
            department.ItemsSource = e.Result;
            if (e.Result.Count(a => a.Name == departmenttext.Text) > 0) department.SelectedItem = e.Result.Single(a => a.Name == departmenttext.Text);
            ThreadStart ts = new ThreadStart(LoadControlled);
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ControlledOU)) new Thread(ts).Start();
        }


        private void save_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.AppStarting;
            save.Content = "Saving...";
            save.IsEnabled = false;
            string d = (department.SelectedItem is Web.Department) ? ((Web.Department)department.SelectedItem).Name : ((Web.Form)department.SelectedItem).Name;
            up.Description = string.Format("{0} {1} in {2}", up.GivenName, up.Surname, d);
            if (isStudent && !string.IsNullOrEmpty(sef)) up.EmailAddress = string.Format(sef, Environment.UserName);
            new Thread(new ParameterizedThreadStart(UpdateUserData)).Start(d);
        }

        private void UpdateUserData(object d)
        {

            up.Save();

            DirectoryEntry usersDE = new DirectoryEntry(adc, adun, adpw);
            DirectorySearcher ds = new DirectorySearcher(usersDE);
            ds.Filter = "(sAMAccountName=" + Environment.UserName + ")";
            ds.PropertiesToLoad.Add("cn");
            SearchResult r = ds.FindOne();
            DirectoryEntry theUserDE = new DirectoryEntry(r.Path, adun, adpw);


            if (theUserDE.Properties["Department"].Count == 0) theUserDE.Properties["Department"].Add(d.ToString());
            else theUserDE.Properties["Department"][0] = d.ToString();
            theUserDE.CommitChanges();

            dep = d.ToString();
            Dispatcher.BeginInvoke(new Action(UpdateUser));

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
            MessageBox.Show("I've reset " + displayname + "'s password to 'password'\nThey will be asked to change it when they log on", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
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
            enable.IsEnabled = disable.IsEnabled = false;
            enable.Content = "Enabling...";
            controlled.Cursor = Cursors.AppStarting;
            OU item = ((TreeViewItem)treeView1.SelectedItem).DataContext as OU;
            new Thread(new ParameterizedThreadStart(Enable)).Start(item);
        }

        private void disable_Click(object sender, RoutedEventArgs e)
        {
            controlled.Cursor = Cursors.AppStarting;
            enable.IsEnabled = disable.IsEnabled = false;
            disable.Content = "Disabling...";
            OU item = ((TreeViewItem)treeView1.SelectedItem).DataContext as OU;
            new Thread(new ParameterizedThreadStart(Disable)).Start(item);
        }

        private void Enable(object o)
        {
            OU item = o as OU;
            foreach (OU ou in item)
            {
                try
                {
                    DirectoryEntry user = new DirectoryEntry("LDAP://" + ou.OUPath);
                    int val = (int)user.Properties["userAccountControl"].Value;
                    user.Properties["userAccountControl"].Value = val & ~0x2;
                    //ADS_UF_NORMAL_ACCOUNT;

                    user.CommitChanges();
                    user.Close();
                }
                catch { }
            }
            Dispatcher.BeginInvoke(new Action(DoneEnabled));
        }

        private void Disable(object o)
        {
            OU item = o as OU;
            foreach (OU ou in item)
            {
                try
                {
                    DirectoryEntry user = new DirectoryEntry("LDAP://" + ou.OUPath);
                    int val = (int)user.Properties["userAccountControl"].Value;
                    user.Properties["userAccountControl"].Value = val | 0x2;
                    //ADS_UF_ACCOUNTDISABLE;

                    user.CommitChanges();
                    user.Close();
                }
                catch { }
            }
            Dispatcher.BeginInvoke(new Action(DoneDisabled));
        }

        private void DoneEnabled()
        {
            controlled.Cursor = Cursors.Arrow;
            enable.IsEnabled = disable.IsEnabled = true;
            enable.Content = "Enable";
            OU item = ((TreeViewItem)treeView1.SelectedItem).DataContext as OU;
            MessageBox.Show(this, "Users in group " + item.Name + " are now Enabled", "Enabled", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DoneDisabled()
        {
            controlled.Cursor = Cursors.Arrow;
            enable.IsEnabled = disable.IsEnabled = true;
            disable.Content = "Disable";
            OU item = ((TreeViewItem)treeView1.SelectedItem).DataContext as OU;
            MessageBox.Show(this, "Users in group " + item.Name + " are now Disabled", "Disabled", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            enable.IsEnabled = disable.IsEnabled = (treeView1.SelectedItem != null && !((TreeViewItem)treeView1.SelectedItem).HasItems);
        }

        private void department_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (department.SelectedItem is Web.Department)
            {
                if (((Web.Department)department.SelectedItem).Name == departmenttext.Text) save.Visibility = System.Windows.Visibility.Hidden;
                else save.Visibility = System.Windows.Visibility.Visible;
            }
            else if (department.SelectedItem is Web.Form)
            {
                if (((Web.Form)department.SelectedItem).Name == departmenttext.Text) save.Visibility = System.Windows.Visibility.Hidden;
                else save.Visibility = System.Windows.Visibility.Visible;
            }
            departmenttext.Text = (department.SelectedItem is Web.Department) ? ((Web.Department)department.SelectedItem).Name : ((Web.Form)department.SelectedItem).Name;
        }
    }
}
