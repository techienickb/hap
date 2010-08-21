using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;

namespace HAP.Silverlight.Browser
{
    public class Common
    {
        public static  Uri GetUri(BItem Data, UriType Type)
        {
            Uri uri;
            switch (Type)
            {
                case UriType.Save:
                    uri = new Uri(HtmlPage.Document.DocumentUri, "api/mycomputer/save/" + GetPath(Data));
                    break;
                case UriType.Delete:
                    uri = new Uri(HtmlPage.Document.DocumentUri, "api/mycomputer/delete/" + GetPath(Data));
                    break;
                case UriType.Zip:
                    uri = new Uri(HtmlPage.Document.DocumentUri, "api/mycomputer/zip/" + GetPath(Data));
                    break;
                case UriType.Unzip:
                    uri = new Uri(HtmlPage.Document.DocumentUri, "api/mycomputer/unzip/" + GetPath(Data));
                    break;
                default:
                    uri = new Uri("about:blank");
                    break;
            }
             return uri;
        }

        public static string GetPath(BItem Data)
        {
            return Data.Path.Remove(0, Data.BType == BType.File ? 9 : 20);
        }
    }

    #region Enums
    public enum UriType { Save, Delete, Zip, Unzip }
    public enum ViewMode { List, SmallIcon, Icon, LargeIcon, Tile }
    public enum MouseMode { NoGo, Move, Normal, Upload }
    public enum BType { Folder, File, Drive }
    public enum AccessControlActions { Change, View, None }
    #endregion

    #region Event
    public delegate void ChangeDirectoryHandler(object sender, BItem e);
    public delegate void BItemChangeHandler(BItem e);
    public delegate void ResortHandler(object sender, bool resort);
    public delegate void SelectHandler(string path);
    #endregion

    public class BItem : IComparable
    {
        private string _name;
        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                if (Changed != null) Changed(this);
            }
        }
        public string Icon { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public BType BType { get; set; }
        public string Path { get; set; }
        public AccessControlActions AccessControl { get; set; }
        public event BItemChangeHandler Changed;
        public event BItemChangeHandler Deleted;
        public void Delete() { if (Deleted != null) Deleted(this); }

        public BItem(string name, string icon, string size, string type, BType btype, string path, AccessControlActions access)
        {
            Name = name;
            Icon = icon;
            Type = type;
            Size = size;
            BType = btype;
            Path = path;
            AccessControl = access;
        }

        public BItem(string name, string icon, string size, string type, BType btype, string path, string access)
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
            return Name.CompareTo(((BItem)obj).Name);
        }
    }

    public class BUserState
    {
        public BUserState(bool Resort, string Data, string oldname)
        {
            this.Resort = Resort;
            this.Data = Data;
            this.oldname = oldname;
        }

        public bool Resort { get; set; }
        public string Data { get; set; }
        public string oldname { get; set; }
    }
}
