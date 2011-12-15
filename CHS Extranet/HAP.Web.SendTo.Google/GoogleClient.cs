using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Documents;
using Google.GData.Client;
using System.Net;
using HAP.Web.Configuration;

namespace HAP.Web.SendTo.Google
{
    public class Client
    {
        private bool loggedIn = false;
        private DocumentsService service;

        public void Login(string username, string password)
        {
            if (loggedIn)
            {
                throw new ApplicationException("Already logged in.");
            }
            try
            {
                service = new DocumentsService("DocListUploader");
                ((GDataRequestFactory)service.RequestFactory).KeepAlive = false;
                if (!string.IsNullOrEmpty(hapConfig.Current.ProxyServer.Address) && hapConfig.Current.ProxyServer.Enabled)
                    ((GDataRequestFactory)service.RequestFactory).Proxy = new WebProxy(hapConfig.Current.ProxyServer.Address, hapConfig.Current.ProxyServer.Port);
                service.setUserCredentials(username, password);
                //force the service to authenticate
                DocumentsListQuery query = new DocumentsListQuery();
                query.NumberToRetrieve = 1;
                service.Query(query);
                loggedIn = true;
            }
            catch (AuthenticationException e)
            {
                loggedIn = false;
                service = null;
                throw e;
            }
        }

        public void Logout()
        {
            loggedIn = false;
            service = null;
        }

        public string Upload(string file)
        {
            if (!loggedIn) throw new AuthenticationException("Please log in before uploading documents");
            return service.UploadDocument(file, null).AlternateUri.ToString();
        }

    }
}
