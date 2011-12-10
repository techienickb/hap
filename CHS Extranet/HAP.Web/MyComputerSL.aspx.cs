using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using HAP.Data;
using System.Web.Security;
using HAP.AD;

namespace HAP.Web
{
    public partial class MyComputerSL : HAP.Web.Controls.Page
    {
        public MyComputerSL() {
            this.SectionTitle = "My School Computer Browser";
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                if (Session["password"] == null)
                {
                    FormsAuthentication.SignOut();
                    FormsAuthentication.RedirectToLoginPage("error=timeout");
                }
                else InitParams.Attributes["value"] = string.Format("token={0}", TokenGenerator.ConvertToToken(ADUser.UserName + "@" + Session["password"].ToString()));
            }
            else InitParams.Attributes["value"] = string.Format("token={0}", TokenGenerator.ConvertToToken(ADUser.UserName + "@NULL"));
        }
    }
}