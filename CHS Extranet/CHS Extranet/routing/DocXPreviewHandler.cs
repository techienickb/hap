using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Security.Authentication;
using WordVisualizer.Core.Extensions;
using Microsoft.Office.DocumentFormat.OpenXml.Packaging;
using WordVisualizer.Core;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using WordVisualizer.Core.Util;
using CHS_Extranet.Configuration;

namespace CHS_Extranet.routing
{
    /// <summary>
    /// Summary description for docx
    /// </summary>
    public class DocXPreviewHandler : IHttpHandler, IMyComputerDisplay
    {

        #region Constants

        /// <summary>
        /// WordProcessingML Namespace
        /// </summary>
        public const string WordProcessingMLNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        /// <summary>
        /// DrawingML Namespace
        /// </summary>
        public const string DrawingMLNamespace = "http://schemas.openxmlformats.org/drawingml/2006/main";

        /// <summary>
        /// WordprocessingDrawing Namespace
        /// </summary>
        public const string WordprocessingDrawingNamespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";

        /// <summary>
        /// Relationships Namespace
        /// </summary>
        public const string RelationshipsNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

        #endregion

        private String _DomainDN;
        private String _ActiveDirectoryConnectionString;
        private PrincipalContext pcontext;
        private UserPrincipal up;
        private GroupPrincipal studentgp;
        private string path;
        private extranetConfig config;

