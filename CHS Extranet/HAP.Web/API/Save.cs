using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.Security;
using System.Net;
using HAP.Web.routing;
using HAP.Web.Configuration;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Xml;
using System.IO;

namespace HAP.Web.API
{
    public class SaveHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new Save(path, drive);
        }
    }

    public class Save : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public Save(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }
        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ExpiresAbsolute = DateTime.Now;
            context.Response.ContentType = "text/plain";
            try
            {
                uncpath unc; string userhome;
                string path = Converter.DriveToUNC(RoutingPath, RoutingDrive, out unc, out userhome);
                StreamReader sr = new StreamReader(context.Request.InputStream);
                string c = sr.ReadToEnd();
                bool folder = c.Contains("\\");

                if (File.Exists(path))
                {
                    FileInfo file = new FileInfo(path);
                    bool extension = c.Contains(file.Extension);
                    string name = file.Name;
                    if (!extension) name = name.Replace(file.Extension, "");
                    if (c.StartsWith("SAVETO:"))
                    {
                        c = c.Remove(0, 7);
                        string p2 = path.Replace(name, c);
                        if (folder) p2 = Converter.DriveToUNC(c);
                        FileInfo f2 = new FileInfo(p2);
                        if (f2.Exists)
                        {
                            context.Response.Write("EXISTS\n");
                            context.Response.Write(f2.Name.Replace(f2.Extension, "") + "\n");
                            context.Response.Write(Converter.UNCtoDrive(file.FullName, unc, userhome) + "\n");
                            context.Response.Write(List.parseLength(file.Length) + "\n");
                            context.Response.Write(file.LastWriteTime.ToShortDateString() + " " + file.LastWriteTime.ToShortTimeString() + "\n");
                            context.Response.Write(Converter.UNCtoDrive(f2.FullName, unc, userhome) + "\n");
                            context.Response.Write(List.parseLength(f2.Length) + "\n");
                            context.Response.Write(f2.LastWriteTime.ToShortDateString() + " " + f2.LastWriteTime.ToShortTimeString());
                            return;
                        }
                    }
                    else
                    {
                        c = c.Remove(0, 10);
                        string p2 = path.Replace(name, c);
                        if (folder) p2 = p2.Replace(file.Directory.Name + "\\", "");
                        File.Delete(p2);
                    }
                    if (folder) file.MoveTo(Converter.DriveToUNC(c) + (extension ? "" : file.Extension));
                    else file.MoveTo(file.FullName.Replace(name, c) + (extension ? "" : file.Extension));
                }
                else
                {
                    DirectoryInfo file = new DirectoryInfo(path);

                    if (c.StartsWith("SAVETO:"))
                    {
                        c = c.Remove(0, 7);
                        string p2 = path.Replace(file.Name, c);
                        if (folder) p2 = Converter.DriveToUNC(p2);
                        DirectoryInfo f2 = new DirectoryInfo(p2);
                        if (f2.Exists)
                        {
                            context.Response.Write("EXISTS\n");
                            context.Response.Write(f2.Name + "\n");
                            context.Response.Write(Converter.UNCtoDrive(file.FullName, unc, userhome) + "\n");
                            context.Response.Write(" \n");
                            context.Response.Write(file.LastWriteTime.ToShortDateString() + " " + file.LastWriteTime.ToShortTimeString() + "\n");
                            context.Response.Write(Converter.UNCtoDrive(f2.FullName, unc, userhome) + "\n");
                            context.Response.Write(" \n");
                            context.Response.Write(f2.LastWriteTime.ToShortDateString() + " " + f2.LastWriteTime.ToShortTimeString());
                            return;
                        }
                    }
                    else
                    {
                        c = c.Remove(0, 10);
                        string p2 = path.Replace(file.Name, c);
                        if (folder) p2 = p2.Replace(file.Parent.Name + "\\", "");
                        Directory.Delete(p2);
                    }
                    if (folder) file.MoveTo(Converter.DriveToUNC(c));
                    else file.MoveTo(file.FullName.Replace(file.Name, c));
                }

                context.Response.Write("DONE");
            }
            catch (Exception e)
            {
                context.Response.Write("ERROR: " + e.ToString() + "\\n" + e.Message);
            }
        }
    }
}