using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Configuration;
using System.ComponentModel;

namespace HAP.Web.BookingSystem
{
    public partial class DayList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                foreach (lesson lesson in hapConfig.Current.BookingSystem.Lessons)
                    lessonsel.Items.Add(lesson.Name);
            }
        }

        public override void DataBind()
        {
            hapConfig config = hapConfig.Current;
            daylistrow.Style.Clear();
            daylistrow.Style.Add("width", (ItemWidth * (config.BookingSystem.Lessons.Count + 1)) + "px");

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
                List<lesson> lessons = new List<lesson>();
                foreach (lesson lesson in config.BookingSystem.Lessons)
                    if (lessonsel.SelectedValue == "All" || lesson.Name == lessonsel.SelectedValue) lessons.Add(lesson);
                headrepeater.DataSource = lessons.ToArray();
                headrepeater.DataBind();
                List<bookingResource> res = new List<bookingResource>();
                foreach (bookingResource r in config.BookingSystem.Resources)
                    if (resourcetype.SelectedValue == "All" && r.Enable) res.Add(r);
                    else if (r.ResourceType == (ResourceType)Enum.Parse(typeof(ResourceType), resourcetype.SelectedValue, true) && r.Enable)
                        res.Add(r);
                dl.DataSource = res.ToArray();
                dl.DataBind();
            }
        }

        [Bindable(true)]
        [Category("Data")]
        [DefaultValue(152)]
        public int ItemWidth { get; set; }

        [Bindable(true)]
        [Category("Data")]
        public DateTime Date { get; set; }

        protected void resourcetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        protected void lessonsel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Page.DataBind();
        }
    }
}