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
using System.IO;
using System.Windows.Browser;

namespace HAP.Silverlight
{
    public partial class MainPage : UserControl
    {
        private string path;
        private string filters;
        private string BaseUri;

        [ScriptableMember()]
        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }
        [ScriptableMember()]
        public void Show()
        {
            this.Visibility = Visibility.Visible;

        }

        public MainPage(string Path, string Filters, string baseuri)
        {
            InitializeComponent();
            path = Path;
            filters = Filters;
            BaseUri = baseuri;
            try
            {
                AllowDrop = true;
            }
            catch
            {
                textBlock1.Text = "Click the '...' button to upload (Drag disabled due to Firefox restrictions)";
            }
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            stackPanel1.Background = new SolidColorBrush(Colors.Transparent);
            if (e.Data == null) return;

            try
            {

                FileInfo[] files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

                if (files == null) return;

                if (stackPanel1.Children.Count == 1 && stackPanel1.Children[0].GetType() == typeof(TextBlock)) stackPanel1.Children.Clear();

                foreach (FileInfo file in files)
                {
                    if (filters.Contains("*.*") || filters.ToLower().Contains(file.Extension.Trim(new char[] { '.' }).ToLower()))
                    {
                        File f = new File();
                        f.BaseUri = BaseUri;
                        f.path = path;
                        f.Fileinfo = file;
                        f.RemoveClick += new RoutedEventHandler(f_RemoveClick);
                        f.Uploaded += new RoutedEventHandler(f_Uploaded);
                        f.Status = "Checking...";
                        stackPanel1.Children.Add(f);
                        f.UpdateUI("");
                        f.Check();
                        if (uploadbutton.Content.ToString() == "Upload") uploadbutton.IsEnabled = clearbutton.IsEnabled = true;
                    }
                    else MessageBox.Show("File Type Not Allowed (" + file.Extension.ToLower() + ")", "This file is not allowed to be uploaded", MessageBoxButton.OK);
                }
            }
            catch { MessageBox.Show("The item you have dragged here is not supported");  }

        }

        void f_RemoveClick(object sender, RoutedEventArgs e)
        {
            stackPanel1.Children.Remove((File)sender);
            if (stackPanel1.Children.Count == 0)
            {
                TextBlock tb = new TextBlock();
                tb.Text = "Drag Files Here...";
                tb.Height = 36;
                tb.FontSize = 20;
                tb.Name = "textblock1";
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                stackPanel1.Children.Add(tb);
                uploadbutton.IsEnabled = clearbutton.IsEnabled = false;
            }
        }

        private void SelectFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = filters;
                dlg.Multiselect = true;

                if (dlg.ShowDialog().Value)
                {
                    if (stackPanel1.Children.Count == 1 && stackPanel1.Children[0].GetType() == typeof(TextBlock)) stackPanel1.Children.Clear();
                    foreach (FileInfo file in dlg.Files)
                    {
                        File f = new File();
                        f.BaseUri = BaseUri;
                        f.path = path;
                        f.Fileinfo = file;
                        f.Status = "Checking...";
                        f.UpdateUI("");
                        f.Uploaded += new RoutedEventHandler(f_Uploaded);
                        f.RemoveClick += new RoutedEventHandler(f_RemoveClick);
                        stackPanel1.Children.Add(f);
                        f.Check();
                        if (uploadbutton.Content.ToString() == "Upload") uploadbutton.IsEnabled = clearbutton.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        void f_Uploaded(object sender, RoutedEventArgs e)
        {
            foreach (File f in stackPanel1.Children)
            {
                if (f.Status.StartsWith("Ready"))
                {
                    f.Status = "Uploading...";
                    f.UpdateUI("");
                    f.Upload();
                    break;
                }
            }
            uploadbutton.IsEnabled = clearbutton.IsEnabled = false;
            uploadbutton.Content = "Upload";
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
            stackPanel1.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void UserControl_DragLeave(object sender, DragEventArgs e)
        {
            stackPanel1.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void UserControl_DragEnter(object sender, DragEventArgs e)
        {
            stackPanel1.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void uploadbutton_Click(object sender, RoutedEventArgs e)
        {
            uploadbutton.IsEnabled = clearbutton.IsEnabled = false;
            uploadbutton.Content = "Uploading...";
            foreach (File f in stackPanel1.Children)
            {
                if (f.Status.StartsWith("Ready"))
                {
                    f.Status = "Uploading...";
                    f.UpdateUI("");
                    f.Upload();
                    break;
                }
            }
        }

        private void clearbutton_Click(object sender, RoutedEventArgs e)
        {
            stackPanel1.Children.Clear();
            TextBlock tb = new TextBlock();
            tb.Text = "Drag Files Here...";
            tb.Height = 36;
            tb.FontSize = 20;
            tb.Name = "textblock1";
            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            stackPanel1.Children.Add(tb);
            uploadbutton.IsEnabled = clearbutton.IsEnabled = false;
        }

    }
}
