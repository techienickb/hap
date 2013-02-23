using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web.Security;

namespace HAP.MyFiles
{
    /// <summary>
    /// Configuring a Web site through a Web interface can be tricky. 
    /// If one is to read and write various files, it is useful to know 
    /// in advance if you have the authority to do so.
    /// 
    /// This class contains a simple answer to a 
    /// potentially complicated question 
    /// "Can I read this file or can I write to this file?"
    /// 
    /// Using the "rule of least privilege", 
    /// one must check not only is access granted but 
    /// is it denied at any point including a possibly recursive check of groups.
    /// 
    /// For this simple check, a look at the user and immediate groups are only checked.
    /// 
    /// This class could be expanded to identify if the applicable allow/deny rule
    /// was explicit or inherited
    /// 
    /// </summary>
    public class UserFileAccessRights {

        private string _path;
        private System.Security.Principal.WindowsIdentity _principal;

        private bool _denyAppendData = false;
        private bool _denyCreateDirectories = false;
        private bool _denyCreateFiles = false;
        private bool _denyDelete = false;
        private bool _denyDeleteSubdirectoriesAndFiles = false;
        private bool _denyExecuteFile = false;
        private bool _denyFullControl = false;
        private bool _denyListDirectory = false;
        private bool _denyModify = false;
        private bool _denyRead = false;
        private bool _denyReadAndExecute = false;
        private bool _denyReadAttributes = false;
        private bool _denyReadData = false;
        private bool _denyReadExtendedAttributes = false;
        private bool _denyTraverse = false;
        private bool _denyWrite = false;
        private bool _denyWriteAttributes = false;
        private bool _denyWriteData = false;
        private bool _denyWriteExtendedAttributes = false;

        private bool _allowAppendData = false;
        private bool _allowCreateDirectories = false;
        private bool _allowCreateFiles = false;
        private bool _allowDelete = false;
        private bool _allowDeleteSubdirectoriesAndFiles = false;
        private bool _allowExecuteFile = false;
        private bool _allowFullControl = false;
        private bool _allowListDirectory = false;
        private bool _allowModify = false;
        private bool _allowRead = false;
        private bool _allowReadAndExecute = false;
        private bool _allowReadAttributes = false;
        private bool _allowReadData = false;
        private bool _allowReadExtendedAttributes = false;
        private bool _allowTraverse = false;
        private bool _allowWrite = false;
        private bool _allowWriteAttributes = false;
        private bool _allowWriteData = false;
        private bool _allowWriteExtendedAttributes = false;

        public bool canAppendData() { return !_denyAppendData&&_allowAppendData; }
        public bool canCreateDirectories() 
            { return !_denyCreateDirectories&&_allowCreateDirectories; }
        public bool canCreateFiles() { return !_denyCreateFiles&&_allowCreateFiles; }
        public bool canDelete() { return !_denyDelete && _allowDelete; }
        public bool canDeleteSubdirectoriesAndFiles() 
            { return !_denyDeleteSubdirectoriesAndFiles && 
                _allowDeleteSubdirectoriesAndFiles; }
        public bool canExecuteFile() { return !_denyExecuteFile && _allowExecuteFile; }
        public bool canFullControl() { return !_denyFullControl && _allowFullControl; }
        public bool canListDirectory() 
            { return !_denyListDirectory && _allowListDirectory; }
        public bool canModify() { return !_denyModify && _allowModify; }
        public bool canRead() { return !_denyRead && _allowRead; }
        public bool canReadAndExecute() 
            { return !_denyReadAndExecute && _allowReadAndExecute; }
        public bool canReadAttributes() 
            { return !_denyReadAttributes && _allowReadAttributes; }
        public bool canReadData() { return !_denyReadData && _allowReadData; }
        public bool canReadExtendedAttributes() 
            { return !_denyReadExtendedAttributes && 
                _allowReadExtendedAttributes; }
        public bool canTraverse() { return !_denyTraverse && _allowTraverse; }
        public bool canWrite() { return !_denyWrite && _allowWrite; }
        public bool canWriteAttributes() 
            { return !_denyWriteAttributes && _allowWriteAttributes; }
        public bool canWriteData() { return !_denyWriteData && _allowWriteData; }
        public bool canWriteExtendedAttributes() 
            { return !_denyWriteExtendedAttributes && 
                _allowWriteExtendedAttributes; }

