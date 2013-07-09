using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using System.Reflection;
using HAP.Web.Configuration;

namespace HAP.Web.Controls
{
    public partial class UpdateChecker : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = Page.User.IsInRole("Domain Admins");
        }

    }
}