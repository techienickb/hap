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

namespace HAP.UserCard
{
    /// <summary>
    /// Interaction logic for NewTicket.xaml
    /// </summary>
    public partial class NewTicket : Window
    {
        public NewTicket()
        {
            InitializeComponent();
        }

        public event Action Done;

        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void notetext_KeyUp(object sender, KeyEventArgs e)
        {
            file.IsEnabled = (!string.IsNullOrWhiteSpace(notetext.Text) && !string.IsNullOrWhiteSpace(subject.Text));
        }

        private void file_Click(object sender, RoutedEventArgs e)
        {
            Web.apiSoapClient c = new Web.apiSoapClient();
            c.setNewTicketCompleted += new EventHandler<Web.setNewTicketCompletedEventArgs>(c_setNewTicketCompleted);
            c.setNewTicketAsync(subject.Text, notetext.Text.Replace("\n", "<br />\n"), room.Text, Environment.UserName);
        }

        void c_setNewTicketCompleted(object sender, Web.setNewTicketCompletedEventArgs e)
        {
            if (e.Error != null) Dialog.ShowMessage(e.Error.ToString(), "Error", DialogIcon.Error);
            else
            {
                if (Done != null) Dispatcher.BeginInvoke(Done);
                Dispatcher.BeginInvoke(new Action(Close));
            }
        }
    }
}
