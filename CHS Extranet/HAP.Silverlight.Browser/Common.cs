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
using System.Collections.Generic;

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
            return Data.Path;
        }
    }

    #region Enums
    public enum UriType { Save, Delete, Zip, Unzip }
    public enum ViewMode { List, SmallIcon, Icon, LargeIcon, Tile }
    public enum MouseMode { NoGo, Move, Normal, Upload }
    #endregion

    #region Event
    public delegate void ChangeDirectoryHandler(object sender, BItem e);
    public delegate void BItemChangeHandler(BItem e);
    public delegate void ResortHandler(object sender, bool resort);
    public delegate void SelectHandler(string path);
    public delegate void AddItemHandler(BrowserItem item);
    public delegate void AddTeeeHandler(HAPTreeNode currentitem, HAPTreeNode newitem);
    public delegate void UpdateTree(BItem bitem);
    public delegate void SetBool(bool allow);
    #endregion

    public class BItem : IComparable
    {
        public service.ComputerBrowserAPIItem Source { get; set; }
        public string Name
        {
            get { return Source.Name; }
            set
            {
                Source.Name = value;
                if (Changed != null) Changed(this);
            }
        }
        public string Icon { get { return Source.Icon; } set { Source.Icon = value; } }
        public string Size { get { return Source.Size; } set { Source.Size = value; } }
        public string Type { get { return Source.Type; } set { Source.Type = value; } }
        public service.BType BType { get { return Source.BType; } set { Source.BType = value; } }
        public string Path { get { return Source.Path; } set { Source.Path = value; } }
        public service.AccessControlActions AccessControl { get { return Source.AccessControl; } set { Source.AccessControl = value; } }
        public event BItemChangeHandler Changed;
        public event BItemChangeHandler Deleted;
        public void Delete() { if (Deleted != null) Deleted(this); }

        public BItem() { }
        public BItem(service.ComputerBrowserAPIItem item)
        {
            Source = item;
        }
        public BItem(service.BType type, string name, service.AccessControlActions access)
        {
            Source = new service.ComputerBrowserAPIItem();
            Source.BType = type;
            Source.Name = name;
            Source.AccessControl = access;
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
