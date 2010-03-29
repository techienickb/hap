﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace CHS_Extranet.Controls
{
    public partial class Rename : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public DirectoryInfo Dir { get; set; }

        protected void yesren_Click(object sender, EventArgs e)
        {
            if (renameitem.Value.StartsWith("F!"))
            {
                FileInfo file = new FileInfo(Path.Combine(Dir.FullName, renameitem.Value.Remove(0, 2)));
                file.MoveTo(Path.Combine(file.Directory.FullName, newname.Text));
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(Path.Combine(Dir.FullName, renameitem.Value));
                dir.MoveTo(Path.Combine(dir.Parent.FullName, newname.Text));
            }
            Page.DataBind();
        }
    }
}