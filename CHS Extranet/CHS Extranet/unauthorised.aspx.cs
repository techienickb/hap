using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using CHS_Extranet.Configuration;

namespace CHS_Extranet
{
    public partial class unauthorised : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            extranetConfig config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
            this.Title = string.Format("{0} - Home Access Plus+ - Unauthorised", config.BaseSettings.EstablishmentName);
        }
    }
}