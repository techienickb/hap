using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HAP.Logon.Tracker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args[0].StartsWith("http")) Application.Run(new Loading(Action.Clear, args[0]));
            else Application.Run(new Loading(Action.Logon, args[1]));
        }
    }
}
