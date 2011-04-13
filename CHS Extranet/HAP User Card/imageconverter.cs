using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows;

namespace HAP.UserCard
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class imageconverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/HAP User Card;component/images/" + (string)value + ".png", UriKind.RelativeOrAbsolute);
            image.EndInit();

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage image = value as BitmapImage;
            return image.UriSource.ToString().Remove(image.UriSource.ToString().LastIndexOf('.')).Remove(0, "pack://application:,,,/HAP User Card;component/images/".Length);
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class htmlconverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            return s.Replace("<br />", "").Replace("&nbsp;", " ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((string)value).Replace("\n", "<br />\n");
        }
    }

    [ValueConversion(typeof(double), typeof(double))]
    public class actualwidthboxconverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value) - 30;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
