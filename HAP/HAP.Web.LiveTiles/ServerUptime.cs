using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using HAP.AD;
using HAP.Web.Configuration;

namespace HAP.Web.LiveTiles
{
    public class ServerUptime
    {
        public static TimeSpan Uptime(string server)
        {
            TimeSpan t = new TimeSpan(0);
            hapConfig config = hapConfig.Current;
            User user = new User();
            user.Authenticate(config.AD.User, config.AD.Password);
            try
            {
                user.ImpersonateContained();
                PerformanceCounter pc = new PerformanceCounter("System", "System Up Time", "", server);
                pc.NextValue();
                t = TimeSpan.FromSeconds(pc.NextValue());
            }
            catch
            {
            }
            finally
            {
                user.EndContainedImpersonate();
            }
            return t;
        }
    }
}