        /// <summary>
        /// Simple accessor
        /// </summary>
        /// <returns></returns>
        public System.Security.Principal.WindowsIdentity getWindowsIdentity() {
            return _principal;
        }
        /// <summary>
        /// Simple accessor
        /// </summary>
        /// <returns></returns>
        public String getPath() {
            return _path;
        }

        /// <summary>
        /// Convenience constructor assumes the current user
        /// </summary>
        /// <param name="path"></param>
        public UserFileAccessRights(string path) :
            this(path, System.Security.Principal.WindowsIdentity.GetCurrent()){}

        public static UserFileAccessRights Get(string path) { return new UserFileAccessRights(path); }

        /// <summary>
        /// Supply the path to the file or directory and a user or group. 
        /// Access checks are done
        /// during instantiation to ensure we always have a valid object
        /// </summary>
        /// <param name="path"></param>
        /// <param name="principal"></param>
        public UserFileAccessRights(string path, 
            System.Security.Principal.WindowsIdentity principal) {
            this._path = path;
            this._principal = principal;
            string username = _principal.Name.Contains('\\') ? _principal.Name.Substring(_principal.Name.IndexOf('\\') + 1) : _principal.Name;
            try {
                System.IO.FileInfo fi = new System.IO.FileInfo(_path);
                AuthorizationRuleCollection acl = fi.GetAccessControl().GetAccessRules
                            (true, true, typeof(NTAccount));
                for (int i = 0; i < acl.Count; i++) {
                    System.Security.AccessControl.FileSystemAccessRule rule = 
                           (System.Security.AccessControl.FileSystemAccessRule)acl[i];
                    if (rule.IdentityReference.Value.ToLower().EndsWith(username.ToLower())) {
                        if (System.Security.AccessControl.AccessControlType.Deny.Equals
                                (rule.AccessControlType)) {
                            if (contains(FileSystemRights.AppendData,rule)) 
                                  _denyAppendData = true;
                            if (contains(FileSystemRights.CreateDirectories,rule)) 
                                  _denyCreateDirectories = true;
                            if (contains(FileSystemRights.CreateFiles,rule)) 
                                  _denyCreateFiles = true;
                            if (contains(FileSystemRights.Delete,rule)) 
                                  _denyDelete = true;
                            if (contains(FileSystemRights.DeleteSubdirectoriesAndFiles,
                                   rule)) _denyDeleteSubdirectoriesAndFiles = true;
                            if (contains(FileSystemRights.ExecuteFile,rule)) 
                                  _denyExecuteFile = true;
                            if (contains(FileSystemRights.FullControl,rule)) 
                                  _denyFullControl = true;
                            if (contains(FileSystemRights.ListDirectory,rule)) 
                                  _denyListDirectory = true;
                            if (contains(FileSystemRights.Modify,rule)) 
                                  _denyModify = true;
                            if (contains(FileSystemRights.Read,rule)) _denyRead = true;
                            if (contains(FileSystemRights.ReadAndExecute,rule)) 
                                  _denyReadAndExecute = true;
                            if (contains(FileSystemRights.ReadAttributes,rule)) 
                                  _denyReadAttributes = true;
                            if (contains(FileSystemRights.ReadData,rule)) 
                                  _denyReadData = true;
                            if (contains(FileSystemRights.ReadExtendedAttributes,rule)) 
                                  _denyReadExtendedAttributes = true;
                            if (contains(FileSystemRights.Traverse,rule)) 
                                  _denyTraverse = true;
                            if (contains(FileSystemRights.Write,rule)) _denyWrite = true;
                            if (contains(FileSystemRights.WriteAttributes,rule)) 
                                  _denyWriteAttributes = true;
                            if (contains(FileSystemRights.WriteData,rule)) 
                                  _denyWriteData = true;
                            if (contains(FileSystemRights.WriteExtendedAttributes,rule)) 
                                  _denyWriteExtendedAttributes = true;
                        }else if (System.Security.AccessControl.AccessControlType.
                                  Allow.Equals(rule.AccessControlType)) {
                            if (contains(FileSystemRights.AppendData, rule))
                                  _allowAppendData = true;
                            if (contains(FileSystemRights.CreateDirectories, rule))
                                  _allowCreateDirectories = true;
                            if (contains(FileSystemRights.CreateFiles, rule))
                                  _allowCreateFiles = true;
                            if (contains(FileSystemRights.Delete, rule))
                                  _allowDelete = true;
                            if (contains(FileSystemRights.DeleteSubdirectoriesAndFiles, 
                                  rule))_allowDeleteSubdirectoriesAndFiles = true;
                            if (contains(FileSystemRights.ExecuteFile, rule))
                                  _allowExecuteFile = true;
                            if (contains(FileSystemRights.FullControl, rule))
                                  _allowFullControl = true;
                            if (contains(FileSystemRights.ListDirectory, rule))
                                  _allowListDirectory = true;
                            if (contains(FileSystemRights.Modify, rule))
                                  _allowModify = true;
                            if (contains(FileSystemRights.Read, rule))_allowRead = true;
                            if (contains(FileSystemRights.ReadAndExecute, rule))
                                  _allowReadAndExecute = true;
                            if (contains(FileSystemRights.ReadAttributes, rule))
                                  _allowReadAttributes = true;
                            if (contains(FileSystemRights.ReadData, rule))
                                  _allowReadData = true;
                            if (contains(FileSystemRights.ReadExtendedAttributes, rule))
                                  _allowReadExtendedAttributes = true;
                            if (contains(FileSystemRights.Traverse, rule))
                                  _allowTraverse = true;
                            if (contains(FileSystemRights.Write, rule))
                                  _allowWrite = true;
                            if (contains(FileSystemRights.WriteAttributes, rule))
                                  _allowWriteAttributes = true;
                            if (contains(FileSystemRights.WriteData, rule))
                                  _allowWriteData = true;
                            if (contains(FileSystemRights.WriteExtendedAttributes, rule))
                                  _allowWriteExtendedAttributes = true;
                        }
                    }
                }

                string[] groups = Roles.GetRolesForUser(_principal.Name.Contains('\\') ? _principal.Name.Substring(_principal.Name.IndexOf('\\') + 1) : _principal.Name);
                for (int j = 0; j < groups.Length; j++) {
                    for (int i = 0; i < acl.Count; i++) {
                        System.Security.AccessControl.FileSystemAccessRule rule = 
                            (System.Security.AccessControl.FileSystemAccessRule)acl[i];
                        if (rule.IdentityReference.Value.ToLower().EndsWith(groups[j].ToLower())) {
                            if (System.Security.AccessControl.AccessControlType.
                                Deny.Equals(rule.AccessControlType)) {
                                if (contains(FileSystemRights.AppendData,rule)) 
                                    _denyAppendData = true;
                                if (contains(FileSystemRights.CreateDirectories,rule)) 
                                    _denyCreateDirectories = true;
                                if (contains(FileSystemRights.CreateFiles,rule)) 
                                    _denyCreateFiles = true;
                                if (contains(FileSystemRights.Delete,rule)) 
                                    _denyDelete = true;
                                if (contains(FileSystemRights.
                                    DeleteSubdirectoriesAndFiles,rule)) 
                                    _denyDeleteSubdirectoriesAndFiles = true;
                                if (contains(FileSystemRights.ExecuteFile,rule)) 
                                    _denyExecuteFile = true;
                                if (contains(FileSystemRights.FullControl,rule)) 
                                    _denyFullControl = true;
                                if (contains(FileSystemRights.ListDirectory,rule)) 
                                    _denyListDirectory = true;
                                if (contains(FileSystemRights.Modify,rule)) 
                                    _denyModify = true;
                                if (contains(FileSystemRights.Read,rule)) 
                                    _denyRead = true;
                                if (contains(FileSystemRights.ReadAndExecute,rule)) 
                                    _denyReadAndExecute = true;
                                if (contains(FileSystemRights.ReadAttributes,rule)) 
                                    _denyReadAttributes = true;
                                if (contains(FileSystemRights.ReadData,rule)) 
                                    _denyReadData = true;
                                if (contains(FileSystemRights.ReadExtendedAttributes,rule)) 
                                    _denyReadExtendedAttributes = true;
                                if (contains(FileSystemRights.Traverse,rule)) 
                                    _denyTraverse = true;
                                if (contains(FileSystemRights.Write,rule)) 
                                    _denyWrite = true;
                                if (contains(FileSystemRights.WriteAttributes,rule)) 
                                    _denyWriteAttributes = true;
                                if (contains(FileSystemRights.WriteData,rule)) 
                                    _denyWriteData = true;
                                if (contains(FileSystemRights.
                                        WriteExtendedAttributes,rule)) 
                                    _denyWriteExtendedAttributes = true;
                            }else if (System.Security.AccessControl.AccessControlType.
                                    Allow.Equals(rule.AccessControlType)) {
                                if (contains(FileSystemRights.AppendData, rule))
                                    _allowAppendData = true;
                                if (contains(FileSystemRights.CreateDirectories, rule))
                                    _allowCreateDirectories = true;
                                if (contains(FileSystemRights.CreateFiles, rule))
                                    _allowCreateFiles = true;
                                if (contains(FileSystemRights.Delete, rule))
                                    _allowDelete = true;
                                if (contains(FileSystemRights.
                                    DeleteSubdirectoriesAndFiles, rule))
                                    _allowDeleteSubdirectoriesAndFiles = true;
                                if (contains(FileSystemRights.ExecuteFile, rule))
                                    _allowExecuteFile = true;
                                if (contains(FileSystemRights.FullControl, rule))
                                    _allowFullControl = true;
                                if (contains(FileSystemRights.ListDirectory, rule))
                                    _allowListDirectory = true;
                                if (contains(FileSystemRights.Modify, rule))
                                    _allowModify = true;
                                if (contains(FileSystemRights.Read, rule))
                                    _allowRead = true;
                                if (contains(FileSystemRights.ReadAndExecute, rule))
                                    _allowReadAndExecute = true;
                                if (contains(FileSystemRights.ReadAttributes, rule))
                                    _allowReadAttributes = true;
                                if (contains(FileSystemRights.ReadData, rule))
                                    _allowReadData = true;
                                if (contains(FileSystemRights.
                                    ReadExtendedAttributes, rule))
                                    _allowReadExtendedAttributes = true;
                                if (contains(FileSystemRights.Traverse, rule))
                                    _allowTraverse = true;
                                if (contains(FileSystemRights.Write, rule))
                                    _allowWrite = true;
                                if (contains(FileSystemRights.WriteAttributes, rule))
                                    _allowWriteAttributes = true;
                                if (contains(FileSystemRights.WriteData, rule))
                                    _allowWriteData = true;
                                if (contains(FileSystemRights.WriteExtendedAttributes, 
                                    rule))_allowWriteExtendedAttributes = true;
                            }
                        }
                    }
                }
            } catch (Exception e) {
                //Deal with IO exceptions if you want
                throw e;
            }
        }

