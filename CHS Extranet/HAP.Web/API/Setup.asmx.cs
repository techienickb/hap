using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections.ObjectModel;
using HAP.Web.Configuration;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI;
using System.IO;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace HAP.Web.API
{
    /// <summary>
    /// Summary description for Sortable
    /// </summary>
    [WebService(Namespace = "http://hap.codeplex.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Setup : System.Web.Services.WebService
    {
        [WebMethod]
        public int AddMapping(string drive, string name, string unc, string enablereadto, string enablewriteto, bool enablemove, string usagemode)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            unc = unc.Replace('/', '\\');
            Config.MySchoolComputerBrowser.Mappings.Add(drive.ToCharArray()[0], name, unc, enablereadto, enablewriteto, enablemove, (MappingUsageMode)Enum.Parse(typeof(MappingUsageMode), usagemode));
            return 0;
        }

        [WebMethod]
        public int RemoveMapping(string drive)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MySchoolComputerBrowser.Mappings.Remove(drive.ToCharArray()[0]);
            return 0;
        }

        [WebMethod]
        public int UpdateMapping(string origdrive, string drive, string name, string unc, string enablereadto, string enablewriteto, bool enablemove, string usagemode)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            DriveMapping m = Config.MySchoolComputerBrowser.Mappings[origdrive.ToCharArray()[0]];
            m.Drive = drive.ToCharArray()[0];
            m.EnableMove = enablemove;
            m.UsageMode = (MappingUsageMode)Enum.Parse(typeof(MappingUsageMode), usagemode);
            m.Name = name;
            m.UNC = unc;
            m.EnableReadTo = enablereadto;
            m.EnableWriteTo = enablewriteto;
            Config.MySchoolComputerBrowser.Mappings.Update(origdrive.ToCharArray()[0], m);
            return 0;
        }

        [WebMethod]
        public int AddFilter(string name, string expression, string enablefor)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MySchoolComputerBrowser.Filters.Add(name, expression, enablefor);
            return 0;
        }

        [WebMethod]
        public int RemoveFilter(string name, string expression)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MySchoolComputerBrowser.Filters.Delete(name, expression);
            return 0;
        }

        [WebMethod]
        public int UpdateFilter(string origname, string origexpression, string name, string expression, string enablefor)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Filter f = Config.MySchoolComputerBrowser.Filters.Find(origname, origexpression);
            f.Name = name;
            f.Expression = expression;
            f.EnableFor = enablefor;
            Config.MySchoolComputerBrowser.Filters.Update(origname, origexpression, f);
            return 0;
        }

        [WebMethod]
        public int AddQServer(string server, string expression, string drive)
        {
            expression = expression.Replace('/', '\\');
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MySchoolComputerBrowser.QuotaServers.Add(server, expression, drive.ToCharArray()[0]);
            return 0;
        }

        [WebMethod]
        public int RemoveQServer(string server, string expression)
        {
            expression = expression.Replace('/', '\\');
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.MySchoolComputerBrowser.QuotaServers.Delete(server, expression);
            return 0;
        }

        [WebMethod]
        public int UpdateQServer(string origserver, string origexpression, string server, string expression, string drive)
        {
            origexpression = origexpression.Replace('/', '\\');
            expression = expression.Replace('/', '\\');
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            QuotaServer q = Config.MySchoolComputerBrowser.QuotaServers.Find(origserver, origexpression);
            q.Server = server;
            q.Drive = drive.ToCharArray()[0];
            q.Expression = expression;
            Config.MySchoolComputerBrowser.QuotaServers.Update(origserver, origexpression, q);
            return 0;
        }

        [WebMethod]
        public int AddResource(string name, string type, bool enabled, bool charging, string admins, bool emailadmins)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Resources.Add(name, (ResourceType)Enum.Parse(typeof(ResourceType), type), admins, enabled, emailadmins, charging);
            return 0;
        }

        [WebMethod]
        public int UpdateResource(string origname, string name, string type, bool enabled, bool charging, string admins, bool emailadmins)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Resource r = Config.BookingSystem.Resources[origname];
            r.Name = name;
            r.Type = (ResourceType)Enum.Parse(typeof(ResourceType), type);
            r.Admins = admins;
            r.Enabled = enabled;
            r.EmailAdmins =emailadmins;
            r.EnableCharging = charging;
            Config.BookingSystem.Resources.Update(origname, r);
            return 0;
        }

        [WebMethod]
        public int RemoveResource(string name)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Resources.Delete(name);
            return 0;
        }

        [WebMethod]
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

        [WebMethod]
        public int EditLesson(string origname, string name, string type, string start, string end)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Lesson l = Config.BookingSystem.Lessons[origname];
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

        [WebMethod]
        public int RemoveLesson(string name)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Lessons.Remove(name);
            return 0;
        }

        [WebMethod]
        public int AddSubject(string subject)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Subjects.Add(subject);
            return 0;
        }

        [WebMethod]
        public int RemoveSubject(string subject)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Subjects.Delete(subject);
            return 0;
        }

        [WebMethod]
        public int UpdateSubject(string origsubject, string subject)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.BookingSystem.Subjects.Update(origsubject, subject);
            return 0;
        }

        [WebMethod]
        public int UpdateTabOrder(string tabs)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Tabs.ReOrder(tabs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            return 0;
        }

        [WebMethod]
        public int UpdateLinkGroup(string origname, string name, string showto)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            LinkGroup g = Config.Homepage.Groups[origname];
            g.ShowTo = showto;
            g.Name = name;
            Config.Homepage.Groups.UpdateGroup(origname, g);
            return 0;
        }

        [WebMethod]
        public int AddLinkGroup(string name, string showto)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups.Add(name, showto);
            return 0;
        }

        [WebMethod]
        public int RemoveLinkGroup(string name)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups.Remove(name);
            return 0;
        }

        [WebMethod]
        public int UpdateLinkGroupOrder(string groups)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups.ReOrder(groups.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            return 0;
        }

        [WebMethod]
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

        [WebMethod]
        public int AddLink(string group, string name, string desc, string icon, string url, string target, string showto)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups[group].Add(name, showto, desc, url, icon, target);
            return 0;
        }

        [WebMethod]
        public int RemoveLink(string group, string name)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups[group].Remove(name);
            return 0;
        }


        [WebMethod]
        public int UpdateLinkOrder(string group, string links)
        {
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            Config.Homepage.Groups[group.Remove(0, 9).Replace('_', ' ')].ReOrder(links.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            return 0;
        }

        [WebMethod]
        public int UpdateTab(string tab)
        {
            string[] t = tab.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            hapConfig Config = HttpContext.Current.Cache["tempConfig"] as hapConfig;
            TabType tt;
            if (Enum.TryParse<TabType>(t[0], out tt))
            {
                Tab Tab = Config.Homepage.Tabs[tt];
                Tab.Name = t[1];
                Tab.ShowTo = t[2];
                if (Tab.Type == TabType.Me)
                {
                    Tab.AllowUpdateTo = t[3];
                    Tab.ShowSpace = bool.Parse(t[4]);
                }
                Config.Homepage.Tabs.UpdateTab(tt, Tab);
                return 0;
            }
            else return 1;
        }

        [WebMethod]
        public ADOU GetADTree(string username, string password, string domain)
        {
            if (username.Length < 2 && password.Length < 2 && domain.Length < 2) throw new Exception("Invailid Domain/Credentials");
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain, username, password);
            DirectoryEntry root = new DirectoryEntry("LDAP://DC=" + domain.Replace(".", ",DC="));
            return FillNode(root);
        }

        private ADOU FillNode(DirectoryEntry root)
        {
            ADOU adou = new ADOU();
            adou.Name = root.Name.Remove(0, 3);
            adou.Icon = (root.Name.StartsWith("DC=") ? "1" : root.SchemaClassName == "group" ? "78" : "2") + ".png";
            if (!root.Name.StartsWith("DC="))
            {
                adou.Url = "javascript:selectad('" + (root.SchemaClassName == "organizationalUnit" ? root.Path : root.Name.Remove(0, 3)) + "', '" + root.SchemaClassName + "');";
            }
            foreach (DirectoryEntry de in root.Children) if ((de.SchemaClassName == "group" || de.SchemaClassName == "container" || de.SchemaClassName == "builtinDomain" || de.SchemaClassName == "organizationalUnit") && (de.Name != "CN=Program Data" && de.Name != "CN=System" && de.Name != "CN=Computers" && de.Name != "CN=Managed Service Accounts" && de.Name != "CN=ForeignSecurityPrincipals")) adou.Items.Add(FillNode(de));
            return adou;
        }
    }
}
