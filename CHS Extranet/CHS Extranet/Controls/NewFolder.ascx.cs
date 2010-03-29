using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using CHS_Extranet.Configuration;
using System.IO;
using System.DirectoryServices.AccountManagement;

namespace CHS_Extranet.Controls
{
    public partial class NewFolder : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) foldername.Text = string.Empty;
        }

        public string Username
        {
            get
            {
                if (Page.User.Identity.Name.Contains('\\'))
                    return Page.User.Identity.Name.Remove(0, Page.User.Identity.Name.IndexOf('\\') + 1);
                else return Page.User.Identity.Name;
            }
        }

        public DirectoryInfo Directory { get; set; }

        protected void yes_Click(object sender, EventArgs e)
        {
            Directory.CreateSubdirectory(foldername.Text);
            Page.DataBind();
        }
    }
}