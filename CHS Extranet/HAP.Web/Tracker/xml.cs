using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Threading;
using System.Web.Security;
using HAP.Web.Configuration;
using System.Management;
using System.Net;
using System.IO;

namespace HAP.Web.Tracker
{
    public class xml
    {
        public static void Clear(string Computer, string DomainName)
        {
            XmlDocument doc = Doc;
            try
            {
                foreach (XmlNode node in doc.SelectNodes(string.Format("/Tracker/Event[@logoffdatetime='' and @computername='{0}' and @domainname='{1}']", Computer, DomainName)))
                    node.Attributes["logoffdatetime"].Value = DateTime.Now.ToString("s");
            }
            catch { }
            Save(doc);
        }

        public static trackerlogentry[] Logon(string Username, string Computer, string DomainName, string IP, string LogonServer, string OS)
        {
            List<trackerlogentry> ll = new List<trackerlogentry>();
            XmlDocument doc = Doc;
            XmlElement e = doc.CreateElement("Event");
            e.SetAttribute("logondatetime", DateTime.Now.ToString("s"));
            e.SetAttribute("logoffdatetime", "");
            e.SetAttribute("computername", Computer);
            e.SetAttribute("domainname", DomainName);
            e.SetAttribute("username", Username);
            e.SetAttribute("ip", IP);
            e.SetAttribute("os", OS);
            e.SetAttribute("logonserver", LogonServer);
            hapConfig hap = hapConfig.Current;
            foreach (XmlNode node in doc.SelectNodes(string.Format("/Tracker/Event[@logoffdatetime='' and @username='{0}' and @domainname='{1}']", Username, DomainName)))
                ll.Add(new trackerlogentry(node));
            doc.SelectSingleNode("/Tracker").AppendChild(e);
            Save(doc);
            return ll.ToArray();
        }

        public static void RemoteLogoff(string Computer, string DomainName)
        {
            XmlDocument doc = Doc;
            try
            {
                ConnectionOptions connoptions = new ConnectionOptions();
                connoptions.Username = hapConfig.Current.ADSettings.ADUsername;
                connoptions.Password = hapConfig.Current.ADSettings.ADPassword;
                ManagementScope scope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", Computer), connoptions);
                scope.Connect();
                ObjectQuery oq = new ObjectQuery("Select Name From Win32_OperatingSystem");
                ManagementObjectSearcher q = new ManagementObjectSearcher(scope, oq);
                foreach (ManagementObject o in q.Get())
                    o.InvokeMethod("Win32Shutdown", new object[] { 4 });
            }
            catch { }
            foreach (XmlNode node in doc.SelectNodes(string.Format("/Tracker/Event[@logoffdatetime='' and @computername='{0}' and @domainname='{1}']", Computer, DomainName)))
                node.Attributes["logoffdatetime"].Value = DateTime.Now.ToString("s");
            Save(doc);
        }

        public static trackerlogentry[] GetLogs(bool loadall)
        {
            List<trackerlogentry> tle = new List<trackerlogentry>();
            if (loadall)
            {
                foreach (FileInfo file in new DirectoryInfo(HttpContext.Current.Server.MapPath("~/App_Data")).GetFiles("tracker*xml"))
                {
                    XmlDocument doc = new XmlDocument();
                    bool loaded = false;
                    while (!loaded)
                    {
                        try
                        {
                            doc.Load(file.FullName);
                            loaded = true;
                        }
                        catch { Thread.Sleep(100); }
                    }
                    foreach (XmlNode node in doc.SelectNodes("/Tracker/Event"))
                        tle.Add(new trackerlogentry(node));
                }
            }
            else
            {
                XmlDocument doc = Doc;
                foreach (XmlNode node in doc.SelectNodes("/Tracker/Event"))
                    tle.Add(new trackerlogentry(node));
            }
            return tle.ToArray();
        }

        static XmlDocument Doc
        {
            get
            {
                bool loaded = false;
                XmlDocument doc = new XmlDocument();
                while (!loaded)
                {
                    try
                    {
                        doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/tracker.xml"));
                        loaded = true;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
                return doc;
            }
        }

        static void Save(XmlDocument doc)
        {
            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            set.IndentChars = "   ";
            set.Encoding = System.Text.Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(HttpContext.Current.Server.MapPath("~/App_Data/Tracker.xml"), set);
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
    }
}