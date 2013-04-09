using HAP.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace HAP.MyFiles
{
    public class UploadInit
    {
        public int maxRequestLength { get; set; }
        public string[] Filters { get; set; }
        public Properties Properties { get; set; }
    }
}
