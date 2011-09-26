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
using HAP.Data.BookingSystem;

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

            ResourceType RoomType = config.BookingSystem.Resources[Room].Type;

            HAP.Data.BookingSystem.BookingSystem bs = new HAP.Data.BookingSystem.BookingSystem(Date);
            foreach (Lesson lesson in config.BookingSystem.Lessons)
                if (Show == "All" || Show == lesson.Name)
                {
                    Booking b = bs.getBooking(Room, lesson.Name);
                    bool bookie = false;
                    if (isAdmin || b.Username == Page.User.Identity.Name) bookie = true;
                    string lessonname = b.Name;
                    if (lessonname.Length > 17) lessonname = lessonname.Remove(17) + "...";
                    if (lessonname.Length > 16 && b.Static) lessonname = lessonname.Remove(14) + "...";
                    if (b.Name == "FREE")
                        writer.Write("<span><a href=\"javascript:book('{0}', '{1}', '{2}');\">FREE</a></span>", Room, RoomType, b.Lesson);
                    else if (!b.Static)
                    {
                        if (RoomType == ResourceType.Laptops && bookie)
                            writer.Write("<span><a href=\"javascript:remove('{0}', '{1}');\" class=\"bookedl\">{2}<i> with {3}</i><u>{4} laptops [{5}] in {6}</u><label>Remove</label></a></span>", Room, b.Lesson, lessonname, b.User.Notes, b.LTCount, b.LTHeadPhones ? "H" : "NH", b.LTRoom);
                        else if (RoomType == ResourceType.Equipment && bookie)
                            writer.Write("<span><a href=\"javascript:remove('{0}', '{1}');\" class=\"bookedl\">{2}<i> with {3} in {4}</i><label>Remove</label></a></span>", Room, b.Lesson, lessonname, b.User.Notes, b.EquipRoom);
                        else if (RoomType == ResourceType.Laptops)
                            writer.Write("<span><span>{0}<i> with {1}</i><u>{2} laptops [{3}] in {4}</u></a></span></span>", lessonname, b.User.Notes, b.LTCount, b.LTHeadPhones ? "H" : "NH", b.LTRoom);
                        else if (RoomType == ResourceType.Equipment && !b.Static)
                            writer.Write("<span><span>{0}<i> with {1} in {2}</i></span></span>", lessonname, b.User.Notes, b.EquipRoom);
                        else if (bookie && !b.Static) writer.Write("<span><a href=\"javascript:remove('{0}', '{1}');\" class=\"booked\">{2}<i> with {3}</i><label>Remove</label></a></span>", Room, b.Lesson, lessonname, b.User.Notes);
                        else writer.Write("<span><span>{0}<i>with {1}</i></span></span>", lessonname, b.User.Notes);
                    }
                    else if (b.Static)
                    {
                        if (isAdmin)
                            writer.Write("<span><a href=\"javascript:book('{0}', '{1}', '{2}');\" class=\"static\"><img src=\"../images/staticb.png\" alt=\"Timetabled Lesson\" />{3}<i>with {4}</i><label>Override</label></a></span>", Room, RoomType, b.Lesson, lessonname, b.User.Notes);
                        else writer.Write("<span><span class=\"static\"><img src=\"../images/staticb.png\" alt=\"Timetabled Lesson\" />{0}<i>with {1}</i></span></span>", lessonname, b.User.Notes);
                    }
                }
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
    }
}