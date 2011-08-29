using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class Filters : List<Filter>
    {
        private XmlDocument doc;
        private XmlNode node;
        public Filters(ref XmlDocument doc) : base()
        {
            this.doc = doc;
            this.node = doc.SelectSingleNode("/hapConfig/mscb/filters");
            foreach (XmlNode n in node.ChildNodes) base.Add(new Filter(n));
        }
        public void Add(string Name, string Expression, string EnableFor)
        {
            XmlElement e = doc.CreateElement("filter");
            e.SetAttribute("name", Name);
            e.SetAttribute("expression", Expression);
            e.SetAttribute("enablefor", EnableFor);
            doc.SelectSingleNode("/hapConfig/mscb/filters").AppendChild(e);
            base.Add(new Filter(e));
        }
        public Filter Find(string name, string expression)
        {
            return this.Single(f => f.Name == name && f.Expression == expression);
        }
        public void Delete(string name, string expression)
        {
            base.Remove(Find(name, expression));
            XmlNode n = null;
            foreach (XmlNode n1 in doc.SelectNodes("/hapConfig/mscb/filters/filter"))
                if (n1.Attributes["name"].Value == name && n1.Attributes["expression"].Value == expression) { n = n1; break; }
            doc.SelectSingleNode("/hapConfig/mscb/filters").RemoveChild(n);
        }
        public void Update(string name, string expression, Filter New)
        {
            int index = IndexOf(Find(name, expression));
            base.RemoveAt(index);
            XmlNode n = null;
            foreach (XmlNode n1 in doc.SelectNodes("/hapConfig/mscb/filters/filter"))
                if (n1.Attributes["name"].Value == name && n1.Attributes["expression"].Value == expression) { n = n1; break; }
            n.Attributes["name"].Value = New.Name;
            n.Attributes["expression"].Value = New.Expression;
            n.Attributes["enablefor"].Value = New.EnableFor;
            base.Insert(index, new Filter(n));
        }
    }
}
