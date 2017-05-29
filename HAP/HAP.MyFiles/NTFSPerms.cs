using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.MyFiles
{
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
        public bool CreateFiles { get; set; }
    }
}
