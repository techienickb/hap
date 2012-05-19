using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Web;
using System.Xml;

namespace HAP.Web.LiveTiles
{
    public class IconCache
    {
        private static Dictionary<string, string> ColourCache
        {
            get
            {
                if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/app_data/iconcache/"))) Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/app_data/iconcache/"));
                if (!File.Exists(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml"))) Save(new Dictionary<string, string>());
                if (HttpContext.Current.Cache["hap-colorcache"] == null)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml"));
                    foreach (XmlNode n in doc.SelectNodes("/colorcache/color"))
                        try
                        {
                            d.Add(n.Attributes["icon"].Value, n.InnerText);
                        }
                        catch (Exception e) { throw new Exception(n.Attributes["icon"].Value, e); }
                    HttpContext.Current.Cache.Insert("hap-colorcache", d);
                }
                return HttpContext.Current.Cache["hap-colorcache"] as Dictionary<string, string>;
            }
        }

        private static void Save(Dictionary<string, string> d)
        {
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml")))
            {
                StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml"));
                sw.WriteLine("<?xml version=\"1.0\"?>");
                sw.WriteLine("<colorcache/>");
                sw.Close();
                sw.Dispose();
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml"));
            doc.SelectSingleNode("/colorcache").RemoveAll();
            foreach (string s in d.Keys)
            {
                XmlElement e = doc.CreateElement("color");
                e.SetAttribute("icon", s);
                e.InnerText = d[s];
                doc.SelectSingleNode("/colorcache").AppendChild(e);
            }
            doc.Save(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml"));
            HttpContext.Current.Cache.Remove("hap-colorcache");
        }

        private static void Save(string key, string value)
        {
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml")))
            {
                StreamWriter sw = File.CreateText(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml"));
                sw.WriteLine("<?xml version=\"1.0\"?>");
                sw.WriteLine("<colorcache/>");
                sw.Close();
                sw.Dispose();
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml"));
            XmlElement e = doc.CreateElement("color");
            e.SetAttribute("icon", key);
            e.InnerText = value;
            doc.SelectSingleNode("/colorcache").AppendChild(e);
            doc.Save(HttpContext.Current.Server.MapPath("~/app_data/iconcache/colors.xml"));
            HttpContext.Current.Cache.Remove("hap-colorcache");
        }

        public static string GetColour(string icon)
        {
            if (ColourCache.ContainsKey(icon.ToLower()))
            {
                Color c = System.Drawing.ColorTranslator.FromHtml(ColourCache[icon.ToLower()]);
                return (c.A.ToString() == "6" || c.A.ToString() == "0" ? "\"\"" : (" { Base: '" + System.Drawing.ColorTranslator.ToHtml(c) + "', Light: '" + System.Drawing.ColorTranslator.ToHtml(Lighten(c, 0.1)) + "', Dark: '" + System.Drawing.ColorTranslator.ToHtml(Darken(c, 0.1)) + "' }"));
            }
            Bitmap b;
            try
            {
                b = new Bitmap(HttpContext.Current.Server.MapPath(icon));
            }
            catch (Exception e) { throw new Exception(icon, e); }
            Save(icon.ToLower(), (b.GetPixel(1, 1).A.ToString() == "6" || b.GetPixel(1, 1).A.ToString() == "0" ? "" : System.Drawing.ColorTranslator.ToHtml(b.GetPixel(1, 1))));
            return (b.GetPixel(1, 1).A.ToString() == "6" || b.GetPixel(1, 1).A.ToString() == "0" ? "\"\"" : (" { Base: '" + System.Drawing.ColorTranslator.ToHtml(b.GetPixel(1, 1)) + "', Light: '" + System.Drawing.ColorTranslator.ToHtml(Lighten(b.GetPixel(1, 1), 0.1)) + "', Dark: '" + System.Drawing.ColorTranslator.ToHtml(Darken(b.GetPixel(1, 1), 0.1)) + "' }"));
        }

        public static Color Lighten(Color inColor, double inAmount)
        {
          return Color.FromArgb(
            inColor.A,
            (int) Math.Min(255, inColor.R + 255 * inAmount),
            (int) Math.Min(255, inColor.G + 255 * inAmount),
            (int) Math.Min(255, inColor.B + 255 * inAmount) );
        }

        public static Color Darken(Color inColor, double inAmount)
        {
          return Color.FromArgb(
            inColor.A,
            (int) Math.Max(0, inColor.R - 255 * inAmount),
            (int) Math.Max(0, inColor.G - 255 * inAmount),
            (int) Math.Max(0, inColor.B - 255 * inAmount) );
        }

        public static string GetIcon(string icon, Size size)
        {
            string name = icon.Remove(icon.LastIndexOf('.')).Remove(0, icon.LastIndexOf("/"));
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/app_data/iconcache/" + name + "-" + size.Width + "x" + size.Height + ".png")))
            {
                Bitmap b = new Bitmap(HttpContext.Current.Server.MapPath(icon));
                if (b.GetPixel(1, 1) != Color.Transparent) b.MakeTransparent(b.GetPixel(1, 1));
                resizeImage(b, size).Save(HttpContext.Current.Server.MapPath("~/app_data/iconcache/" + name + "-" + size.Width + "x" + size.Height + ".png"), System.Drawing.Imaging.ImageFormat.Png);
            }
            return HttpContext.Current.Server.MapPath("~/app_data/iconcache/" + name + "-" + size.Width + "x" + size.Height + ".png");
        }

        private static Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }
    }
}
