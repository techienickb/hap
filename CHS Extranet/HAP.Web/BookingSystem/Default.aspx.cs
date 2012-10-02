using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using HAP.BookingSystem;
using HAP.AD;

namespace HAP.Web.BookingSystem
{
    public partial class _new : HAP.Web.Controls.Page
    {
        public _new()
        {
            this.SectionTitle = Localize("bookingsystem/bookingsystem");
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            List<Resource> rez = new List<Resource>();
            if (RouteData.Values.Count == 0)
                foreach (Resource r in config.BookingSystem.Resources.Values)
                {
                    if (isVisible(r.ShowTo, r.HideFrom)) rez.Add(r);
                }
            else if (RouteData.Values.ContainsKey("resource"))
            {
                foreach (string s in RouteData.GetRequiredString("resource").Split(new char[] { ',' }))
                    foreach (Resource r in config.BookingSystem.Resources.Values)
                    {
                        if (isVisible(r.ShowTo, r.HideFrom) && r.Name.ToLower() == s.Trim().ToLower()) rez.Add(r);
                    }
            }
            else if (RouteData.Values.ContainsKey("type"))
            {
                foreach (string s in RouteData.GetRequiredString("type").Split(new char[] { ',' }))
                    foreach (Resource r in config.BookingSystem.Resources.Values)
                    {
                        if (isVisible(r.ShowTo, r.HideFrom) && r.Type == (ResourceType)Enum.Parse(typeof(ResourceType), s.Trim())) rez.Add(r);
                    }
            }
            if (rez.Count == 0 && !isBSAdmin) Response.Redirect("~/unauthorised.aspx");
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
                if (date.Date <= terms[0].StartDate.Date) date = terms[0].StartDate;
                else if (date.Date > terms[terms.Count - 1].EndDate.Date) throw new ArgumentOutOfRangeException("Current Date", "The Current Date is After the Last Date in the Term!");
                else
                {
                    bool found = false;
                    foreach (Term t in terms)
                        if (date.Date >= t.StartDate.Date && date.Date <= t.EndDate.Date)
                        {
                            found = true;
                            if (date.Date >= t.HalfTerm.StartDate.Date && date.Date <= t.HalfTerm.EndDate.Date)
                            {
                                date = t.HalfTerm.EndDate;
                                if (date.DayOfWeek == DayOfWeek.Friday) date = date.AddDays(3);
                                else if (date.DayOfWeek == DayOfWeek.Saturday) date = date.AddDays(2);
                                else if (date.DayOfWeek == DayOfWeek.Sunday) date = date.AddDays(1);
                            }
                        }
                    if (!found) for (int i = 0; i < terms.Count - 1; i++)
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
                {
                    if (isVisible(r.ShowTo, r.HideFrom))
                    {
                        string years = r.Years;
                        if (string.IsNullOrEmpty(years) || years == "Inherit") years = "Year 7, Year 8, Year 9, Year 10, Year 11, Year 12, Year 13, A-Level";
                        List<string> years1 = new List<string>();
                        List<string> quant = new List<string>();
                        foreach (string y in years.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            years1.Add("\"" + y.Trim() + "\"");
                        foreach (string q in r.Quantities.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            quant.Add("\"" + q.Trim() + "\"");
                        s.Add(string.Format("new resource(\"{0}\", \"{1}\", [ {2} ], [ {3} ], {4}, {5}, \"{6}\")", r.Name, r.Type, string.Join(", ", years1.ToArray()), string.Join(", ", quant.ToArray()), isReadOnly(r.ReadOnlyTo, r.ReadWriteTo).ToString().ToLower(), isMultiLesson(r.MultiLessonTo, r.Admins).ToString().ToLower(), r.MaxMultiLesson));
                    }
                }
                return "[" + string.Join(", ", s.ToArray()) + "]";
            }
        }

        private bool isVisible(string showto, string hidefrom)
        {
            if (showto == "All" || isBSAdmin || User.IsInRole("Domain Admins")) return true;
            foreach (string s in hidefrom.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (ADUser.UserName.ToLower().Equals(s.ToLower().Trim())) return false;
            foreach (string s in showto.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (ADUser.UserName.ToLower().Equals(s.ToLower().Trim())) return true;
            return false;
        }

        private bool isReadOnly(string readonlyto, string readwriteto)
        {
            if ((readonlyto == "" && readwriteto == "") || isBSAdmin || User.IsInRole("Domain Admins")) return false;
            foreach (string s in readonlyto.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (ADUser.UserName.ToLower().Equals(s.ToLower().Trim())) return true;
            foreach (string s in readwriteto.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (ADUser.UserName.ToLower().Equals(s.ToLower().Trim())) return false;
            if (readonlyto == "" && readwriteto != "") return true;
            return false;
        }

        private bool isMultiLesson(string mutlilesson, string resadmins)
        {
            if (!config.BookingSystem.MultiLesson) return false;
            if (mutlilesson == "All" || isBSAdmin || User.IsInRole("Domain Admins") || isResAdmin(resadmins)) return true;
            foreach (string s in mutlilesson.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (ADUser.UserName.ToLower().Equals(s.ToLower().Trim())) return true;
                else if (User.IsInRole(s.Trim())) return true;
            return false;
        }

        private bool isResAdmin(string admins)
        {
            foreach (string s in admins.Split(new char[] { ',' }))
                if (s.Trim().ToLower().Equals(ADUser.UserName.ToLower())) return true;
                else if (User.IsInRole(s.Trim())) return true;
            return false;
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