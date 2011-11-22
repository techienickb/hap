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
using System.IO;
using System.Xml;

namespace HAP.Web.BookingSystem.admin
{
    public partial class Default : HAP.Web.Controls.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SaveButton.Click += new EventHandler(SaveButton_Click);
            ListView1.ItemDeleting += new EventHandler<ListViewDeleteEventArgs>(ListView1_ItemDeleting);
            ABR.ItemDeleting += new EventHandler<ListViewDeleteEventArgs>(ABR_ItemDeleting);
        }

        void ListView1_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            new HAP.Data.BookingSystem.BookingSystem().deleteStaticBooking1(e.Values[1].ToString(), e.Values[2].ToString(), int.Parse(e.Values[0].ToString()));
        }

        public Default()
        {
            this.SectionTitle = "Booking System - Admin";
        }

        public Resource[] getResources()
        {
            List<Resource> resources = new List<Resource>();
            foreach (Resource br in hapConfig.Current.BookingSystem.Resources.Values)
                resources.Add(br);
            return resources.ToArray();
        }
        public Lesson[] getLessons()
        {
            List<Lesson> Lessons = new List<Lesson>();
            foreach (Lesson les in hapConfig.Current.BookingSystem.Lessons)
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
                foreach (UserInfo user in ADUtils.FindUsers(OUVisibility.BookingSystem))
                    if (user.Notes == user.UserName) cache.Add(new CustomDataType(string.Format("{0}", user.UserName), user.UserName.ToLower()));
                    else cache.Add(new CustomDataType(string.Format("{0} - ({1})", user.UserName, user.Notes), user.UserName.ToLower()));
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

        public Template[] getTemplates()
        {
            List<Template> ts = new List<Template>();
            Templates t = new Templates();
            foreach (Resource resource in hapConfig.Current.BookingSystem.Resources.Values)
            {
                Template t1 = new Template();
                t1.ID = resource.Name;
                if (t.ContainsKey(resource.Name))
                    t1 = (t[resource.Name]);
                else
                {
                    t1.Subject = t["general"].Subject;
                    t1.Content = t["general"].Content;
                }
                ts.Add(t1);
                Template t2 = new Template();
                t2.ID = resource.Name + "admin";
                if (t.ContainsKey(resource.Name + "admin"))
                    t2 = t[resource.Name + "admin"];
                else
                {
                    t2.Subject = t["generaladmin"].Subject;
                    t2.Content = t["generaladmin"].Content;
                }
                ts.Add(t2);
            }
            return ts.ToArray();
        }

        protected void etemplates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Templates t = new Templates();
            if (t.ContainsKey((string)e.CommandArgument)) t.Remove((string)e.CommandArgument);
            Template t1 = new Template();
            t1.Content = ((TextBox)e.Item.FindControl("eeditor")).Text;
            t1.ID = (string)e.CommandArgument;
            t1.Subject = ((TextBox)e.Item.FindControl("esubject")).Text;
            t.Add(t1.ID, t1);
            t.Save();
            etemplates.DataBind();
        }

        protected void importSIMS_Click(object sender, EventArgs args)
        {
            if (!File.Exists(Server.MapPath("~/app_data/timetable.xml"))) throw new ArgumentNullException("SIMS Tabletable Export", "The SIMS.net Timetable Export has not been found in the " + Server.MapPath("~/app_data") + "folder.  The export path you enter into the SIMS Report is: '" + Server.MapPath("~/App_Data/Timetable.xml") + "'");
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/app_data/timetable.xml"));
            XmlDocument sb = new XmlDocument();
            sb.Load(Server.MapPath("~/app_data/StaticBookings.xml"));
            UserInfo[] users = ADUtils.FindUsers(OUVisibility.BookingSystem);
            foreach (XmlNode n in doc.SelectNodes("/SuperStarReport/Record"))
            {
                string res = "";
                try {
                     res = n.SelectSingleNode("Name").InnerText;
                }
                catch { continue; }
                string user = "";
                if (users.Count(u => u.Notes.ToLower() == n.SelectSingleNode("MainTeacher").InnerText.ToLower()) > 0)
                    user = users.Single(u => u.Notes.ToLower() == n.SelectSingleNode("MainTeacher").InnerText.ToLower()).UserName;
                else if (users.Count(u => u.DisplayName.ToLower().EndsWith(n.SelectSingleNode("MainTeacher").InnerText.ToLower().Split(new char[] { ' ' })[n.SelectSingleNode("MainTeacher").InnerText.ToLower().Split(new char[] { ' ' }).Length - 1])) == 1)
                        user = users.Single(u => u.DisplayName.ToLower().EndsWith(n.SelectSingleNode("MainTeacher").InnerText.ToLower().Split(new char[] { ' ' })[n.SelectSingleNode("MainTeacher").InnerText.ToLower().Split(new char[] { ' ' }).Length - 1])).UserName;
                if (string.IsNullOrWhiteSpace(user)) throw new ArgumentOutOfRangeException("MainTeacher", "User cannot be found from " + n.SelectSingleNode("MainTeacher").InnerText);
                string name = n.SelectSingleNode("Description").InnerText;
                if (n.SelectSingleNode("YearGroup") != null) name = n.SelectSingleNode("YearGroup").InnerText.Replace("  ", " ") + " " + name;
                string d = n.SelectSingleNode("Name1").InnerText.Split(new char[] { ':' })[0];
                int day;
                if (int.TryParse(d.Substring(d.Length - 2, 1), out day)) day = int.Parse(d.Substring(d.Length - 2, 2));
                else if (!int.TryParse(d.Substring(d.Length - 1, 1), out day)) day = ConvertDayToInt(d);
                string lesson = n.SelectSingleNode("Name1").InnerText.Split(new char[] { ':' })[1];
                lesson = config.BookingSystem.Lessons.Single(l => l.Name.EndsWith(" " + lesson)).Name;
                if (sb.SelectSingleNode("/Bookings").ChildNodes.Count == 0 || sb.SelectSingleNode("/Bookings/Booking[@day='" + day + "' and @lesson='" + lesson + "' and @room='" + res + "']") == null)
                {
                    XmlElement e = sb.CreateElement("Booking");
                    e.SetAttribute("day", day.ToString());
                    e.SetAttribute("lesson", lesson);
                    e.SetAttribute("room", res);
                    e.SetAttribute("name", name);
                    e.SetAttribute("username", user);
                    sb.SelectSingleNode("/Bookings").AppendChild(e);
                }
                else
                {
                    XmlNode e = sb.SelectSingleNode("/Bookings/Booking[@day='" + day + "' and @lesson='" + lesson + "' and @room='" + res + "']");
                    e.Attributes["name"].Value = name;
                    e.Attributes["username"].Value = user;
                }
            }
            doc.Save(HttpContext.Current.Server.MapPath("~/App_Data/StaticBookings.xml"));
            message.Text = "Timetabled Lessons Loaded from SIMS Export, you can now delete the exported file";
        }

        private int ConvertDayToInt(string day)
        {
            string[] s = { "MonA", "TueA", "WedA", "ThuA", "FriA", "MonB", "TueB", "WedB", "ThuB", "FriB" };
            int[] d = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            for (int i = 0; i < s.Length; i++)
                if (s[i] == day) return d[i];
            return -1;
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