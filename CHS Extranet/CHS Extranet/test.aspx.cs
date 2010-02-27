using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using CHS_Extranet.Configuration;

namespace CHS_Extranet
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            extranetConfig config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;

            basesettings.Text = string.Format("establishmentname={0} establishmentcode={1} studentphotohandler={2} studentemailformat={3}", config.BaseSettings.EstablishmentName, config.BaseSettings.EstablishmentCode, config.BaseSettings.StudentPhotoHandler, config.BaseSettings.StudentEmailFormat);
            adsettings.Text = string.Format("adusername={0} adpassword={1} adconnectionstring={2}", config.ADSettings.ADUsername, config.ADSettings.ADPassword, config.ADSettings.ADConnectionString);

            List<string> uncs = new List<string>();

            foreach (uncpath path in config.UNCPaths)
            {
                uncs.Add(string.Format("<div>drive={0}, unc={1}, enablereadto={2}, enablewriteto={3}, name={4}</div>", path.Drive, path.UNC, path.EnableReadTo, path.EnableWriteTo, path.Name));
            }

            unc.Text = string.Join("\n", uncs.ToArray());

            List<string> linkss = new List<string>();

            foreach (homepagelink link in config.HomePageLinks)
            {
                linkss.Add(string.Format("<div>name={0}, showto={1}, description={2}, linklocation={3}</div>", link.Name, link.ShowTo, link.Description, link.LinkLocation));
            }

            links.Text = string.Join("\n", linkss.ToArray());

        }
    }
}