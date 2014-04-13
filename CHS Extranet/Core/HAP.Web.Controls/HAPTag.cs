using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;

namespace HAP.Web.Controls
{
    [ToolboxData("<{0}:HAPTag runat=\"server\" />")]
    public class HAPTag : WebControl
    {
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.WriteLine("<meta name=\"author\" content=\"Nick Brown - nb development\" />");
            output.Write("<meta name=\"generator\" content=\"" + Assembly.GetExecutingAssembly().GetName().Name + " - Version: " + HAPVersion.ToString() + "\" />");
            
        }

        public static Version HAPVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
        }
    }
}
