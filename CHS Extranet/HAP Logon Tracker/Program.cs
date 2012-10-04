﻿using System;
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
            bool silent = false;
            foreach (string s in args)
                if (s.ToLower().EndsWith("silent")) { silent = true; break; }
            if (args.Length == 0) Application.Run(new Loading(Action.Logon, "https://folders.crickhowell-hs.powys.sch.uk/hap/", silent));
            else if (args[0].StartsWith("http")) Application.Run(new Loading(Action.Clear, args[0], silent));
            else Application.Run(new Loading(Action.Logon, args[1], silent));
        }
    }
}
