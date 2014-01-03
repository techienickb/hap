using HAP.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAP.Timetable
{
    public class Register : IRegister
    {
        public RegistrationPath[] RegisterCSS()
        {
            return new RegistrationPath[] { 
                new RegistrationPath { Path = "~/style/fullcalendar.css", LoadOn = new string[] { "/hap/timetable.aspx" }}, 
                new RegistrationPath { Path = "~/style/timetable.css", LoadOn = new string[] { "/hap/timetable.aspx" }} 
            };
        }

        public RegistrationPath[] RegisterJSStart()
        {
            throw new NotImplementedException();
        }

        public RegistrationPath[] RegisterJSBeforeHAP()
        {
            throw new NotImplementedException();
        }

        public RegistrationPath[] RegisterJSAfterHAP()
        {
            return new RegistrationPath[] {
                new RegistrationPath { Path = "~/Scripts/fullcalendar.min.js", LoadOn = new string[] { "/hap/timetable.aspx" } }
            };
        }

        public RegistrationPath[] RegisterJSEnd()
        {
            throw new NotImplementedException();
        }
    }
}
