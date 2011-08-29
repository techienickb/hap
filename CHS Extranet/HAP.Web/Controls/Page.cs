using System;
using System.Web;
using System.Web.Security;


namespace HAP.Web.Controls
{
    /// <summary>
    /// Summary description for Page
    /// </summary>
    public class Page : System.Web.UI.Page
    {
        public Page() : base() { }
        private HAP.AD.User _ADUser = null;
        public HAP.AD.User ADUser
        {
            get
            {
                if (_ADUser == null) _ADUser =((HAP.AD.User)Membership.GetUser());
                return _ADUser;
            }
        }
    }
}