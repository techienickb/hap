using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace HAP.UserCard
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        public Dialog()
        {
            InitializeComponent();
        }

        public bool isDialog { get; private set; }

        public Nullable<bool> ShowDialog(string text, [Optional, DefaultParameterValue("User Card Info")] string caption, [Optional, DefaultParameterValue(DialogIcon.Info)] DialogIcon Icon)
        {
            this.text.Text = text;
            this.Title = caption;
            BitmapImage icon = new BitmapImage();
            icon.BeginInit();
            if (Icon == DialogIcon.Warning) icon.UriSource = new Uri("pack://application:,,,/HAP User Card;component/Images/256.png");
            else if (Icon == DialogIcon.Error) icon.UriSource = new Uri("pack://application:,,,/HAP User Card;component/Images/262.png");
            else if (Icon == DialogIcon.Stop) icon.UriSource = new Uri("pack://application:,,,/HAP User Card;component/Images/263.png");
            else icon.UriSource = new Uri("pack://application:,,,/HAP User Card;component/Images/261.png");
            icon.EndInit();
            image1.Source = icon;
            isDialog = true;
            return ShowDialog();
        }

        public void Show(string text, [Optional, DefaultParameterValue("User Card Info")] string caption, [Optional, DefaultParameterValue(DialogIcon.Info)] DialogIcon Icon)
        {
            this.text.Text = text;
            this.Title = caption;
            BitmapImage icon = new BitmapImage();
            icon.BeginInit();
            if (Icon == DialogIcon.Warning) icon.UriSource = new Uri("pack://application:,,,/HAP User Card;component/Images/256.png");
            else if (Icon == DialogIcon.Error) icon.UriSource = new Uri("pack://application:,,,/HAP User Card;component/Images/262.png");
            else if (Icon == DialogIcon.Stop) icon.UriSource = new Uri("pack://application:,,,/HAP User Card;component/Images/263.png");
            else icon.UriSource = new Uri("pack://application:,,,/HAP User Card;component/Images/261.png");
            icon.EndInit();
            image1.Source = icon;
            isDialog = false;
            Show();
        }

        public static void ShowMessage(string text, [Optional, DefaultParameterValue("User Card Info")] string caption, [Optional, DefaultParameterValue(DialogIcon.Info)] DialogIcon Icon)
        {
            new Dialog().Show(text, caption, Icon);
        }

        public static Nullable<bool> ShowMessageDialog(string text, [Optional, DefaultParameterValue("User Card Info")] string caption, [Optional, DefaultParameterValue(DialogIcon.Info)] DialogIcon Icon)
        {
            return new Dialog().ShowDialog(text, caption, Icon);
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            if (isDialog) DialogResult = true;
            Close();
        }
    }

    public enum DialogIcon { Info, Error, Stop, Warning }
}
