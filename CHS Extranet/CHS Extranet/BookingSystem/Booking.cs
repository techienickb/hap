using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using CHS_Extranet.Configuration;
using System.Configuration;
using System.Xml;
using CHS_Extranet.HelpDesk;

namespace CHS_Extranet.BookingSystem
{
    public class Booking
    {
        public Booking() { }
        public Booking(XmlNode node)
        {
            //nt.Parse(node.Attributes["room"].Value), node.Attributes["bookingfor"].Value, node.Attributes["bookingby"].Value, true)
            this.Day = int.Parse(node.Attributes["day"].Value);
            this.Lesson = int.Parse(node.Attributes["lesson"].Value);
            this.Room = node.Attributes["room"].Value;
            this.Name = node.Attributes["name"].Value;
            this.Username = node.Attributes["username"].Value;
            this.Static = true;
            if (node.Attributes["ltcount"] != null) this.LTCount = int.Parse(node.Attributes["ltcount"].Value);
            if (node.Attributes["ltroom"] != null) this.LTRoom = node.Attributes["ltroom"].Value;
            if (node.Attributes["ltheadphones"] != null) this.LTHeadPhones = bool.Parse(node.Attributes["ltheadphones"].Value);
            else this.LTHeadPhones = false;
        }

        public Booking(XmlNode node, int day)
        {
            //nt.Parse(node.Attributes["room"].Value), node.Attributes["bookingfor"].Value, node.Attributes["bookingby"].Value, true)
            this.Day = day;
            this.Lesson = int.Parse(node.Attributes["lesson"].Value);
            this.Room = node.Attributes["room"].Value;
            this.Name = node.Attributes["name"].Value;
            this.Username = node.Attributes["username"].Value;
            this.Static = false;
            if (node.Attributes["ltcount"] != null) this.LTCount = int.Parse(node.Attributes["ltcount"].Value);
            if (node.Attributes["ltroom"] != null) this.LTRoom = node.Attributes["ltroom"].Value;
            if (node.Attributes["ltheadphones"] != null) this.LTHeadPhones = bool.Parse(node.Attributes["ltheadphones"].Value);
            else this.LTHeadPhones = false;
        }

        public Booking(int day, int lesson, string room, string name, string username)
        {
            //nt.Parse(node.Attributes["room"].Value), node.Attributes["bookingfor"].Value, node.Attributes["bookingby"].Value, true)
            this.Day = day;
            this.Lesson = lesson;
            this.Room = room;
            this.Name = name;
            this.Username = username;
            this.Static = false;
        }
        public string Room { get; set; }
        public int Day { get; set; }
        public int Lesson { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public bool LTHeadPhones { get; set; }
        public string LTRoom { get; set; }
        public int LTCount { get; set; }
        public bool Static { get; set; }

        public UserInfo User
        {
            get
            {
                return ADUtil.GetUserInfo(this.Username);
            }
        }
    }
}
