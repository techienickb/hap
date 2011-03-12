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
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            dbup.Visible = (config.Tracker.Provider != "XML" && new DirectoryInfo(Server.MapPath("~/App_Data/")).GetFiles("tracker*xml").Length > 0);
        }

        hapConfig config;
        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - Logon Tracker", config.BaseSettings.EstablishmentName);
        }
    }
}