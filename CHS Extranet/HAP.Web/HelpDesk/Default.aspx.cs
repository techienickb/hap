using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.AD;

namespace HAP.Web.HelpDesk
{
    public partial class New : HAP.Web.Controls.Page
    {
        public New() { this.SectionTitle = Localize("helpdesk/helpdesk"); }

        protected void Page_Load(object sender, EventArgs e)
        {
            adminbookingpanel.Visible = adminupdatepanel.Visible = User.IsInRole("Domain Admins");
            if (User.IsInRole("Domain Admins"))
            {
                userlist.Items.Clear();
                foreach (UserInfo user in ADUtils.FindUsers(Configuration.OUVisibility.HelpDesk))
                    if (user.DisplayName == user.UserName) userlist.Items.Add(new ListItem(user.UserName, user.UserName.ToLower()));
                    else userlist.Items.Add(new ListItem(string.Format("{0} - ({1})", user.UserName, user.DisplayName), user.UserName.ToLower()));
                userlist.SelectedValue = ADUser.UserName.ToLower();
            }
        }
    }
}