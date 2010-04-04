using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Configuration;
using CHS_Extranet.Configuration;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.IO;

namespace CHS_Extranet.BookingSystem
{
    public class BookingCalendar : Calendar, INamingContainer
    {
        public BookingCalendar() : base()
        {
            // since this control will be used for displaying
            // events, set these properties as a default
            extranetConfig config = extranetConfig.Current;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            string _DomainDN = connObj.ConnectionString.Remove(0, connObj.ConnectionString.IndexOf("DC="));
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            UserPrincipal up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, "Domain Admins");
            _isAdmin = up.IsMemberOf(gp);

            this.SelectionMode = CalendarSelectionMode.Day;
            this.maxday = config.BookingSystem.MaxDays;
            foreach (AdvancedBookingRight right in BookingSystem.BookingRights)
                if (right.Username == Username)
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
            html.Write(s.Replace("<th class=\"dayhead\" align=\"center\" abbr=\"Saturday\" scope=\"col\" style=\"color:#646464;background-color:White;font-size:8pt;font-weight:bold;\">Sat</th><th class=\"dayhead\" align=\"center\" abbr=\"Sunday\" scope=\"col\" style=\"color:#646464;background-color:White;font-size:8pt;font-weight:bold;\">Sun</th>", ""));
            if (!isAdmin) html.WriteLine("<div margin=\"4px 2px; text-align: center;\">You can select a day up to " + this.maxday + " days from today</div>");
            html.WriteLine("<!--{0}-->", this.maxday);
        }

        private string _dayField;
        private int maxday;

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
                else cell.CssClass += " " + s;
            }
        }

        #region Login
        private bool _isAdmin;
        protected bool isAdmin { get { return _isAdmin; }}

        public string Username
        {
            get
            {
                if (HttpContext.Current.User.Identity.Name.Contains('\\'))
                    return HttpContext.Current.User.Identity.Name.Remove(0, HttpContext.Current.User.Identity.Name.IndexOf('\\') + 1);
                else return HttpContext.Current.User.Identity.Name;
            }
        }

        #endregion
    }
}