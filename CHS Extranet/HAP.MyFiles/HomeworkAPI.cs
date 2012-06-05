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
            return new MyFiles.Homework.Homeworks().Homework.Where(h => h.IsVisible() != MyFiles.Homework.UserNodeMode.None && DateTime.Parse(h.End) < DateTime.Now).OrderBy(h => h.Teacher).ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate = "{teacher}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public MyFiles.Homework.Homework[] Teacher(string teacher)
        {
            return new MyFiles.Homework.Homeworks().Homework.Where(h => h.Teacher == teacher).ToArray();
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "add1", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Add1(string name, string start, string end, string description, UserNode[] nodes)
        {
            Add(HttpContext.Current.User.Identity.Name, name, start, end, description, nodes);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "add", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Add(string teacher, string name, string start, string end, string description, UserNode[] nodes)
        {
            MyFiles.Homework.Homework h = new MyFiles.Homework.Homework(teacher);
            h.Name = name;
            h.Start = start;
            h.End = end;
            h.Description = HttpUtility.UrlDecode(description, System.Text.Encoding.Default);
            h.UserNodes.AddRange(nodes);
            new MyFiles.Homework.Homeworks().Add(h);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "{teacher}/{start}/{end}/{name}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Remove(string teacher, string name, string start, string end)
        {
            Homeworks h = new Homeworks();
            h.Remove(h.Homework.Single(hw => hw.Teacher == teacher && hw.Name == name && hw.Start == start.Replace('.', ':') && hw.End == end.Replace('.', ':')));
        }

        [OperationContract]
        [WebGet(UriTemplate = "{teacher}/{start}/{end}/{name}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public MyFiles.Homework.Homework Item(string teacher, string name, string start, string end)
        {
            return new MyFiles.Homework.Homeworks().Homework.Single(h => h.Teacher == teacher && h.Name == name && h.Start == start.Replace('.', ':') && h.End == end.Replace('.', ':'));
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
    }
}