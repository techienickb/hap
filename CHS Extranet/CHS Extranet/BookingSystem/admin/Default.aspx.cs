using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CHS_Extranet.Configuration;
using System.Configuration;

namespace CHS_Extranet.BookingSystem.admin
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SaveButton.Click += new EventHandler(SaveButton_Click);
            staticbookingsgrid.RowDeleting += new GridViewDeleteEventHandler(staticbookingsgrid_RowDeleting);
            ABR.ItemDeleting += new EventHandler<ListViewDeleteEventArgs>(ABR_ItemDeleting);
            extranetConfig config = ConfigurationManager.GetSection("extranetConfig") as extranetConfig;
            this.Title = string.Format("{0} - Home Access Plus+ - IT Booking System - Admin", config.BaseSettings.EstablishmentName);
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
                    t[x] = term;
                }

                t.SaveTerms();
                message.Text = "Saved";
            }
            catch { }

        }

        void staticbookingsgrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            new BookingSystem().deleteStaticBooking1(int.Parse(e.Values[1].ToString()), int.Parse(e.Values[2].ToString()), int.Parse(e.Values[0].ToString()));
        }
    }
}