        public Dictionary<string, bool> ToCollection()
        {
            Dictionary<string, bool> col = new Dictionary<string, bool>();
            col.Add("AppendData", canAppendData());
            col.Add("CreateDirs", canCreateDirectories());
            col.Add("CreateFiles", canCreateFiles());
            col.Add("Delete", canDelete());
            col.Add("DeleteSubDirsOrFiles", canDeleteSubdirectoriesAndFiles());
            col.Add("Execute", canExecuteFile());
            col.Add("FullControl", canFullControl());
            col.Add("ListDirs", canListDirectory());
            col.Add("Modify", canModify());
            col.Add("ReadExecute", canReadAndExecute());
            col.Add("ReadAttr", canReadAttributes());
            col.Add("ReadData", canReadData());
            col.Add("ReadExAttr", canReadExtendedAttributes());
            col.Add("Traverse", canTraverse());
            col.Add("Write", canWrite());
            col.Add("WriteAttr", canWriteAttributes());
            col.Add("WriteData", canWriteData());
            col.Add("WriteExAttr", canWriteExtendedAttributes());
            return col;
        }

        public string ToCollectionString()
        {
            List<string> _s = new List<string>();
            foreach (string s in ToCollection().Keys)
                _s.Add(s + ": " + ToCollection()[s].ToString().ToLower());
            return "{ " + string.Join(", ", _s.ToArray()) + " }";
        }

