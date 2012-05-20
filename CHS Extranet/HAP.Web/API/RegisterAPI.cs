using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace HAP.Web.API
{
    public class APIRoutes
    {
        public static void Register(RouteCollection collection)
        {
            RouteTable.Routes.Add(new Route("api/test", new TestHandler()));
            RouteTable.Routes.Add(new Route("api/js", new JSHandler()));
            RouteTable.Routes.Add(new Route("api/tiles/icons/{width}/{height}/{*path}", new TileIconHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/thumb/{*path}", new ThumbsHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/{ext}.ico", new IconHandler()));
            RouteTable.Routes.Add(new Route("api/myfiles-upload/{drive}/{*path}", new MyFiles_UploadHandler()));
            RouteTable.Routes.Add(new Route("api/myfiles-permalink/{drive}/{*path}", new MyFiles_PermaLinkHandler()));
            WebServiceHostFactory factory = new WebServiceHostFactory();
            //RouteTable.Routes.Add(new ServiceRoute("api/livetiles", factory, typeof(HAP.Web.LiveTiles.API)));
            RouteTable.Routes.Add(new ServiceRoute("api/setup", factory, typeof(setup)));
            RouteTable.Routes.Add(new ServiceRoute("api/help", factory, typeof(Help)));


            //load apis in the bin folder
            foreach (FileInfo assembly in new DirectoryInfo(HttpContext.Current.Server.MapPath("~/bin/")).GetFiles("*.dll").Where(fi => fi.Name != "HAP.Web.dll" && fi.Name != "HAP.Web.Configuration.dll"))
            {
                Assembly a = Assembly.LoadFrom(assembly.FullName);
                foreach (Type type in a.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(HAP.Web.Configuration.ServiceAPI), false).Length > 0)
                        RouteTable.Routes.Add(new ServiceRoute(((HAP.Web.Configuration.ServiceAPI)type.GetCustomAttributes(typeof(HAP.Web.Configuration.ServiceAPI), false)[0]).Name, factory, type));
                }
            }

            RouteTable.Routes.Add(new Route("api", new api()));

        }
    }
}