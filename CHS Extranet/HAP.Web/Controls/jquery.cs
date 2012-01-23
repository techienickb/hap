using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HAP.Web.Controls
{
    [ToolboxData("<{0}:jQuery runat=\"server\" />")]
    public class jQuery : Control
    {
        protected override void RenderControl(HtmlTextWriter output)
        {
            output.WriteLine("<script src=\"" + ResolveUrl("~/Scripts/jquery-1.7.1.min.js") + "\" type=\"text/javascript\"></script>");
            output.WriteLine("<script src=\"" + ResolveUrl("~/Scripts/jquery-ui-1.8.16.custom.min.js") + "\" type=\"text/javascript\"></script>");
            Regex regex = new Regex("(XBLWP7|ZuneWP|iPhone|iPad|Android|BlackBerry)", RegexOptions.IgnoreCase);
            if (regex.IsMatch(HttpContext.Current.Request.UserAgent))
            {
                output.WriteLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
                output.WriteLine("<link href=\"" + ResolveUrl("~/style/jquery.mobile-1.0.css") + "\"  rel=\"stylesheet\" type=\"text/css\" />");
                output.WriteLine("<script src=\"" + ResolveUrl("~/Scripts/jquery.mobile-1.0.min.js") + "\" type=\"text/javascript\"></script>");
            }
            output.WriteLine("<script src=\"" + ResolveUrl("~/api/js") + "\" type=\"text/javascript\"></script>");
        }
    }
}
