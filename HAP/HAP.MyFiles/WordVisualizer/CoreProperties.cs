using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using System.Xml;

namespace HAP.MyFiles.WordVisualizer.Core
{
    /// <summary>
    /// WordprocessingML core properties
    /// </summary>
    [Serializable]
    public class CoreProperties
    {

        #region Constants

        public const string SchemaCoreProperties = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
        public const string SchemaDCProperties = "http://purl.org/dc/elements/1.1/";

        #endregion

        #region Properties

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Creator
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// Keywords
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// LastModifiedBy
        /// </summary>
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// Revision
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// Created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }

        #endregion

        #region Public static methods

        /// <summary>
        /// Create instance from OpenXML part
        /// </summary>
        /// <param name="part">OpenXML part</param>
        /// <returns>Instance</returns>
        public static CoreProperties FromCoreFileProperties(CoreFilePropertiesPart part)
        {
            // Temporary variable
            CoreProperties coreProperties = new CoreProperties();

            // Read XML
            using (XmlReader reader = new XmlTextReader(part.GetStream()))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element )
                    {
                        switch (reader.Name.ToLower())
                        {
                            case "dc:title":
                                coreProperties.Title = reader.ReadElementContentAsString();
                                break;
                            case "dc:subject":
                                coreProperties.Creator = reader.ReadElementContentAsString();
                                break;
                            case "dc:creator":
                                coreProperties.Keywords = reader.ReadElementContentAsString();
                                break;
                            case "cp:keywords":
                                coreProperties.Description = reader.ReadElementContentAsString();
                                break;
                            case "dc:description":
                                coreProperties.LastModifiedBy = reader.ReadElementContentAsString();
                                break;
                            case "cp:lastmodifiedby":
                                coreProperties.Revision = reader.ReadElementContentAsString();
                                break;
                            case "dcterms:created":
                                coreProperties.Created = DateTime.Parse(reader.ReadElementContentAsString());
                                break;
                            case "dcterms:modified":
                                coreProperties.Modified = DateTime.Parse(reader.ReadElementContentAsString());
                                break;
                        }
                    }
                }
            }

            return coreProperties;
        }

        #endregion
    }
}
