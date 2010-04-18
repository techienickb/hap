using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WordVisualizer.Core.Extensions
{
    /// <summary>
    /// HttpResponse extension methods
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Write line to response stream
        /// </summary>
        /// <param name="response">Response stream</param>
        public static void WriteLine(this HttpResponse response)
        {
            response.Write("\r\n");
        }

        /// <summary>
        /// Write line to response stream
        /// </summary>
        /// <param name="response">Response stream</param>
        /// <param name="s">The string to write to the response stream</param>
        public static void WriteLine(this HttpResponse response, string s)
        {
            response.Write(s);
            response.Write("\r\n");
        }
    }
}
