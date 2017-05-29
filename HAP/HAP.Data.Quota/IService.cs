using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace HAP.Data.Quota
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        QuotaInfo GetQuota(string username, string fileshare);

        [OperationContract]
        QuotaInfo GetQuotaFromPath(string path);
    }
}
