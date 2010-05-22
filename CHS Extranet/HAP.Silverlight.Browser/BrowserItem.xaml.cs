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

namespace HAP.Silverlight.Browser
{
    public partial class BrowserItem : UserControl, IComparable
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
                image1.Source = image2.Source = image3.Source = image4.Source = image5.Source = new BitmapImage(new Uri(_data.Icon, UriKind.Relative));
                this.AllowDrop = (_data.BType == BType.Folder);
            }
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
        public event EventHandler ReSort;
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
                else Tile.BorderBrush = Icon.BorderBrush = List.BorderBrush = Tile.Background = Icon.Background = List.Background = SmallIcon.BorderBrush = MediumIcon.BorderBrush = SmallIcon.Background = MediumIcon.Background = new SolidColorBrush(Colors.Transparent);
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
                if (_data.BType == BType.Folder)
                {
                    MessageBox.Show("Navigation to folder");
                    if (DirectoryChange != null) DirectoryChange(this, this._data);
                }
                else MessageBox.Show("Downloading file");
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { e.Handled = true; Save(); }
        }

        public int Save()
        {
            return Save(true);
        }

        public int Save(bool resort)
        {
            if (textBox1.Text == "") { MessageBox.Show("I Can't Rename this to Nothing", "Error", MessageBoxButton.OK); return -1; }
            _data.Name = name1.Text = name2.Text = name3.Text = name4.Text = name5.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox1.Text;
            this.IsRename = false;
            if (ReSort != null && resort) ReSort(this, new EventArgs());
            return 0;
            //so some saving stuff;
        }

        public int Delete()
        {
            return 0;
            //so some saving stuff;
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

        public BItem(string name, string icon, string size, string type, BType btype, string path)
        {
            Name = name;
            Icon = icon;
            Type = type;
            Size = size;
            BType = btype;
            Path = path;
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((BItem)obj).Name);
        }
    }

    public enum BType { Folder, File, Drive }
}
