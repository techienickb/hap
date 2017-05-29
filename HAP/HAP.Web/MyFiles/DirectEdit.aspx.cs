using HAP.MyFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HAP.Web.MyFiles
{
    public partial class DirectEdit : HAP.Web.Controls.Page
    {
        public DirectEdit()
        {
            this.SectionTitle = Localize("myfiles/directedit");
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string hap
        {
            get
            {
                return DirectEditToken.ConvertToToken(HttpContext.Current.Request.Cookies["token"].Value + "|" + HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value + "|" + HttpContext.Current.Request.Url.ToString().ToLower().Replace("myfiles/directedit", "download"));
            }
        }
    }
}