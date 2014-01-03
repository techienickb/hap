using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Web.Configuration
{
    public interface IRegister
    {
        /// <summary>
        /// Register for CSS inclusion
        /// </summary>
        /// <returns>String Array of Registration Paths</returns>
        RegistrationPath[] RegisterCSS();
        /// <summary>
        /// Register for JS Inclusion at the Start (in Head), before JQuery is Registered
        /// </summary>
        /// <returns>String Array of Registration Paths</returns>
        RegistrationPath[] RegisterJSStart();
        /// <summary>
        /// Register for JS Inclusion before HAP JS is Registered
        /// </summary>
        /// <returns>String Array of Registration Paths</returns>
        RegistrationPath[] RegisterJSBeforeHAP();
        /// <summary>
        /// Register for JS Inclusion After HAP JS is Registered
        /// </summary>
        /// <returns>String Array of Registration Paths</returns>
        RegistrationPath[] RegisterJSAfterHAP();
        /// <summary>
        /// Register for JS Inclusion at the end of the page
        /// </summary>
        /// <returns>String Array of Registration Paths</returns>
        RegistrationPath[] RegisterJSEnd();
        
    }

    public class RegistrationPath
    {
        /// <summary>
        /// App Relative Path to the Source File
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Array of Regular Expressions on which to load this script (RefererUri)
        /// </summary>
        public string[] LoadOn { get; set; }
    }
}
