using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.BookingSystem
{
    public struct AdvancedBookingRight
    {
        public AdvancedBookingRight(string username, int weeksahead, int numperweek)
        {
            this.username = username;
            this.weeksahead = weeksahead;
            this.numperweek = numperweek;
        }

        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        private int weeksahead, numperweek;

        public int Weeksahead
        {
            get { return weeksahead; }
            set { weeksahead = value; }
        }

        public int Numperweek
        {
            get { return numperweek; }
            set { numperweek = value; }
        }

    }
}