using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHS_Extranet.routing
{
    interface IMyComputerDisplay : IHttpHandler
    {
        string RoutingPath { get; set; }
        string RoutingDrive { get; set; }
    }
}