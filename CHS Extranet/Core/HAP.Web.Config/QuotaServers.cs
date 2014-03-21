using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.Web.Configuration
{
    public class QuotaServers : List<QuotaServer>
    {
        private XmlDocument doc;
        private XmlNode node;
        public QuotaServers(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/myfiles/quotaservers");
            foreach (XmlNode n in node.ChildNodes) base.Add(new QuotaServer(n));
        }
        public void Add(string Server, string Expression, char Drive)
        {
            XmlElement e = doc.CreateElement("quotaserver");
            e.SetAttribute("server", Server);
            e.SetAttribute("drive", Drive.ToString());
            e.InnerText = HttpContext.Current.Server.HtmlEncode(Expression);
            doc.SelectSingleNode("/hapConfig/myfiles/quotaservers").AppendChild(e);
            base.Add(new QuotaServer(e));
        }
        public QuotaServer Find(string Server, string Expression)
        {
            return this.Single(q => q.Server == Server && q.Expression == Expression);
        }
        public void Delete(string Server, string Expression)
        {
            base.Remove(Find(Server, Expression));
            XmlNode n = null;
            foreach (XmlNode n1 in node.ChildNodes) { if (n1.Attributes["server"].Value == Server && n1.InnerText == HttpContext.Current.Server.HtmlEncode(Expression)) n = n1; break; }
            node.RemoveChild(n);
        }
        public void Update(string server, string expression, QuotaServer New)
        {
            int i = IndexOf(Find(server, expression));
            base.RemoveAt(i);
            XmlNode n = null;
            foreach (XmlNode n1 in node.ChildNodes) { if (n1.Attributes["server"].Value == server && n1.InnerText == expression) n = n1; break; }
            XmlNode n2 = n.Clone();
            n2.Attributes["server"].Value = New.Server;
            n2.Attributes["drive"].Value = New.Drive.ToString();
            n2.InnerText = HttpContext.Current.Server.HtmlEncode(New.Expression);
            node.ReplaceChild(n2, n);
            base.Insert(i, new QuotaServer(n2));
        }
    }
}
