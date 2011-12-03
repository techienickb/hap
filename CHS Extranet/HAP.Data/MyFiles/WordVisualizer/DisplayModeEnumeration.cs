using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Data.MyFiles.WordVisualizer.Core.Handlers
{
    /// <summary>
    /// Document display mode
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// Display preview in HTML
        /// </summary>
        Display = 0,

        /// <summary>
        /// Download document
        /// </summary>
        Download = 1,

        /// <summary>
        /// 
        /// </summary>
        RenderPart = 2
    }
}
