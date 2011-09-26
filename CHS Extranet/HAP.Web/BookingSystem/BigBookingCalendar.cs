using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Configuration;
using HAP.Web.Configuration;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.IO;
using HAP.Data.BookingSystem;

namespace HAP.Web.BookingSystem
{
    public class BigBookingCalendar : Calendar, INamingContainer
    {
        public BigBookingCalendar() : base()
        {
            // since this control will be used for displaying
            // events, set these properties as a default
            config = hapConfig.Current;

            this.SelectionMode = CalendarSelectionMode.Day;
            this.maxday = config.BookingSystem.MaxDays;
            foreach (AdvancedBookingRight right in HAP.Data.BookingSystem.BookingSystem.BookingRights)
                if (right.Username == HttpContext.Current.User.Identity.Name)
                    this.maxday = 7 * right.Weeksahead;

            Terms terms = new Terms();
            if (Terms.getTerm(DateTime.Now).Name == null)
            {
                int y = 0;
                for (int x = terms.Count - 1; x >= 0; x--)
                    if (terms[x].StartDate > DateTime.Now) y = x;


                if (DateTime.Now < terms[y].StartDate)
                    this.maxday += (terms[y].StartDate - DateTime.Now).Days;
            }
            else
            {
                Term term = Terms.getTerm(DateTime.Now);
                int y = 0;
                for (int x = 0; x < terms.Count; x++)
                    if (terms[x].Equals(term)) y = x + 1;
                if (y == 3) y = 2;
                if (DateTime.Now.AddDays(this.maxday) < terms[y].StartDate && DateTime.Now.AddDays(this.maxday) > term.EndDate)
                    this.maxday += (terms[y].StartDate - DateTime.Now).Days - 7;
                int dow = 0;
                switch (DateTime.Now.DayOfWeek)
                {
                    case DayOfWeek.Monday: this.maxday += 7; dow = 7; break;
                    case DayOfWeek.Tuesday: this.maxday += 6; dow = 6; break;
                    case DayOfWeek.Wednesday: this.maxday += 5; dow = 5; break;
                    case DayOfWeek.Thursday: this.maxday += 4; dow = 4; break;
                    case DayOfWeek.Friday: this.maxday += 3; dow = 3; break;
                    case DayOfWeek.Saturday: this.maxday += 2; dow = 2; break;
                    case DayOfWeek.Sunday: this.maxday += 1; dow = 1; break;
                }

                if (DateTime.Now.AddDays(dow) >= term.HalfTerm.StartDate && DateTime.Now.AddDays(dow) <= term.HalfTerm.EndDate)
                    this.maxday += 7;
                if (DateTime.Now.AddDays(7 + dow) >= term.HalfTerm.StartDate && DateTime.Now.AddDays(7 + dow) <= term.HalfTerm.EndDate)
                    this.maxday += 7;
            }

        }

        protected override void Render(HtmlTextWriter html)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter tr = new StringWriter(sb);
            HtmlTextWriter writer = new HtmlTextWriter(tr);
            base.Render(writer);
            writer.Flush();
            writer.Close();
            tr.Flush();
            tr.Close();
            String s = sb.ToString();
            s = s.Replace(" style=\"color:Black\" title=\"Go to the previous month\">", " title=\"Go to the previous month\">");
            s = s.Replace(" style=\"color:Black\" title=\"Go to the next month\">", " title=\"Go to the next month\">");
            html.Write(s.Replace("<th class=\"dayhead\" align=\"center\" abbr=\"Saturday\" scope=\"col\" style=\"color:#646464;background-color:White;font-size:10pt;font-weight:bold;\">Sat</th>", "").Replace("<th class=\"dayhead\" align=\"center\" abbr=\"Sunday\" scope=\"col\" style=\"color:#646464;background-color:White;font-size:10pt;font-weight:bold;\">Sun</th>", ""));
            if (!isAdmin) html.WriteLine("<div margin=\"4px 2px; text-align: center;\">You can select a day up to " + this.maxday + " days from today</div>");
        }

        private int maxday;
        private hapConfig config;

        protected override void OnDayRender(TableCell cell, CalendarDay day)
        {
            base.OnDayRender(cell, day);
            int dotw = 0;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday: dotw = -1; break;
                case DayOfWeek.Tuesday: dotw = -2; break;
                case DayOfWeek.Wednesday: dotw = -3; break;
                case DayOfWeek.Thursday: dotw = -4; break;
                case DayOfWeek.Friday: dotw = -5; break;
            }
            if (day.IsWeekend || (day.Date < DateTime.Now.AddDays(dotw))) cell.Visible = false;
            else if (day.Date > DateTime.Now.AddDays(dotw) && day.Date.AddDays(1) < DateTime.Now) cell.Controls.Clear();
            else
            {
                string s = Terms.isTerm(day.Date);
                if (s == "invalid" || ((DateTime.Now.AddDays(this.maxday) < day.Date) && !isAdmin))
                {
                    cell.Controls.Clear();
                    cell.Controls.Add(new LiteralControl(day.DayNumberText));
                }
                else
                {
                    cell.Controls.Clear();
                    cell.Controls.Add(new LiteralControl(string.Format("<a target=\"_top\" href=\"./#" + day.Date.ToShortDateString() + "\" id=\"" + day.Date.ToShortDateString().Replace('/', '-') + "\">" + day.DayNumberText + "</a>")));
                    HAP.Data.BookingSystem.BookingSystem bs = new HAP.Data.BookingSystem.BookingSystem(day.Date.Date);
                    cell.CssClass += " " + s;
                    LiteralControl lc = new LiteralControl("<div class=\"QuickView\">");
                    foreach (Resource resource in config.BookingSystem.Resources.Values)
                        if (resource.Enabled)
                        {
                            lc.Text += "<div>";
                            foreach (Lesson lesson in config.BookingSystem.Lessons)
                                lc.Text += string.Format("<span class=\"{0}\" title=\"{1} {2}: {0}\"></span>", (!bs.isStatic(resource.Name, lesson.Name) && bs.islessonFree(resource.Name, lesson.Name)) ? "free" : "booked", lesson.Name, resource.Name);
                            lc.Text += "</div>";
                        }
                    lc.Text += "</div>";
                    cell.Controls.AddAt(0, lc);
                }
            }
        }

        #region Login
        protected bool isAdmin
        {
            get
            {
                bool vis = false;
                foreach (string s in hapConfig.Current.BookingSystem.Admins.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Page.User.IsInRole(s);
                if (vis) return true;
                return Page.User.IsInRole("Domain Admins");
            }
        }
        #endregion
    }
}