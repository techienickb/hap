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

namespace HAP.Silverlight.Browser
{
    public partial class MaxButton : UserControl
    {
        public MaxButton()
        {
            InitializeComponent();
            if (Max)
            {
                image1.Visibility = System.Windows.Visibility.Collapsed;
                image2.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                image1.Visibility = System.Windows.Visibility.Visible;
                image2.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        public event RoutedEventHandler Click;

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

        public bool Max 
        {
            get
            {
                try
                {
                    if (HtmlPage.Document.Body.GetAttribute("class") == "max") return true;
                }
                catch { }
                return false;
            }
        }

        private void BBorder1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BBorder1.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 187, 202, 219));
            BBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 239, 244, 249));
            BBorder1.Background = new SolidColorBrush(Colors.Transparent);
            if (Max)
            {
                image2.Visibility = System.Windows.Visibility.Collapsed;
                image1.Visibility = System.Windows.Visibility.Visible;
                HtmlPage.Document.Body.RemoveAttribute("class");
            }
            else
            {
                image2.Visibility = System.Windows.Visibility.Visible;
                image1.Visibility = System.Windows.Visibility.Collapsed;
                HtmlPage.Document.Body.SetAttribute("class", "max");
            }
            if (Click != null) Click(this, new RoutedEventArgs());
        }
    }
}
