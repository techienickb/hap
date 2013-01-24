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
using System.Diagnostics;
using System.IO;

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
            QuotaInfo q = new QuotaInfo();
            Decimal qFree = 0;
            Decimal qTotal = 0;
            Decimal qUsed = 0;
            try
            {
                Quota = FSRMQuotaManager.GetQuota(path);
                qFree = Math.Round((Decimal)Quota.QuotaLimit - (Decimal)Quota.QuotaUsed, 0);
                qUsed = (Decimal)Quota.QuotaUsed;
                qTotal = (Decimal)Quota.QuotaLimit;
                q.Free = Convert.ToDouble(qFree.ToString());
                q.Total = Convert.ToDouble(qTotal.ToString());
                q.Used = Convert.ToDouble(qUsed.ToString());
            }
            catch
            {
                path = GetPath(path);
                try
                {
                    Quota = FSRMQuotaManager.GetQuota(path);
                    qFree = (Decimal)Quota.QuotaLimit - (Decimal)Quota.QuotaUsed;
                    qUsed = (Decimal)Quota.QuotaUsed;
                    qTotal = (Decimal)Quota.QuotaLimit;
                    q.Free = Convert.ToDouble(qFree.ToString());
                    q.Total = Convert.ToDouble(qTotal.ToString());
                    q.Used = Convert.ToDouble(qUsed.ToString());
                }
                catch
                {
                }
            }
            return q;
        }

        public static string GetPath(string uncPath)
        {
            try
            {
                uncPath = uncPath.Replace(@"\\", "");
                string[] uncParts = uncPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (uncParts.Length < 2) return "UNRESOLVED UNC PATH: " + uncPath;
                ManagementScope scope = new ManagementScope(@"\\" + uncParts[0] + @"\root\cimv2");
                SelectQuery query = new SelectQuery("Select * From Win32_Share Where Name = '" + uncParts[1] + "'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                string path = string.Empty;
                foreach (ManagementObject obj in searcher.Get())
                {
                    path = obj["path"].ToString();
                }
                if (uncParts.Length > 2)
                {
                    for (int i = 2; i < uncParts.Length; i++)
                        path = path.EndsWith(@"\") ? path + uncParts[i] : path + @"\" + uncParts[i];
                }

                return path;
            }
            catch
            {
                return "ERROR WITH UNC PATH: " + uncPath;
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
