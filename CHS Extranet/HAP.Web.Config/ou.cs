﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HAP.Web.Configuration
{
    public class ou
    {
        public ou(XmlNode node)
        {
            Name = node.Attributes["name"].Value;
            Path = node.Attributes["path"].Value;
            Ignore = node.Attributes["Ignore"] == null ? false : bool.Parse(node.Attributes["Ignore"].Value);
        }

        public string Name { get; private set; }
        public string Path { get; private set; }
        public bool Ignore { get; private set; }
    }
}
