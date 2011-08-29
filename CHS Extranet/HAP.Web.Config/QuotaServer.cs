using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace HAP.Web.Configuration
{
    public class QuotaServer
    {
        private XmlNode node;
        public QuotaServer(XmlNode node)
        {
            this.node = node;
            Server = node.Attributes["server"].Value;
            Expression = HttpContext.Current.Server.HtmlDecode(node.InnerText);
            Drive = node.Attributes["drive"].Value.ToCharArray()[0];
        }

        public string Expression { get; set; }
        public string Server { get; set; }
        public char Drive { get; set; }
    }
}
