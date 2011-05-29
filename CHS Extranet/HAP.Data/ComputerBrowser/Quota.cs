﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HAP.Data.Quota;
using HAP.Web.Configuration;

namespace HAP.Data.ComputerBrowser
{
    public class Quota
    {
        public static HAP.Data.Quota.QuotaInfo GetQuota(string username, string share)
        {
            quotaserver server = null;
            foreach (quotaserver s in hapConfig.Current.MyComputer.quotaservers)
                if (share.ToLower().StartsWith(string.Format(s.Expression, username).ToLower()))
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
            return c.GetQuota(username, server.Drive);
        }
    }
}