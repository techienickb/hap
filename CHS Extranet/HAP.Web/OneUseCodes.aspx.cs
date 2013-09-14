using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HAP.Web
{
    public partial class OneUseCodes : HAP.Web.Controls.Page
    {
        public OneUseCodes()
        {
            this.SectionTitle = "One Use Codes";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ExpireAllCodes.Visible = User.IsInRole("Domain Admins");
            gencodes.Visible = User.IsInRole("Domain Admins") || HttpContext.Current.Request.Cookies["token"].Value == "NTLM" || HttpContext.Current.Request.Cookies["token"].Value == "Negotiate";
            if (Request.QueryString["gencodes"] == "1")
            {
                for (var i = 0; i < 3; i++)
                {
                    bool set = false;
                    while (set == false)
                    {
                        Random randomNumberGenerator = new Random();
                        string code = string.Concat(randomNumberGenerator.Next(0, 9), randomNumberGenerator.Next(0, 9), randomNumberGenerator.Next(0, 9), randomNumberGenerator.Next(0, 9));
                        if (!HAP.AD.OneUse.Current.ContainsKey(code))
                        {
                            set = true;
                            HAP.AD.OneUse.Current.AddCode(code, HAP.AD.TokenGenerator.ConvertToPlain(HttpContext.Current.Request.Cookies["token"].Value), ADUser.UserName);
                        }

                    }
                    i++;
                }
                gencodes.Visible = false;
            }
            repeater.DataSource = HAP.AD.OneUse.Current.Values;
            repeater.DataBind();
        }

        protected void ExpireAllCodes_Click(object sender, EventArgs e)
        {
            HAP.AD.OneUse.Current.RemoveCodes();
        }

        protected void gencodes_Click(object sender, EventArgs e)
        {
            Response.Redirect("login.aspx?ReturnUrl=OneUseCodes.aspx&codes=gen");
        }
    }
}