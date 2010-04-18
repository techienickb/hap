using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;

namespace HAP.Web.BookingSystem
{
    public class DayListRow : WebControl
    {
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
        }

        [Bindable(true)]
        [Category("Data")]
        public string Room { get; set; }
        [Bindable(true)]
        [Category("Data")]
        public DateTime Date { get; set; }
        [Bindable(true)]
        [Category("Data")]
        public string Show { get; set; }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            hapConfig config = hapConfig.Current;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            string _DomainDN = connObj.ConnectionString.Remove(0, connObj.ConnectionString.IndexOf("DC="));
            PrincipalContext pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            UserPrincipal up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);

            ResourceType RoomType = config.BookingSystem.Resources[Room].ResourceType;

            BookingSystem bs = new BookingSystem(Date);
            foreach (lesson lesson in config.BookingSystem.Lessons)
                if (Show == "All" || Show == lesson.Name)
                {
                    Booking b = bs.getBooking(Room, lesson.OldID.ToString());
                    if (b.Name == "FREE" || lesson.OldID == -1) b = bs.getBooking(Room, lesson.Name);
                    bool bookie = false;
                    if (Page.User.IsInRole("Domain Admins") || b.Username == Username) bookie = true;
                    string lessonname = b.Name;
                    if (lessonname.Length > 17) lessonname = lessonname.Remove(17) + "...";
                    if (b.Name == "FREE")
                        writer.Write("<span><a href=\"javascript:book('{0}', '{1}', '{2}');\">FREE</a></span>", Room, RoomType, b.Lesson);
                    else if (b.Static)
                        writer.Write("<span><span class=\"static\"><img src=\"../images/staticb.png\" alt=\"Timetabled Lesson\" />{0}<i>with {1}</i></span></span>", lessonname, b.User.Notes);
                    else if (RoomType == ResourceType.Laptops && bookie)
                        writer.Write("<span><a href=\"javascript:remove('{0}', '{1}');\" class=\"bookedl\">{2}<i> with {3}</i><u>{4} laptops [{5}] in {6}</u><label>Remove</label></a></span>", Room, b.Lesson, lessonname, b.User.Notes, b.LTCount, b.LTHeadPhones ? "H" : "NH", b.LTRoom);
                    else if (RoomType == ResourceType.Equipment && bookie)
                        writer.Write("<span><a href=\"javascript:remove('{0}', '{1}');\" class=\"bookedl\">{2}<i> with {3} in {4}</i><label>Remove</label></a></span>", Room, b.Lesson, lessonname, b.User.Notes, b.EquipRoom);
                    else if (RoomType == ResourceType.Laptops)
                        writer.Write("<span><span>{0}<i> with {1}</i><u>{2} laptops [{3}] in {4}</u><label>Remove</label></a></span>", lessonname, b.User.Notes, b.LTCount, b.LTHeadPhones ? "H" : "NH", b.LTRoom);
                    else if (RoomType == ResourceType.Equipment)
                        writer.Write("<span><span>{0}<i> with {1} in {2}</i></span></span>", lessonname, b.User.Notes, b.EquipRoom);
                    else if (bookie) writer.Write("<span><a href=\"javascript:remove('{0}', '{1}');\" class=\"booked\">{2}<i> with {3}</i><label>Remove</label></a></span>", Room, b.Lesson, lessonname, b.User.Notes);
                    else writer.Write("<span><span>{0}<i>with {1}</i></span></span>", lessonname, b.User.Notes);
                }
        }

        private string Username
        {
            get
            {
                if (Page.User.Identity.Name.Contains('\\'))
                    return Page.User.Identity.Name.Remove(0, Page.User.Identity.Name.IndexOf('\\') + 1);
                else return Page.User.Identity.Name;
            }
        }
    }
}