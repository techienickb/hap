using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using HAP.Web.Configuration;

namespace HAP.Web.Controls
{
    [DefaultProperty("StringPath")]
    [ToolboxData("<{0}:LocalResource StringPath=\"\" runat=\"server\" />")]
    public class LocalResource : Control
    {
        [Bindable(true)]
        [Category("Data")]
        public string StringPath { get; set; }
        [Bindable(true)]
        [Category("Data")]
        public string StringPath2 { get; set; }
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue(" ")]
        public string Seperator { get; set; }

        protected XmlDocument _doc
        {
            get
            {
                if (HttpContext.Current.Cache.Get("hapLocal") == null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~/App_LocalResources/" + hapConfig.Current.Local + "/Strings.xml"));
                    HttpContext.Current.Cache.Insert("hapLocal", doc, new System.Web.Caching.CacheDependency(HttpContext.Current.Server.MapPath("~/App_LocalResources/" + hapConfig.Current.Local + "/Strings.xml")));
                }
                return (XmlDocument)HttpContext.Current.Cache.Get("hapLocal");
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                writer.Write(_doc.SelectSingleNode("/hapStrings/" + StringPath.ToLower()).InnerText);
            }
            catch (Exception e) { throw new ArgumentOutOfRangeException("The string " + StringPath + " cannot be found", e); }
            if (!string.IsNullOrEmpty(StringPath2))
            {
                writer.Write(Seperator);
                try
                {
                    writer.Write(_doc.SelectSingleNode("/hapStrings/" + StringPath2.ToLower()).InnerText);
                }
                catch (Exception e) { throw new ArgumentOutOfRangeException("The string " + StringPath2 + " cannot be found", e); }
            }
        }
    }
}
