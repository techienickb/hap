using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.DirectoryServices;
using HAP.Web.Configuration;
using System.Xml;
using System.Runtime.InteropServices;
using System.Drawing;

namespace HAP.Web
{
    public partial class Default : HAP.Web.Controls.Page
    {
        public Default()
        {
            this.SectionTitle = "Home";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                List<LinkGroup> groups = new List<LinkGroup>();
                foreach (LinkGroup group in config.Homepage.Groups.Values)
                    if (group.ShowTo == "All") groups.Add(group);
                    else if (group.ShowTo != "None")
                    {
                        bool vis = false;
                        foreach (string s in group.ShowTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                            if (!vis) vis = User.IsInRole(s);
                        if (vis) groups.Add(group);
                    }
                homepagelinks.DataSource = homepageheaders.DataSource = groups.ToArray();
                homepagelinks.DataBind(); homepageheaders.DataBind();
            }
        }

        protected string gettiles(object o)
        {
            List<string> s = new List<string>();
            string group = ((HAP.Web.Configuration.LinkGroup)o).Name.Replace(" ", "").Replace("'", "").Replace(",", "").Replace(".", "").Replace("*", "").Replace("&", "").Replace("/", "").Replace("\\", "");
            foreach (HAP.Web.Configuration.Link link in ((HAP.Web.Configuration.LinkGroup)o).FilteredLinks)
            {
                string s1 = "{ Type: \"" + link.Type;
                s1 += "\" , Data: {  Group: \"" + group;
                s1 += "\", Name: \"" + link.Name + "\", Url: \"" + link.Url;
                s1 += "\", Target: \"" + link.Target;
                s1 += "\", Description: \"" + link.Description;
                s1 += "\", Icon: \"" + (string.IsNullOrEmpty(link.Icon) || link.Icon.StartsWith("#") ? "" : string.Format("api/tiles/icons/{0}/{1}/{2}", 64, 64, link.Icon.Remove(0, 2)));
                s1 += "\", Color: " + (string.IsNullOrEmpty(link.Icon) || link.Icon.StartsWith("#") ? "\"\"" : HAP.Web.LiveTiles.IconCache.GetColour(link.Icon)) + " } }";
                s.Add(s1);
            }
            return string.Join(", \n", s.ToArray());
        }
    }
}