using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Web.Configuration
{
    public class ServiceAPI : Attribute
    {
        public string Name { get; set; }

        public ServiceAPI(string name)
        {
            this.Name = name;
        }
    }

    public class HandlerAPI : Attribute
    {
        public string Name { get; set; }

        public HandlerAPI(string name)
        {
            this.Name = name;
        }
    }
}
