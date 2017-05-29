using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public interface IConfig
    {
        string Name { get; }
        void Save(XmlElement xmlElement, ref XmlDocument doc);
        void Load(XmlElement xmlElement);
        void Init(XmlElement xmlElement, ref XmlDocument doc);
    }
}
