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
        [Obsolete()]
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
            base.RenderChildren(w);
            w.Close();
            writer.WriteFullBeginTag("script");
            writer.Write(new Microsoft.Ajax.Utilities.Minifier().MinifyJavaScript(stringWriter.ToString().Replace("<script type=\"text/javascript\">", "").Replace("</script>","")));
            writer.WriteEndTag("script");
#endif
        }
    }
}