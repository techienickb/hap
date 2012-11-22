using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using HAP.Web.Configuration;
using System.Web;
using HAP.Data;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace HAP.Web.API
{
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class setup
    {
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AddOU", BodyStyle = WebMessageBodyStyle.Wrapped)]
        public int AddOU(string path, string name, string visibility)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.AD.OUs.Add(name, HttpUtility.UrlDecode(path, System.Text.Encoding.Default), (OUVisibility)Enum.Parse(typeof(OUVisibility), visibility));
            return 0;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RemoveOU", BodyStyle = WebMessageBodyStyle.Wrapped)]
        public int RemoveOU(string name)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.AD.OUs.Remove(name);
            return 0;
        }

        [OperationContract]
        [WebInvoke(Method="POST", RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json, UriTemplate="/AddMapping", BodyStyle=WebMessageBodyStyle.Wrapped)]
        public int AddMapping(string drive, string name, string unc, string enablereadto, string enablewriteto, bool enablemove, string usagemode)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            unc = unc.Replace('/', '\\');
            Config.MyFiles.Mappings.Add(drive.ToCharArray()[0], name, unc, enablereadto, enablewriteto, enablemove, (MappingUsageMode)Enum.Parse(typeof(MappingUsageMode), usagemode));
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RemoveMapping", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int RemoveMapping(string drive)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MyFiles.Mappings.Remove(drive.ToCharArray()[0]);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateMapping", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int UpdateMapping(string origdrive, string drive, string name, string unc, string enablereadto, string enablewriteto, bool enablemove, string usagemode)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            DriveMapping m = Config.MyFiles.Mappings[origdrive.ToCharArray()[0]];
            m.Drive = drive.ToCharArray()[0];
            m.EnableMove = enablemove;
            m.UsageMode = (MappingUsageMode)Enum.Parse(typeof(MappingUsageMode), usagemode);
            m.Name = name;
            m.UNC = unc.Replace('/', '\\');
            m.EnableReadTo = enablereadto;
            m.EnableWriteTo = enablewriteto;
            Config.MyFiles.Mappings.Update(origdrive.ToCharArray()[0], m);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AddFilter", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int AddFilter(string name, string expression, string enablefor)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MyFiles.Filters.Add(name, expression.Replace("/", "\\"), enablefor);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RemoveFilter", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int RemoveFilter(string name, string expression)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MyFiles.Filters.Delete(name, expression.Replace("/", "\\"));
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateFilter", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int UpdateFilter(string origname, string origexpression, string name, string expression, string enablefor)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Filter f = Config.MyFiles.Filters.Find(origname, origexpression.Replace("/", "\\"));
            f.Name = name;
            f.Expression = expression.Replace("/", "\\");
            f.EnableFor = enablefor;
            Config.MyFiles.Filters.Update(origname, origexpression.Replace("/", "\\"), f);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AddQServer", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int AddQServer(string server, string expression, string drive)
        {
            expression = expression.Replace('/', '\\');
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MyFiles.QuotaServers.Add(server, expression, drive.ToCharArray()[0]);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RemoveQServer", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int RemoveQServer(string server, string expression)
        {
            expression = expression.Replace('/', '\\');
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MyFiles.QuotaServers.Delete(server, expression);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateQServer", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int UpdateQServer(string origserver, string origexpression, string server, string expression, string drive)
        {
            origexpression = origexpression.Replace('/', '\\');
            expression = expression.Replace('/', '\\');
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            QuotaServer q = Config.MyFiles.QuotaServers.Find(origserver, origexpression);
            q.Server = server;
            q.Drive = drive.ToCharArray()[0];
            q.Expression = expression;
            Config.MyFiles.QuotaServers.Update(origserver, origexpression, q);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AddResource", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int AddResource(string name, string type, bool enabled, bool charging, string admins, bool emailadmins, string showto, string hidefrom, string quantities, string years, string readonlyto, string readwriteto)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Resources.Add(name, (ResourceType)Enum.Parse(typeof(ResourceType), type), admins, enabled, emailadmins, charging, showto, hidefrom, years, quantities, readonlyto, readwriteto);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateResource", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int UpdateResource(string origname, string name, string type, bool enabled, bool charging, string admins, bool emailadmins, string showto, string hidefrom, string years, string quantities, string readonlyto, string readwriteto)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Resource r = Config.BookingSystem.Resources[origname];
            r.Name = name;
            r.Type = (ResourceType)Enum.Parse(typeof(ResourceType), type);
            r.Admins = admins;
            r.Enabled = enabled;
            r.EmailAdmins = emailadmins;
            r.EnableCharging = charging;
            r.ShowTo = showto;
            r.HideFrom = hidefrom;
            r.Quantities = quantities;
            r.ReadWriteTo = readwriteto;
            r.ReadOnlyTo = readonlyto;
            r.Years = years;
            Config.BookingSystem.Resources.Update(origname, r);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RemoveResource", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int RemoveResource(string name)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Resources.Delete(name);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AddLesson", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int AddLesson(string name, string type, string start, string end)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            int h = int.Parse(start.Substring(0, 2)) + (start.Contains("PM") ? 12 : 0);
            if (h == 12) h = 0;
            else if (h == 24) h = 12;
            int h2 = int.Parse(end.Substring(0, 2)) + (end.Contains("PM") ? 12 : 0);
            if (h2 == 12) h2 = 0;
            else if (h2 == 24) h2 = 12;
            Config.BookingSystem.Lessons.Add(name,
                (LessonType)Enum.Parse(typeof(LessonType), type),
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, h, int.Parse(start.Substring(3, 2)), 00),
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, h2, int.Parse(end.Substring(3, 2)), 00));
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/EditLesson", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int EditLesson(string origname, string name, string type, string start, string end)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Lesson l = Config.BookingSystem.Lessons.Get(origname);
            l.Name = name;
            l.Type = (LessonType)Enum.Parse(typeof(LessonType), type);
            int h = int.Parse(start.Substring(0, 2)) + (start.Contains("PM") ? 12 : 0);
            if (h == 12) h = 0;
            else if (h == 24) h = 12;
            int h2 = int.Parse(end.Substring(0, 2)) + (end.Contains("PM") ? 12 : 0);
            if (h2 == 12) h2 = 0;
            else if (h2 == 24) h2 = 12;
            l.StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, h, int.Parse(start.Substring(3, 2)), 00);
            l.EndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, h2, int.Parse(end.Substring(3, 2)), 00);
            Config.BookingSystem.Lessons.Update(origname, l);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RemoveLesson", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int RemoveLesson(string name)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Lessons.Remove(name);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AddSubject", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int AddSubject(string subject)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Subjects.Add(subject);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RemoveSubject", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int RemoveSubject(string subject)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Subjects.Delete(subject);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateSubject", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int UpdateSubject(string origsubject, string subject)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Subjects.Update(origsubject, subject);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateLinkGroup", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int UpdateLinkGroup(string origname, string name, string showto, string subtitle, string hidehomepage, string hidetopmenu, string hidehomepagelink)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            LinkGroup g = Config.Homepage.Groups[origname];
            g.ShowTo = showto;
            g.Name = name;
            g.SubTitle = subtitle;
            g.HideHomePage = bool.Parse(hidehomepage);
            g.HideTopMenu = bool.Parse(hidetopmenu);
            g.HideHomePageLink = bool.Parse(hidehomepagelink);
            Config.Homepage.Groups.UpdateGroup(origname, g);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AddLinkGroup", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int AddLinkGroup(string name, string showto, string subtitle, string hidehomepage, string hidetopmenu, string hidehomepagelink)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups.Add(name, showto, subtitle, bool.Parse(hidehomepage), bool.Parse(hidetopmenu), bool.Parse(hidehomepagelink));
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RemoveLinkGroup", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int RemoveLinkGroup(string name)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups.Remove(name);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateLinkGroupOrder", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int UpdateLinkGroupOrder(string groups)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups.ReOrder(groups.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateLink", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int UpdateLink(string group, string origname, string name, string desc, string icon, string url, string target, string showto)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Link l = Config.Homepage.Groups[group].Single(a => a.Name == origname);
            l.ShowTo = showto;
            l.Description = desc;
            l.Url = url;
            l.Target = target;
            l.Icon = icon;
            l.Name = name;
            Config.Homepage.Groups[group].UpdateLink(origname, l);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AddLink", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int AddLink(string group, string name, string desc, string icon, string url, string target, string showto)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups[group].Add(name, showto, desc, url, icon, target);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RemoveLink", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int RemoveLink(string group, string name)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups[group].Remove(name);
            return 0;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateLinkOrder", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public int UpdateLinkOrder(string group, string links)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups[group.Remove(0, 9).Replace('_', ' ')].ReOrder(links.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            return 0;
        }

        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetADTree", BodyStyle=WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public ADOU GetADTree(string username, string password, string domain)
        {
            try
            {
                if (username.Length < 2 && password.Length < 2 && domain.Length < 2) throw new Exception("Invailid Domain/Credentials");
                HAP.AD.User _user = new AD.User();
                _user.Authenticate(username, password, domain);
                try
                {
                    _user.ImpersonateContained();
                    PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, username, password);
                    DirectoryEntry root = new DirectoryEntry("LDAP://DC=" + domain.Replace(".", ",DC="));
                    return FillNode(root);
                }
                finally { _user.EndContainedImpersonate(); }
            }
            catch (Exception ex) { HAP.Web.Logging.EventViewer.Log("Setup API", ex.ToString() + "\nMessage:\n" + ex.Message + "\nStack Trace:\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error); return null; }
        }

        private ADOU FillNode(DirectoryEntry root)
        {
            try
            {
                ADOU adou = new ADOU();
                adou.Name = root.Name.Remove(0, 3);
                adou.Icon = (root.Name.StartsWith("DC=") ? "1" : root.SchemaClassName == "group" ? "78" : "2") + ".png";
                if (!root.Name.StartsWith("DC="))
                {
                    adou.Path = (root.SchemaClassName == "organizationalUnit" ? root.Path : root.Name.Remove(0, 3));
                    adou.Type = root.SchemaClassName;
                }

                foreach (DirectoryEntry de in root.Children) if ((de.SchemaClassName == "group" || de.SchemaClassName == "container" || de.SchemaClassName == "builtinDomain" || de.SchemaClassName == "organizationalUnit") && (de.Name != "CN=Program Data" && de.Name != "CN=System" && de.Name != "CN=Computers" && de.Name != "CN=Managed Service Accounts" && de.Name != "CN=ForeignSecurityPrincipals"))
                    {
                        ADOU a = FillNode(de);
                        if (a != null) adou.Items.Add(a);
                    }
                return adou;
            }
            catch (Exception ex) { HAP.Web.Logging.EventViewer.Log("Setup API -> FillNode(DirectoryEntry room)", ex.ToString() + "\nMessage:\n" + ex.Message + "\nStack Trace:\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error); return null; }
        }
    }
}
