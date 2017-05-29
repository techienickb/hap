using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP.Web.Configuration;

namespace HAP.Data.ComputerBrowser
{
    public class UNCPath
    {
        public string Drive { get; set; }
        public string EnableReadTo { get; set; }
        public string EnableWriteTo { get; set; }
        public string Name { get; set; }
        public string UNC { get; set; }
        public bool EnableMove { get; set; }
        public MappingUsageMode Usage { get; set; }
    }
}
