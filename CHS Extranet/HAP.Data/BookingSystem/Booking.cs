using System;
using System.Web;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using HAP.Web.Configuration;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;
using HAP.AD;

namespace HAP.Data.BookingSystem
{
    public class Booking
    {
        public Booking() { }
        public Booking(XmlNode node)
        {
            //nt.Parse(node.Attributes["room"].Value), node.Attributes["bookingfor"].Value, node.Attributes["bookingby"].Value, true)
            if (node.Attributes["date"] != null) this.Date = DateTime.Parse(node.Attributes["date"].Value);
            if (node.Attributes["day"] != null) this.Day = int.Parse(node.Attributes["day"].Value);
            this.Lesson = node.Attributes["lesson"].Value;
            this.Room = node.Attributes["room"].Value;
            this.Name = node.Attributes["name"].Value;
            this.Username = node.Attributes["username"].Value;
            this.Static = true;
            if (node.Attributes["ltcount"] != null) this.LTCount = int.Parse(node.Attributes["ltcount"].Value);
            if (node.Attributes["ltroom"] != null) this.LTRoom = node.Attributes["ltroom"].Value;
            if (node.Attributes["ltheadphones"] != null) this.LTHeadPhones = bool.Parse(node.Attributes["ltheadphones"].Value);
            else this.LTHeadPhones = false;
            if (node.Attributes["equiproom"] != null) this.EquipRoom = node.Attributes["equiproom"].Value;
            if (node.Attributes["uid"] != null) this.uid = node.Attributes["uid"].Value;
        }

        public Booking(XmlNode node, int day)
        {
            //nt.Parse(node.Attributes["room"].Value), node.Attributes["bookingfor"].Value, node.Attributes["bookingby"].Value, true)
            if (node.Attributes["date"] != null) this.Date = DateTime.Parse(node.Attributes["date"].Value);
            this.Day = day;
            this.Lesson = node.Attributes["lesson"].Value;
            this.Room = node.Attributes["room"].Value;
            this.Name = node.Attributes["name"].Value;
            this.Username = node.Attributes["username"].Value;
            this.Static = false;
            if (node.Attributes["ltcount"] != null) this.LTCount = int.Parse(node.Attributes["ltcount"].Value);
            if (node.Attributes["ltroom"] != null) this.LTRoom = node.Attributes["ltroom"].Value;
            if (node.Attributes["ltheadphones"] != null) this.LTHeadPhones = bool.Parse(node.Attributes["ltheadphones"].Value);
            else this.LTHeadPhones = false;
            if (node.Attributes["equiproom"] != null) this.EquipRoom = node.Attributes["equiproom"].Value;
            if (node.Attributes["uid"] != null) this.uid = node.Attributes["uid"].Value;
        }

        public Booking(int day, string lesson, string room, string name, string username)
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
        public string Lesson { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public bool LTHeadPhones { get; set; }
        public string LTRoom { get; set; }
        public string EquipRoom { get; set; }
        public int LTCount { get; set; }
        public bool Static { get; set; }
        public string uid { get; set; }
        public DateTime Date { get; set; }
        public UserInfo User
        {
            get
            {
                Dictionary<string, UserInfo> cache;
                if (HttpContext.Current.Cache["userdetailcache"] != null)
                    cache = HttpContext.Current.Cache["userdetailcache"] as Dictionary<string, UserInfo>;
                else cache = new Dictionary<string, UserInfo>();
                if (!cache.ContainsKey(this.Username))
                {
                    cache.Add(this.Username, AD.ADUtils.FindUserInfos(this.Username)[0]);
                    if (HttpContext.Current.Cache["userdetailcache"] != null) HttpContext.Current.Cache.Remove("userdetailcache");
                    HttpContext.Current.Cache.Insert("userdetailcache", cache, new System.Web.Caching.CacheDependency(new string[] { }, new string[] { }), DateTime.Now.AddHours(1), TimeSpan.Zero);
                }
                
                return cache[this.Username];
            }
        }
    }

    public class JSONBooking
    {
        public JSONBooking() {}
        public JSONBooking(Booking b)
        {
            this.Room = b.Room;
            this.Lesson = b.Lesson;
            this.Name = b.Name;
            this.Username = b.Username;
            try
            {
                this.DisplayName = b.User.Notes;
            }
            catch { this.DisplayName = b.Username; }
            this.Static = b.Static;
            try
            {
                this.LTCount = b.LTCount;
                this.LTHeadPhones = b.LTHeadPhones;
                this.LTRoom = b.LTRoom;
                this.EquipRoom = b.EquipRoom;
            }
            catch { }
        }
        public string Room { get; set; }
        public string Lesson { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool LTHeadPhones { get; set; }
        public string LTRoom { get; set; }
        public string EquipRoom { get; set; }
        public int LTCount { get; set; }
        public bool Static { get; set; }
        public string Username { get; set; }
    }
}
