using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using HAP.Web.Configuration;

namespace HAP.Web
{
    public partial class unauthorised : HAP.Web.Controls.Page
    {
        public unauthorised()
        {
            SectionTitle = "Unauthorised";
        }
    }
}