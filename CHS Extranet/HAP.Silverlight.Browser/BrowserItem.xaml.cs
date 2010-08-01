using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Browser;
using System.Text.RegularExpressions;

namespace HAP.Silverlight.Browser
{
    public partial class BrowserItem : UserControl, IComparable, IBitem
    {
        public BrowserItem()
        {
            InitializeComponent();
        }
        public BrowserItem(BItem data)
        {
            InitializeComponent();
            Data = data;
        }

        public BrowserItem(BItem data, double space)
        {
            InitializeComponent();
            Data = data;
            IsSpace = true;
            Space = space;
        }

        private BItem _data;
        public BItem Data
        {
            get { return _data; }
            set
            {
                _data = value;
                name1.Text = name2.Text = name3.Text = name4.Text = name5.Text = textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = _data.Name;
                size.Text = _data.Size;
                type.Text = _data.Type;

                image1.Source = image2.Source = image3.Source = image4.Source = image5.Source = new BitmapImage(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + _data.Icon));
                this.AllowDrop = (_data.BType == BType.Folder) && _data.AccessControl == AccessControlActions.Change;
                key1.Visibility = key2.Visibility = key3.Visibility = key4.Visibility = key5.Visibility = (_data.AccessControl == AccessControlActions.Change ? Visibility.Collapsed : System.Windows.Visibility.Visible);
                tooltip1.Content = tooltip2.Content = tooltip3.Content = tooltip4.Content = tooltip5.Content = (_data.AccessControl == AccessControlActions.None ? "This file/folder may not be accessible" : "This file/folder has restrictive permissions");
            }
        }

        public double Space
        {
            get { return frespace.Value; }
            set { frespace.Value = value; freespace.Text = value.ToString() + "%"; }
        }

        public bool IsSpace
        {
            get { return frespace.Visibility == System.Windows.Visibility.Visible ? true : false; }
            set { frespace.Visibility = freespace.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; }
        }

