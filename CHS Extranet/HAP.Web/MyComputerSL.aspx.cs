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

namespace HAP.Web
{
    public partial class MyComputerSL : HAP.Web.Controls.Page
    {
        public MyComputerSL() {
            this.SectionTitle = "My School Computer Browser";
        }



        protected void Page_Load(object sender, EventArgs e)
        {


            InitParams.Attributes["value"] = string.Format("token={0}", TokenGenerator.ConvertToToken(ADUser.UserName + "@" + Session["password"].ToString()));
        }
    }
}