using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace CHS_Extranet.Controls
{
    public partial class Delete : System.Web.UI.UserControl
    {
        public DirectoryInfo Dir { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void yesdel_Click(object sender, EventArgs e)
        {
            bool isFile = deleteitem.Value.StartsWith("F!");
            if (isFile) File.Delete(Path.Combine(Dir.FullName, deleteitem.Value.Remove(0, 2)));
            else Directory.Delete(Path.Combine(Dir.FullName, deleteitem.Value), true);
            Page.DataBind();
        }
    }
}