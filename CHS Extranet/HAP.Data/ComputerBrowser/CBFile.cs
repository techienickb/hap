using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using HAP.Web.Configuration;
using System.Web;
using System.Web.Security;
using HAP.AD;

namespace HAP.Data.ComputerBrowser
{
    public class CBFile
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string Icon { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public BType BType { get; set; }

        public CBFile() { }

        public CBFile(FileInfo file, UNCPath unc)
        {
            Extension = file.Extension;
            Type = "File";
            Name = file.Name + (file.Name.Contains(file.Extension) ? "" : file.Extension);

            FileIcon fi;
            if (FileIcon.TryGet(Extension, out fi))
            {
                Type = fi.Type;
                Name = Name.Remove(Name.LastIndexOf(file.Extension));
            }
            if (Type == "File")
            {
                try
                {
                    RegistryKey rkRoot = Registry.ClassesRoot;
                    string keyref = rkRoot.OpenSubKey(file.Extension).GetValue("").ToString();
                    Type = rkRoot.OpenSubKey(keyref).GetValue("").ToString();
                    Name = Name.Remove(Name.LastIndexOf(file.Extension));
                }
                catch { Type = "File"; }
            }
            if (Type != "File")
            {
                Icon = "images/icons/" + ParseForImage(file);
                if (Icon.EndsWith(".ico")) Icon = "api/mycomputer/" + ParseForImage(file);
            }
            else Icon = "images/icons/file.png";
            if (file.Extension.ToLower().Equals(".png") || file.Extension.ToLower().Equals(".jpg") || file.Extension.ToLower().Equals(".jpeg") || file.Extension.ToLower().Equals(".gif") || file.Extension.ToLower().Equals(".bmp") || file.Extension.ToLower().Equals(".wmf"))
                Icon = "api/mycomputer/thumb/" + Converter.UNCtoDrive2(file.FullName, unc).Replace('&', '^');
            BType = ComputerBrowser.BType.File;
            CreatedTime = file.CreationTime;
            ModifiedTime = file.LastWriteTime;
            Size = parseLength(file.Length);
            Path = Converter.UNCtoDrive(file.FullName, unc);
        }

        public CBFile(FileInfo file, UNCPath unc, User user)
        {
            Extension = file.Extension;
            Type = "File";
            Name = file.Name + (file.Name.Contains(file.Extension) ? "" : file.Extension);
            
            FileIcon fi;
            if (FileIcon.TryGet(Extension, out fi))
            {
                Type = fi.Type;
                Name = Name.Remove(Name.LastIndexOf(file.Extension));
            }
            if (Type == "File")
            {
                try
                {
                    RegistryKey rkRoot = Registry.ClassesRoot;
                    string keyref = rkRoot.OpenSubKey(file.Extension).GetValue("").ToString();
                    Type = rkRoot.OpenSubKey(keyref).GetValue("").ToString();
                    Name = Name.Remove(Name.LastIndexOf(file.Extension));
                }
                catch { Type = "File"; }
            }
            if (Type != "File")
            {
                Icon = "images/icons/" + ParseForImage(file);
                if (Icon.EndsWith(".ico")) Icon = "api/mycomputer/" + ParseForImage(file);
            } else Icon = "images/icons/file.png";
            if (file.Extension.ToLower().Equals(".png") || file.Extension.ToLower().Equals(".jpg") || file.Extension.ToLower().Equals(".jpeg") || file.Extension.ToLower().Equals(".gif") || file.Extension.ToLower().Equals(".bmp") || file.Extension.ToLower().Equals(".wmf"))
                Icon = "api/mycomputer/thumb/" + Converter.UNCtoDrive2(file.FullName, unc, user).Replace('&', '^');
            BType = ComputerBrowser.BType.File;
            CreatedTime = file.CreationTime;
            ModifiedTime = file.LastWriteTime;
            Size = parseLength(file.Length);
            Path = Converter.UNCtoDrive(file.FullName, unc, user);
        }

        public static string ParseForImage(object ExtentionOrName)
        {
            string e;
            if (ExtentionOrName.GetType() == typeof(FileInfo))
            {
                e = ((FileInfo)ExtentionOrName).Extension.ToLower();
                if (string.IsNullOrEmpty(e)) return "file.png";
            }
            else
            {
                e = ((DirectoryInfo)ExtentionOrName).Name.ToLower();
                e = e.Replace("my ", "");
                switch (e)
                {
                    case "pictures": return "mypictures.png";
                    case "video":
                    case "videos": return "myvideos.png";
                    case "music": return "mymusic.png";
                    case "settings": return "settings.png";
                    case "favorites": return "myfavs.png";
                    case "downloads": return "mydownloads.png";
                    default: return "folder.png";
                }
            }
            if (e.StartsWith(".")) e = e.Remove(0, 1);
            switch (e)
            {
                case "txt": return "txt.png";
                case "docx": return "doc.png";
                case "doc": return "doc.png";
                case "docm": return "doc.png";
                case "dot": return "dot.png";
                case "dotx": return "dot.png";
                case "xls": return "xls.png";
                case "xlsx": return "xls.png";
                case "xlt": return "xlt.png";
                case "xltx": return "xlt.png";
                case "csv": return "csv.png";
                case "cs": return "cs.png";
                case "config": return "config.png";
                case "ppt": return "ppt.png";
                case "pptx": return "ppt.png";
                case "pps": return "pps.png";
                case "ppsx": return "pps.png";
                case "pot": return "pot.png";
                case "potx": return "pot.png";
                case "pub": return "pub.png";
                case "pdf": return "pdf.png";
                case "rft": return "rft.png";
                case "mp3": return "mp3.png";
                case "wma": return "mp3.png";
                case "wav": return "mp3.png";
                case "avi": return "vid.png";
                case "wmv": return "vid.png";
                case "mpg": return "vid.png";
                case "mpeg": return "vid.png";
                case "mp4": return "vid.png";
                case "lnk": return "htm.png";
                case "htm": return "htm.png";
                case "html": return "htm.png";
                case "png": return "png.png";
                case "jpg": return "jpg.png";
                case "jpeg": return "jpg.png";
                case "gif": return "gif.png";
                case "bmp": return "bmp.png";
                case "xml": return "xml.png";
                case "xaml": return "xaml.png";
                case "zip": return "zip.png";
                case "rar": return "rar.png";
                case "exe": return "exe.png";
                case "msi": return "msi.png";
                default:
                    try
                    {
                        foreach (FileInfo file in new DirectoryInfo(HttpContext.Current.Server.MapPath("~/images/icons/")).GetFiles())
                            if (file.Name.Replace(file.Extension, "").ToLower().Contains(e.ToLower()))
                                return file.Name;
                    }
                    catch { }
                    return e + ".ico";

            }
        }

        public CBFile(DirectoryInfo dir, UNCPath unc, User user)
        {
            Extension = dir.Extension;
            Type = "File Folder";
            Name = dir.Name;
            Icon = "images/icons/" + MyComputerItem.ParseForImage(dir);
            BType = ComputerBrowser.BType.Folder;
            CreatedTime = dir.CreationTime;
            ModifiedTime = dir.LastWriteTime;
            Path = Converter.UNCtoDrive(dir.FullName, unc, user);
        }

        public static string parseLength(object size)
        {
            decimal d = decimal.Parse(size.ToString() + ".00");
            string[] s = { "bytes", "KB", "MB", "GB", "TB", "PB" };
            int x = 0;
            while (d > 1024)
            {
                d = d / 1024;
                x++;
            }
            d = Math.Round(d, 2);
            return d.ToString() + " " + s[x];
        }
    }
}
