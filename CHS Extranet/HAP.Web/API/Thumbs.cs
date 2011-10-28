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

    public class Thumbs : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return true; } }

        public Thumbs(string path, string drive)
        {
            RoutingPath = path;
            RoutingDrive = drive;
        }
        public string RoutingPath { get; set; }
        public string RoutingDrive { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            HAP.AD.User u = Membership.GetUser() as HAP.AD.User;
            u.ImpersonateContained();
            try
            {
                Context = context;
                config = hapConfig.Current;
                DriveMapping unc;
                string path = Converter.DriveToUNC(RoutingPath.Replace('^', '&'), RoutingDrive, out unc, ((HAP.AD.User)Membership.GetUser()));
                FileInfo file = new FileInfo(path);
                FileStream fs = file.OpenRead();
                Image image = Image.FromStream(fs);
                Image thumb = FixedSize(image, 64, 64);
                image.Dispose();
                fs.Close();
                fs.Dispose();

                MemoryStream memstr = new MemoryStream();
                thumb.Save(memstr, ImageFormat.Png);
                context.Response.Clear();
                context.Response.ExpiresAbsolute = DateTime.Now;
                context.Response.ContentType = Converter.MimeType(".png");
                context.Response.Buffer = true;
                context.Response.AppendHeader("Content-Disposition", "inline; filename=\"" + file.Name + "\"");
                context.Response.AddHeader("Content-Length", memstr.Length.ToString());
                context.Response.Clear();
                memstr.WriteTo(context.Response.OutputStream);
                context.Response.Flush();
                file = null;
            }
            finally
            {
                u.EndContainedImpersonate();
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

        private hapConfig config;
        private HttpContext Context;
    }
}