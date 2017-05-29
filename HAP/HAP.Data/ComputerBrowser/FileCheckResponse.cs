using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HAP.Web.Configuration;
using HAP.AD;

namespace HAP.Data.ComputerBrowser
{
    public class FileCheckResponse
    {
        public FileCheckResponse() { Code = FileCheckResponseCode.Deny; FileSize = "-1"; }
        public FileCheckResponse(FileInfo file, UNCPath unc, User user)
        {
            Thumb = "images/icons/" + MyComputerItem.ParseForImage(file);
            if (file.Exists)
            {
                Code = FileCheckResponseCode.Exists;
                FileName = file.Name;
                Extension = file.Extension;
                FilePath = Converter.UNCtoDrive(file.FullName, unc, user);
                FileSize = Converter.parseLength(file.Length);
                DateModified = file.LastWriteTime;
                DateCreated = file.CreationTime;
            }
            else Code = FileCheckResponseCode.OK;
        }

        public FileCheckResponse(string path, UNCPath unc, User user)
            : this(new FileInfo(path), unc, user)
        {
        }

        public FileCheckResponseCode Code { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        private string FilePath { get; set; }
        public string Thumb { get; set; }
        public string FileSize { get; set; }
        public Nullable<DateTime> DateModified { get; set; }
        public Nullable<DateTime> DateCreated { get; set; }


    }

    public enum FileCheckResponseCode { OK, Exists, Deny }
}
