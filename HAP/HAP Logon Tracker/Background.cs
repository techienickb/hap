using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HAP.Tracker.UI
{
    public partial class Background : Form
    {
        public Background()
        {
            InitializeComponent();
            Main = new Main();
        }

        public Main Main { get; set; }

        private void Background_Load(object sender, EventArgs e)
        {
        }

        private void Background_Shown(object sender, EventArgs e)
        {
            Main.ShowDialog(this);
            this.Close();
        }
    }
}
