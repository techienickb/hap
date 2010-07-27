using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;

namespace HAP.Web
{
    public partial class MyComputerSL : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInitComplete(EventArgs e)
        {
            this.Title = string.Format("{0} - Home Access Plus+ - My School Computer", hapConfig.Current.BaseSettings.EstablishmentName);
        }
    }
}