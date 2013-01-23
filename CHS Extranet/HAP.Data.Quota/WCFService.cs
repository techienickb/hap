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
                if (!EventLog.SourceExists("HAP+ Quota Service")) EventLog.CreateEventSource("HAP+ Quota Service", "Application");
                qFree = Math.Round((Decimal)Quota.QuotaLimit - (Decimal)Quota.QuotaUsed, 0);
                qUsed = (Decimal)Quota.QuotaUsed;
                qTotal = (Decimal)Quota.QuotaLimit;
                EventLog.WriteEntry("HAP+ Quota Service", path + "\nFREE: " + qFree + "\nUSED: " + qUsed + "\nTOTAL: " + qTotal, EventLogEntryType.Information);
                q.Free = Convert.ToDouble(qFree.ToString());
                q.Total = Convert.ToDouble(qTotal.ToString());
                q.Used = Convert.ToDouble(qUsed.ToString());
            }
            catch (Exception ex)
            {
#if DEBUG
                if (!EventLog.SourceExists("HAP+ Quota Service")) EventLog.CreateEventSource("HAP+ Quota Service", "Application");
                EventLog.WriteEntry("HAP+ Quota Service", path + "\n\n" + ex.ToString() + "\n\n" + ex.Message + "\n\n" + ex.StackTrace, EventLogEntryType.Error);
#endif
                path = GetPath(path);
                try
                {
                    Quota = FSRMQuotaManager.GetQuota(path);
                    EventLog.WriteEntry("HAP+ Quota Service", "Type: " + Quota.QuotaLimit.GetType().ToString(), EventLogEntryType.Information);
                    qFree = (Decimal)Quota.QuotaLimit - (Decimal)Quota.QuotaUsed;
                    qUsed = (Decimal)Quota.QuotaUsed;
                    qTotal = (Decimal)Quota.QuotaLimit;
                    q.Free = Convert.ToDouble(qFree.ToString());
                    q.Total = Convert.ToDouble(qTotal.ToString());
                    q.Used = Convert.ToDouble(qUsed.ToString());
                    EventLog.WriteEntry("HAP+ Quota Service", path + "\nFREE: " + qFree + "\nUSED: " + qUsed + "\nTOTAL: " + qTotal, EventLogEntryType.Information);
                }
                catch (Exception e)
                {
#if DEBUG
                    EventLog.WriteEntry("HAP+ Quota Service", path + "\n\n" + e.ToString() + "\n\n" + e.Message + "\n\n" + e.StackTrace, EventLogEntryType.Error);
#endif
                }
            }
            return q;
        }

        public static string GetPath(string uncPath)
        {
            try
            {
                // remove the "\\" from the UNC path and split the path
                uncPath = uncPath.Replace(@"\\", "");
                string[] uncParts = uncPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (uncParts.Length < 2)
                    return "UNRESOLVED UNC PATH: " + uncPath;
                // Get a connection to the server as found in the UNC path
                ManagementScope scope = new ManagementScope(@"\\" + uncParts[0] + @"\root\cimv2");
                // Query the server for the share name
                SelectQuery query = new SelectQuery("Select * From Win32_Share Where Name = '" + uncParts[1] + "'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                Console.WriteLine("Here tom");
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
#if DEBUG
                if (!EventLog.SourceExists("HAP+ Quota Service")) EventLog.CreateEventSource("HAP+ Quota Service", "Application");
                EventLog.WriteEntry("HAP+ Quota Service", "Error Resolving Path: " + uncPath + "\n\n" + ex.Message + "\n\n" + ex.StackTrace, EventLogEntryType.Error);
#endif
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
