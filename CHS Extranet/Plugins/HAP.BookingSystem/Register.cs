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
            throw new NotImplementedException();
        }

        public RegistrationPath[] RegisterJSStart()
        {
            throw new NotImplementedException();
        }

        public RegistrationPath[] RegisterJSBeforeHAP()
        {
            return new RegistrationPath[] { new RegistrationPath { LoadOn = new string[] { "bookingsystem" }, Path = "~/scripts/jquery.ba-hashchange.min.js" } };
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