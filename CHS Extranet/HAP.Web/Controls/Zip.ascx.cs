using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Web.Security;

namespace HAP.Web.Controls
{
    public partial class Zip : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public DirectoryInfo Dir { get; set; }

        protected void ok_Click(object sender, EventArgs e)
        {
            ((HAP.AD.User)Membership.GetUser()).Impersonate();
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Dir.FullName, zipitem.Value));
            FastZip fastZip = new FastZip();
            fastZip.CreateZip(dir.FullName.TrimEnd(new char[] { '\\' }) + ".zip", dir.FullName, true, "");
            ((HAP.AD.User)Membership.GetUser()).EndImpersonate();
            Page.DataBind();
        }
    }
}