        /// <summary>
        /// Simply displays all allowed rights
        /// 
        /// Useful if say you want to test for write access and find
        /// it is false;
        /// <xmp>
        /// UserFileAccessRights rights = new UserFileAccessRights(txtLogPath.Text);
        /// System.IO.FileInfo fi = new System.IO.FileInfo(txtLogPath.Text);
        /// if (rights.canWrite() && rights.canRead()) {
        ///     lblLogMsg.Text = "R/W access";
        /// } else {
        ///     if (rights.canWrite()) {
        ///        lblLogMsg.Text = "Only Write access";
        ///     } else if (rights.canRead()) {
        ///         lblLogMsg.Text = "Only Read access";
        ///     } else {
        ///         lblLogMsg.CssClass = "error";
        ///         lblLogMsg.Text = rights.ToString()
        ///     }
        /// }
        /// 
        /// </xmp>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString() {
            string str = "";
        
            if (canAppendData()) {if(!String.IsNullOrEmpty(str)) str+=
                ",";str+="AppendData";}
            if (canCreateDirectories()) {if(!String.IsNullOrEmpty(str)) str+=
                ",";str+="CreateDirectories";}
            if (canCreateFiles()) {if(!String.IsNullOrEmpty(str)) str+=
                ",";str+="CreateFiles";}
            if (canDelete()) {if(!String.IsNullOrEmpty(str)) str+=
                ",";str+="Delete";}
            if (canDeleteSubdirectoriesAndFiles()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "DeleteSubdirectoriesAndFiles"; }
            if (canExecuteFile()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "ExecuteFile"; }
            if (canFullControl()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "FullControl"; }
            if (canListDirectory()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "ListDirectory"; }
            if (canModify()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "Modify"; }
            if (canRead()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "Read"; }
            if (canReadAndExecute()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "ReadAndExecute"; }
            if (canReadAttributes()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "ReadAttributes"; }
            if (canReadData()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "ReadData"; }
            if (canReadExtendedAttributes()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "ReadExtendedAttributes"; }
            if (canTraverse()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "Traverse"; }
            if (canWrite()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "Write"; }
            if (canWriteAttributes()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "WriteAttributes"; }
            if (canWriteData()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "WriteData"; }
            if (canWriteExtendedAttributes()) { if (!String.IsNullOrEmpty(str)) 
                str += ","; str += "WriteExtendedAttributes"; }
            if (String.IsNullOrEmpty(str))
                str = "None";
            return str;
        }

        /// <summary>
        /// Convenience method to test if the right exists within the given rights
        /// </summary>
        /// <param name="right"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool contains(System.Security.AccessControl.FileSystemRights right,
            System.Security.AccessControl.FileSystemAccessRule rule ) {
            return (((int)right & (int)rule.FileSystemRights) == (int)right);
        }    
    }
}
