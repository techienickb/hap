using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.Configuration;

namespace CHS_Extranet
{
    public partial class upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Xaml1.InitParameters = "UploadPage=FileUpload.ashx/" + Request.QueryString["path"] + "/,UploadChunkSize=131072";
        }
    }
}