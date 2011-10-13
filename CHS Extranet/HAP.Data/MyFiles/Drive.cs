using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Data.MyFiles
{
    public class Drive
    {
        public Drive() { }
        public Drive(string name, string icon, decimal space, string path, AccessControlActions actions)
        {
            Name = name;
            Icon = icon;
            Space = space;
            Path = path;
            Actions = actions;
        }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Path { get; set; }
        public decimal Space { get; set; }
        public AccessControlActions Actions { get; set; }
        public int CompareTo(object obj)
        {
            return Name.CompareTo(((File)obj).Name);
        } 
    }
}
