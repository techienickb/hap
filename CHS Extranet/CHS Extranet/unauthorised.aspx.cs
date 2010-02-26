using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace CHS_Extranet
{
    public partial class unauthorised : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = string.Format("{0} - Home Access Plus+ - Unauthorised", ConfigurationManager.AppSettings["SchoolName"]);
        }
    }
}