        private bool isAuth(uncpath path)
        {
            if (path.EnableReadTo == "All") return true;
            else if (path.EnableReadTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableReadTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, s);
                    if (!vis) vis = up.IsMemberOf(gp);
                }
                return vis;
            }
            return false;
        }

        private bool isWriteAuth(uncpath path)
        {
            if (path == null) return true;
            if (path.EnableWriteTo == "All") return true;
            else if (path.EnableWriteTo != "None")
            {
                bool vis = false;
                foreach (string s in path.EnableWriteTo.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    GroupPrincipal gp = GroupPrincipal.FindByIdentity(pcontext, s);
                    if (!vis) vis = up.IsMemberOf(gp);
                }
                return vis;
            }
            return false;
        }

        public string Username
        {
            get
            {
                if (HttpContext.Current.User.Identity.Name.Contains('\\'))
                    return HttpContext.Current.User.Identity.Name.Remove(0, HttpContext.Current.User.Identity.Name.IndexOf('\\') + 1);
                else return HttpContext.Current.User.Identity.Name;
            }
        }

        #region IWordDocumentRenderer Members

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="context">Current http context</param>
        public void ProcessRequest(System.Web.HttpContext context)
        {
            config = extranetConfig.Current;
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[config.ADSettings.ADConnectionString];
            if (connObj != null) _ActiveDirectoryConnectionString = connObj.ConnectionString;
            if (string.IsNullOrEmpty(_ActiveDirectoryConnectionString))
                throw new Exception("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            if (_ActiveDirectoryConnectionString.StartsWith("LDAP://"))
                _DomainDN = _ActiveDirectoryConnectionString.Remove(0, _ActiveDirectoryConnectionString.IndexOf("DC="));
            else throw new Exception("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            pcontext = new PrincipalContext(ContextType.Domain, null, _DomainDN, config.ADSettings.ADUsername, config.ADSettings.ADPassword);
            up = UserPrincipal.FindByIdentity(pcontext, IdentityType.SamAccountName, Username);

            string userhome = up.HomeDirectory;
            if (!userhome.EndsWith("\\")) userhome += "\\";
            path = RoutingPath.Replace('^', '&');
            uncpath unc = null;
            if (RoutingDrive == "N") path = Path.Combine(up.HomeDirectory, path.Replace('/', '\\'));
            else
            {
                unc = config.UNCPaths[RoutingDrive];
                if (unc == null || !isWriteAuth(unc)) context.Response.Redirect("/Extranet/unauthorised.aspx", true);
                else
                {
                    path = Path.Combine(string.Format(unc.UNC, Username), path.Replace('/', '\\'));
                }
            }

            // Open document
            using (WordprocessingDocument document = WordprocessingDocument.Open(path, false))
            {
                // Fetch necessary document parts
                CoreProperties coreProperties = CoreProperties.FromCoreFileProperties(document.CoreFilePropertiesPart);

                // Set content type
                context.Response.ContentType = "text/html";

                // Set response code
                context.Response.StatusCode = 200;

                // Render
                context.Response.WriteLine("<html>");

                context.Response.WriteLine("  <head>");
                context.Response.WriteLine("    <title>" + coreProperties.Title + "</title>");
                context.Response.WriteLine("    <link href=\"/extranet/basestyle.css\" rel=\"stylesheet\" type=\"text/css\" />");
                RenderStyles(context, document);
                context.Response.WriteLine("  </head>");

                context.Response.WriteLine("  <body>");

                // Download link
                RenderDownloadLink(context);

                // Core properties
                RenderCoreProperties(context, coreProperties);

                // Document
                context.Response.WriteLine("   <div id=\"Document\">");
                RenderDocument(context, document);
                context.Response.WriteLine("   </div>");

                // Footer
                RenderFooter(context);

                context.Response.WriteLine("  </body>");

                context.Response.WriteLine("</html>");
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Render download link
        /// </summary>
        /// <param name="context">Current http context</param>
        private void RenderDownloadLink(System.Web.HttpContext context)
        {
            context.Response.WriteLine("   <div id=\"Download\">");
            context.Response.WriteLine("     Download document: <a href=\"/extranet/download/" + RoutingDrive + "/" + RoutingPath + "\">" + Path.GetFileName(path) + "</a>");
            context.Response.WriteLine("   </div>");
        }

        /// <summary>
        /// Render internal relationship link
        /// </summary>
        /// <param name="context">Current http context</param>
        /// <param name="partRelationship">Part relationship id</param>
        /// <return>Link to renderer</return>
        private string RenderInternalRelationshipLink(System.Web.HttpContext context, string partRelationship)
        {
            return "";
        }

        /// <summary>
        /// Render core properties
        /// </summary>
        /// <param name="context">Current http context</param>
        /// <param name="coreProperties">Core properties instance</param>
        private void RenderCoreProperties(System.Web.HttpContext context, CoreProperties coreProperties)
        {
            context.Response.WriteLine("   <div id=\"CoreProperties\">");
            context.Response.WriteLine("     <h1 id=\"Title\">Document properties</h1>");
            context.Response.WriteLine("     <table id=\"Details\">");
            context.Response.WriteLine("       <tr>");
            context.Response.WriteLine("         <th>Author</th>");
            context.Response.WriteLine("         <th>Title</th>");
            context.Response.WriteLine("         <th>Subject</th>");
            context.Response.WriteLine("         <th>Keywords</th>");
            context.Response.WriteLine("       </tr>");
            context.Response.WriteLine("       <tr>");
            context.Response.WriteLine("         <td>&nbsp;" + coreProperties.Creator + "</td>");
            context.Response.WriteLine("         <td>&nbsp;" + coreProperties.Title + "</td>");
            context.Response.WriteLine("         <td>&nbsp;" + coreProperties.Subject + "</td>");
            context.Response.WriteLine("         <td>&nbsp;" + coreProperties.Keywords + "</td>");
            context.Response.WriteLine("       </tr>");
            context.Response.WriteLine("       <tr>");
            context.Response.WriteLine("         <th>Description</th>");
            context.Response.WriteLine("         <th>Date created</th>");
            context.Response.WriteLine("         <th>Date modified</th>");
            context.Response.WriteLine("         <th>Revision</th>");
            context.Response.WriteLine("       </tr>");
            context.Response.WriteLine("       <tr>");
            context.Response.WriteLine("         <td>&nbsp;" + coreProperties.Description + "</td>");
            context.Response.WriteLine("         <td>&nbsp;" + coreProperties.Created.ToShortDateString() + " " + coreProperties.Created.ToShortTimeString() + "</td>");
            context.Response.WriteLine("         <td>&nbsp;" + coreProperties.Modified.ToShortDateString() + " " + coreProperties.Modified.ToShortTimeString() + "</td>");
            context.Response.WriteLine("         <td>&nbsp;" + coreProperties.Revision + "</td>");
            context.Response.WriteLine("       </tr>");
            context.Response.WriteLine("     </table>");
            context.Response.WriteLine("   </div>");
        }

        /// <summary>
        /// Render footer
        /// </summary>
        /// <param name="context">Current http context</param>
        private void RenderFooter(System.Web.HttpContext context)
        {
        }

        /// <summary>
        /// Create CSS properties from XmlReader instance
        /// </summary>
        /// <param name="reader">XmlReader instance containing style information</param>
        /// <returns>CSS style information (e.g. font-weight: bold;)</returns>
        private string CreateCssProperties(XmlReader reader)
        {
            StringBuilder returnValue = new StringBuilder();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower())
                    {
                        // pPr properties
                        case "w:left":
                        case "w:right":
                        case "w:top":
                        case "w:bottom":
                            returnValue.Append("border-" + reader.Name.ToLower().Replace("w:", "") + ": ");

                            if (reader.HasAttributes)
                            {
                                string size = reader.GetAttribute("w:sz");
                                if (!string.IsNullOrEmpty(size))
                                {
                                    size = (int.Parse(size) / 8).ToString();
                                    returnValue.Append(size + "pt ");
                                }

                                returnValue.Append("solid ");

                                string color = reader.GetAttribute("w:color");
                                if (!string.IsNullOrEmpty(color))
                                {
                                    returnValue.Append("#" + color + " ");
                                }
                            }
                            returnValue.Append(";");
                            break;
                        case "w:shd":
                            if (reader.HasAttributes)
                            {
                                string val = reader.GetAttribute("w:fill");
                                if (!string.IsNullOrEmpty(val))
                                {
                                    returnValue.Append("background-color: #" + val + ";");
                                }
                            }
                            break;

                        // rPr properties
                        case "w:rfonts":
                            if (reader.HasAttributes)
                            {
                                string val = reader.GetAttribute("w:ascii");
                                if (!string.IsNullOrEmpty(val))
                                {
                                    returnValue.Append("font-family: " + val + ";");
                                }
                            }
                            break;
                        case "w:b":
                            returnValue.Append("font-weight: bold;");
                            break;
                        case "w:color":
                            if (reader.HasAttributes)
                            {
                                string val = reader.GetAttribute("w:val");
                                if (!string.IsNullOrEmpty(val))
                                {
                                    returnValue.Append("color: #" + val + ";");
                                }
                            }
                            break;
                        case "w:caps":
                        case "w:smallcaps":
                            returnValue.Append("font-variant: small-caps;");
                            break;
                        case "w:em":
                        case "w:i":
                            returnValue.Append("font-style: italic;");
                            break;
                        case "w:strike":
                            returnValue.Append("text-decoration: striketrough;");
                            break;
                        case "w:sz":
                            if (reader.HasAttributes)
                            {
                                string val = reader.GetAttribute("w:val");
                                if (!string.IsNullOrEmpty(val))
                                {
                                    val = (int.Parse(val) / 2).ToString();
                                    returnValue.Append("font-size: " + val + "pt;");
                                }
                            }
                            break;
                        case "w:u":
                            returnValue.Append("text-decoration: underline;");
                            break;
                    }
                }
            }

            return returnValue.ToString();
        }

        /// <summary>
        /// Get default style name
        /// </summary>
        /// <param name="document">WordprocessingDocument</param>
        /// <returns>Default style name</returns>
        private string GetDefaultStyleName(WordprocessingDocument document)
        {
            XNamespace w = WordProcessingMLNamespace;

            XDocument xDoc = XDocument.Load(
                XmlReader.Create(
                    new StreamReader(document.MainDocumentPart.StyleDefinitionsPart.GetStream())
                 )
            );

            string defaultStyle = (string)(from l_style in xDoc
                                           .Root
                                           .Elements(w + "style")
                                           where (string)l_style.Attribute(w + "type") == "paragraph" &&
                                                 (string)l_style.Attribute(w + "default") == "1"
                                           select l_style
                                       ).First().Attribute(w + "styleId");

            return defaultStyle.Replace(" ", "");
        }

        /// <summary>
        /// Render styles
        /// </summary>
        /// <param name="context">Current http context</param>
        /// <param name="document">WordprocessingDocument</param>
        private void RenderStyles(System.Web.HttpContext context, WordprocessingDocument document)
        {
            XNamespace w = WordProcessingMLNamespace;

            XDocument xDoc = XDocument.Load(
                XmlReader.Create(
                    new StreamReader(document.MainDocumentPart.StyleDefinitionsPart.GetStream())
                 )
            );

            // Fetch classes
            var cssClasses = from l_style in xDoc
                                    .Root
                                    .Descendants(w + "style")
                             let l_style_pPr = l_style
                                          .Elements(w + "pPr")
                                          .FirstOrDefault()
                             let l_style_rPr = l_style
                                          .Elements(w + "rPr")
                                          .FirstOrDefault()
                             select new
                             {
                                 CssClassName = ((string)l_style.Attribute(w + "styleId")).Replace(" ", ""),
                                 ClassName = ((string)l_style.Element(w + "name").Attribute(w + "val")).Replace(" ", ""),
                                 ParagraphStyle = l_style_pPr != null ? CreateCssProperties(l_style_pPr.CreateReader()) : "",
                                 RunStyle = l_style_rPr != null ? CreateCssProperties(l_style_rPr.CreateReader()) : ""
                             };

            // Write Css
            context.Response.WriteLine("<style>");
            context.Response.WriteLine("<!--");
            foreach (var c in cssClasses)
            {
                context.Response.Write("#Document ." + c.CssClassName + "{");
                context.Response.Write(c.ParagraphStyle);
                context.Response.Write(c.RunStyle);
                context.Response.Write("}");
                context.Response.WriteLine();
            }
            context.Response.WriteLine("-->");
            context.Response.WriteLine("</style>");
        }

        /// <summary>
        /// Render document
        /// </summary>
        /// <param name="context">Current http context</param>
        /// <param name="document">WordprocessingDocument</param>
        private void RenderDocument(System.Web.HttpContext context, WordprocessingDocument document)
        {
            XNamespace w = WordProcessingMLNamespace;
            XNamespace rels = RelationshipsNamespace;
            XNamespace a = DrawingMLNamespace;
            XNamespace wp = WordprocessingDrawingNamespace;

            XName w_r = w + "r";
            XName w_ins = w + "ins";
            XName w_hyperlink = w + "hyperlink";

            XDocument xDoc = XDocument.Load(
                XmlReader.Create(
                    new StreamReader(document.MainDocumentPart.GetStream())
                 )
            );

            string defaultStyle = GetDefaultStyleName(document);

            // Fetch paragraphs
            var paragraphs = from l_paragraph in xDoc
                                        .Root
                                        .Element(w + "body")
                                        .Descendants(w + "p")
                             let l_paragraph_styleNode = l_paragraph
                                         .Elements(w + "pPr")
                                         .Elements(w + "pStyle")
                                         .FirstOrDefault()
                             let l_paragraph_inlineStyleNode = l_paragraph
                                         .Elements(w + "pPr")
                                         .FirstOrDefault()
                             select new
                             {
                                 ParagraphElement = l_paragraph,
                                 CssClass = l_paragraph_styleNode != null ? ((string)l_paragraph_styleNode.Attribute(w + "val")).Replace(" ", "") : defaultStyle,
                                 CssStyles = l_paragraph_inlineStyleNode != null ? CreateCssProperties(l_paragraph_inlineStyleNode.CreateReader()) : "",
                                 Runs = l_paragraph.Elements().Where(z => z.Name == w_r || z.Name == w_ins || z.Name == w_hyperlink)
                             };

            // Write paragraphs
            foreach (var paragraph in paragraphs)
            {
                context.Response.Write(String.Format("<p class=\"{0}\" style=\"{1}\">", paragraph.CssClass, paragraph.CssStyles));

                // Fetch runs
                var runs = from l_run in paragraph.Runs
                           let l_run_styleNode = l_run
                                       .Elements(w + "rPr")
                                       .FirstOrDefault()
                           let l_run_inheritStyle = l_run
                                       .Elements(w + "rPr")
                                       .Elements(w + "rStyle")
                                       .FirstOrDefault()
                           let l_run_graphics = l_run
                                       .Elements(w + "drawing")
                           select new
                           {
                               Run = l_run,
                               RunType = l_run.Name.LocalName,
                               CssStyles = l_run_styleNode != null ? CreateCssProperties(l_run_styleNode.CreateReader()) : "",
                               CssClass = l_run_inheritStyle != null ? (string)l_run_inheritStyle.Attribute(w + "val") : "",
                               Text = l_run.Descendants(w + "t").StringConcatenate(element => (string)element),
                               BreakBefore = l_run.Element(w + "br") != null,
                               ExternalRelationId = (l_run.Name == w_hyperlink ? (string)l_run.Attribute(rels + "id") : ""),
                               Graphics = l_run_graphics
                           };

                // Write runs
                foreach (var run in runs)
                {
                    // Break before?
                    if (run.BreakBefore)
                    {
                        context.Response.WriteLine("<br />");
                    }

                    // Write run
                    context.Response.Write(
                        String.Format("<span class=\"{0}\" style=\"{1}\">",
                            run.CssClass,
                            run.CssStyles ?? ""
                        )
                    );

                    // Is it an hyperlink?
                    if (run.RunType == "hyperlink")
                    {
                        ExternalRelationship relation = (from rel in document.MainDocumentPart.ExternalRelationships
                                                         where rel.Id == run.ExternalRelationId
                                                         select rel).FirstOrDefault() as ExternalRelationship;
                        if (relation != null)
                        {
                            context.Response.Write("<a href=\"" + relation.Uri.ToString() + "\">");
                        }
                    }

                    // Fetch graphics
                    var graphics = from l_graphic in run.Graphics
                                   let l_graphic_blip = l_graphic
                                             .Descendants(a + "blip")
                                             .Where(x => x.Attribute(rels + "embed") != null)
                                             .FirstOrDefault()
                                   let l_graphic_extent = l_graphic
                                             .Descendants(wp + "extent")
                                             .FirstOrDefault()
                                   select new
                                   {
                                       Width = l_graphic_extent != null ? EmuUnit.EmuToPixels((int)l_graphic_extent.Attribute("cx")) : 0,
                                       Height = l_graphic_extent != null ? EmuUnit.EmuToPixels((int)l_graphic_extent.Attribute("cy")) : 0,
                                       ExternalRelationId = l_graphic_blip != null ? (string)l_graphic_blip.Attribute(rels + "embed") : ""
                                   };

                    // Write graphics
                    foreach (var graphic in graphics)
                    {
                        // Write graphic
                        context.Response.Write(
                            String.Format("<div style=\"float:left; clear:none; width:{1}px; height:{2}px; margin:5px;\"><img src=\"{0}\" width=\"{1}\" height=\"{2}\"/></div>",
                                RenderInternalRelationshipLink(context, graphic.ExternalRelationId),
                                graphic.Width,
                                graphic.Height
                            )
                        );
                    }

                    // Write text
                    context.Response.Write(RenderPlainText(context, run.Text));

                    // End hyperlink
                    if (run.RunType == "hyperlink")
                    {
                        context.Response.Write("</a>");
                    }

                    // End run
                    context.Response.Write("</span>");
                }

                context.Response.Write("</p>");
                context.Response.WriteLine();
            }
        }

        /// <summary>
        /// Render plain text
        /// </summary>
        /// <param name="context">Current http context</param>
        /// <param name="text">Text to render</param>
        /// <returns>Rendered text</returns>
        private static string RenderPlainText(System.Web.HttpContext context, string text)
        {
            return context.Server.HtmlEncode(text);
        }

        #endregion


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string RoutingPath { get; set; }

        public string RoutingDrive { get; set; }
    }
}