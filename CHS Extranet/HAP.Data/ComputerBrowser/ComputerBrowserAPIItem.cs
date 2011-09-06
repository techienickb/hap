using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using HAP.Web.Configuration;
using HAP.AD;

namespace HAP.Data.ComputerBrowser
{
    public class ComputerBrowserAPIItem : CBFile
    {
        public string Download { get; set; }
        public AccessControlActions AccessControl { get; set; }

        public ComputerBrowserAPIItem() : base()
        {
        }

        public ComputerBrowserAPIItem(FileInfo file, UNCPath unc, User user, AccessControlActions access, [Optional, DefaultParameterValue("")] string download) : base(file, unc, user)
        {
            AccessControl = access;
            Download = download;
        }

        public ComputerBrowserAPIItem(DirectoryInfo dir, UNCPath unc, User user, AccessControlActions access, [Optional, DefaultParameterValue("")] string download) : base(dir, unc, user)
        {
            AccessControl = access;
            Download = download;
        }

        public ComputerBrowserAPIItem(string name, string icon, string size, string type, BType btype, string path, AccessControlActions access, [Optional, DefaultParameterValue("")] string download)
        {
            Name = name;
            Icon = icon;
            Type = type;
            Size = size;
            BType = btype;
            Path = path;
            AccessControl = access;
            Download = download;
        }

        public ComputerBrowserAPIItem(string name, string icon, string size, string type, BType btype, string path, string access, [Optional, DefaultParameterValue("")] string download)
        {
            Name = name;
            Icon = icon;
            Type = type;
            Size = size;
            BType = btype;
            Path = path;
            Download = download;
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
