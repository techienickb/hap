using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DiskQuotaTypeLibrary;
using Microsoft.Storage;
using System.Runtime.InteropServices;

namespace HAP.Data.Quota
{
    public class WCFService : IService
    {
        public QuotaInfo GetQuota(string username, string fileshare)
        {
            DiskQuotaControlClass dqc = new DiskQuotaControlClass();
            QuotaInfo qi = new QuotaInfo();
            //Initializes the control to the specified path
            dqc.Initialize(fileshare, true);
            qi.Used = dqc.FindUser(username).QuotaUsed;
            qi.Total = dqc.FindUser(username).QuotaLimit;
            return qi;
        }

        public QuotaInfo GetQuotaFromPath(string path)
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
            catch (Exception)
            {
                return new QuotaInfo();
            }
        }
    }

    [DataContract]
    public class QuotaInfo
    {
        public QuotaInfo() { Used = Free = Total = -1; }

        [DataMember]
        public double Used { get; set; }

        [DataMember]
        public double Free { get; set; }

        [DataMember]
        public double Total { get; set; }

    }
}
