using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Web;
using HAP.MyFiles.Homework;
using HAP.Web.Configuration;
using System.DirectoryServices;
using HAP.AD;
using HAP.Data.ComputerBrowser;
using System.IO;
namespace HAP.MyFiles
{
    [ServiceAPI("api/homework")]
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class HomeworkAPI
    {
        [OperationContract]
        [WebGet(UriTemplate="", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
        public Homework.Homework[] All()
        {
            return new Homework.Homeworks().Homework.OrderBy(h => h.Teacher).ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate = "my", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public MyFiles.Homework.Homework[] My()
        {
            return new MyFiles.Homework.Homeworks().Homework.Where(h => h.IsVisible() != "None" && DateTime.Parse(h.End) > DateTime.Now).OrderBy(h => h.Teacher).ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate = "{teacher}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public MyFiles.Homework.Homework[] Teacher(string teacher)
        {
            return new MyFiles.Homework.Homeworks().Homework.Where(h => h.Teacher == teacher).ToArray();
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "add1", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Add1(string name, string start, string end, string description, string path, UserNode[] nodes)
        {
            Add(HttpContext.Current.User.Identity.Name, name, start, end, path, description, nodes);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "add", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Add(string teacher, string name, string start, string end, string path, string description, UserNode[] nodes)
        {
            MyFiles.Homework.Homework h = new MyFiles.Homework.Homework(teacher);
            h.Name = name;
            h.Start = start;
            h.End = end;
            h.Description = HttpUtility.UrlDecode(description, System.Text.Encoding.Default);
            h.UserNodes.AddRange(nodes);
            h.Token = HttpContext.Current.Request.Cookies["token"].Value;
            h.Path = path;
            User u = new User();
            u.Authenticate(h.Teacher, TokenGenerator.ConvertToPlain(h.Token));
            DriveMapping mapping;
            string p = Converter.DriveToUNC(h.Path.Remove(0, 1), h.Path.Substring(0, 1), out mapping, u);
            u.ImpersonateContained();
            try
            {
                if (!Directory.Exists(p)) Directory.CreateDirectory(p);
                if (!Directory.Exists(Path.Combine(p, h.Name))) Directory.CreateDirectory(Path.Combine(p, h.Name));
            }
            finally
            {
                u.EndContainedImpersonate();
            }
            new MyFiles.Homework.Homeworks().Add(h);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "edit", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Edit(string teacher, string name, string start, string end, string newname, string newstart, string newend, string description, string path, UserNode[] nodes)
        {
            Homeworks h = new Homeworks();
            Homework.Homework orig = h.Homework.Single(hw => hw.Teacher == teacher && hw.Name == name && hw.Start == start.Replace('.', ':') && hw.End == end.Replace('.', ':'));
            Homework.Homework updated = orig;
            updated.Name = name;
            updated.Start = start;
            updated.End = end;
            updated.Path = path;
            updated.Description = HttpUtility.UrlDecode(description, System.Text.Encoding.Default);
            updated.UserNodes.Clear();
            updated.UserNodes.AddRange(nodes);
            User u = new User();
            u.Authenticate(updated.Teacher, TokenGenerator.ConvertToPlain(updated.Token));
            DriveMapping mapping;
            string p = Converter.DriveToUNC(updated.Path.Remove(0, 1), updated.Path.Substring(0, 1), out mapping, u);
            u.ImpersonateContained();
            try
            {
                if (!Directory.Exists(p)) Directory.CreateDirectory(p);
                if (!Directory.Exists(Path.Combine(p, updated.Name))) Directory.CreateDirectory(Path.Combine(p, updated.Name));
            }
            finally
            {
                u.EndContainedImpersonate();
            }
            h.Update(orig, updated);
        }


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "remove", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Remove(string teacher, string name, string start, string end)
        {
            Homeworks h = new Homeworks();
            h.Remove(h.Homework.Single(hw => hw.Teacher == teacher && hw.Name == name && hw.Start == start.Replace('-', '/').Replace('.', ':') && hw.End == end.Replace('-', '/').Replace('.', ':')));
        }

        [OperationContract]
        [WebGet(UriTemplate = "{teacher}/{start}/{end}/{name}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public MyFiles.Homework.Homework Item(string teacher, string name, string start, string end)
        {
            return new MyFiles.Homework.Homeworks().Homework.Single(h => h.Teacher == teacher && h.Name == name && h.Start == start.Replace('-', '/').Replace('.', ':') && h.End == end.Replace('-', '/').Replace('.', ':'));
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "search", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string[] Search(string query)
        {
            List<string> res = new List<string>();
            DirectoryEntry root = new DirectoryEntry("LDAP://DC=" + hapConfig.Current.AD.UPN.Replace(".", ",DC="), hapConfig.Current.AD.User, hapConfig.Current.AD.Password);
            DirectorySearcher searcher = new DirectorySearcher(root);
            searcher.PropertiesToLoad.Add("cn");
            searcher.PropertiesToLoad.Add("displayName");
            searcher.Filter = "(&(cn=" + query + ")(objectCategory=user))";

            SearchResultCollection results = searcher.FindAll();
            foreach (SearchResult r in results)
            {
                res.Add(r.Properties["cn"][0].ToString() + "|" + r.Properties["displayName"][0].ToString());
            }
            searcher = new DirectorySearcher(root);
            searcher.PropertiesToLoad.Add("cn");
            searcher.PropertiesToLoad.Add("displayName");
            searcher.Filter = "(&(displayName=" + query + ")(objectCategory=user))";

            results = searcher.FindAll();
            foreach (SearchResult r in results)
            {
                res.Add(r.Properties["cn"][0].ToString() + "|" + r.Properties["displayName"][0].ToString());
            }
            return res.ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate = "Exists/{teacher}/{name}/{start}/{end}/{Drive}/{*Path}")]
        public Properties Exists(string Drive, string Path, string teacher, string name, string start, string end)
        {
            try
            {
                return Properties(teacher, name, start.Replace('-', '/').Replace('.', ':'), end.Replace('-', '/').Replace('.', ':'), Drive, Path);
            }
            catch
            {
                return new Properties();
            }
        }

        [OperationContract]
        [WebGet(UriTemplate = "Properties/{teacher}/{start}/{end}/{name}/{Drive}/{*Path}")]
        public Properties Properties(string teacher, string name, string start, string end, string Drive, string Path)
        {
            Properties ret = new Properties();
            Path = "/" + Path;
            hapConfig config = hapConfig.Current;
            List<File> Items = new List<File>();
            Homework.Homework i = Item(teacher, name, start, end);
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                user.Authenticate(i.Teacher, TokenGenerator.ConvertToPlain(i.Token));
            }
            DriveMapping mapping;
            string path = Converter.DriveToUNC(Path, Drive, out mapping, user);
            HAP.Data.SQL.WebEvents.Log(DateTime.Now, "MyFiles.Properties", user.UserName, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser.Platform, HttpContext.Current.Request.Browser.Browser + " " + HttpContext.Current.Request.Browser.Version, HttpContext.Current.Request.UserHostName, "Requesting properties of: " + path);
            user.ImpersonateContained();
            try
            {
                FileAttributes attr = System.IO.File.GetAttributes(path);
                //detect whether its a directory or file
                ret = ((attr & FileAttributes.Directory) == FileAttributes.Directory) ? new Properties(new DirectoryInfo(path), mapping, user) : new Properties(new FileInfo(path), mapping, user);
            }
            finally { user.EndContainedImpersonate(); }

            return ret;
        }
    }
}