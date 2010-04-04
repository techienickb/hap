using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Globalization;
using CHS_Extranet.Configuration;
using System.Configuration;

namespace CHS_Extranet.BookingSystem
{
    public struct HalfTerm
    {
        public HalfTerm(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        DateTime startDate, endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
    }

    public struct Term
    {
        public Term(string name, DateTime startDate, DateTime endDate, int startWeekNum, HalfTerm halfTerm)
        {
            this.name = name;
            this.startDate = startDate;
            this.endDate = endDate;
            this.startWeekNum = startWeekNum;
            this.halfTerm = halfTerm;
        }

        HalfTerm halfTerm;

        public HalfTerm HalfTerm
        {
            get { return this.halfTerm; }
            set { this.halfTerm = value; }
        }

        int startWeekNum;

        public int StartWeekNum
        {
            get { return startWeekNum; }
            set { startWeekNum = value; }
        }

        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        DateTime startDate, endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public int WeekNum(DateTime date)
        {
            if ((date >= this.startDate) && (date <= this.endDate))
            {
                if (extranetConfig.Current.BookingSystem.TwoWeekTimetable)
                {
                    System.Globalization.Calendar cal = CultureInfo.InvariantCulture.Calendar;
                    int x = cal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    if ((date >= this.startDate) && (date < this.halfTerm.StartDate))
                    {
                        int y = cal.GetWeekOfYear(this.startDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                        return (((x - y) % 2) == 0) ? this.startWeekNum : this.startWeekNum + 1;
                    }
                    else if ((date > this.halfTerm.EndDate) && (date <= this.EndDate))
                    {
                        int y = cal.GetWeekOfYear(this.halfTerm.EndDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                        return (((x - y) % 2) == 0) ? this.startWeekNum + 1 : this.startWeekNum;
                    }
                    else return 0;
                }
                else return 1;
            }
            else return -1;
        }
    }

    public class Terms : List<Term>
    {

        public Terms()
        {
            ReadTerms();
        }

        public static string isTerm(DateTime day)
        {
            foreach (Term term in new Terms())
                if (term.StartDate.Date <= day.Date && term.EndDate >= day.Date)
                {
                    if (term.HalfTerm.StartDate <= day.Date && term.HalfTerm.EndDate >= day.Date)
                        return "HalfTerm " + term.Name.Replace(" ", "");
                    else return "Term " + term.Name.Replace(" ", "");
                }
            return "invalid";
        }

        public static Term getTerm(DateTime day)
        {
            foreach (Term term in new Terms())
                if (term.StartDate.Date <= day.Date && term.EndDate >= day.Date)
                    return term;
            return new Term();
        }

        public void ReadTerms()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/TermDates.xml"));

            foreach (XmlNode node in doc.SelectNodes("/Terms/Term"))
            {
                XmlNode halfTerm = node.SelectSingleNode("HalfTerm");
                string[] s = halfTerm.Attributes["startDate"].Value.Split(new char[] { '/' });
                string[] s2 = halfTerm.Attributes["endDate"].Value.Split(new char[] { '/' });
                HalfTerm ht = new HalfTerm(new DateTime(int.Parse(s[2]), int.Parse(s[1]), int.Parse(s[0])), new DateTime(int.Parse(s2[2]), int.Parse(s2[1]), int.Parse(s2[0])));
                s = node.Attributes["startDate"].Value.Split(new char[] { '/' });
                s2 = node.Attributes["endDate"].Value.Split(new char[] { '/' });
                Add(new Term(node.Attributes["name"].Value, new DateTime(int.Parse(s[2]), int.Parse(s[1]), int.Parse(s[0])), new DateTime(int.Parse(s2[2]), int.Parse(s2[1]), int.Parse(s2[0])), int.Parse(node.Attributes["startWeekNum"].Value), ht));
            }
        }

        public void SaveTerms()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/TermDates.xml"));

            for (int x = 0; x < this.Count; x++)
            {
                Term term = this[x];
                XmlNode node = doc.SelectNodes("/Terms/Term")[x];
                node.Attributes["name"].Value = term.Name;
                node.Attributes["startDate"].Value = term.StartDate.ToString("dd/MM/yyyy");
                node.Attributes["endDate"].Value = term.EndDate.ToString("dd/MM/yyyy");
                node.Attributes["startWeekNum"].Value = term.StartWeekNum.ToString();
                node.SelectSingleNode("HalfTerm").Attributes["startDate"].Value = term.HalfTerm.StartDate.ToString("dd/MM/yyyy");
                node.SelectSingleNode("HalfTerm").Attributes["endDate"].Value = term.HalfTerm.EndDate.ToString("dd/MM/yyyy");
            }

            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/TermDates.xml"));
        }
    }
}