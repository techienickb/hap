using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DiskQuotaTypeLibrary;

namespace HAP.Data.Quota
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in both code and config file together.
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
