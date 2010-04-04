using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using CHS_Extranet.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Xml;
using CHS_Extranet.HelpDesk;

namespace CHS_Extranet.BookingSystem
{
    public partial class BookingPopup : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void book_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/App_Data/Bookings.xml"));
            XmlElement node = doc.CreateElement("Booking");
            node.SetAttribute("date", Date.ToShortDateString());
            string lessonint = bookingvars.Value.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[1];
            node.SetAttribute("lesson", lessonint);
            string roomstr = bookingvars.Value.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[0];
            extranetConfig config = extranetConfig.Current;
            if (config.BookingSystem.Resources[roomstr].ResourceType == ResourceType.Laptops)
            {
                node.SetAttribute("ltroom", BookLTRoom.Text);
                node.SetAttribute("ltcount", lt16.Checked ? "16" : "32");
                node.SetAttribute("ltheadphones", headphones.Checked.ToString());
            }
            node.SetAttribute("room", roomstr);
            node.SetAttribute("username", (isAdmin) ? userlist.SelectedValue : Username);
            string year = "Year " + BookYear.SelectedItem.Text + " ";
            if (BookYear.SelectedValue == "") year = "";
            node.SetAttribute("name", year + BookLesson.Text);
            doc.SelectSingleNode("/Bookings").AppendChild(node);
            #region Laptop Trolley Additional Booking
            if (config.BookingSystem.Resources[roomstr].ResourceType == ResourceType.Laptops)
            {
                BookingSystem bs = new BookingSystem(Date);
                if ((int.Parse(lessonint) > 1) && bs.islessonFree(roomstr, (int.Parse(lessonint) - 1)))
                {
                    node = doc.CreateElement("Booking");
                    node.SetAttribute("date", Date.ToShortDateString());
                    node.SetAttribute("lesson", (int.Parse(lessonint) - 1).ToString());
                    node.SetAttribute("room", roomstr);
                    node.SetAttribute("ltroom", "--");
                    node.SetAttribute("ltcount", lt16.Checked ? "16" : "32");
                    node.SetAttribute("ltheadphones", headphones.Checked.ToString());
                    node.SetAttribute("username", "systemadmin");
                    node.SetAttribute("name", "UNAVAILABLE");
                    if (BookYear.SelectedValue == "") year = "";
                    doc.SelectSingleNode("/Bookings").AppendChild(node);
                }
                if (bs.islessonFree(roomstr, int.Parse(lessonint) + 1))
                {
                    node = doc.CreateElement("Booking");
                    node.SetAttribute("date", Date.ToShortDateString());
                    node.SetAttribute("lesson", (int.Parse(lessonint) + 1).ToString());
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
            #endregion

            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(Server.MapPath("~/App_Data/Bookings.xml"), set);
            doc.Save(writer);
            writer.Flush();
            writer.Close();
            Booking booking = new BookingSystem(Date).getBooking(roomstr, int.Parse(lessonint));
            iCalGenerator.Generate(booking, Date);
            if (config.BookingSystem.Resources[roomstr].ResourceType == ResourceType.Laptops) iCalGenerator.Generate(booking, Date, "Nick");
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
            if (isAdmin)
            {
                userlist.Items.Clear();
                foreach (UserInfo user in ADUtil.FindUsers())
                    if (string.IsNullOrEmpty(user.Notes))
                        userlist.Items.Add(new ListItem(user.LoginName, user.LoginName.ToLower()));
                    else
                        userlist.Items.Add(new ListItem(string.Format("{0} - ({1})", user.LoginName, user.Notes), user.LoginName.ToLower()));
                userlist.SelectedValue = Username.ToLower();
                bookingadmin1.Visible = true;
            }
            else bookingadmin1.Visible = false;
            date.Text = Date.ToLongDateString();
            if (!isAdmin)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Server.MapPath("~/App_Data/Bookings.xml"));
                int max = extranetConfig.Current.BookingSystem.MaxBookingsPerWeek;
                foreach (AdvancedBookingRight right in BookingSystem.BookingRights)
                    if (right.Username == Username)
                        max = right.Numperweek;
                int x = 0;
                foreach (DateTime d in getWeekDates())
                    x += doc.SelectNodes("/Bookings/Booking[@date='" + d.ToShortDateString() + "' and @username='" + Username + "']").Count;
                if (x > max) { manybookings.Visible = true; bookingform.Visible = book.Visible = false; }
                else { manybookings.Visible = false; bookingform.Visible = book.Visible = true; }
            }
        }

        #region Login

        protected bool isAdmin
        {
            get
            {
                extranetConfig config = extranetConfig.Current;
                ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
                string _DomainDN = connObj.ConnectionString.Remove(0, connObj.ConnectionString.IndexOf("DC="));
                PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
                UserPrincipal up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, "Domain Admins");
                return up.IsMemberOf(gp);
            }
        }

        public string Username
        {
            get
            {
                if (Page.User.Identity.Name.Contains('\\'))
                    return Page.User.Identity.Name.Remove(0, Page.User.Identity.Name.IndexOf('\\') + 1);
                else return Page.User.Identity.Name;
            }
        }

        #endregion
    }
}