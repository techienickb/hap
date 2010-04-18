using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.Web.routing
{
    public interface IMyComputerDisplay : IHttpHandler
    {
        string RoutingPath { get; set; }
        string RoutingDrive { get; set; }
    }
}