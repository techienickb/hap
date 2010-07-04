using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace HAP.Web
{
    public class MyComputerItem
    {
        public MyComputerItem(string name, string description, string path, string image, bool rightclick)
        {
            Name = name;
            Description = description;
            Path = path;
            Image = image;
            RightClick = rightclick;
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
                e = ((DirectoryInfo)ExtentionOrName).Name;
                switch (e) 
                {
                    case "My Pictures": return "mypictures.png"; 
                    case "My Videos": return "myvideos.png"; 
                    case "My Music": return "mymusic.png"; 
                    case "My Settings": return "settings.png"; 
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
                        return "file.png";
                    }
                    catch
                    {
                        return "file.png";
                    }
                    
            }
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Path { get; set; }
        public bool RightClick { get; set; }
    }
}