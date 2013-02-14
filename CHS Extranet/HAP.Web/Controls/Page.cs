using System;
using System.Web;
using System.Web.Security;
using System.Xml;
using HAP.Web.Configuration;


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
        public string SectionTitle { get; set; }
        public hapConfig config;
        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            this.Title = string.Format("{0} - {2} - {1}", config.School.Name, SectionTitle, Localize("homeaccessplus"));
        }

        public string Localize(string StringPath)
        {
            return Localizable.Localize(StringPath);//_strings.SelectSingleNode("/hapStrings/" + StringPath.ToLower()).InnerText;
        }
    }
}