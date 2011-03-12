using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HAP.Web.Tracker
{
    public partial class XML2SQL : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            cantupgrade.Visible = HAP.Web.Configuration.hapConfig.Current.Tracker.Provider == "XML";
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