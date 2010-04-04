using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CHS_Extranet.Configuration;
using System.Configuration;
using System.ComponentModel;

namespace CHS_Extranet.BookingSystem
{
    public partial class DayList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void DataBind()
        {
            

            DayName.Text = Date.DayOfWeek.ToString() + " " + Date.Day;

            string term = Terms.isTerm(Date.Date);
            if (term == "invalid")
            {
                dl.Visible = false;
                noday.Text = "<h2>The date selected is in a holiday period, please use the calendar to choose another day</h2>";
            }
            else if (term.StartsWith("Half"))
            {
                dl.Visible = false;
                noday.Text = "<h2>The date selected is in a Half Term Holiday, please use the calendar to choose another day</h2>";
            }
            else
            {
                dl.Visible = true; noday.Text = string.Empty;

                List<string> s = new List<string>();
                extranetConfig config = extranetConfig.Current;
                for (int x = 0; x < config.BookingSystem.LessonsPerDay; x++)
                    s.Add((x + 1).ToString());
                headrepeater.DataSource = s.ToArray();
                headrepeater.DataBind();
                List<bookingResource> res = new List<bookingResource>();
                foreach (bookingResource r in config.BookingSystem.Resources)
                    res.Add(r);
                dl.DataSource = res.ToArray();
                dl.DataBind();
            }
        }

        [Bindable(true)]
        [Category("Data")]
        public DateTime Date { get; set; }
    }
}