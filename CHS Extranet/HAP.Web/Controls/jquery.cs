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
    [ToolboxData("<{0}:jQuery runat=\"server\" />")]
    public class jQuery : WebControl
    {
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.WriteLine("<script src=\"" + ResolveClientUrl("~/Scripts/jquery-1.6.2.min.js") + "\" type=\"text/javascript\"></script>");
            output.WriteLine("<script src=\"" + ResolveClientUrl("~/Scripts/jquery-ui-1.8.14.custom.min.js") + "\" type=\"text/javascript\"></script>");
            
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
        }
    }
}
