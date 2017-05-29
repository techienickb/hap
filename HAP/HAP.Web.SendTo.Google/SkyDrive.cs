using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HAP.Web.Configuration;
using Microsoft.Http;

namespace HAP.Web.SendTo
{
    public class SkyDrive
    {
        public static void UploadFileToSkydrive(FileInfo file, string accessToken)
        {
            string uploaduri = string.Format("https://apis.live.net/v5.0/me/skydrive/files/{0}?access_token={1}",
                file.Name, accessToken);

            HttpClient httpClient = new HttpClient();

            if (!string.IsNullOrEmpty(hapConfig.Current.ProxyServer.Address) && hapConfig.Current.ProxyServer.Enabled)
                httpClient.TransportSettings.Proxy = new WebProxy(hapConfig.Current.ProxyServer.Address, hapConfig.Current.ProxyServer.Port);

            HttpContent c = HttpContent.Create(file.OpenRead());

            HttpResponseMessage response = httpClient.Put(uploaduri, c);
        }
    }
}
