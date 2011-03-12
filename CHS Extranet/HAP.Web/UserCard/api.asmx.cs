using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using HAP.Web.Configuration;
using HAP.Data.UserCard;

namespace HAP.Web.UserCard
{
    /// <summary>
    /// Summary description for api
    /// </summary>
    [WebService(Namespace = "http://hap.codeplex.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class api : System.Web.Services.WebService
    {

        [WebMethod]
        public Init getInit()
        {
            return new Init();
        }

        [WebMethod]
        public Department[] getDepartments()
        {
            return new Sys().getDepartments();
        }

        [WebMethod]
        public Form[] getForms()
        {
            return new Sys().getForms();
        }

        [WebMethod]
        public Form[] getFormsIn(string ou)
        {
            return new Sys().getForms(ou);
        }

        [WebMethod]
        public string ResetPassword(string username)
        {
            try
            {
                return "I've reset " + username + "'s password to 'password'\nThey will be prompted to change it when the log on";
            }
            catch (Exception e) { return e.ToString(); }
        }
    }
}
