using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Data.SQL;

namespace HAP.Web.Tracker
{
    public partial class WebLogs : HAP.Web.Controls.Page
    {
        public WebLogs()
        {
            this.SectionTitle = "Web Logon Tracker";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<DateTime> d = new List<DateTime>();
            foreach (WebTrackerEvent entry in WebEvents.Events)
            {
                DateTime dt = new DateTime(entry.DateTime.Year, entry.DateTime.Month, 1);
                if (!d.Contains(dt)) d.Add(dt);
            }
            dates.DataSource = d.ToArray();
            dates.DataBind();
        }
    }
}