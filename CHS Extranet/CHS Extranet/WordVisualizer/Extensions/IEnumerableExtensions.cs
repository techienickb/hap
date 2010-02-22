using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordVisualizer.Core.Extensions
{
    /// <summary>
    /// IEnumerable extensions
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Concatenate strings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="func">Function delegate</param>
        /// <returns>Concatenated string</returns>
        public static string StringConcatenate<T>(this IEnumerable<T> source, Func<T, string> func)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T item in source)
                sb.Append(func(item));
            return sb.ToString();
        }
    }


 

}

