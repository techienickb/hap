using HAP.BookingSystem;
using HAP.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HAP.Web.BookingSystem
{
    public partial class Cal : HAP.Web.Controls.Page
    {
        public Cal()
        {
            this.SectionTitle = Localize("bookingsystem/bookingsystem");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string[] BodyCode { get; set; }

        protected string JSRes
        {
            get
            {
                List<string> s = new List<string>();
                foreach (Resource r in config.BookingSystem.Resources.Values.Where(r1 => isVisible(r1.ShowTo, r1.HideFrom)))
                    s.Add("\"" + r.Name + "\"");
                return string.Join(", ", s.ToArray());
            }
        }

        protected string JSLessons
        {
            get
            {
                List<string> s = new List<string>();
                foreach (Lesson l in config.BookingSystem.Lessons)
                {
                    s.Add("{" + string.Format("\"Name\": \"{0}\", \"Start\": \"{1}:{2}\", \"End\": \"{3}:{4}\", \"FromStart\": null, \"FromEnd\": null, \"Type\": \"{5}\"", l.Name, l.StartTime.Hour, l.StartTime.Minute, l.EndTime.Hour, l.EndTime.Minute, l.Type) + "}");
                }
                return string.Join(", ", s.ToArray());
            }
        }

        protected string JSTermDates
        {
            get
            {
                List<string> terms = new List<string>();
                foreach (Term t in new HAP.BookingSystem.Terms())
                    terms.Add("{ " + string.Format(" name: '{0}', start: new Date({1}, {2}, {3}), end: new Date({4}, {5}, {6})",
                        t.Name,
                        t.StartDate.Year, t.StartDate.Month - 1, t.StartDate.Day,
                        t.EndDate.Year, t.EndDate.Month - 1, t.EndDate.Day) + ", halfterm: { " +
                        string.Format("start: new Date({0}, {1}, {2}), end: new Date({3}, {4}, {5})",
                        t.HalfTerm.StartDate.Year, t.HalfTerm.StartDate.Month - 1, t.HalfTerm.StartDate.Day,
                        t.HalfTerm.EndDate.Year, t.HalfTerm.EndDate.Month - 1, t.HalfTerm.EndDate.Day) + " } }");
                return string.Join(", ", terms.ToArray());
            }
        }

        protected string JSUser
        {
            get
            {
                int maxday = config.BookingSystem.MaxDays;
                int maxbookings = config.BookingSystem.MaxBookingsPerWeek;
                if (User.IsInRole("Domain Admins") || isBSAdmin) maxbookings = -1;
                foreach (AdvancedBookingRight right in HAP.BookingSystem.BookingSystem.BookingRights)
                    if (right.Username.ToLower() == User.Identity.Name.ToLower())
                    {
                        maxday = 7 * right.Weeksahead;
                        maxbookings = right.Numperweek;
                    }

                Terms terms = new Terms();
                if (Terms.getTerm(DateTime.Now).Name == null)
                {
                    int y = 0;
                    for (int x = terms.Count - 1; x >= 0; x--)
                        if (terms[x].StartDate > DateTime.Now) y = x;


                    if (DateTime.Now < terms[y].StartDate)
                        maxday += (terms[y].StartDate - DateTime.Now).Days;
                }
                else
                {
                    Term term = Terms.getTerm(DateTime.Now);
                    int y = 0;
                    for (int x = 0; x < terms.Count; x++)
                        if (terms[x].Equals(term)) y = x + 1;
                    if (y == terms.Count) y = terms.Count - 1;

                    int dow = 0;
                    switch (DateTime.Now.DayOfWeek)
                    {
                        case DayOfWeek.Monday: maxday += 7; dow = 7; break;
                        case DayOfWeek.Tuesday: maxday += 6; dow = 6; break;
                        case DayOfWeek.Wednesday: maxday += 5; dow = 5; break;
                        case DayOfWeek.Thursday: maxday += 4; dow = 4; break;
                        case DayOfWeek.Friday: maxday += 3; dow = 3; break;
                        case DayOfWeek.Saturday: maxday += 2; dow = 2; break;
                        case DayOfWeek.Sunday: maxday += 1; dow = 1; break;
                    }

                    for (int x = 1; x < maxday / 7; x++)
                        if (DateTime.Now.AddDays(dow).AddDays(7 * x) < terms[y].StartDate && DateTime.Now.AddDays(dow).AddDays(7 * x) > term.EndDate)
                        {
                            maxday += (terms[y].StartDate - DateTime.Now.AddDays(dow)).Days - (7 * x);
                            break;
                        }

                    if (DateTime.Now.AddDays(dow) >= term.HalfTerm.StartDate && DateTime.Now.AddDays(dow) <= term.HalfTerm.EndDate) maxday += 7;

                    for (int x = 1; x < maxday / 7; x++)
                        if (DateTime.Now.AddDays((7 * x) + dow) >= term.HalfTerm.StartDate && DateTime.Now.AddDays((7 * x) + dow) <= term.HalfTerm.EndDate)
                            maxday += 7;
                }

                List<string> ss = new List<string>();
                foreach (Resource r in config.BookingSystem.Resources.Values)
                {
                    if (User.IsInRole("Domain Admins") || isBSAdmin) ss.Add("'" + r.Name + "'");
                    else foreach (string s in r.Admins.Split(new char[] { ',' }))
                            if (s.Trim().ToLower().Equals(ADUser.UserName.ToLower())) ss.Add("'" + r.Name + "'");
                            else if (User.IsInRole(s.Trim())) ss.Add("'" + r.Name + "'");
                }
                if (User.IsInRole("Domain Admins") || isBSAdmin) return string.Format("username: '{0}', isAdminOf: [ {1} ], isBSAdmin: {2}, minDate: new Date({3}, {4}, {5}), maxDate: new Date({6}, {7}, {8}), maxBookings: {9}", ADUser.UserName, string.Join(", ", ss.ToArray()), (User.IsInRole("Domain Admins") || isBSAdmin).ToString().ToLower(), DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day, new HAP.BookingSystem.Terms()[new HAP.BookingSystem.Terms().Count - 1].EndDate.Year, new HAP.BookingSystem.Terms()[new HAP.BookingSystem.Terms().Count - 1].EndDate.Month - 1, new HAP.BookingSystem.Terms()[new HAP.BookingSystem.Terms().Count - 1].EndDate.Day, maxbookings);
                return string.Format("username: '{0}', isAdminOf: [ {1} ], isBSAdmin: {2}, minDate: new Date({3}, {4}, {5}), maxDate: new Date({6}, {7}, {8}), maxBookings: {9}", ADUser.UserName, string.Join(", ", ss.ToArray()), (User.IsInRole("Domain Admins") || isBSAdmin).ToString().ToLower(), DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day, DateTime.Now.AddDays(maxday).Year, DateTime.Now.AddDays(maxday).Month - 1, DateTime.Now.AddDays(maxday).Day - 1, maxbookings);
            }
        }

        private bool isVisible(string showto, string hidefrom)
        {
            if (showto == "All" || isBSAdmin || User.IsInRole("Domain Admins")) return true;
            foreach (string s in hidefrom.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (User.Identity.Name.ToLower().Equals(s.ToLower().Trim())) return false;
                else if (User.IsInRole(s.Trim())) return false;
            foreach (string s in showto.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (User.Identity.Name.ToLower().Equals(s.ToLower().Trim())) return true;
                else if (User.IsInRole(s.Trim())) return true;
            return false;
        }

        public bool isBSAdmin
        {
            get
            {
                foreach (string s in config.BookingSystem.Admins.Split(new char[] { ',' }))
                    if (s.Trim().ToLower().Equals(ADUser.UserName.ToLower())) return true;
                    else if (User.IsInRole(s.Trim())) return true;
                return false;
            }
        }
    }
}