using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace HAP.Data.BookingSystem
{
    public struct BookingKey
    {
        private int day;

        public int Day
        {
            get { return day; }
            set { day = value; }
        }
        private string lesson;

        public string Lesson
        {
            get { return lesson; }
            set { lesson = value; }
        }

        private string room;
        public string Room
        {
            get { return room; }
            set { room = value; }
        }

        public BookingKey(int Day, string Lesson, string Room)
        {
            this.day = Day;
            this.lesson = Lesson;
            this.room = Room;
        }

        public BookingKey(XmlNode node)
        {
            this.day = int.Parse(node.Attributes["day"].Value);
            this.lesson = node.Attributes["lesson"].Value;
            this.room = node.Attributes["room"].Value;
        }


        public static BookingKey parseBooking(int Day, string Lesson, string Room) { return new BookingKey(Day, Lesson, Room); }
    }
}