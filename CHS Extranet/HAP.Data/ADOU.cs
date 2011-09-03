using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;

namespace HAP.Data
{
    [Serializable]
    public class ADOU
    {
        public ADOU() { Items = new Collection<ADOU>(); }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Collection<ADOU> Items { get; set; }
    }
}