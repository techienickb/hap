using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Web.Configuration
{
    public class JSLink : Attribute
    {
        public string FileName { get; set; }

        public JSLink(string FileName)
        {
            this.FileName = FileName;
        }
    }
}
