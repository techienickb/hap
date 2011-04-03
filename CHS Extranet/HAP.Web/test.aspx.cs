using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HAP.Web
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(User.Identity.Name);
        }

        protected void ask_Click(object sender, EventArgs e)
        {
            resp.Text = HAP.Data.ComputerBrowser.Quota.GetQuota(Username, "\\\\chs01\\" + Username + ".$").Used.ToString();
            resp.Text += "<br />" + HAP.Data.ComputerBrowser.Quota.GetQuota(Username, "\\\\chs01\\" + Username + ".$").Total.ToString();
        }

        public string Username
        {
            get
            {
                if (Context.User.Identity.Name.Contains('\\'))
                    return Context.User.Identity.Name.Remove(0, Context.User.Identity.Name.IndexOf('\\') + 1);
                else return Context.User.Identity.Name;
            }
        }
    }
}