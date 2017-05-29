using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.HelpDesk
{
    public class Stats
    {
        public int OpenTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int NewTickets { get; set; }
        public UserStats HighestUser { get; set; }
    }

    public class UserStats
    {
        public string Username { get; set; }
        public int Tickets { get; set; }
    }
}
