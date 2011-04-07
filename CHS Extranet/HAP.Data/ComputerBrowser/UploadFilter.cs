using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Data.ComputerBrowser
{
    public class UploadFilter
    {
        public string Name { get; set; }
        public string Filter { get; set; }
        public string EnableFor { get; set; }
        public override string ToString()
        {
            return string.Format("{0} ({2})|{1}", this.Name, this.Filter, this.Filter.Replace(";", "\\ "));
        }
    }
}
