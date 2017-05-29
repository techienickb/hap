using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using System.Net;
using System.ServiceModel.Description;

namespace HAP.Data.Quota.Service
{
    partial class QuotaService : ServiceBase
    {
        public QuotaService()
        {
            InitializeComponent();
        }

        ServiceHost host;
        protected override void OnStart(string[] args)
        {
            host = new ServiceHost(typeof(HAP.Data.Quota.WCFService));
            string urlService = "net.tcp://" + Dns.GetHostName() + ":8010/HAPQuotaService";

            // Instruct the ServiceHost that the type

            // The binding is where we can choose what

            // transport layer we want to use. HTTP, TCP ect.

            NetTcpBinding tcpBinding = new NetTcpBinding();
            tcpBinding.TransactionFlow = false;
            tcpBinding.Security.Transport.ProtectionLevel =
               System.Net.Security.ProtectionLevel.EncryptAndSign;
            tcpBinding.Security.Transport.ClientCredentialType =
               TcpClientCredentialType.Windows;
            tcpBinding.Security.Mode = SecurityMode.None;
            // <- Very crucial


            // Add a endpoint

            host.AddServiceEndpoint(typeof(HAP.Data.Quota.IService), tcpBinding, urlService);

            // A channel to describe the service.

            // Used with the proxy scvutil.exe tool
            string urlMeta = "";

            ServiceMetadataBehavior metadataBehavior;
            metadataBehavior =
              host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (metadataBehavior == null)
            {
                // This is how I create the proxy object

                // that is generated via the svcutil.exe tool

                metadataBehavior = new ServiceMetadataBehavior();
                metadataBehavior.HttpGetUrl = new Uri("http://" + Dns.GetHostName() + ":8011/HAPQuotaService");
                metadataBehavior.HttpGetEnabled = true;
                metadataBehavior.ToString();
                host.Description.Behaviors.Add(metadataBehavior);
                urlMeta = metadataBehavior.HttpGetUrl.ToString();
            }

            host.Open();
        }

        protected override void OnStop()
        {
            host.Close();
        }
    }
}
