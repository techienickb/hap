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
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using HAP.Data.ComputerBrowser;
using System.Drawing.Drawing2D;
using System.Web.SessionState;
using HAP.AD;
using HAP.MyFiles;

namespace HAP.Web.API
{
    public class ThumbsHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string path = requestContext.RouteData.Values["path"] as string;
            string drive = path.Substring(0, 1);
            path = path.Remove(0, 1);
            return new Thumbs(path, drive);
        }
    }

    public class Thumbs : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        public Thumbs(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }
        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            hapConfig config = hapConfig.Current;
            User user = new User();
            if (config.AD.AuthenticationMode == Web.Configuration.AuthMode.Forms)
            {
                HttpCookie token = HttpContext.Current.Request.Cookies["token"];
                if (token == null) throw new AccessViolationException("Token Cookie Missing, user not logged in correctly");
                user.Authenticate(HttpContext.Current.User.Identity.Name, TokenGenerator.ConvertToPlain(token.Value));
            }
            user.ImpersonateContained();
            try
            {
                Context = context;
                config = hapConfig.Current;
                DriveMapping unc;
                string path = Converter.DriveToUNC(RoutingPath.Replace('^', '&'), RoutingDrive, out unc, ((HAP.AD.User)Membership.GetUser()));
                FileInfo file = new FileInfo(path);
                Image thumb;
                try
                {
                    Bitmap b = new ShellThumbnail().GetThumbnail(file.FullName);
                    thumb = b;// FixedSize(b, 64, 64);
                    if (thumb == null) throw new NullReferenceException();
                }
                catch
                {
                    FileStream fs = file.OpenRead();
                    Image image = Image.FromStream(fs);
                    thumb = FixedSize(image, 64, 64);
                    image.Dispose();
                    fs.Close();
                    fs.Dispose();
                }
                MemoryStream m = new MemoryStream();
                thumb.Save(m, ImageFormat.Png);

                context.Response.Clear();
                context.Response.ExpiresAbsolute = DateTime.Now;
                context.Response.ContentType = Converter.MimeType(".png");
                context.Response.Buffer = true;
                context.Response.AppendHeader("Content-Disposition", "inline; filename=\"" + file.Name + "\"");
                context.Response.AppendHeader("Content-Length", m.Length.ToString());
                context.Response.Clear();
                m.WriteTo(context.Response.OutputStream);
                context.Response.Flush();
                file = null;
                user.EndContainedImpersonate();
            }
            catch
            {
                DriveMapping unc;
                string path = Converter.DriveToUNC(RoutingPath.Replace('^', '&'), RoutingDrive, out unc, ((HAP.AD.User)Membership.GetUser()));
                FileInfo file = new FileInfo(path);
                string Icon = HAP.MyFiles.File.ParseForImage(file);
                user.EndContainedImpersonate();
                if (Icon.EndsWith(".ico")) context.Response.Redirect(VirtualPathUtility.ToAbsolute("~/api/mycomputer/" + Icon));
                else context.Response.Redirect(VirtualPathUtility.ToAbsolute("~/images/icons/" + Icon));
            }
        }

        private Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        private HttpContext Context;
    }
}