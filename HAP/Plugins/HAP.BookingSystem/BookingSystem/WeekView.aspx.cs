using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Globalization;

namespace HAP.Web.BookingSystem
{
    public partial class WeekView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            if (Request.QueryString.Count > 0) date = DateTime.Parse(Request.QueryString[0]);
            if (date.DayOfWeek == DayOfWeek.Saturday) date = date.AddDays(1);
            else if (date.DayOfWeek == DayOfWeek.Sunday) date = date.AddDays(2);
            else if (date.DayOfWeek == DayOfWeek.Tuesday) date = date.AddDays(-1);
            else if (date.DayOfWeek == DayOfWeek.Wednesday) date = date.AddDays(-2);
            else if (date.DayOfWeek == DayOfWeek.Thursday) date = date.AddDays(-3);
            else if (date.DayOfWeek == DayOfWeek.Friday) date = date.AddDays(-4);
            DateTime[] dates = { date, date.AddDays(1), date.AddDays(2), date.AddDays(3), date.AddDays(4) };
            rep.DataSource = dates;
            rep.DataBind();
        }

        private hapConfig config;

        protected override void OnInitComplete(EventArgs e)
        {
            config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - IT Booking System", config.School.Name);
        }
    }
}