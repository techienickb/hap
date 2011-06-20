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
                foreach (string s in hapConfig.Current.BookingSystem.AdminGroups.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Page.User.IsInRole(s);
                if (vis) return true;
                return Page.User.IsInRole("Domain Admins");
            }
        }

        List<string> log = new List<string>();

        protected void import_Click(object sender, EventArgs e)
        {
            if (File.Exists(Server.MapPath("~/App_Data/import.log"))) File.Delete(Server.MapPath("~/App_Data/import.log"));
            FileInfo file = new FileInfo(Server.MapPath("~/App_Data/import.log"));
            StreamWriter sw = file.CreateText();
            try
            {
                sw.WriteLine("Starting Import...");
                hapConfig config = hapConfig.Current;
                StreamReader sr = new StreamReader(importfile.FileContent);
                string line = sr.ReadLine();
                line = sr.ReadLine();
                int i = 0;
                while (!string.IsNullOrEmpty(line))
                {
                    i++;
                    string logline = "Importing line " + i;
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(Server.MapPath("~/App_Data/Bookings.xml"));
                        string[] s = line.Split(new char[] { ',' });
                        string day = s[6].Replace("\"", "").Split(new char[] { ':' })[0];
                        HAP.Data.BookingSystem.BookingSystem bs = new HAP.Data.BookingSystem.BookingSystem(DayToDate(day));
                        string lesson = "Lesson " + s[6].Replace("\"", "").Split(new char[] { ':' })[1];
                        if (bs.islessonFree(s[0].Replace("\"", ""), lesson))
                        {
                            XmlElement node = doc.CreateElement("Booking");
                            node.SetAttribute("date", bs.Date.ToShortDateString());
                            node.SetAttribute("lesson", lesson);
                            string roomstr = s[0].Replace("\"", "");
                            node.SetAttribute("room", roomstr);
                            string username = s[4].Replace("\"", "");
                            if (username.Contains('@')) username = username.Remove(username.IndexOf('@'));
                            node.SetAttribute("username", username);
                            string name = s[5].Replace("\"", "");
                            if (name.Contains('/')) name = name.Remove(name.IndexOf('/'));
                            name += " " + s[3].Replace("\"", "");
                            node.SetAttribute("name", name);
                            doc.SelectSingleNode("/Bookings").AppendChild(node);

                            logline += "...GENERATING..." + name + " on " + bs.Date.ToShortDateString() + " during " + lesson + "...";

                            XmlWriterSettings set = new XmlWriterSettings();
                            set.Indent = true;
                            set.IndentChars = "   ";
                            set.Encoding = System.Text.Encoding.UTF8;
                            XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/Bookings.xml"), set);
                            doc.Save(writer);
                            writer.Flush();
                            writer.Close();

                            logline += "SENDING iCAL...";

                            Booking b = bs.getBooking(roomstr, lesson);
                            iCalGenerator.Generate(b, bs.Date);
                            if (config.BookingSystem.Resources[roomstr].EmailAdmin) iCalGenerator.Generate(b, bs.Date, config.BaseSettings.AdminEmailUser);
                            logline += "DONE";
                        }
                        else logline += "...FAILED - Lesson Exists";
                    }
                    catch (Exception ex)
                    {
                        logline += "...FAILED - ERROR\n";
                        logline += ex.ToString();
                    }
                    sw.WriteLine(logline);
                }
                results.Text = "<script type=\"text/javascript\">alert('Import Complete\nsee log file for more information (~/BookingSystem/import.log)')</script>";
            }
            catch (Exception ex)
            {
                sw.WriteLine(ex.ToString());
                results.Text = "<script type=\"text/javascript\">alert('A Major Error Occured during the Import\nSee log file for more information (~/BookingSystem/import.log)')</script>";
            }
            finally
            {
                sw.Flush();
                sw.Close();
            }
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