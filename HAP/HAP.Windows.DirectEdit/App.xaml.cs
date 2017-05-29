using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows;

namespace HAP.Win.DirectEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
                if (!hasAdministrativeRight)
                {
                    RunElevated(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    if (!Registry.ClassesRoot.GetSubKeyNames().Contains("hap"))
                    {
                        RegistryKey key = Registry.ClassesRoot.CreateSubKey("hap");
                        key.SetValue(string.Empty, "URL:HAP Protocol");
                        key.SetValue("URL Protocol", string.Empty);
                        RegistryKey di = key.CreateSubKey("DefaultIcon");
                        di.SetValue(string.Empty, System.Reflection.Assembly.GetExecutingAssembly().Location + ",0");
                        key = key.CreateSubKey(@"shell\open\command");
                        key.SetValue(string.Empty, System.Reflection.Assembly.GetExecutingAssembly().Location + " " + "%1");
                        MessageBox.Show("HAP+ DirectEdit Registered");
                    }
                    else
                    {
                        Registry.ClassesRoot.DeleteSubKeyTree("hap");
                        MessageBox.Show("HAP+ DirectEdit Unregistered");
                    }
                }
                Application.Current.Shutdown(0);
            }
            else
            {

            }
        }


        private static bool RunElevated(string fileName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Verb = "runas";
            processInfo.FileName = fileName;
            try
            {
                Process p = Process.Start(processInfo);
                p.WaitForExit();
                return true;
            }
            catch (Win32Exception)
            {
            }
            return false;
        }
    }
}
