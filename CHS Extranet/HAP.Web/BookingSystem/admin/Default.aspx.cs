using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Configuration;

namespace HAP.Web.BookingSystem.admin
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SaveButton.Click += new EventHandler(SaveButton_Click);
            staticbookingsgrid.RowDeleting += new GridViewDeleteEventHandler(staticbookingsgrid_RowDeleting);
            ABR.ItemDeleting += new EventHandler<ListViewDeleteEventArgs>(ABR_ItemDeleting);
            hapConfig config = hapConfig.Current;
            this.Title = string.Format("{0} - Home Access Plus+ - IT Booking System - Admin", config.BaseSettings.EstablishmentName);
        }

        public bookingResource[] getResources()
        {
            List<bookingResource> resources = new List<bookingResource>();
            foreach (bookingResource br in hapConfig.Current.BookingSystem.Resources)
                resources.Add(br);
            return resources.ToArray();
        }
        public lesson[] getLessons()
        {
            List<lesson> Lessons = new List<lesson>();
            foreach (lesson les in hapConfig.Current.BookingSystem.Lessons)
                Lessons.Add(les);
            return Lessons.ToArray();
        }

        void ABR_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            new BookingSystem().deleteBookingRights1(e.Values[0].ToString());
        }

        void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                message.Text = "";
                Terms t = new Terms();
                for (int x = 0; x < termdates.Items.Count; x++)
                {
                    Term term = t[x];
                    HalfTerm ht = term.HalfTerm;
                    foreach (Control c in termdates.Items[x].Controls)
                        switch (c.ID)
                        {
                            case "termname":
                                term.Name = ((TextBox)c).Text;
                                break;
                            case "termstartdate":
                                term.StartDate = DateTime.Parse(((TextBox)c).Text);
                                break;
                            case "termenddate":
                                term.EndDate = DateTime.Parse(((TextBox)c).Text);
                                break;
                            case "week1":
                                term.StartWeekNum = ((RadioButton)c).Checked ? 1 : 2;
                                break;
                            case "halftermstart":
                                ht.StartDate = DateTime.Parse(((TextBox)c).Text);
                                break;
                            case "halftermend":
                                ht.EndDate = DateTime.Parse(((TextBox)c).Text);
                                break;
                        }
                    term.HalfTerm = ht;
                    t[x] = term;
                }

                t.SaveTerms();
                message.Text = "Saved";
            }
            catch { }

        }

        void staticbookingsgrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            new BookingSystem().deleteStaticBooking1(e.Values[1].ToString(), e.Values[2].ToString(), int.Parse(e.Values[0].ToString()));
        }
    }
}