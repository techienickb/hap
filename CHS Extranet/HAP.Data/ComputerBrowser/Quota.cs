using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HAP.Data.Quota;
using HAP.Web.Configuration;
using Microsoft.Storage;
using System.Runtime.InteropServices;

namespace HAP.Data.ComputerBrowser
{
    public class Quota
    {
        public static HAP.Data.Quota.QuotaInfo GetQuota(string username, string share)
        {
            QuotaServer server = null;
            foreach (QuotaServer s in hapConfig.Current.MySchoolComputerBrowser.QuotaServers)
                if (share.ToLower().StartsWith(s.Expression.Replace("%username%", username).ToLower()))
                    server = s;
            if (server == null) throw new Exception("Can't find quota server");
            string endPointAddr = "net.tcp://" + server.Server + ":8010/HAPQuotaService";
            NetTcpBinding tcpBinding = new NetTcpBinding();
            tcpBinding.TransactionFlow = false;
            tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            tcpBinding.Security.Mode = SecurityMode.None; 
            EndpointAddress endpointAddress = new EndpointAddress(endPointAddr);
            ServiceClient c = new ServiceClient(tcpBinding, endpointAddress);
            return c.GetQuota(username, server.Drive.ToString() + ":");
        }

        public static HAP.Data.Quota.QuotaInfo GetQuota(string path)
        {
            IFsrmQuotaManager FSRMQuotaManager = new FsrmQuotaManagerClass();
            IFsrmQuota Quota = null;

            try
            {
                Quota = FSRMQuotaManager.GetQuota(path);
                QuotaInfo q = new QuotaInfo();
                q.Free = (int)Quota.QuotaLimit - (int)Quota.QuotaUsed;
                q.Used = (int)Quota.QuotaUsed;
                q.Total = (int)Quota.QuotaLimit;
                return q;
            }
            catch (COMException e)
            {
                unchecked
                {
                    if (e.ErrorCode == (int)0x80045301)
                    {
                        throw new NullReferenceException("No Quota");
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }
    }
}
