using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using HAP.Data.BookingSystem;
using HAP.AD;

namespace HAP.Web.BookingSystem
{
    public partial class _new : HAP.Web.Controls.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<Resource> rez = new List<Resource>();
            foreach (Resource r in config.BookingSystem.Resources.Values)
                if (isVisible(r.ShowTo)) rez.Add(r);
            resources1.DataSource = resources2.DataSource = rez.ToArray();
            resources1.DataBind(); resources2.DataBind();
            lessons.DataSource = config.BookingSystem.Lessons;
            lessons.DataBind();
            subjects.DataSource = config.BookingSystem.Subjects.ToArray();
            subjects.DataBind();
            adminbookingpanel.Visible = adminlink.Visible = isBSAdmin || User.IsInRole("Domain Admins");
            if (isBSAdmin || User.IsInRole("Domain Admins"))
            {
                userlist.Items.Clear();
                foreach (UserInfo user in ADUtils.FindUsers(OUVisibility.BookingSystem))
                    if (user.DisplayName == user.UserName)
                        userlist.Items.Add(new ListItem(user.UserName, user.UserName.ToLower()));
                    else
                        userlist.Items.Add(new ListItem(string.Format("{0} - ({1})", user.UserName, user.Notes), user.UserName.ToLower()));
                userlist.SelectedValue = ADUser.UserName.ToLower();
            }
            try
            {
                DateTime d = CurrentDate;
                BodyCode = new string[] { "", "" };
            }
            catch (ArgumentOutOfRangeException) { BodyCode = new string[] { " style=\"display: none;\"", "You are current outside of the set terms, please get an Admin to set the new year terms" }; }
        }

        protected string[] BodyCode { get; set; }

        protected DateTime CurrentDate
        {
            get
            {
                DateTime date = DateTime.Now;
                Terms terms = new Terms();
                if (date.DayOfWeek == DayOfWeek.Saturday) date = date.AddDays(2);
                else if (date.DayOfWeek == DayOfWeek.Sunday) date = date.AddDays(1);
                if (date < terms[0].StartDate) date = terms[0].StartDate;
                else if (date > terms[terms.Count - 1].EndDate) throw new ArgumentOutOfRangeException("Current Date", "The Current Date is After the Last Date in the Term!");
                else
                {
                    bool found = false;
                    foreach (Term t in terms)
                        if (date >= t.StartDate && date <= t.EndDate)
                        {
                            found = true;
                            if (date >= t.HalfTerm.StartDate && date <= t.HalfTerm.EndDate)
                            {
                                date = t.HalfTerm.EndDate;
                                if (date.DayOfWeek == DayOfWeek.Friday) date = date.AddDays(3);
                                else if (date.DayOfWeek == DayOfWeek.Saturday) date = date.AddDays(2);
                                else if (date.DayOfWeek == DayOfWeek.Sunday) date = date.AddDays(1);
                            }
                        }
                    if (!found) for (int i = 0; i < terms.Count - 2; i++)
                        {
                            if (date > terms[i].EndDate && date < terms[i + 1].StartDate)
                                date = terms[i + 1].StartDate;
                        }
                }

                return date;
            }
        }

        protected string JSResources
        {
            get
            {
                List<string> s = new List<string>();
                foreach (Resource r in config.BookingSystem.Resources.Values)
                    if (isVisible(r.ShowTo)) s.Add(string.Format("new resource(\"{0}\", \"{1}\")", r.Name, r.Type));
                return "[" + string.Join(", ", s.ToArray()) + "]";
            }
        }

        private bool isVisible(string showto)
        {
            if (showto == "All" || isBSAdmin || User.IsInRole("Domain Admins")) return true;
            foreach (string s in showto.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (ADUser.UserName.ToLower().Equals(s.ToLower().Trim())) return true;
            return false;
        }

        protected string JSTermDates
        {
            get
            {
                List<string> terms = new List<string>();
                foreach (Term t in new HAP.Data.BookingSystem.Terms())
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
                foreach (AdvancedBookingRight right in HAP.Data.BookingSystem.BookingSystem.BookingRights)
                    if (right.Username == User.Identity.Name)
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
                    if (DateTime.Now.AddDays(maxday) < terms[y].StartDate && DateTime.Now.AddDays(maxday) > term.EndDate)
                        maxday += (terms[y].StartDate - DateTime.Now).Days - 7;
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

                    if (DateTime.Now.AddDays(dow) >= term.HalfTerm.StartDate && DateTime.Now.AddDays(dow) <= term.HalfTerm.EndDate)
                        maxday += 7;
                    if (DateTime.Now.AddDays(7 + dow) >= term.HalfTerm.StartDate && DateTime.Now.AddDays(7 + dow) <= term.HalfTerm.EndDate)
                        maxday += 7;
                }

                List<string> ss = new List<string>();
                foreach (Resource r in config.BookingSystem.Resources.Values)
                {
                    if (User.IsInRole("Domain Admins")) ss.Add("'" + r.Name + "'");
                    else if (isBSAdmin) ss.Add("'" + r.Name + "'");
                    else foreach (string s in r.Admins.Split(new char[] { ',' }))
                            if (s.Trim().ToLower().Equals(ADUser.UserName.ToLower())) ss.Add("'" + r.Name + "'");
                }
                if (User.IsInRole("Domain Admins")) return string.Format("username: '{0}', isAdminOf: [ {1} ], isBSAdmin: {2}, minDate: new Date({3}, {4}, {5}), maxDate: new Date({6}, {7}, {8}), maxBookings: {9}", ADUser.UserName, string.Join(", ", ss.ToArray()), (User.IsInRole("Domain Admins") || isBSAdmin).ToString().ToLower(), DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day, new HAP.Data.BookingSystem.Terms()[new HAP.Data.BookingSystem.Terms().Count - 1].EndDate.Year, new HAP.Data.BookingSystem.Terms()[new HAP.Data.BookingSystem.Terms().Count - 1].EndDate.Month - 1, new HAP.Data.BookingSystem.Terms()[new HAP.Data.BookingSystem.Terms().Count - 1].EndDate.Day, maxbookings);
                return string.Format("username: '{0}', isAdminOf: [ {1} ], isBSAdmin: {2}, minDate: new Date({3}, {4}, {5}), maxDate: new Date({6}, {7}, {8}), maxBookings: {9}", ADUser.UserName, string.Join(", ", ss.ToArray()), (User.IsInRole("Domain Admins") || isBSAdmin).ToString().ToLower(), DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day, DateTime.Now.AddDays(maxday).Year, DateTime.Now.AddDays(maxday).Month - 1, DateTime.Now.AddDays(maxday).Day - 1, maxbookings);
            }
        }

        public bool isBSAdmin
        {
            get
            {
                foreach (string s in config.BookingSystem.Admins.Split(new char[] { ',' }))
                    if (s.Trim().ToLower().Equals(ADUser.UserName.ToLower())) return true;
                return false;
            }
        }
    }
}