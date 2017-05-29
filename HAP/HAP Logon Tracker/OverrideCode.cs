using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HAP.Tracker.UI
{
    public partial class OverrideCode : Form
    {
        public OverrideCode(string code)
        {
            InitializeComponent();
            Code = code;
        }
        private string Code;

        private void Done_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == Code)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show(this, "The code supplied is wrong!", "Wrong Code", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

    }
}
