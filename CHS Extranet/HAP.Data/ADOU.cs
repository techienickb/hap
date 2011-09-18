using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace HAP.Data
{
    [Serializable]
    [DataContract]
    public class ADOU
    {
        public ADOU() { Items = new Collection<ADOU>(); }
        [DataMember]
        public string Icon { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public Collection<ADOU> Items { get; set; }
    }
}