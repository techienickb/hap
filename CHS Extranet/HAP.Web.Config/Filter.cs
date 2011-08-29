using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class Filter
    {
        private XmlNode node;
        public Filter(XmlNode node)
        {
            this.node = node;
            Name = node.Attributes["name"].Value;
            Expression = node.Attributes["expression"].Value;
            EnableFor = node.Attributes["enablefor"].Value;
        }

        public string Name { get; set; }
        public string Expression { get; set; }
        public string EnableFor { get; set; }
    }
}
