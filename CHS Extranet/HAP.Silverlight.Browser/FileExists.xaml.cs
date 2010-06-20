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
using System.Text.RegularExpressions;

namespace HAP.Silverlight.Browser
{
    public partial class FileExists : ChildWindow
    {
        public FileExists()
        {
            InitializeComponent();
        }

        public FileExists(string filename, string path, string size, string datemod, string path2, string size2, string datemod2, ImageSource imageuri)
        {
            InitializeComponent();
            image1.Source = image2.Source = imageuri;
            Regex reg = new Regex("\\(\\d\\)", RegexOptions.IgnoreCase);
            Match m = reg.Match(filename);
            int i = 1;
            if (m.Success) i = int.Parse(m.Value.Remove(m.Value.IndexOf(')')).Remove(0, 1));
            i++;
            this.name3.Text = string.Format("The file you are copying will be renamed \"{0} ({1})\"", filename, i);
            this.name1.Text = this.name2.Text = filename;
            this.path1.Text = string.Format("{0} ({1})", filename, path);
            this.path2.Text = string.Format("{0} ({1})", filename, path2);
            this.size1.Text = string.Format("Size: {0}", size);
            this.size2.Text = string.Format("Size: {0}", size2);
            this.date1.Text = string.Format("Date modified: {0}", datemod);
            this.date2.Text = string.Format("Date modified: {0}", datemod);

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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileExistComplete != null) FileExistComplete(this, ReplaceResult.Cancel);
            this.DialogResult = false;
        }
    }

    public enum ReplaceResult { Replace, Cancel, Rename }
    public delegate void FileExistHandler(object sender, ReplaceResult e);
}

