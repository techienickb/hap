using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Configuration;
using HAP.Web.HelpDesk;
using HAP.AD;
using HAP.Data.BookingSystem;

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
        public CustomDataType[] getUsers()
        {
            List<CustomDataType> cache;
            if (HttpContext.Current.Cache["userddlcache"] != null)
                cache = HttpContext.Current.Cache["userddlcache"] as List<CustomDataType>;
            else
            {
                cache = new List<CustomDataType>();
                foreach (UserInfo user in ADUtil.FindUsers())
                    if (string.IsNullOrEmpty(user.Notes)) cache.Add(new CustomDataType(user.LoginName, user.LoginName.ToLower()));
                    else cache.Add(new CustomDataType(string.Format("{0} - ({1})", user.LoginName, user.Notes), user.LoginName.ToLower()));
                HttpContext.Current.Cache.Insert("userddlcache", cache, new System.Web.Caching.CacheDependency(new string[] { }, new string[] { }), DateTime.Now.AddHours(1), TimeSpan.Zero);
            }
            return cache.ToArray();
        }

        public Day[] getDays()
        {
            List<Day> Days = new List<Day>();
            Days.Add(new Day("Monday (1)", 1));
            Days.Add(new Day("Tuesday (2)", 2));
            Days.Add(new Day("Wednesday (3)", 3));
            Days.Add(new Day("Thursday (4)", 4));
            Days.Add(new Day("Friday (5)", 5));
            if (hapConfig.Current.BookingSystem.TwoWeekTimetable)
            {
                Days.Add(new Day("Monday (6)", 6));
                Days.Add(new Day("Tuesday (7)", 7));
                Days.Add(new Day("Wednesday (8)", 8));
                Days.Add(new Day("Thursday (9)", 9));
                Days.Add(new Day("Friday (10)", 10));
            }
            return Days.ToArray();
        }

        void ABR_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            new HAP.Data.BookingSystem.BookingSystem().deleteBookingRights1(e.Values[0].ToString());
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
            new HAP.Data.BookingSystem.BookingSystem().deleteStaticBooking1(e.Values[1].ToString(), e.Values[2].ToString(), int.Parse(e.Values[0].ToString()));
        }
    }

    public class Day
    {
        public Day(string Name, int Value) { this.Name = Name; this.Value = Value; }
        public string Name { get; private set; }
        public int Value { get; private set; }
    }

    public class CustomDataType
    {
        public CustomDataType(string Key, string Value) { this.Key = Key; this.Value = Value; }
        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}