using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace HAP.Web.Controls
{
    public partial class Unzip : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public DirectoryInfo Dir { get; set; }

        protected void ok_Click(object sender, EventArgs e)
        {
            FileInfo file = new FileInfo(Path.Combine(Dir.FullName, unzipitem.Value.Remove(0, 2)));
            DirectoryInfo dir = file.Directory;
            if (unziptox.Checked) dir = dir.CreateSubdirectory(file.Name.Replace(file.Extension, ""));
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(file.FullName, dir.FullName, "");
            Page.DataBind();
        }
    }
}