using HAP.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAP.BookingSystem
{
    public class Register : IRegister
    {
        public RegistrationPath[] RegisterCSS()
        {
            return new RegistrationPath[] { 
                new RegistrationPath { LoadOn = new string[] { "bookingsystem/cal.aspx" }, Path = "~/style/fullcalendar.css" },
                new RegistrationPath { LoadOn = new string[] { "bookingsystem/cal.aspx" }, Path = "~/style/fullcalendar.print.css" },
                new RegistrationPath { LoadOn = new string[] { "bookingsystem" }, Path = "~/style/bookingsystem.css" }
            };
        }

        public RegistrationPath[] RegisterJSStart()
        {
            throw new NotImplementedException();
        }

        public RegistrationPath[] RegisterJSBeforeHAP()
        {
            return new RegistrationPath[] { 
                new RegistrationPath { LoadOn = new string[] { "bookingsystem" }, Path = "~/scripts/jquery.ba-hashchange.min.js" },
                new RegistrationPath { LoadOn = new string[] { "bookingsystem/cal.aspx" }, Path = "~/scripts/fullcalendar.min.js" }
            };
        }

        public RegistrationPath[] RegisterJSAfterHAP()
        {
            throw new NotImplementedException();
        }

        public RegistrationPath[] RegisterJSEnd()
        {
            throw new NotImplementedException();
        }
    }
}