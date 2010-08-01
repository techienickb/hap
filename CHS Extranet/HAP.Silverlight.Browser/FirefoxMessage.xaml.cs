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

namespace HAP.Silverlight.Browser
{
    public partial class FirefoxMessage : ChildWindow
    {
        public FirefoxMessage()
        {
            InitializeComponent();
        }

        public static string GetCookie()
        {
            string[] cookies = HtmlPage.Document.Cookies.Split(';');
            foreach (string cookie in cookies)
            {
                string[] keyValue = cookie.Split('=');

                if (keyValue.Length == 2)
                {
                    if (keyValue[0].ToString() == "hapslwarn")
                        return keyValue[1];
                }
            }
            return null;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            if (checkBox1.IsChecked.Value)
            {
                DateTime expireDate = DateTime.Now.AddMonths(1);
                string newCookie = "hapslwarn=ok;expires=" + expireDate.ToString("R");
                HtmlPage.Document.SetProperty("cookie", newCookie);
            }
        }



    }
}

