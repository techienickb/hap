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

namespace HAP.Silverlight.Browser
{
    public class HAPTreeNode : TreeViewItem
    {
        public HAPTreeNode() : base()
        {
            Padding = new Thickness(0);
        }
        public BItem BItem { get; set; }
    }
}
