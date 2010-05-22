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

namespace HAP.Silverlight.Browser
{
    public partial class UploadItem : UserControl
    {
        public UploadItem()
        {
            InitializeComponent();
            Progress.Value = 0;
        }

        public UploadItem(FileInfo file) : this()
        {
            File = file;
        }

        public UploadItem(string name) : this()
        {
            Name.Text = name;
            State = UploadItemState.Debug;
            Progress.Value = 30;
        }

        private FileInfo _file;
        public FileInfo File {

            get { return _file; }
            set
            {
                _file = value;
                Name.Text = _file.Name.Replace(File.Extension, "");
                Check();
            }
        }

        public UploadItemState State { get; set; }

        public double Value { get { return Progress.Value; } }

        private void Check()
        {
            //run the check routine
        }
    }

    public enum UploadItemState { Checking, Ready, Uploading, Done, Debug }
}
