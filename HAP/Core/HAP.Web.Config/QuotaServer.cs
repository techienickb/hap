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
            FSRM = node.Attributes["fsrm"] == null ? false : bool.Parse(node.Attributes["fsrm"].Value);
            DFSTarget = node.Attributes["dfstarget"] == null ? "" : node.Attributes["dfstarget"].Value;
        }

        public string Expression { get; set; }
        public string Server { get; set; }
        public char Drive { get; set; }
        public bool FSRM { get; set; }
        public string DFSTarget { get; set; }
    }
}
