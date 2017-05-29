using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HAP.Web.Tracker
{
    public partial class XML2SQL : HAP.Web.Controls.Page
    {
        public XML2SQL()
        {
            this.SectionTitle = "Logon Tracker - XML 2 SQL Converter";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cantupgrade.Visible = config.Tracker.Provider == "XML";
            canupgrade.Visible = !cantupgrade.Visible;
        }

        protected void upgrade_Click(object sender, EventArgs e)
        {
            HAP.Data.SQL.Tracker.UpgradeFromXML();
            upgraded.Text = "<h2>Import Complete</h2>";
            canupgrade.Visible = cantupgrade.Visible = false;
        }
    }
}