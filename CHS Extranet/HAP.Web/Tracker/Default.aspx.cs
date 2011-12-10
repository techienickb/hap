using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.IO;

namespace HAP.Web.Tracker
{
    public partial class Default : HAP.Web.Controls.Page
    {
        public Default() { this.SectionTitle = "Logon Tracker"; }
        protected void Page_Load(object sender, EventArgs e)
        {
            dbup.Visible = (config.Tracker.Provider != "XML" && new DirectoryInfo(Server.MapPath("~/App_Data/")).GetFiles("tracker*xml").Length > 0);
            weblog.Visible = config.Tracker.Provider != "XML";
        }
    }
}