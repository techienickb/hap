using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;

namespace HAP.Web.Tracker
{
    public partial class live : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        hapConfig config;
        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - Logon Tracker - Live Tracker", config.BaseSettings.EstablishmentName);
        }

        protected void refreshtimer_Tick(object sender, EventArgs e)
        {
            ListView1.DataBind();
        }
    }
}