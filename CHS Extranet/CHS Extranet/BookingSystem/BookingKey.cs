using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace CHS_Extranet.BookingSystem
{
    public struct BookingKey
    {
        private int day;

        public int Day
        {
            get { return day; }
            set { day = value; }
        }
        private int lesson;

        public int Lesson
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

        public BookingKey(int Day, int Lesson, string Room)
        {
            this.day = Day;
            this.lesson = Lesson;
            this.room = Room;
        }

        public BookingKey(XmlNode node)
        {
            this.day = int.Parse(node.Attributes["day"].Value);
            this.lesson = int.Parse(node.Attributes["lesson"].Value);
            this.room = node.Attributes["room"].Value;
        }


        public static BookingKey parseBooking(int Day, int Lesson, string Room) { return new BookingKey(Day, Lesson, Room); }
    }
}