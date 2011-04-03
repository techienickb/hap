using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using HAP.Web.Configuration;

namespace HAP.Data.ComputerBrowser
{
    public class ComputerBrowserAPIItem : CBFile
    {
        public string Download { get; private set; }
        public AccessControlActions AccessControl { get; private set; }

        public ComputerBrowserAPIItem() : base()
        {
        }

        public ComputerBrowserAPIItem(FileInfo file, uncpath unc, string userhome, AccessControlActions access, [Optional, DefaultParameterValue("")] string download) : base(file, unc, userhome)
        {
            AccessControl = access;
            Download = download;
        }

        public ComputerBrowserAPIItem(DirectoryInfo dir, uncpath unc, string userhome, AccessControlActions access, [Optional, DefaultParameterValue("")] string download) : base(dir, unc, userhome)
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
