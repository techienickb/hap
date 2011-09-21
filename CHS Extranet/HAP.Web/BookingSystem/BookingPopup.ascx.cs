﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using HAP.Web.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Xml;
using HAP.Web.HelpDesk;
using HAP.AD;
using HAP.Data.BookingSystem;

namespace HAP.Web.BookingSystem
{
    public partial class BookingPopup : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (isAdmin)
                {
                    userlist.Items.Clear();
                    foreach (UserInfo user in ADUtils.FindUsers())
                        if (user.Notes == user.UserName)
                            userlist.Items.Add(new ListItem(user.UserName, user.UserName.ToLower()));
                        else
                            userlist.Items.Add(new ListItem(string.Format("{0} - ({1})", user.UserName, user.Notes), user.UserName.ToLower()));
                    userlist.SelectedValue = Page.User.Identity.Name.ToLower();
                    bookingadmin1.Visible = true;
                }
                else bookingadmin1.Visible = false;
            }
        }

        protected void book_Click(object sender, EventArgs e)
        {
            XmlDocument doc = HAP.Data.BookingSystem.BookingSystem.BookingsDoc;
            XmlElement node = doc.CreateElement("Booking");
            node.SetAttribute("date", Date.ToShortDateString());
            string lessonint = bookingvars.Value.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[1];
            node.SetAttribute("lesson", lessonint);
            string roomstr = bookingvars.Value.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            hapConfig config = hapConfig.Current;
            if (config.BookingSystem.Resources[roomstr].Type == ResourceType.Laptops)
            {
                node.SetAttribute("ltroom", BookLTRoom.Text);
                node.SetAttribute("ltcount", lt16.Checked ? "16" : "32");
                node.SetAttribute("ltheadphones", headphones.Checked.ToString());
            }
            else if (config.BookingSystem.Resources[roomstr].Type == ResourceType.Equipment)
                node.SetAttribute("equiproom", equiproom.Text);
            node.SetAttribute("room", roomstr);
            node.SetAttribute("uid", ((isAdmin) ? userlist.SelectedValue : Page.User.Identity.Name) + DateTime.Now.ToString(iCalGenerator.DateFormat));
            node.SetAttribute("username", (isAdmin) ? userlist.SelectedValue : Page.User.Identity.Name);
            string year = "Year " + BookYear.SelectedItem.Text + " ";
            if (BookYear.SelectedValue == "") year = "";
            node.SetAttribute("name", year + BookLesson.Text);
            doc.SelectSingleNode("/Bookings").AppendChild(node);
            #region Charging
            if (config.BookingSystem.Resources[roomstr].EnableCharging)
            {
                HAP.Data.BookingSystem.BookingSystem bs = new HAP.Data.BookingSystem.BookingSystem(Date);
                int index = config.BookingSystem.Lessons.FindIndex(l => l.Name == lessonint);
                if (index > 0 && bs.islessonFree(roomstr, config.BookingSystem.Lessons[index - 1].Name))
                {
                    node = doc.CreateElement("Booking");
                    node.SetAttribute("date", Date.ToShortDateString());
                    node.SetAttribute("lesson", config.BookingSystem.Lessons[index - 1].Name);
                    node.SetAttribute("room", roomstr);
                    node.SetAttribute("ltroom", "--");
                    node.SetAttribute("ltcount", lt16.Checked ? "16" : "32");
                    node.SetAttribute("ltheadphones", headphones.Checked.ToString());
                    node.SetAttribute("username", "systemadmin");
                    node.SetAttribute("name", "UNAVAILABLE");
                    if (BookYear.SelectedValue == "") year = "";
                    doc.SelectSingleNode("/Bookings").AppendChild(node);
                }
                if (index < config.BookingSystem.Lessons.Count - 1)
                {
                    if (bs.islessonFree(roomstr, config.BookingSystem.Lessons[index + 1].Name))
                    {
                        node = doc.CreateElement("Booking");
                        node.SetAttribute("date", Date.ToShortDateString());
                        node.SetAttribute("lesson", config.BookingSystem.Lessons[index + 1].Name);
                        node.SetAttribute("room", roomstr);
                        node.SetAttribute("ltroom", "--");
                        node.SetAttribute("ltcount", lt16.Checked ? "16" : "32");
                        node.SetAttribute("ltheadphones", headphones.Checked.ToString());
                        node.SetAttribute("username", "systemadmin");
                        node.SetAttribute("name", "CHARGING");
                        if (BookYear.SelectedValue == "") year = "";
                        doc.SelectSingleNode("/Bookings").AppendChild(node);
                    }
                }
            }
            #endregion
            HAP.Data.BookingSystem.BookingSystem.BookingsDoc = doc;
            Booking booking = new HAP.Data.BookingSystem.BookingSystem(Date).getBooking(roomstr, lessonint);
            if (config.SMTP.Enabled)
            {
                iCalGenerator.Generate(booking, Date);
                if (config.BookingSystem.Resources[roomstr].EmailAdmins) iCalGenerator.Generate(booking, Date, true);
            }
            BookYear.SelectedIndex = 0;
            BookLesson.Text = BookLTRoom.Text = "";
            Page.DataBind();
        }

        private DateTime[] getWeekDates()
        {
            List<DateTime> dates = new List<DateTime>();
            if (Date.DayOfWeek == DayOfWeek.Monday)
            {
                dates.Add(Date); dates.Add(Date.AddDays(1));
                dates.Add(Date.AddDays(2)); dates.Add(Date.AddDays(3));
                dates.Add(Date.AddDays(4));
            }
            else if (Date.DayOfWeek == DayOfWeek.Tuesday)
            {
                dates.Add(Date); dates.Add(Date.AddDays(1));
                dates.Add(Date.AddDays(2)); dates.Add(Date.AddDays(3));
                dates.Add(Date.AddDays(-1));
            }
            else if (Date.DayOfWeek == DayOfWeek.Wednesday)
            {
                dates.Add(Date); dates.Add(Date.AddDays(1));
                dates.Add(Date.AddDays(2)); dates.Add(Date.AddDays(-1));
                dates.Add(Date.AddDays(-2));
            }
            else if (Date.DayOfWeek == DayOfWeek.Tuesday)
            {
                dates.Add(Date); dates.Add(Date.AddDays(1));
                dates.Add(Date.AddDays(-1)); dates.Add(Date.AddDays(-2));
                dates.Add(Date.AddDays(-3));
            }
            else
            {
                dates.Add(Date); dates.Add(Date.AddDays(-1));
                dates.Add(Date.AddDays(-2)); dates.Add(Date.AddDays(-3));
                dates.Add(Date.AddDays(-4));
            }
            return dates.ToArray();
        }

        public DateTime Date { get; set; }

        public override void DataBind()
        {
            base.DataBind();
            date.Text = Date.ToLongDateString();
            if (!isAdmin)
            {
                XmlDocument doc = HAP.Data.BookingSystem.BookingSystem.BookingsDoc;
                int max = hapConfig.Current.BookingSystem.MaxBookingsPerWeek;
                foreach (AdvancedBookingRight right in HAP.Data.BookingSystem.BookingSystem.BookingRights)
                    if (right.Username == Page.User.Identity.Name)
                        max = right.Numperweek;
                int x = 0;
                foreach (DateTime d in getWeekDates())
                    x += doc.SelectNodes("/Bookings/Booking[@date='" + d.ToShortDateString() + "' and @username='" + Page.User.Identity.Name + "']").Count;
                if (x > max) { manybookings.Visible = true; bookingform.Visible = book.Visible = false; }
                else { manybookings.Visible = false; bookingform.Visible = book.Visible = true; }
            }
            hapConfig.Current.BookingSystem.Subjects.Sort();
            subjects.DataSource = hapConfig.Current.BookingSystem.Subjects.ToArray();
            subjects.DataBind();
        }

        #region Login

        protected bool isAdmin
        {
            get
            {
                bool vis = false;
                foreach (string s in hapConfig.Current.BookingSystem.Admins.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s);
                if (vis) return true;
                return HttpContext.Current.User.IsInRole("Domain Admins");
            }
        }
        #endregion
    }
}