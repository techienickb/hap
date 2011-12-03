using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Security.Authentication;
using HAP.Data.MyFiles.WordVisualizer.Core.Extensions;
using Microsoft.Office.DocumentFormat.OpenXml.Packaging;
using HAP.Data.MyFiles.WordVisualizer.Core;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using HAP.Data.MyFiles.WordVisualizer.Core.Util;
using HAP.Web.Configuration;
using System.Web.Security;
using HAP.Data.ComputerBrowser;
using System.Web.SessionState;

namespace HAP.Data.MyFiles
{
    /// <summary>
    /// Summary description for docx
    /// </summary>
    public class DocXPreview
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

        private string path;
        private HAP.Web.Configuration.hapConfig config;

        public static string Render(string Path)
        {
            string s = "";
            using (WordprocessingDocument document = WordprocessingDocument.Open(Path, false))
            {
                s += RenderStyles(document);
                s += "   <div id=\"Document\">";
                s += RenderDocument(document);
                s += "   </div>";
            }
            return s;
        }

        #region Private methods

        static string RenderInternalRelationshipLink(string partRelationship)
        {
            return "";
        }

        static string CreateCssProperties(XmlReader reader)
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

        static string GetDefaultStyleName(WordprocessingDocument document)
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

        static string RenderStyles(WordprocessingDocument document)
        {
            string s = "";
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
            s += "<style>\n";
            s += "<!--\n";
            foreach (var c in cssClasses)
            {
                s += "#Document ." + c.CssClassName + "{\n";
                s += c.ParagraphStyle;
                s += c.RunStyle;
                s += "}\n";
            }
            s += "-->\n";
            s += "</style>\n";
            return s;
        }

        static string RenderDocument(WordprocessingDocument document)
        {
            string s = "";
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
                s += String.Format("<p class=\"{0}\" style=\"{1}\">\n", paragraph.CssClass, paragraph.CssStyles);

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
                        s += "<br />";
                    }

                    // Write run
                    s += String.Format("<span class=\"{0}\" style=\"{1}\">",
                            run.CssClass,
                            run.CssStyles ?? ""
                        );

                    // Is it an hyperlink?
                    if (run.RunType == "hyperlink")
                    {
                        ExternalRelationship relation = (from rel in document.MainDocumentPart.ExternalRelationships
                                                         where rel.Id == run.ExternalRelationId
                                                         select rel).FirstOrDefault() as ExternalRelationship;
                        if (relation != null)
                        {
                            s += "<a href=\"" + relation.Uri.ToString() + "\">";
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
                        s += 
                            String.Format("<div style=\"float:left; clear:none; width:{1}px; height:{2}px; margin:5px;\"><img src=\"{0}\" width=\"{1}\" height=\"{2}\"/></div>",
                                RenderInternalRelationshipLink(graphic.ExternalRelationId),
                                graphic.Width,
                                graphic.Height
                            );
                    }

                    // Write text
                    s += RenderPlainText(run.Text);

                    // End hyperlink
                    if (run.RunType == "hyperlink")
                    {
                        s += "</a>";
                    }

                    // End run
                    s += "</span>";
                }

                s += "</p>\n\n";
            }
            return s;
        }

        /// <summary>
        /// Render plain text
        /// </summary>
        /// <param name="context">Current http context</param>
        /// <param name="text">Text to render</param>
        /// <returns>Rendered text</returns>
        static string RenderPlainText(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }

        #endregion

    }
}