using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Data.ComputerBrowser
{
    public class ComputerBrowserAPIItem
    {
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public string Size { get; private set; }
        public string Type { get; private set; }
        public BType BType { get; private set; }
        public string Path { get; private set; }
        public AccessControlActions AccessControl { get; private set; }

        public ComputerBrowserAPIItem()
        {
        }

        public ComputerBrowserAPIItem(string name, string icon, string size, string type, BType btype, string path, AccessControlActions access)
        {
            Name = name;
            Icon = icon;
            Type = type;
            Size = size;
            BType = btype;
            Path = path;
            AccessControl = access;
        }

        public ComputerBrowserAPIItem(string name, string icon, string size, string type, BType btype, string path, string access)
        {
            Name = name;
            Icon = icon;
            Type = type;
            Size = size;
            BType = btype;
            Path = path;
            AccessControl = (AccessControlActions)Enum.Parse(typeof(AccessControlActions), access, true);
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((ComputerBrowserAPIItem)obj).Name);
        }
    }
    public enum BType { Root, Folder, File, Drive }
    public enum AccessControlActions { Change, View, None }
}
