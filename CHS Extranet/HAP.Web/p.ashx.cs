using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using CHS.Base64;

namespace HAP.Web
{
    /// <summary>
    /// Summary description for p
    /// </summary>
    public class p : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/jpg";
            string photo = Pupils.getPhoto(context.Request.QueryString["upn"]);
            if (string.IsNullOrEmpty(photo))
            {
                Bitmap nopic = new Bitmap(142, 183);
                Graphics gfx = Graphics.FromImage(nopic);
                gfx.Clear(Color.White);
                gfx.DrawString("Picture Not Found", SystemFonts.MessageBoxFont, new SolidBrush(Color.DarkRed), 35, 20);
                gfx.DrawString("The student does not have a picture", SystemFonts.MessageBoxFont, SystemBrushes.WindowText, new RectangleF(5, 60, 137, 40));
                MemoryStream mem = new MemoryStream();
                nopic.Save(mem, ImageFormat.Jpeg);
                mem.WriteTo(context.Response.OutputStream);
            }
            else
            {
                Byte[] b = Base64Encoder.FromBase64(photo);
                context.Response.OutputStream.Write(b, 0, b.Length);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}