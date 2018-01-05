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

            RouteTable.Routes.Add(new Route("api/css", new JSHandler(JSType.CSS)));
            RouteTable.Routes.Add(new Route("api/js/start", new JSHandler(JSType.Start)));
            RouteTable.Routes.Add(new Route("api/js/beforehap", new JSHandler(JSType.BeforeHAP)));
            RouteTable.Routes.Add(new Route("api/js/hap", new JSHandler(JSType.HAP)));
            RouteTable.Routes.Add(new Route("api/js/afterhap", new JSHandler(JSType.AfterHAP)));
            RouteTable.Routes.Add(new Route("api/js/end", new JSHandler(JSType.End)));


            RouteTable.Routes.Add(new Route("api/mypic", new MyPicHandler()));
            RouteTable.Routes.Add(new Route("api/tiles/icons/{width}/{height}/{*path}", new TileIconHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/thumb/{*path}", new ThumbsHandler()));
            RouteTable.Routes.Add(new Route("api/mycomputer/{ext}.ico", new IconHandler()));
            RouteTable.Routes.Add(new Route("api/myfiles-upload/{drive}/{*path}", new MyFiles_UploadHandler()));
            RouteTable.Routes.Add(new Route("myfiles/directedit/{drive}/{*path}", new PageRouteHandler("~/myfiles/directedit.aspx")));
            RouteTable.Routes.Add(new Route("api/homework-upload/{teacher}/{name}/{start}/{end}/{drive}/{*path}", new Homework_UploadHandler()));
            RouteTable.Routes.Add(new Route("api/myfiles-permalink/{drive}/{*path}", new MyFiles_PermaLinkHandler()));
            RouteTable.Routes.Add(new Route("api/home-peramlink/{hash}", new Home_PermaLinkHandler()));
            WebServiceHostFactory factory = new WebServiceHostFactory();
            //RouteTable.Routes.Add(new ServiceRoute("api/livetiles", factory, typeof(HAP.Web.LiveTiles.API)));
            RouteTable.Routes.Add(new ServiceRoute("api/setup", factory, typeof(setup)));
            RouteTable.Routes.Add(new ServiceRoute("api/help", factory, typeof(Help)));
            RouteTable.Routes.Add(new ServiceRoute("api/announcement", factory, typeof(Announcement)));


            //load apis in the bin folder
            foreach (FileInfo assembly in new DirectoryInfo(HttpContext.Current.Server.MapPath("~/bin/")).GetFiles("*.dll").Where(fi => fi.Name != "HAP.Web.dll" && fi.Name != "HAP.Web.Configuration.dll" && !fi.Name.StartsWith("Microsoft")))
            {
                Assembly a = Assembly.LoadFrom(assembly.FullName);
                foreach (Type type in a.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(HAP.Web.Configuration.ServiceAPI), false).Length > 0)
                        RouteTable.Routes.Add(new ServiceRoute(((HAP.Web.Configuration.ServiceAPI)type.GetCustomAttributes(typeof(HAP.Web.Configuration.ServiceAPI), false)[0]).Name, factory, type));
                    if (type.GetCustomAttributes(typeof(HAP.Web.Configuration.HandlerAPI), false).Length > 0) {
                        var instance = (IRouteHandler)Activator.CreateInstance(type);
                        RouteTable.Routes.Add(new Route(((HAP.Web.Configuration.HandlerAPI)type.GetCustomAttributes(typeof(HAP.Web.Configuration.HandlerAPI), false)[0]).Name, instance));
                    }
                }
            }

            RouteTable.Routes.Add(new Route("api", new api()));

        }
    }
}