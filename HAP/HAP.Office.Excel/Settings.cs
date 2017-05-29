using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HAP.Office.Excel
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            url.Text = Properties.Settings.Default.Url;
            username.Text = Properties.Settings.Default.Username;
            password.Text = Properties.Settings.Default.Password;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Url = url.Text;
            Properties.Settings.Default.Username = username.Text;
            Properties.Settings.Default.Password = password.Text;
            Properties.Settings.Default.Save();
        }
    }
}
