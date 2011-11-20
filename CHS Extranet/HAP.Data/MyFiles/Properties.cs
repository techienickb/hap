using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HAP.Data.ComputerBrowser;
using HAP.Web.Configuration;
using HAP.AD;
using Microsoft.Win32;

namespace HAP.Data.MyFiles
{
    public class Properties
    {
        public string Name { get; set; }
        public string DateCreated { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string DateModified { get; set; }
        public string DateAccessed { get; set; }
        public string Contents { get; set; }
        public string Icon { get; set; }
        public string Extension { get; set; }

        public Properties() { }
        public Properties(FileInfo file, DriveMapping mapping, User user)
        {
            Name = file.Name + (file.Name.Contains(file.Extension) ? "" : file.Extension);
            Extension = file.Extension;
            DateCreated = file.CreationTime.ToString();
            DateModified = file.LastWriteTime.ToString();
            DateAccessed = file.LastAccessTime.ToString();
            Location = Converter.UNCtoDrive(file.Directory.FullName, mapping, user).Replace(":", "");
            Size = File.parseLength(file.Length);
            FileIcon fi;
            if (FileIcon.TryGet(Extension, out fi)) Type = fi.Type;
            if (Type == "File")
            {
                try
                {
                    RegistryKey rkRoot = Registry.ClassesRoot;
                    string keyref = rkRoot.OpenSubKey(file.Extension).GetValue("").ToString();
                    Type = rkRoot.OpenSubKey(keyref).GetValue("").ToString();
                }
                catch { Type = "File"; }
            }
            if (Type != "File")
            {
                Icon = "../images/icons/" + File.ParseForImage(file);
                if (Icon.EndsWith(".ico")) Icon = "../api/mycomputer/" + File.ParseForImage(file);
            }
            else Icon = "../images/icons/file.png";
        }

        public Properties(DirectoryInfo dir, DriveMapping mapping, User user)
        {
            Name = dir.Name;
            DateCreated = dir.CreationTime.ToString();
            Location = Converter.UNCtoDrive(dir.Parent.FullName, mapping, user).Replace(":", "");
            long s = 0;
            Contents = dir.GetFiles().Length + " Files, ";
            Contents += dir.GetDirectories().Length + " Folders";
            foreach (FileInfo f in dir.GetFiles("*.*", SearchOption.AllDirectories))
                s += f.Length;
            Size = File.parseLength(s);
            Type = "File Folder";
            if (Type != "File")
            {
                Icon = "../images/icons/" + File.ParseForImage(dir);
                if (Icon.EndsWith(".ico")) Icon = "../api/mycomputer/" + File.ParseForImage(dir);
            }
            else Icon = "../images/icons/file.png";
        }
    }
}
