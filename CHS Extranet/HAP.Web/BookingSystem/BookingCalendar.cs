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
using System.Xml;

namespace HAP.Web.BookingSystem
{
    public class BookingCalendar : Calendar, INamingContainer
    {
        public BookingCalendar() : base()
        {
            // since this control will be used for displaying
            // events, set these properties as a default
            hapConfig config = hapConfig.Current;
            _isAdmin = HttpContext.Current.User.IsInRole("Domain Admins");

            this.SelectionMode = CalendarSelectionMode.DayWeek;
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
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter calendar = new HtmlTextWriter(sw);
            base.Render(calendar);

            // Load the XHTML to a XML document for processing
            XmlDocument xml = new XmlDocument();
            xml.Load(new StringReader(sw.ToString()));
            // The Calendar control renders as a table, so navigate to the
            // second TR which has the day headers.
            XmlElement root = xml.DocumentElement;
            XmlNode oldNode = root.SelectNodes("/table/tr")[1];
            XmlNode sundayNode = oldNode.ChildNodes[6];
            XmlNode saturdayNode = oldNode.ChildNodes[7];
            XmlNode newNode = oldNode;
            newNode.RemoveChild(sundayNode);
            newNode.RemoveChild(saturdayNode);
            root.ReplaceChild(oldNode, newNode);


            oldNode = root.SelectNodes("/table/tr")[0];
            newNode = oldNode;
            newNode.ChildNodes[0].Attributes["colspan"].Value = "6";
            root.ReplaceChild(oldNode, newNode);

            XmlElement newroot = root;

            int i = 0;
            foreach (XmlNode node in root.SelectNodes("/table/tr")) {
                if (node.ChildNodes.Count == 1 && i > 0)
                    newroot.RemoveChild(node);
                i++;
            }


            // Replace the buffer
            html.WriteLine(newroot.OuterXml);

            if (!isAdmin) html.WriteLine("<div margin=\"4px 2px; text-align: center;\">You can select a day up to " + this.maxday + " days from today</div>");
            html.WriteLine("<!--{0}-->", this.maxday);
        }

        private int maxday;

        protected override void OnDayRender(TableCell cell, CalendarDay day)
        {
            if (!day.IsWeekend) base.OnDayRender(cell, day);
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