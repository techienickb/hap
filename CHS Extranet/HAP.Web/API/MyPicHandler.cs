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
using System.DirectoryServices;
using System.Drawing.Drawing2D;

namespace HAP.Web.API
{
    public class MyPicHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MyPic();
        }
    }

    public class MyPic : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                HAP.AD.User _user = new HAP.AD.User();
                _user.Authenticate(hapConfig.Current.AD.User, hapConfig.Current.AD.Password);
                string errorlist = "";
                try
                {
                    _user.ImpersonateContained();
                    using (DirectorySearcher dsSearcher = new DirectorySearcher())
                    {
                        errorlist += "Creating Directory Search and Searching for then current user\n";
                        dsSearcher.Filter = "(&(objectClass=user) (sAMAccountName=" + ((HAP.AD.User)Membership.GetUser()).UserName + "))";
                        errorlist += "Using filter: " + dsSearcher.Filter + "\n";
                        dsSearcher.PropertiesToLoad.Add("thumbnailPhoto");
                        SearchResultCollection results = dsSearcher.FindAll();

                        errorlist += "Found " + results.Count + " results, processing 1st result\n";
                        if (results.Count > 0)
                        {
                            errorlist += "Found " + results[0].Properties["thumbnailPhoto"].Count + " thumnbnailPhotos\n";
                            if (results[0].Properties["thumbnailPhoto"].Count > 0)
                            {
                                byte[] data = results[0].Properties["thumbnailPhoto"][0] as byte[];
                                if (data != null)
                                {
                                    errorlist += "Data found, making picture\n";
                                    using (MemoryStream s = new MemoryStream(data))
                                    {
                                        context.Response.ContentType = "image/png";
                                        MemoryStream m = new MemoryStream();
                                        Image i = Bitmap.FromStream(s);
                                        FixedSize(i, 92, 92).Save(m, ImageFormat.Png);
                                        m.WriteTo(context.Response.OutputStream);
                                    }
                                }
                                //else context.Response.Redirect("~/api/tiles/icons/92/92/images/icons/metro/folders-os/UserNo-Frame.png");
                            }
                            //else context.Response.Redirect("~/api/tiles/icons/92/92/images/icons/metro/folders-os/UserNo-Frame.png");
                        }
                        //else context.Response.Redirect("~/api/tiles/icons/92/92/images/icons/metro/folders-os/UserNo-Frame.png");
                    }
                    throw new Exception();
                }
                catch (Exception e)
                {
                    throw new Exception(errorlist, e);
                    //context.Response.Redirect("~/api/tiles/icons/128/128/images/icons/metro/folders-os/UserNo-Frame.png");
                }
                finally {  }
            }
            else context.Response.Redirect("~/api/tiles/icons/92/92/images/icons/metro/folders-os/UserNo-Frame.png");
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

    }
}