﻿using System;
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
    public partial class OrganiseButton : UserControl
    {
        public OrganiseButton()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return textBlock1.Text; }
            set { textBlock1.Text = value; }
        }

        public bool IsRename
        {
            get { return cRename.IsEnabled; }
            set { cRename.IsEnabled = value; }
        }

        public bool IsDelete
        {
            get { return cDelete.IsEnabled; }
            set { cDelete.IsEnabled = value; }
        }

        public event RoutedEventHandler Delete;
        public event RoutedEventHandler Rename;
        public event RoutedEventHandler SelectAllClick;

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
            context.VerticalOffset = 25 - e.GetPosition(BBorder).Y;
            context.HorizontalOffset = 0 - e.GetPosition(BBorder).Y;
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (SelectAllClick != null) SelectAllClick(sender, e);
        }

        private void cDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Delete != null) Delete(sender, e);
            cDelete.IsEnabled = cRename.IsEnabled = false;
        }

        private void cRename_Click(object sender, RoutedEventArgs e)
        {
            if (Rename != null) Rename(sender, e);
            cDelete.IsEnabled = cRename.IsEnabled = false;
        }
    }
}