using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DiskQuotaTypeLibrary;
using Microsoft.Storage;
using System.Runtime.InteropServices;
using System.Management;

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
            catch
            {
                try
                {
                    Quota = FSRMQuotaManager.GetQuota(GetPath(path));
                    QuotaInfo q = new QuotaInfo();
                    q.Free = (int)Quota.QuotaLimit - (int)Quota.QuotaUsed;
                    q.Used = (int)Quota.QuotaUsed;
                    q.Total = (int)Quota.QuotaLimit;
                    return q;
                }
                catch
                {
                }
                return new QuotaInfo();
            }
        }

        string GetPath(string uncPath)
        {
            try
            {
                // remove the "\\" from the UNC path and split the path
                uncPath = uncPath.Replace(@"\\", "");
                string[] uncParts = uncPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (uncParts.Length < 2)
                    return "[UNRESOLVED UNC PATH: " + uncPath + "]";
                // Get a connection to the server as found in the UNC path
                ManagementScope scope = new ManagementScope(@"\\" + uncParts[0] + @"\root\cimv2");
                // Query the server for the share name
                SelectQuery query = new SelectQuery("Select * From Win32_Share Where Name = '" + uncParts[1] + "'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                // Get the path
                string path = string.Empty;
                foreach (ManagementObject obj in searcher.Get())
                {
                    path = obj["path"].ToString();
                }

                // Append any additional folders to the local path name
                if (uncParts.Length > 2)
                {
                    for (int i = 2; i < uncParts.Length; i++)
                        path = path.EndsWith(@"\") ? path + uncParts[i] : path + @"\" + uncParts[i];
                }

                return path;
            }
            catch (Exception ex)
            {
                return "[ERROR RESOLVING UNC PATH: " + uncPath + ": " + ex.Message + "]";
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
