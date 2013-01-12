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

        public bool isHDAdmin
        {
            get
            {
                foreach (string s in config.HelpDesk.Admins.Split(new char[] { ',' }))
                    if (s.Trim().ToLower().Equals(ADUser.UserName.ToLower())) return true;
                    else if (User.IsInRole(s.Trim())) return true;
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            adminbookingpanel.Visible = adminupdatepanel.Visible = isHDAdmin;
            if (adminupdatepanel.Visible)
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