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
using HAP.Silverlight.Browser.service;
using System.ServiceModel;

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
            if (!string.IsNullOrEmpty(data.Size) && data.BType == service.BType.Drive)
            {
                double s;
                if (double.TryParse(data.Size, out s))
                {
                    if (s > -1)
                    {
                        IsSpace = true;
                        Space = s;
                    }
                    else { size.Text = ""; }
                } else size.Text = "";
            }
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

                image1.Source = image2.Source = image3.Source = image4.Source = image5.Source = new BitmapImage(new Uri(HtmlPage.Document.DocumentUri, _data.Icon));
                this.AllowDrop = (_data.BType == service.BType.Folder) && _data.AccessControl == service.AccessControlActions.Change;
                key1.Visibility = key2.Visibility = key3.Visibility = key4.Visibility = key5.Visibility = (_data.AccessControl == service.AccessControlActions.Change ? Visibility.Collapsed : System.Windows.Visibility.Visible);
                tooltip1.Content = tooltip2.Content = tooltip3.Content = tooltip4.Content = tooltip5.Content = (_data.AccessControl == service.AccessControlActions.None ? "This file/folder may not be accessible" : "This file/folder has restrictive permissions");
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
                if (_data.BType == service.BType.File) HtmlPage.PopupWindow(new Uri(HtmlPage.Document.DocumentUri, Data.Source.Download), "_hapdownload", new HtmlPopupWindowOptions());
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
            if (newname == "" || newname.ToLower().Equals(_data.Name.ToLower()))
            {
                if (MessageBox.Show("Please enter a different name for this file/folder or press cancel to keep the current name.", "Error", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    this.IsRename = false;
                return -1;
            }
            string _d = _data.Source.Path.Remove(0, _data.Source.Path.LastIndexOf('\\'));
            _d = _d.Replace(_data.Source.Name, newname);
            _d = _data.Source.Path.Remove(_data.Source.Path.LastIndexOf('\\')) + _d;
            apiSoapClient soap = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
            soap.SaveCompleted += new EventHandler<SaveCompletedEventArgs>(soap_SaveCompleted);
            soap.SaveAsync((CBFile)_data.Source, _d, false, new BUserState(resort, _d, _data.Name));

            _data.Name = name1.Text = name2.Text = name3.Text = name4.Text = name5.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox1.Text = newname;
            if (_data.Icon.Contains("NewFolder")) { _data.Icon.Replace("NewFolder", "folder"); Data = _data; }
            return 0;
            //so some saving stuff;
        }

        void soap_SaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke( new EventHandler<SaveCompletedEventArgs>(soap_SaveCompleted2), sender, e);
        }

        private BUserState state;

        void soap_SaveCompleted2(object sender, SaveCompletedEventArgs e)
        {
            state = (BUserState)e.UserState;
            if (e.Error != null)
                MessageBox.Show(e.Error.ToString());
            else if (e.Result.Count == 0)
            {
                this.IsRename = false;
                _data.Path = _data.Path.Replace(state.oldname, _data.Name);
                if (ReSort != null && state.Resort) ReSort(this, true);
            }
            else
            {
                state = (BUserState)e.UserState;
                FileExists fe = new FileExists(_data.Source, e.Result[0], image1.Source);
                fe.FileExistComplete += new FileExistHandler(fe_FileExistComplete);
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

                string _d = _data.Source.Path.Remove(0, _data.Source.Path.LastIndexOf('\\'));
                _d = _d.Replace(_data.Source.Name, newname);
                _d = _data.Source.Path.Remove(_data.Source.Path.LastIndexOf('\\')) + _d;
                apiSoapClient soap = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
                soap.SaveCompleted += new EventHandler<SaveCompletedEventArgs>(soap_SaveCompleted);
                soap.SaveAsync((CBFile)_data.Source, _d, e == ReplaceResult.Replace, new BUserState(state.Resort, _d, _data.Name));

                _data.Name = name1.Text = name2.Text = name3.Text = name4.Text = name5.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox1.Text = newname;
            }
        }

        public int Move(bool resort, string folder)
        {
            apiSoapClient soap1 = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
            soap1.SaveCompleted += new EventHandler<SaveCompletedEventArgs>(soap1_SaveCompleted);
            soap1.SaveAsync(this._data.Source, folder + "\\" + _data.Name, false, new BUserState(resort, folder + "\\" + _data.Name, _data.Name));
            return 0;
        }

        void soap1_SaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new EventHandler<SaveCompletedEventArgs>(soap1_SaveCompleted2), sender, e);
        }


        public void soap1_SaveCompleted2(object sender, SaveCompletedEventArgs e)
        {
            state = (BUserState)e.UserState;
            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            else if (e.Result.Count == 0)
            { 
                ((WrapPanel)Parent).Children.Remove(this);
                if (ReSort != null && state.Resort) ReSort(this, true);
            }
            else 
            {
                FileExists fe = new FileExists(_data.Source, e.Result[0], image1.Source);
                fe.FileExistComplete += new FileExistHandler(fe_FileExistComplete3);
                fe.Show();
            }
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
                //_data.Name = name1.Text = name2.Text = name3.Text = name4.Text = name5.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox1.Text = newname;
                apiSoapClient soap1 = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
                soap1.SaveCompleted += new EventHandler<SaveCompletedEventArgs>(soap1_SaveCompleted);
                soap1.SaveAsync(this._data.Source, _d, e == ReplaceResult.Replace, new BUserState(state.Resort, _d, _data.Name));
            }
        }

        public int Delete()
        {
            apiSoapClient soap = new apiSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(new Uri(HtmlPage.Document.DocumentUri, "api.asmx").ToString()));
            soap.DeleteCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(soap_DeleteCompleted);
            if (MessageBox.Show("Are you sure you want to delete\n" + _data.Name + "?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK) soap.DeleteAsync(_data.Path);
            return 0;
            //so some saving stuff;
        }

        void soap_DeleteCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(soap_DeleteCompleted2), sender, e);
        }


        private void soap_DeleteCompleted2(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null) MessageBox.Show(e.Error.ToString());
            else
            {
                _data.Delete();
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
}
