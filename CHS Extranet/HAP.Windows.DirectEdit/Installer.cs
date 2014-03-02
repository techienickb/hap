using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace HAP.Win.DirectEdit
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            RegistryKey key = Registry.ClassesRoot.CreateSubKey("hap");
            key.SetValue(string.Empty, "URL:HAP Protocol");
            key.SetValue("URL Protocol", string.Empty);
            RegistryKey di = key.CreateSubKey("DefaultIcon");
            di.SetValue(string.Empty, System.Reflection.Assembly.GetExecutingAssembly().Location + ",0");
            key = key.CreateSubKey(@"shell\open\command");
            key.SetValue(string.Empty, System.Reflection.Assembly.GetExecutingAssembly().Location + " " + "%1");
        }
    }
}
