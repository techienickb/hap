using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Web.Services;
using System.ServiceModel.Web;
using HAP.Data.MyFiles.Homework;

namespace HAP.Web.API
{
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Homework
    {
        [OperationContract]
        [WebGet(UriTemplate="", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
        public Data.MyFiles.Homework.Homework[] All()
        {
            return new Data.MyFiles.Homework.Homeworks().Homework.OrderBy(h => h.Teacher).ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate = "my", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public Data.MyFiles.Homework.Homework[] My()
        {
            return new Data.MyFiles.Homework.Homeworks().Homework.Where(h => h.IsVisible() != Data.MyFiles.Homework.UserNodeMode.None).OrderBy(h => h.Teacher).ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate = "{teacher}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public Data.MyFiles.Homework.Homework[] Teacher(string teacher)
        {
            return new Data.MyFiles.Homework.Homeworks().Homework.Where(h => h.Teacher == teacher).ToArray();
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "add", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Add(string teacher, string name, string start, string end, string description, UserNode[] nodes)
        {
            Data.MyFiles.Homework.Homework h = new Data.MyFiles.Homework.Homework(teacher);
            h.Name = name;
            h.Start = start;
            h.End = end;
            h.Description = HttpUtility.UrlDecode(description, System.Text.Encoding.Default);
            h.UserNodes.AddRange(nodes);
            new Data.MyFiles.Homework.Homeworks().Add(h);
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
        public Data.MyFiles.Homework.Homework Item(string teacher, string name, string start, string end)
        {
            return new Data.MyFiles.Homework.Homeworks().Homework.Single(h => h.Teacher == teacher && h.Name == name && h.Start == start.Replace('.', ':') && h.End == end.Replace('.', ':'));
        }
    }
}