using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;
using System.Configuration;
using HAP.Web.Configuration;

namespace HAP.Web.Controls
{
    public partial class ChangePassword : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public string Username
        {
            get
            {
                if (Page.User.Identity.Name.Contains('\\'))
                    return Page.User.Identity.Name.Remove(0, Page.User.Identity.Name.IndexOf('\\') + 1);
                else return Page.User.Identity.Name;
            }
        }

        protected void ChangePass_Click(object sender, EventArgs e)
        {
            hapConfig config = hapConfig.Current;
            try
            {
                DirectoryEntry usersDE = new DirectoryEntry(ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString].ConnectionString, Username, currentpass.Text);
                DirectorySearcher ds = new DirectorySearcher(usersDE);
                ds.Filter = "(sAMAccountName=" + Username + ")";
                SearchResult r = ds.FindOne();
                DirectoryEntry user = r.GetDirectoryEntry();
                if (user == null) throw new Exception("I can't find your username");
                user.Invoke("ChangePassword", currentpass.Text, newpass.Text);
                user.CommitChanges();
                errormess.Text = "Password Changed";
            }
            catch (Exception ex) { errormess.Text = ex.Message; }
            savepass.Enabled = true;
            currentpass.Text = newpass.Text = confpass.Text = "";
        }
    }
}