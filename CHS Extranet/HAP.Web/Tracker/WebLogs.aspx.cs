﻿using System;
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
            Dictionary<DateTime, int> data = new Dictionary<DateTime, int>();
            List<DateTime> d = new List<DateTime>();
            foreach (WebTrackerEvent entry in WebEvents.Events)
            {
                DateTime dt = new DateTime(entry.DateTime.Year, entry.DateTime.Month, 1);
                DateTime dt1 = new DateTime(entry.DateTime.Year, entry.DateTime.Month, entry.DateTime.Day, 12, 0, 0);
                if (!d.Contains(dt)) d.Add(dt);
                if (!data.ContainsKey(dt1)) data.Add(dt1, 1);
                else data[dt1]++;
            }
            List<string> s = new List<string>();
            foreach (DateTime dt2 in data.Keys)
                s.Add(string.Format("['{0}', {1}]", dt2.ToString("yyyy-MM-dd h:mmtt"), data[dt2]));
            Data = string.Join(", ", s.ToArray());


            dates.DataSource = d.ToArray();
            dates.DataBind();
        }

        protected string Data { get; set; }
    }
}