using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;

namespace CHS
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:Navigation runat=\"server\" Selected=\"\" />")]
    public class Navigation : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Selected { get; set; }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            writer.Write("</div>");
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
                if (HttpContext.Current.Cache["CHSheader"] == null)
                {
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://www.crickhowell-hs.powys.sch.uk/it/");
                    req.Accept = "text/html";
                    req.UserAgent = "CHS Internal Header/CHSIH 1.0";
                    WebResponse res = req.GetResponse();
                    HttpContext.Current.Cache.Insert("CHSheader", res.GetResponseStream(), null, DateTime.Now.AddHours(1), TimeSpan.Zero);
                }
                StreamReader sr = new StreamReader((Stream)HttpContext.Current.Cache["CHSheader"]);
                string line = "";
                bool startwrite = false;
                while (line != null)
                {
                    if (line.Contains("<div id=\"navigation\">"))
                        startwrite = true;
                    if (!string.IsNullOrEmpty(Selected) && line.Contains(Selected))
                        line = line.Replace("<li class=\"", "<li class=\"current_page_item ");
                    if (line.Contains("</div>")) startwrite = false;
                    if (startwrite)
                        output.WriteLine(line);
                    line = sr.ReadLine();
                }
        }
    }
}
