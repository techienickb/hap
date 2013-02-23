using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HAP.Web.Configuration;
using HAP.AD;
using Microsoft.Win32;
using System.Web;
using HAP.Data.ComputerBrowser;
using System.Security.AccessControl;
using System.Security.Principal;

namespace HAP.MyFiles
{
    public class Properties
    {
        public AccessControlActions Actions { get; set; }
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
        public string Permissions { get; set; }

        public Properties() { }
        public Properties(FileInfo file, DriveMapping mapping, User user)
        {
            Actions = isWriteAuth(mapping) ? HAP.MyFiles.AccessControlActions.Change : HAP.MyFiles.AccessControlActions.View;
            Name = file.Name + (file.Name.Contains(file.Extension) ? "" : file.Extension);
            Permissions = "";
            Extension = file.Extension;
            DateCreated = file.CreationTime.ToString();
            DateModified = file.LastWriteTime.ToString();
            DateAccessed = file.LastAccessTime.ToString();
            Location = HttpUtility.UrlEncode(Converter.UNCtoDrive(file.Directory.FullName, mapping, user).Replace(":", "")).Replace('+', ' ').Replace("%", "|").Replace("|5c", "\\");
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

        /// <summary>
        /// Create Properties for quick use
        /// </summary>
        /// <param name="dir">Directory</param>
        /// <param name="user">User</param>
        /// <param name="mapping">Drive Mapping</param>
        public Properties(DirectoryInfo dir, User user, DriveMapping mapping)
        {
            DriveMapping m;
            Actions = isWriteAuth(mapping) ? HAP.MyFiles.AccessControlActions.Change : HAP.MyFiles.AccessControlActions.View;
            Permissions = "";
            if (Actions == HAP.MyFiles.AccessControlActions.Change)
            {
                try { System.IO.File.Create(System.IO.Path.Combine(dir.FullName, "temp.ini")).Close(); System.IO.File.Delete(System.IO.Path.Combine(dir.FullName, "temp.ini")); }
                catch { Actions = HAP.MyFiles.AccessControlActions.View; }
            }
            if (dir.FullName.Contains(".zip")) Actions = AccessControlActions.ZIP;
            else 
            { 
                try { dir.GetDirectories(); }
                catch { Actions = HAP.MyFiles.AccessControlActions.None; }
            }
            Name = (dir.FullName == Converter.DriveToUNC("", mapping.Drive.ToString(), out m, user) + '\\') ? mapping.Name : dir.Name;
            Location = HttpUtility.UrlEncode(Converter.UNCtoDrive(dir.FullName, mapping, user).Replace(":", "")).Replace('+', ' ').Replace("%", "|").Replace("|5c", "\\");
            Type = "File Folder";
            if (Type != "File")
            {
                Icon = "../images/icons/" + File.ParseForImage(dir);
                if (Icon.EndsWith(".ico")) Icon = "../api/mycomputer/" + File.ParseForImage(dir);
            }
            else Icon = "../images/icons/file.png";
        }

        public Properties(DirectoryInfo dir, DriveMapping mapping, User user)
        {
            Name = dir.Name;
            DateCreated = dir.CreationTime.ToString();
            DriveMapping m;
            Actions = isWriteAuth(mapping) ? HAP.MyFiles.AccessControlActions.Change : HAP.MyFiles.AccessControlActions.View;
            try
            {
                Permissions = UserFileAccessRights.Get(dir.FullName).ToCollectionString();
            }
            catch { }
            if (Actions == HAP.MyFiles.AccessControlActions.Change && hapConfig.Current.MyFiles.WriteChecks)
            {
                try { System.IO.File.Create(System.IO.Path.Combine(dir.FullName, "temp.ini")).Close(); System.IO.File.Delete(System.IO.Path.Combine(dir.FullName, "temp.ini")); }
                catch { Actions = HAP.MyFiles.AccessControlActions.View; }
            }
            try { dir.GetDirectories(); }
            catch { Actions = HAP.MyFiles.AccessControlActions.None; }
            if (dir.FullName == Converter.DriveToUNC("", mapping.Drive.ToString(), out m, user) + "\\")
            {
                Location = null;
            }
            else
            {
                Location = HttpUtility.UrlEncode(Converter.UNCtoDrive(dir.Parent.FullName, mapping, user).Replace(":", "")).Replace('+', ' ').Replace("%", "|").Replace("|5c", "\\");

                long s = 0;
                Contents = dir.GetFiles().Length + " Files, ";
                Contents += dir.GetDirectories().Length + " Folders";
                foreach (FileInfo f in dir.GetFiles("*.*", SearchOption.AllDirectories))
                    s += f.Length;
                Size = File.parseLength(s);
            }
            Type = "File Folder";
            if (Type != "File")
            {
                Icon = "../images/icons/" + File.ParseForImage(dir);
                if (Icon.EndsWith(".ico")) Icon = "../api/mycomputer/" + File.ParseForImage(dir);
            }
            else Icon = "../images/icons/file.png";
        }

        private bool isWriteAuth(DriveMapping path)
        {
            if (path == null) return true;
            if (path.EnableWriteTo == "All") return true;
            else if (path.EnableWriteTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableWriteTo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = HttpContext.Current.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }
    }
}
