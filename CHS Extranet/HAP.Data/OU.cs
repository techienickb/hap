﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Data
{
    public class OU : List<OU>
    {
        public OU() { }
        public OU(string name, string oupath, bool show) : base()
        {
            Name = name;
            OUPath = oupath;
            Show = show;
        }
        public OU(string oupath, bool show) : base()
        {
            OUPath = oupath;
            Name = oupath.Remove(0, oupath.IndexOf('/') + 1);
            Name = Name.Remove(Name.IndexOf(','));
            Name = Name.Remove(0, Name.IndexOf('=') + 1);
            Show = show;
        }
        public string Name { get; set; }
        public string OUPath { get; set; }
        public bool Show { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
