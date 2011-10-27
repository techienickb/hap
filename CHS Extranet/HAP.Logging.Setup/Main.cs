using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace HAP.Logging.Setup
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            create.Enabled = !EventLog.SourceExists("Home Access Plus+");
        }

        private void create_Click(object sender, EventArgs e)
        {
            create.Enabled = false;
            if (!EventLog.SourceExists("Home Access Plus+")) EventLog.CreateEventSource("Home Access Plus+", "Home Access Plus+");
            MessageBox.Show("Event Viewer Log Created", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