        public bool IsRename
        {
            get { return textBox1.Visibility == System.Windows.Visibility.Visible ? true : false; }
            set
            {
                if (value)
                {
                    textBox1.Visibility = textBox2.Visibility = textBox3.Visibility = textBox4.Visibility = textBox5.Visibility = System.Windows.Visibility.Visible;
                    switch (Mode)
                    {
                        case ViewMode.Tile: textBox1.Focus(); textBox1.SelectAll(); break;
                        case ViewMode.List: textBox2.Focus(); textBox2.SelectAll(); break;
                        case ViewMode.LargeIcon: textBox3.Focus(); textBox3.SelectAll(); break;
                        case ViewMode.SmallIcon: textBox4.Focus(); textBox4.SelectAll(); break;
                        case ViewMode.Icon: textBox5.Focus(); textBox5.SelectAll(); break;
                    }
                }
                else textBox1.Visibility = textBox2.Visibility = textBox3.Visibility = textBox4.Visibility = textBox5.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public ViewMode Mode
        {
            get
            {
                if (List.Visibility == System.Windows.Visibility.Visible) return ViewMode.List;
                else if (Icon.Visibility == System.Windows.Visibility.Visible) return ViewMode.LargeIcon;
                else return ViewMode.Tile;
            }
            set
            {
                switch (value)
                {
                    case ViewMode.List:
                        List.Visibility = System.Windows.Visibility.Visible;
                        SmallIcon.Visibility = MediumIcon.Visibility = Tile.Visibility = Icon.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ViewMode.LargeIcon:
                        Icon.Visibility = System.Windows.Visibility.Visible;
                        SmallIcon.Visibility = MediumIcon.Visibility = Tile.Visibility = List.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ViewMode.SmallIcon:
                        SmallIcon.Visibility = System.Windows.Visibility.Visible;
                        Icon.Visibility = MediumIcon.Visibility = Tile.Visibility = List.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ViewMode.Icon:
                        MediumIcon.Visibility = System.Windows.Visibility.Visible;
                        SmallIcon.Visibility = Icon.Visibility = Tile.Visibility = List.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    default:
                        Tile.Visibility = System.Windows.Visibility.Visible;
                        SmallIcon.Visibility = MediumIcon.Visibility = Icon.Visibility = List.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
            }
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Active)
            {
                Tile.BorderBrush = Icon.BorderBrush = List.BorderBrush = SmallIcon.BorderBrush = MediumIcon.BorderBrush = Resources["LBB"] as SolidColorBrush;
                Tile.Background = Icon.Background = List.Background = SmallIcon.Background = MediumIcon.Background = Resources["hover"] as LinearGradientBrush;
            }
            else Tile.Background = Icon.Background = List.Background = SmallIcon.Background = MediumIcon.Background = Resources["ActiveBG"] as LinearGradientBrush;
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Active) Tile.BorderBrush = Icon.BorderBrush = List.BorderBrush = Tile.Background = Icon.Background = List.Background = SmallIcon.BorderBrush = MediumIcon.BorderBrush = SmallIcon.Background = MediumIcon.Background = new SolidColorBrush(Colors.Transparent);
            else Tile.Background = Icon.Background = List.Background = SmallIcon.Background = MediumIcon.Background = new SolidColorBrush(Colors.Transparent);
        }

        public event EventHandler Activate;
        public event ResortHandler ReSort;
        public event ChangeDirectoryHandler DirectoryChange;

        private bool _active;
        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                if (_active)
                {
                    Tile.BorderBrush = Icon.BorderBrush = List.BorderBrush = SmallIcon.BorderBrush = MediumIcon.BorderBrush = Resources["ABB"] as SolidColorBrush;
                    Tile.Background = Icon.Background = List.Background = SmallIcon.Background = MediumIcon.Background = Resources["ActiveBG"] as LinearGradientBrush;
                }
                else
                {
                    Tile.BorderBrush = Icon.BorderBrush = List.BorderBrush = Tile.Background = Icon.Background = List.Background = SmallIcon.BorderBrush = MediumIcon.BorderBrush = SmallIcon.Background = MediumIcon.Background = new SolidColorBrush(Colors.Transparent);
                    if (IsRename) IsRename = false;
                }
            }
        }

        public long LastTicks = 0;
        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Active = true;
            if (Activate != null) Activate(this, new EventArgs());
            if ((DateTime.Now.Ticks - LastTicks) < 2310000)
            {
                if (_data.BType == BType.File) HtmlPage.Window.Navigate(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + this._data.Path));
                else if (DirectoryChange != null) DirectoryChange(this, this._data);
            }
            LastTicks = DateTime.Now.Ticks;
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Active)
            {
                Active = true;
                if (Activate != null) Activate(this, new EventArgs());
            }
        }

        public int Save()
        {
            if (textBox1.Text.ToLower() == _data.Name.ToLower())
            {
                this.IsRename = false;
                return -1;
            }
            return Save(true);
        }

        public int Save(bool resort)
        {
            string newname = "";
            switch (Mode)
            {
                case ViewMode.Tile: newname = textBox1.Text; break;
                case ViewMode.List: newname = textBox2.Text; break;
                case ViewMode.LargeIcon: newname = textBox3.Text; break;
                case ViewMode.SmallIcon: newname = textBox4.Text; break;
                case ViewMode.Icon: newname = textBox5.Text; break;
            }
            if (newname == "") { MessageBox.Show("I Can't Rename this to Nothing", "Error", MessageBoxButton.OK); return -1; }

            string _d = "";
            _d = "SAVETO:" + newname;

            WebClient saveclient = new WebClient();
            saveclient.UploadStringCompleted += new UploadStringCompletedEventHandler(saveclient_UploadStringCompleted);
            saveclient.UploadStringAsync(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + _data.Path.Replace("/api/mycomputer/list/", "/api/mycomputer/save/").Replace("/Download/", "/api/mycomputer/save/")), "POST", _d, new BUserState(resort, _d, _data.Name));

            _data.Name = name1.Text = name2.Text = name3.Text = name4.Text = name5.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox1.Text = newname;
            if (_data.Icon.Contains("NewFolder")) { _data.Icon.Replace("NewFolder", "folder"); Data = _data; }
            return 0;
            //so some saving stuff;
        }

        public void saveclient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new UploadStringCompletedEventHandler(saveclient_UploadStringCompleted2), sender, e);
        }

        private BUserState state;

        public void saveclient_UploadStringCompleted2(object sender, UploadStringCompletedEventArgs e)
        {
            state = (BUserState)e.UserState;
            if (e.Result == "DONE")
            {
                this.IsRename = false;
                _data.Path = _data.Path.Replace(state.oldname, _data.Name);
                if (ReSort != null && state.Resort) ReSort(this, true);
            }
            else if (e.Result.StartsWith("EXISTS"))
            {
                string[] res = e.Result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                state = (BUserState)e.UserState;
                FileExists fe = new FileExists(res[1], res[2], res[3], res[4], res[5], res[6], res[7], image1.Source);
                fe.FileExistComplete += new FileExistHandler(fe_FileExistComplete);
            }
            else
            {
                MessageBox.Show(e.Result);
            }
        }

        private void fe_FileExistComplete(object sender, ReplaceResult e)
        {
            Dispatcher.BeginInvoke(new FileExistHandler(fe_FileExistComplete2), sender, e);
        }

        private void fe_FileExistComplete2(object sender, ReplaceResult e)
        {
            if (e == ReplaceResult.Cancel)
            {
                _data.Name = state.oldname;
                switch (Mode)
                {
                    case ViewMode.Tile: textBox1.Text = _data.Name; break;
                    case ViewMode.List: textBox2.Text = _data.Name; break;
                    case ViewMode.LargeIcon: textBox3.Text = _data.Name; break;
                    case ViewMode.SmallIcon: textBox4.Text = _data.Name; break;
                    case ViewMode.Icon: textBox5.Text = _data.Name; break;
                }
            }
            else
            {
                string newname = "";
                switch (Mode)
                {
                    case ViewMode.Tile: newname = textBox1.Text; break;
                    case ViewMode.List: newname = textBox2.Text; break;
                    case ViewMode.LargeIcon: newname = textBox3.Text; break;
                    case ViewMode.SmallIcon: newname = textBox4.Text; break;
                    case ViewMode.Icon: newname = textBox5.Text; break;
                }
                if (e == ReplaceResult.Rename)
                {
                    Regex reg = new Regex("\\(\\d\\)", RegexOptions.IgnoreCase);
                    Match m = reg.Match(newname);
                    int i = 1;
                    if (m.Success) i = int.Parse(m.Value.Remove(m.Value.IndexOf(')')).Remove(0, 1));
                    i++;
                    if (m.Success)
                        newname = newname.Replace(m.Value, "(" + i + ")");
                    else newname += " (" + i + ")";
                }

                string _d = "";
                if (e == ReplaceResult.Replace)
                    _d = "OVERWRITE:";
                else _d = "SAVETO:";
                _d += newname;

                WebClient saveclient = new WebClient();
                saveclient.UploadStringCompleted += new UploadStringCompletedEventHandler(saveclient_UploadStringCompleted);
                saveclient.UploadStringAsync(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + _data.Path.Replace("/api/mycomputer/list/", "/api/mycomputer/save/").Replace("/Download/", "/api/mycomputer/save/")), "POST", _d, new BUserState(state.Resort, _d, _data.Name));

                _data.Name = name1.Text = name2.Text = name3.Text = name4.Text = name5.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox1.Text = newname;
            }
        }

        public int Move(bool resort, string folder)
        {
            string _d = "";
            _d = "SAVETO:" + folder.Replace("/Extranet/api/mycomputer/list/", "").Replace('/', '\\') + "\\" + _data.Name;

            WebClient saveclient = new WebClient();
            saveclient.UploadStringCompleted += new UploadStringCompletedEventHandler(saveclient_UploadStringCompleted3);
            saveclient.UploadStringAsync(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + _data.Path.Replace("/api/mycomputer/list/", "/api/mycomputer/save/").Replace("/Download/", "/api/mycomputer/save/")), "POST", _d, new BUserState(resort, _d, _data.Name));
            return 0;
        }

        public void saveclient_UploadStringCompleted3(object sender, UploadStringCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new UploadStringCompletedEventHandler(saveclient_UploadStringCompleted4), sender, e);
        }

        public void saveclient_UploadStringCompleted4(object sender, UploadStringCompletedEventArgs e)
        {
            state = (BUserState)e.UserState;
            if (e.Result == "DONE")
            {
                ((WrapPanel)Parent).Children.Remove(this);
                if (ReSort != null && state.Resort) ReSort(this, true);
            }
            else if (e.Result.Contains("EXISTS"))
            {
                string[] res = e.Result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                FileExists fe = new FileExists(res[1], res[2], res[3], res[4], res[5], res[6], res[7], image1.Source);
                fe.FileExistComplete += new FileExistHandler(fe_FileExistComplete3);
                fe.Show();
            }
            else MessageBox.Show(e.Result);
        }

        private void fe_FileExistComplete3(object sender, ReplaceResult e)
        {
            Dispatcher.BeginInvoke(new FileExistHandler(fe_FileExistComplete4), sender, e);
        }

        private void fe_FileExistComplete4(object sender, ReplaceResult e)
        {
            if (e != ReplaceResult.Cancel)
            {
                string _d = state.Data;
                if (e == ReplaceResult.Rename)
                {
                    Regex reg = new Regex("\\(\\d\\)", RegexOptions.IgnoreCase);
                    Match m = reg.Match(_d);
                    int i = 1;
                    if (m.Success) i = int.Parse(m.Value.Remove(m.Value.IndexOf(')')).Remove(0, 1));
                    i++;
                    if (m.Success)
                        _d = _d.Replace(m.Value, "(" + i + ")");
                    else _d += " (" + i + ")";
                }
                else _d.Replace("SAVETO", "OVERWRITE");
                //_data.Name = name1.Text = name2.Text = name3.Text = name4.Text = name5.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox1.Text = newname;

                WebClient saveclient = new WebClient();
                saveclient.UploadStringCompleted += new UploadStringCompletedEventHandler(saveclient_UploadStringCompleted3);
                saveclient.UploadStringAsync(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + _data.Path.Replace("/api/mycomputer/list/", "/api/mycomputer/save/").Replace("/Download/", "/api/mycomputer/save/")), "POST", _d, new BUserState(state.Resort, _d, _data.Name));
            }
        }

        public int Delete()
        {
            if (MessageBox.Show("Are you sure you want to delete\n" + _data.Name + "?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                WebClient deleteclient = new WebClient();
                deleteclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(deleteclient_DownloadStringCompleted);
                deleteclient.DownloadStringAsync(new Uri(HtmlPage.Document.DocumentUri.Scheme + "://" + HtmlPage.Document.DocumentUri.Host + _data.Path.Replace("/api/mycomputer/list/", "/api/mycomputer/delete/").Replace("/Download/", "/api/mycomputer/delete/")), false);
            }
            return 0;
            //so some saving stuff;
        }

        private void deleteclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new DownloadStringCompletedEventHandler(deleteclient_DownloadStringCompleted2), sender, e);
        }
        private void deleteclient_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Result.StartsWith("ERROR"))
                MessageBox.Show(e.Result);
            else
            {
                ((WrapPanel)Parent).Children.Remove(this);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { e.Handled = true; Save(); }
            else if (e.Key == Key.Divide) e.Handled = true;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            textBox1.Text = textBox2.Text;
            textBox1_KeyDown(sender, e);
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            textBox1.Text = textBox3.Text;
            textBox1_KeyDown(sender, e);
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            textBox1.Text = textBox4.Text;
            textBox1_KeyDown(sender, e);
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            textBox1.Text = textBox5.Text;
            textBox1_KeyDown(sender, e);
        }

        public int CompareTo(object obj)
        {
            return _data.Name.CompareTo(((BrowserItem)obj).Data.Name);
        }

        public event DragEventHandler ItemDrop;

        private void LayoutRoot_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            Active = true;
            if (Activate != null) Activate(this, new EventArgs());
            if (ItemDrop != null) ItemDrop(sender, e);
        }

        public void Hover()
        {
            if (!Active)
            {
                Tile.BorderBrush = Icon.BorderBrush = List.BorderBrush = SmallIcon.BorderBrush = MediumIcon.BorderBrush = Resources["LBB"] as SolidColorBrush;
                Tile.Background = Icon.Background = List.Background = SmallIcon.Background = MediumIcon.Background = Resources["hover"] as LinearGradientBrush;
            }
            else Tile.Background = Icon.Background = List.Background = SmallIcon.Background = MediumIcon.Background = Resources["ActiveBG"] as LinearGradientBrush;
        }

        public void Leave()
        {
            if (!Active) Tile.BorderBrush = Icon.BorderBrush = List.BorderBrush = Tile.Background = Icon.Background = List.Background = SmallIcon.BorderBrush = MediumIcon.BorderBrush = SmallIcon.Background = MediumIcon.Background = new SolidColorBrush(Colors.Transparent);
            else Tile.Background = Icon.Background = List.Background = SmallIcon.Background = MediumIcon.Background = new SolidColorBrush(Colors.Transparent);
        }
    }

    public delegate void ChangeDirectoryHandler(object sender, BItem e);

    public class BItem : IComparable
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public BType BType { get; set; }
        public string Path { get; set; }
        public AccessControlActions AccessControl { get; set; }

        public BItem(string name, string icon, string size, string type, BType btype, string path, AccessControlActions access)
        {
            Name = name;
            Icon = icon;
            Type = type;
            Size = size;
            BType = btype;
            Path = path;
            AccessControl = access;
        }

        public BItem(string name, string icon, string size, string type, BType btype, string path, string access)
        {
            Name = name;
            Icon = icon;
            Type = type;
            Size = size;
            BType = btype;
            Path = path;
            AccessControl = (AccessControlActions)Enum.Parse(typeof(AccessControlActions), access, true);
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((BItem)obj).Name);
        }
    }

    public enum BType { Folder, File, Drive }

    public class BUserState
    {
        public BUserState(bool Resort, string Data, string oldname)
        {
            this.Resort = Resort;
            this.Data = Data;
            this.oldname = oldname;
        }

        public bool Resort { get; set; }
        public string Data { get; set; }
        public string oldname { get; set; }
    }

    public enum AccessControlActions { Change, View, None }
}
