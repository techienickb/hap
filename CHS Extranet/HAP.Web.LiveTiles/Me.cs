using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using HAP.Web.Configuration;
using System.DirectoryServices;
using System.IO;

namespace HAP.Web.LiveTiles
{
    public class Me
    {
        public static Me GetMe
        {
            get
            {
                return new Me();
            }
        }

        private hapConfig config;

        public Me()
        {
            config = hapConfig.Current;
            User = ((HAP.AD.User)Membership.GetUser());
            Name = User.DisplayName;
            HAP.AD.User _user = new HAP.AD.User();
            _user.Authenticate(hapConfig.Current.AD.User, hapConfig.Current.AD.Password);
            string errorlist = "";
            try
            {
                _user.ImpersonateContained();
                using (DirectorySearcher dsSearcher = new DirectorySearcher())
                {
                    errorlist += "Creating Directory Search and Searching for then current user\n";
                    dsSearcher.Filter = "(&(objectClass=user) (sAMAccountName=" + ((HAP.AD.User)Membership.GetUser()).UserName + "))";
                    errorlist += "Using filter: " + dsSearcher.Filter + "\n";
                    dsSearcher.PropertiesToLoad.Add("thumbnailPhoto");
                    SearchResultCollection results = dsSearcher.FindAll();

                    errorlist += "Found " + results.Count + " results, processing 1st result\n";
                    if (results.Count > 0)
                    {
                        errorlist += "Found " + results[0].Properties["thumbnailPhoto"].Count + " thumnbnailPhotos\n";
                        if (results[0].Properties["thumbnailPhoto"].Count > 0)
                        {
                            byte[] data = results[0].Properties["thumbnailPhoto"][0] as byte[];
                            if (data != null)
                            {
                                errorlist += "Data found, making picture url\n";
                                Photo = "~/api/mypic";
                            }
                            else throw new Exception();
                        }
                        else throw new Exception();
                    }
                    else throw new Exception();
                }
            }
            catch
            {
                _user.EndContainedImpersonate();
                if (!hapConfig.Current.School.HidePhotoErrors) HAP.Web.Logging.EventViewer.Log("HAP.Web.API.MyPic", errorlist, System.Diagnostics.EventLogEntryType.Error, true);
                Photo = null;
            }
            finally { _user.EndContainedImpersonate(); }
            Email = User.Email == null ? "" : User.Email;
            OtherData = new Dictionary<string, string>();
            try { OtherData.Add("Comment", User.Comment); }
            catch { }
            try { OtherData.Add("Notes", User.Notes); }
            catch { }
            try { OtherData.Add("LastLoginDate", User.LastLoginDate.ToString()); }
            catch { }
            try { OtherData.Add("LastName", User.LastName); }
            catch { }
            try { OtherData.Add("FirstName", User.FirstName); }
            catch { }
            try { OtherData.Add("EmployeeID", User.EmployeeID); }
            catch { }
            try { OtherData.Add("DisplayName", User.DisplayName); }
            catch { }
            try { OtherData.Add("MiddleNames", User.MiddleNames); }
            catch { }
        }

        public string Name { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> OtherData { get; set; }

        private HAP.AD.User User;
    }
}
