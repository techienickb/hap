using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Xml;
using HAP.Web.Configuration;

namespace HAP.Web.Controls
{
    [ToolboxData("<{0}:WrappedLocalResource runat=\"server\"> </{0}:WrappedLocalResource>")]
    public class WrappedLocalResource : System.Web.UI.HtmlControls.HtmlContainerControl
    {
        public override Control NamingContainer { get { return null; } }
        public override string UniqueID { get { return ""; } }

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

        [DefaultValue("div")]
        public string Tag
        {
            get;
            set;
        }


        public override string TagName
        {
            get { return Tag; }
        }

        protected override void RenderAttributes(HtmlTextWriter writer)
        {
            foreach (string s in this.Attributes.Keys) 
                if (this.Attributes[s].StartsWith("#"))
                    try
                    {
                        this.Attributes[s] = _doc.SelectSingleNode("/hapStrings/" + this.Attributes[s].Remove(0, 1).ToLower()).InnerText;
                    }
                    catch (Exception e) { throw new IndexOutOfRangeException("The String '" + this.Attributes[s].Remove(0, 1).ToLower() + "' cannot be found", e); }
            base.RenderAttributes(writer);
        }
    }
}