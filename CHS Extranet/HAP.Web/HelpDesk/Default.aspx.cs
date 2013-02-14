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
            List<string> users = new List<string>();
            foreach (string s in config.HelpDesk.Admins.Split(new char[] { ',' }))
                try 
                { 
                    foreach (string s2 in System.Web.Security.Roles.GetUsersInRole(s.Trim())) if (!users.Contains(s2.ToLower())) users.Add(s2.ToLower()); 
                } 
                catch  
                { 
                    users.Add(s.Trim().ToLower()); 
                }

            adminbookingpanel.Visible = adminupdatepanel.Visible = isHDAdmin;
            if (adminupdatepanel.Visible)
            {
                userlist.Items.Clear();
                userlist2.Items.Clear();
                foreach (UserInfo user in ADUtils.FindUsers(Configuration.OUVisibility.HelpDesk))
                {
                    if (user.DisplayName == user.UserName) userlist.Items.Add(new ListItem(user.UserName, user.UserName.ToLower()));
                    else userlist.Items.Add(new ListItem(string.Format("{0} - ({1})", user.UserName, user.DisplayName), user.UserName.ToLower()));
                    if (users.Contains(user.UserName.ToLower()))
                    {
                        if (user.DisplayName == user.UserName) userlist2.Items.Add(new ListItem(user.UserName, user.UserName.ToLower()));
                        else userlist2.Items.Add(new ListItem(string.Format("{0} - ({1})", user.UserName, user.DisplayName), user.UserName.ToLower()));
                    }
                }
                userlist.SelectedValue = userlist2.SelectedValue = ADUser.UserName.ToLower();
            }
        }
    }
}