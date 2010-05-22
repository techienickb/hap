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

namespace HAP.Silverlight.Browser
{
    public partial class FileExists : ChildWindow
    {
        public FileExists()
        {
            InitializeComponent();
        }

        private void CReplace_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).BorderBrush = Resources["LBB"] as SolidColorBrush;
            ((Border)sender).Background = Resources["hover"] as LinearGradientBrush;
        }

        private void CReplace_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).BorderBrush = ((Border)sender).Background =new SolidColorBrush(Colors.Transparent);
        }

        private void CReplace_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Border)sender).Background = Resources["ActiveBG"] as LinearGradientBrush;
        }

        public event FileExistHandler FileExistComplete;

        private void CReplace_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //replace file
            if (FileExistComplete != null) FileExistComplete(this, ReplaceResult.Rename);
            this.DialogResult = true;
        }

        private void CDont_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (FileExistComplete != null) FileExistComplete(this, ReplaceResult.Cancel);
            this.DialogResult = false;
        }

        private void CRename_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (FileExistComplete != null) FileExistComplete(this, ReplaceResult.Rename);
            this.DialogResult = true;
        }
    }

    public enum ReplaceResult { Replace, Cancel, Rename }
    public delegate void FileExistHandler(object sender, ReplaceResult e);
}

