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
using System.Windows.Resources;

namespace HAP.Silverlight.Browser
{
    public partial class ViewButton : UserControl
    {
        public ViewButton()
        {
            InitializeComponent();
            Mode = ViewMode.Tile;
        }

        public string Text
        {
            get { return textBlock1.Text; }
            set { textBlock1.Text = value; }
        }

        public event EventHandler Click;

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            BBorder1.Background = Resources["BarBg"] as LinearGradientBrush;
            BBorder1.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 187, 202, 219));
            BBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 239, 244, 249));
        }

        private void BBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            BBorder1.Background = BBorder1.BorderBrush = BBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void BBorder1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
            BBorder1.Background = Resources["DownBg"] as LinearGradientBrush;
        }

        private void BBorder1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BBorder1.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 187, 202, 219));
            BBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 239, 244, 249));
            BBorder1.Background = new SolidColorBrush(Colors.Transparent);
            context.IsOpen = true;
            context.VerticalOffset = 25 - e.GetPosition(BBorder1).Y;
        }

        public ViewMode Mode { get; set; }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item.Header.ToString() == "Large Icons") Mode = ViewMode.LargeIcon;
            else if (item.Header.ToString() == "List") Mode = ViewMode.List;
            else if (item.Header.ToString() == "Tiles") Mode = ViewMode.Tile;
            else if (item.Header.ToString() == "Small Icons") Mode = ViewMode.SmallIcon;
            else if (item.Header.ToString() == "Medium Icons") Mode = ViewMode.Icon;
            if (Click != null) Click(this, new EventArgs());
        }
    }
}
