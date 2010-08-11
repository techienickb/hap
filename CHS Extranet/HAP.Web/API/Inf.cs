using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Web.Routing;

namespace HAP.Web.API
{
	public class InfHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new Inf();
		}
	}

	public class Inf : IHttpHandler
	{

		public bool IsReusable { get { return true; } }

		public void ProcessRequest(HttpContext context)
		{
			context.Response.Clear();
			context.Response.ExpiresAbsolute = DateTime.Now;
			context.Response.ContentType = "text/plain";
			context.Response.Write("Page Identity: " + context.User.Identity.Name + "\n");
			context.Response.Write("Windows Identity: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name + "\n");
			context.Response.Write("Thread Identity: " + System.Threading.Thread.CurrentPrincipal.Identity.Name + "\n");
	
		}
	}
}