using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using HAP.Web.Configuration;

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

        public CBFile(FileInfo file, UNCPath unc, string userhome)
        {
            Extension = file.Extension;
            Type = "File";
            Name = file.Name + (file.Name.Contains(file.Extension) ? "" : file.Extension);
            try
            {
                RegistryKey rkRoot = Registry.ClassesRoot;
                string keyref = rkRoot.OpenSubKey(file.Extension).GetValue("").ToString();
                Type = rkRoot.OpenSubKey(keyref).GetValue("").ToString();
                Name = Name.Remove(Name.LastIndexOf(file.Extension));
            }
            catch { Type = "File"; }
            Icon = "images/icons/" + MyComputerItem.ParseForImage(file);
            if (file.Extension.ToLower().Equals(".png") || file.Extension.ToLower().Equals(".jpg") || file.Extension.ToLower().Equals(".jpeg") || file.Extension.ToLower().Equals(".gif") || file.Extension.ToLower().Equals(".bmp") || file.Extension.ToLower().Equals(".wmf"))
                Icon = "api/mycomputer/thumb/" + Converter.UNCtoDrive2(file.FullName, unc, userhome).Replace('&', '^');
            BType = ComputerBrowser.BType.File;
            CreatedTime = file.CreationTime;
            ModifiedTime = file.LastWriteTime;
            Size = parseLength(file.Length);
            Path = Converter.UNCtoDrive(file.FullName, unc, userhome);
        }

        public CBFile(DirectoryInfo dir, UNCPath unc, string userhome)
        {
            Extension = dir.Extension;
            Type = "File Folder";
            Name = dir.Name;
            Icon = "images/icons/" + MyComputerItem.ParseForImage(dir);
            BType = ComputerBrowser.BType.Folder;
            CreatedTime = dir.CreationTime;
            ModifiedTime = dir.LastWriteTime;
            Path = Converter.UNCtoDrive(dir.FullName, unc, userhome);
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
