using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.IO;
using System.Security.Authentication;
using Microsoft.Win32;

namespace CHS_Extranet
{
    /// <summary>
    /// Summary description for f
    /// </summary>
    public class f : IHttpHandler
    {

        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private GroupPrincipal studentgp;

        public string Username
        {
            get
            {
                if (HttpContext.Current.User.Identity.Name.Contains('\\'))
                    return HttpContext.Current.User.Identity.Name.Remove(0, HttpContext.Current.User.Identity.Name.IndexOf('\\') + 1);
                else return HttpContext.Current.User.Identity.Name;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings["ADConnectionString"];
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP:/"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, ConfigurationManager.AppSettings["ADUsername"], ConfigurationManager.AppSettings["ADPassword"]);
            studentgp = GroupPrincipal.FindByIdentity(pcontext, ConfigurationManager.AppSettings["StudentGroup"]);
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);

            string userhome = up.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            string p = context.Request.PathInfo.Substring(1, 1);
            string path = context.Request.PathInfo.Remove(0, 2);
            if (p == "N") path = up.HomeDirectory + path.Replace('/', '\\');
            else if (p == "W") path = ConfigurationManager.AppSettings["SharedDocsUNC"] + path.Replace('/', '\\');
            else if (p == "T") path = ConfigurationManager.AppSettings["RMStaffUNC"] + path.Replace('/', '\\');
            else if (p == "R") path = ConfigurationManager.AppSettings["AdminSharedUNC"] + path.Replace('/', '\\');
            else if (p == "H") path = string.Format(ConfigurationManager.AppSettings["AdminServerUNC"], Username) + path.Replace('/', '\\');

            if (up.IsMemberOf(studentgp) && (p == "T" || p == "H" || p == "R"))
            {
                context.Response.Redirect("/extranet/unauthorised.aspx", true);
            }
            else
            {
                FileInfo file = new FileInfo(path);
                context.Response.ContentType = MimeType(file.Extension);
                if (string.IsNullOrEmpty(context.Request.QueryString["inline"]))
                context.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + file.Name + "\"");
                else context.Response.AppendHeader("Content-Disposition", "inline; filename=\"" + file.Name + "\"");
                context.Response.AddHeader("Content-Length", file.Length.ToString("F0"));
                context.Response.Clear();
                context.Response.TransmitFile(file.FullName);
                context.Response.Flush();
                context.Response.End();
            }
        }

        public static string MimeType(string Extension)
        {
            string mime = "application/octetstream";
            if (string.IsNullOrEmpty(Extension))
                return mime;
            string ext = Extension.ToLower();
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(ext);
            if (rk != null && rk.GetValue("Content Type") != null)
                mime = rk.GetValue("Content Type").ToString();
            return mime;

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}