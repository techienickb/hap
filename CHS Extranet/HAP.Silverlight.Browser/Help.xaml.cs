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
using System.Windows.Browser;
using System.Reflection;

namespace HAP.Silverlight.Browser
{
    public partial class Help : ChildWindow
    {
        public Help()
        {
            InitializeComponent();
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] parts = asm.FullName.Split(',');
            string version = parts[1].Remove(0, parts[1].IndexOf('=') + 1);
            browserversion.Text = "Browser Version: " + version;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HtmlPage.PopupWindow(new Uri("http://chsextranet.codeplex.com"), "_codeplex", new HtmlPopupWindowOptions());
        }
    }
}

