using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAP.Win.MyFiles.JSON
{
    public class JSONUser
    {
        public bool isValid { get; set; }
        public string Username { get;set; }
        public string Token1 { get; set; }
        public string FirstName { get; set; }
        public string Token2 { get; set; }
        public string Token2Name { get; set; }
        public string SiteName { get; set; }
        public string[] ToString()
        {
            return new string[] { Token1, Token2, Token2Name };
        }
    }

    public class JSONDrive
    {
        public JSONDrive() { }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Path { get; set; }
        public decimal Space { get; set; }
        public AccessControlActions Actions { get; set; }
    }

    public class JSONFile
    {
        public NTFSPerms Permissions { get; set; }

        public string CreationTime { get; set; }
        public string ModifiedTime { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public AccessControlActions Actions { get; set; }
    }

    public class NTFSPerms
    {
        public bool AppendData { get; set; }
        public bool CreateDirs { get; set; }
        public bool Delete { get; set; }
        public bool DeleteSubDirsOrFiles { get; set; }
        public bool Execute { get; set; }
        public bool FullControl { get; set; }
        public bool ListDirs { get; set; }
        public bool Modify { get; set; }
        public bool ReadExecute { get; set; }
        public bool ReadAttr { get; set; }
        public bool ReadData { get; set; }
        public bool ReadExAttr { get; set; }
        public bool Traverse { get; set; }
        public bool Write { get; set; }
        public bool WriteAttr { get; set; }
        public bool WriteData { get; set; }
        public bool WriteExAttr { get; set; }
    }

    public enum AccessControlActions { Change, View, None, ZIP }
}
