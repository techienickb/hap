using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Xml;
using HAP.Web.Configuration;
using System.IO;

namespace HAP.Web.Controls
{
    [ToolboxData("<{0}:CompressJS runat=\"server\"> </{0}:CompressJS>")]
    public class CompressJS : System.Web.UI.HtmlControls.HtmlContainerControl
    {
        public override Control NamingContainer { get { return null; } }
        public override string UniqueID { get { return ""; } }

        [DefaultValue("div")]
        public string Tag
        {
            get;
            set;
        }

        protected override void Render(HtmlTextWriter writer)
        {
#if DEBUG
            base.Render(writer);
#else
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter w = new HtmlTextWriter(stringWriter);
            base.Render(w);
            w.Close();
            writer.Write(new JavaScriptMinifier().Minify(stringWriter.ToString())); 
#endif
        }

        public override string TagName
        {
            get { return Tag; }
        }
    }
}