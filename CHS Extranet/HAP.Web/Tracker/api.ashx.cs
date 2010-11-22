using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using HAP.Web.Configuration;
using System.Management;
using System.DirectoryServices;
using System.Configuration;
using System.Security.Principal;
using System.Web.Security;
using System.Threading;

namespace HAP.Web.Tracker
{
    /// <summary>
    /// Summary description for api
    /// </summary>
    public class api : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string op = context.Request.QueryString["op"];
            context.Response.Clear();
            context.Response.ExpiresAbsolute = DateTime.Now;
            context.Response.ContentType = "text/plain";
            StreamReader sr = new StreamReader(context.Request.InputStream);
            string c = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            XmlDocument doc = new XmlDocument();
            doc.Load(context.Server.MapPath("~/App_Data/tracker.xml"));
            if (op == "clear")
            {
                try
                {
                    foreach (XmlNode node in doc.SelectNodes(string.Format("/Tracker/Event[@logoffdatetime='' and @computername='{0}']", c)))
                        node.Attributes["logoffdatetime"].Value = DateTime.Now.ToString("s");
                }
                catch { }
                context.Response.Write("Done");
            }
            else if (op == "remotelogoff")
            {
                try
                {
                    ConnectionOptions connoptions = new ConnectionOptions();
                    connoptions.Username = hapConfig.Current.ADSettings.ADUsername;
                    connoptions.Password = hapConfig.Current.ADSettings.ADPassword;
                    ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", c), connoptions);
                    scope.Connect();
                    ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
                    ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
                    foreach (ManagementObject o in q.Get())
                        o.InvokeMethod("Win32Shutdown", new object[] { 4 });
                }
                catch { }
                foreach (XmlNode node in doc.SelectNodes(string.Format("/Tracker/Event[@logoffdatetime='' and @computername='{0}']", c)))
                    node.Attributes["logoffdatetime"].Value = DateTime.Now.ToString("s");

                context.Response.Write("Done");
            }
            else
            {
                try
                {
                    XmlElement e = doc.CreateElement("Event");
                    e.SetAttribute("logondatetime", DateTime.Now.ToString("s"));
                    e.SetAttribute("logoffdatetime", "");
                    hapConfig hap = hapConfig.Current;
                    string username = "", domainname = "";
                    foreach (string s in c.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (s.StartsWith("username")) username = s.Split(new char[] { '|' })[1];
                        else if (s.StartsWith("domainname")) domainname = s.Split(new char[] { '|' })[1];
                        e.SetAttribute(s.Split(new char[] { '|' })[0], s.Split(new char[] { '|' })[1]);
                    }
                    List<string> resp = new List<string>();
                    foreach (XmlNode node in doc.SelectNodes(string.Format("/Tracker/Event[@logoffdatetime='' and @username='{0}' and @domainname='{1}']", username, domainname)))
                        resp.Add(string.Format("{0}|{1}", node.Attributes["computername"].Value, node.Attributes["logondatetime"].Value));
                    if (resp.Count > 0 && isStudent(username)) context.Response.Write("EXISTS\nStudent:" + hap.Tracker.MaxStudentLogons + "!" + hap.Tracker.OverrideCode + "\n" + string.Join("\n", resp.ToArray()));
                    else if (resp.Count > 0 && isAdmin(username)) context.Response.Write("EXISTS\nAdmin:0!" + hap.Tracker.OverrideCode + "\n" + string.Join("\n", resp.ToArray()));
                    else if (resp.Count > 0) context.Response.Write("EXISTS\nStaff:" + hap.Tracker.MaxStaffLogons + "!" + hap.Tracker.OverrideCode + "\n" + string.Join("\n", resp.ToArray()));
                    else context.Response.Write("Done");
                    doc.SelectSingleNode("/Tracker").AppendChild(e);
                }
                catch { context.Response.Write("Done"); }
            }
            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(context.Server.MapPath("~/App_Data/Tracker.xml"), set);
            bool saved = false;
            while (!saved)
            {
                try
                {
                    doc.Save(writer);
                    writer.Flush();
                    writer.Close();
                    saved = true;
                }
                catch
                {
                    Thread.Sleep(10);
                }
            }
        }

        private bool isAdmin(string username)
        {
            foreach (string s in Roles.GetRolesForUser(username))
                if (s == "Domain Admins") return true;
            return false;
        }

        private bool isStudent(string username)
        {
            return Roles.IsUserInRole(username, hapConfig.Current.ADSettings.StudentsGroupName);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}