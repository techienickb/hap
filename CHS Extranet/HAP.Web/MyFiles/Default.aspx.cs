﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HAP.Web.Configuration;
using System.Web.Configuration;
using System.Configuration;
using System.IO;

namespace HAP.Web.MyFiles
{
    public partial class Default : HAP.Web.Controls.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The max file size in bytes
        /// </summary>
        protected int maxRequestLength
        {
            get
            {
                HttpRuntimeSection section = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;

                if (section != null)
                    return section.MaxRequestLength * 1024; // Cofig Value
                else
                    return 4096 * 1024; // Default Value
            }
        }

        protected string AcceptedExtensions
        {
            get
            {
                List<string> filters = new List<string>();
                foreach (Filter f in config.MySchoolComputerBrowser.Filters)
                    if (isAuth(f) && f.Expression == "*.*") return "";
                    else if (isAuth(f))
                        filters.Add(f.Name + " - " + f.Expression.Trim());
                return string.Join("\n ", filters.ToArray());
            }
        }

        protected string DropZoneAccepted
        {
            get
            {
                List<string> filters = new List<string>();
                foreach (Filter f in config.MySchoolComputerBrowser.Filters)
                    if (isAuth(f) && f.Expression == "*.*") return "";
                    else if (isAuth(f))
                        foreach (string s in f.Expression.Split(new char[] { ';' }))
                            filters.Add("f:" + HAP.Data.MyFiles.File.GetMimeType(s.Trim().ToLower().Remove(0, 1)));
                return " " + string.Join(" ", filters.ToArray());
            }
        }


        private bool isAuth(Filter filter)
        {
            if (filter.EnableFor == "All") return true;
            else if (filter.EnableFor != "None")
            {
                bool vis = false;
                foreach (string s in filter.EnableFor.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    if (!vis) vis = Page.User.IsInRole(s.Trim());
                return vis;
            }
            return false;
        }

        protected void uploadbtn_Click(object sender, EventArgs e)
        {
            ADUser.Impersonate();
            DriveMapping mapping;
            string path = HAP.Data.ComputerBrowser.Converter.DriveToUNC(p.Value, out mapping);
            if (isAuth(uploadedfiles.PostedFile.FileName)) uploadedfiles.PostedFile.SaveAs(Path.Combine(path, uploadedfiles.PostedFile.FileName));
            ADUser.EndImpersonate();
        }

        private bool isAuth(string extension)
        {
            foreach (Filter filter in config.MySchoolComputerBrowser.Filters)
                if (filter.Expression.Contains(extension)) return true;
            return isAuth(config.MySchoolComputerBrowser.Filters.Single(fil => fil.Name == "All Files"));
        }
    }
}