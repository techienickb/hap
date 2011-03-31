using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP.Web.Configuration;

namespace HAP.Data.ComputerBrowser
{
    public class PrimaryResponse
    {
        public ComputerBrowserAPIItem[] Items { get; set; }
        public uploadfilter[] Filters { get; set; }
        public string HAPName { get; set; }
        public string HAPVerion { get; set; }
    }
}
