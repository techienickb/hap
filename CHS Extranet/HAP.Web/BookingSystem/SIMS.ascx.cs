using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;
using HAP.Web.Configuration;
using HAP.Data.BookingSystem;

namespace HAP.Web.BookingSystem
{
    public partial class SIMS : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = isAdmin;
        }

        protected bool isAdmin
        {
            get
            {
                bool vis = false;
                foreach (string s in hapConfig.Current.BookingSystem.Admins.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Page.User.IsInRole(s.Trim());
                if (vis) return true;
                return Page.User.IsInRole("Domain Admins");
            }
        }

        List<string> log = new List<string>();

        protected void import_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            if (File.Exists(Server.MapPath("~/App_Data/import.log"))) File.Delete(Server.MapPath("~/App_Data/import.log"));
            FileInfo file = new FileInfo(Server.MapPath("~/App_Data/import.log"));
            StreamWriter sw = file.CreateText();
            sw.Flush();
            sw.Close();
        }

        private DateTime DayToDate(string day)
        {
            DayOfWeek dow = DaytoDayofWeek(day);
            DateTime dt = DateTime.Now;
            while (dt.DayOfWeek != dow)
                dt = dt.AddDays(1);
            return dt;
        }

        private DayOfWeek DaytoDayofWeek(string day)
        {
            switch (day)
            {
                case "Mon": return DayOfWeek.Monday;
                case "Tue": return DayOfWeek.Tuesday;
                case "Wed": return DayOfWeek.Wednesday;
                case "Thu": return DayOfWeek.Thursday;
                case "Fri": return DayOfWeek.Friday;
                case "Sat": return DayOfWeek.Friday;
            }
            return DayOfWeek.Sunday;
        }
    }
}