using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace HAP.Silverlight
{
    public class uploadstate
    {
        public uploadstate(HttpWebRequest request, File file)
        {
            Request = request;
            File = file;
        }

        public HttpWebRequest Request { get; set; }
        public File File { get; set; }
    }
}
