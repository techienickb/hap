using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using HAP.Web.Configuration;
using System.IO;
using System.DirectoryServices.AccountManagement;
using System.Web.Security;

namespace HAP.Web.Controls
{
    public partial class NewFolder : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) foldername.Text = string.Empty;
        }

        public DirectoryInfo Directory { get; set; }

        protected void yes_Click(object sender, EventArgs e)
        {
            ((HAP.AD.User)Membership.GetUser()).Impersonate();
            Directory.CreateSubdirectory(foldername.Text);
            Page.DataBind();
            ((HAP.AD.User)Membership.GetUser()).EndImpersonate();
        }
    }
}