using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Security.Cryptography;

namespace HAP.Config
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).FullName, "web.config"));

            XmlNode hapConfig = doc.SelectSingleNode("/configuration/hapConfig");
            #region Base Settings
            XmlNode basesettings = hapConfig.SelectSingleNode("basesettings");

            basesettings.Attributes["establishmentname"].Value = base_Name.Text;
            basesettings.Attributes["establishmentcode"].Value = base_Code.Text;
            basesettings.Attributes["studentphotohandler"].Value = base_StudentPhotoEnable.Checked ? base_StudentPhoto.Text : string.Empty;
            basesettings.Attributes["adminemailaddress"].Value = base_AdminEmail.Text;
            basesettings.Attributes["adminemailuser"].Value = base_AdminUsername.Text;
            basesettings.Attributes["studentemailformat"].Value = base_StudentEmailFormat.Text;
            basesettings.Attributes["smtpserver"].Value = base_SMTPAddress.Text;
            basesettings.Attributes["smtpserverport"].Value = base_SMTPPort.Text;
            basesettings.Attributes["smtpserverssl"].Value = base_SMTPSSL.Checked.ToString();
            basesettings.Attributes["smtpserverusername"].Value = base_SMTPAuth.Checked ? base_SMTPUsername.Text : string.Empty;
            basesettings.Attributes["smtpserverpassword"].Value = base_SMTPAuth.Checked ? base_SMTPPassword.Text : string.Empty;
            doc.SelectSingleNode("/configuration/system.web/authorization/allow").Attributes["roles"].Value = string.Format("{0} Easylink", base_Code.Text);
            #endregion

            #region AD Settings
            XmlNode adsettings = hapConfig.SelectSingleNode("adsettings");
            XmlNode constring = doc.SelectSingleNode("/configuration/connectionStrings/add[@name='ADConnectionString']");

            constring.Attributes["connectionString"].Value = string.Format("LDAP://{0}/DC={1}", ad_dc.Text, ad_domainname.Text.Replace(".", ",DC="));
            adsettings.Attributes["adusername"].Value = doc.SelectSingleNode("/configuration/system.web/membership/providers/add").Attributes["connectionUsername"].Value = doc.SelectSingleNode("/configuration/system.web/roleManager/providers/add").Attributes["connectionUsername"].Value = ad_Username.Text;
            adsettings.Attributes["adpassword"].Value = doc.SelectSingleNode("/configuration/system.web/membership/providers/add").Attributes["connectionPassword"].Value = doc.SelectSingleNode("/configuration/system.web/roleManager/providers/add").Attributes["connectionPassword"].Value = ad_Password.Text;
            adsettings.Attributes["studentsgroupname"].Value = ad_Student.Text;
            #endregion

            #region Home Page Links
            XmlNode hpl = hapConfig.SelectSingleNode("homepagelinks");
            hpl.RemoveAll();
            XmlElement umd = doc.CreateElement("add");
            umd.SetAttribute("name", "Update My Details");
            umd.SetAttribute("showto", hpl_updatedetails.Text);
            hpl.AppendChild(umd);
            foreach (DataGridViewRow row in homepagelinks.Rows)
            {
                if (!row.IsNewRow)
                {
                    XmlElement el = doc.CreateElement("add");
                    el.SetAttribute("name", row.Cells[0].Value.ToString());
                    el.SetAttribute("description", row.Cells[1].Value.ToString());
                    el.SetAttribute("showto", row.Cells[2].Value.ToString());
                    el.SetAttribute("linklocation", row.Cells[3].Value.ToString());
                    el.SetAttribute("icon", row.Cells[4].Value.ToString());
                    hpl.AppendChild(el);
                    if (row.Cells[0].Value.ToString() == "Help Desk")
                    {
                        XmlDocument doc1 = new XmlDocument();
                        doc1.Load(Path.Combine(Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).FullName, "HelpDesk", "web.config"));
                        doc1.SelectSingleNode("/configuration/system.web/authorization/allow").Attributes["roles"].Value = row.Cells[2].Value.ToString();
                        doc1.Save(Path.Combine(Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).FullName, "HelpDesk", "web.config"));                        
                    }
                    else if (row.Cells[0].Value.ToString() == "Booking System")
                    {
                        XmlDocument doc1 = new XmlDocument();
                        doc1.Load(Path.Combine(Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).FullName, "BookingSystem", "web.config"));
                        doc1.SelectSingleNode("/configuration/location/system.web/authorization/allow").Attributes["roles"].Value = row.Cells[2].Value.ToString();
                        doc1.Save(Path.Combine(Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).FullName, "BookingSystem", "web.config"));
                    }
                }
            }
            #endregion

            #region UNCPaths
            XmlNode uncp = hapConfig.SelectSingleNode("uncpaths");
            uncp.RemoveAll();
            foreach (DataGridViewRow row in uncpaths.Rows)
            {
                if (!row.IsNewRow)
                {
                    XmlElement el = doc.CreateElement("add");
                    el.SetAttribute("drive", row.Cells[0].Value.ToString());
                    el.SetAttribute("name", row.Cells[1].Value.ToString());
                    el.SetAttribute("unc", row.Cells[2].Value.ToString());
                    el.SetAttribute("enablereadto", row.Cells[3].Value.ToString());
                    el.SetAttribute("enablewriteto", row.Cells[4].Value.ToString());
                    uncp.AppendChild(el);
                }
            }
            #endregion

            #region Upload Filters
            XmlNode filters = hapConfig.SelectSingleNode("uploadfilters");
            filters.RemoveAll();
            foreach (DataGridViewRow row in uncpaths.Rows)
            {
                if (!row.IsNewRow)
                {
                    XmlElement el = doc.CreateElement("add");
                    el.SetAttribute("name", row.Cells[0].Value.ToString());
                    el.SetAttribute("filter", row.Cells[1].Value.ToString());
                    el.SetAttribute("enablefor", row.Cells[2].Value.ToString());
                    filters.AppendChild(el);
                }
            }
            #endregion

            #region AnnouncementBox
            XmlNode ab = hapConfig.SelectSingleNode("announcementbox");
            ab.Attributes["enableeditto"].Value = announcementBox_EditTo.Text;
            ab.Attributes["showto"].Value = announcementBox_ShowTo.Text;
            #endregion

            #region Booking System
            XmlNode bs = hapConfig.SelectSingleNode("bookingsystem");
            bs.Attributes["maxbookingsperweek"].Value = bs_max.Value.ToString();
            bs.Attributes["maxdays"].Value = bs_maxdays.Value.ToString();
            bs.Attributes["twoweektimetable"].Value = bs_twoweek.Checked.ToString();

            XmlNode res = bs.SelectSingleNode("resources");
            res.RemoveAll();
            foreach (DataGridViewRow row in Resources.Rows)
            {
                if (!row.IsNewRow)
                {
                    XmlElement el = doc.CreateElement("add");
                    el.SetAttribute("name", row.Cells[0].Value.ToString());
                    el.SetAttribute("type", row.Cells[1].Value.ToString());
                    if (bool.Parse(row.Cells[2].Value.ToString()))
                        el.SetAttribute("emailadmin", row.Cells[2].Value.ToString());
                    if (bool.Parse(row.Cells[3].Value.ToString()))
                        el.SetAttribute("enablecharging", row.Cells[3].Value.ToString());
                    res.AppendChild(el);
                }
            }

            XmlNode lessons = bs.SelectSingleNode("lessons");
            lessons.RemoveAll();
            foreach (DataGridViewRow row in Resources.Rows)
            {
                if (!row.IsNewRow)
                {
                    XmlElement el = doc.CreateElement("add");
                    el.SetAttribute("name", row.Cells[0].Value.ToString());
                    if (row.Cells[1].Value.ToString() != "Lesson")
                        el.SetAttribute("type", row.Cells[1].Value.ToString());
                    el.SetAttribute("starttime", row.Cells[2].Value.ToString());
                    el.SetAttribute("endtime", row.Cells[3].Value.ToString());
                    lessons.AppendChild(el);
                }
            }
            #endregion

            doc.Save(Path.Combine(Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).FullName, "web.config"));
            MessageBox.Show("Saved", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void base_SMTPAuth_CheckedChanged(object sender, EventArgs e)
        {
            base_SMTPUsername.Enabled = base_SMTPPassword.Enabled = base_SMTPAuth.Checked;
        }

        private void ad_domainname_KeyUp(object sender, KeyEventArgs e)
        {
            ad_Username.Text = string.Format("{0}\\Administrator", ad_domainname.Text.Split(new char[] { '.' })[0]);
        }

        private void base_Code_KeyUp(object sender, KeyEventArgs e)
        {
            ad_Student.Text = string.Format("{0} Students", base_Code.Text);
        }

        private void base_StudentPhotoEnable_CheckedChanged(object sender, EventArgs e)
        {
            base_StudentPhoto.Enabled = base_StudentPhotoEnable.Checked;
        }

        private void ad_dc_KeyUp(object sender, KeyEventArgs e)
        {
            if (ad_dc.Text.Contains('.'))
            {
                ad_domainname.Text = ad_dc.Text.Remove(0, ad_dc.Text.IndexOf('.') + 1);
                ad_Username.Text = string.Format("{0}\\Administrator", ad_domainname.Text.Split(new char[] { '.' })[0]);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).FullName, "web.config"));

            XmlNode hapConfig = doc.SelectSingleNode("/configuration/hapConfig");
            #region Base Settings
            XmlNode basesettings = hapConfig.SelectSingleNode("basesettings");

            base_Name.Text = basesettings.Attributes["establishmentname"].Value;
            base_Code.Text = basesettings.Attributes["establishmentcode"].Value;
            base_StudentPhoto.Text = basesettings.Attributes["studentphotohandler"].Value;
            base_StudentPhotoEnable.Checked = base_StudentPhoto.Text != string.Empty;
            base_AdminEmail.Text = basesettings.Attributes["adminemailaddress"].Value;
            base_AdminUsername.Text = basesettings.Attributes["adminemailuser"].Value;
            base_StudentEmailFormat.Text = basesettings.Attributes["studentemailformat"].Value;
            base_SMTPAddress.Text = basesettings.Attributes["smtpserver"].Value;
            base_SMTPPort.Text = basesettings.Attributes["smtpserverport"].Value;
            base_SMTPSSL.Checked = bool.Parse(basesettings.Attributes["smtpserverssl"].Value);
            base_SMTPUsername.Text = basesettings.Attributes["smtpserverusername"].Value;
            base_SMTPPassword.Text = basesettings.Attributes["smtpserverpassword"].Value;
            base_SMTPAuth.Checked = (base_SMTPUsername.Text != string.Empty);
            #endregion

            #region AD Settings
            XmlNode adsettings = hapConfig.SelectSingleNode("adsettings");
            XmlNode constring = doc.SelectSingleNode("/configuration/connectionStrings/add[@name='ADConnectionString']");

            ad_dc.Text = constring.Attributes["connectionString"].Value.Substring(7, constring.Attributes["connectionString"].Value.LastIndexOf('/'));
            ad_domainname.Text = constring.Attributes["connectionString"].Value.Remove(0, constring.Attributes["connectionString"].Value.LastIndexOf('/') + 1).Replace("DC=", "").Replace(",", ".");
            ad_Username.Text = adsettings.Attributes["adusername"].Value;
            ad_Password.Text = adsettings.Attributes["adpassword"].Value;
            ad_Student.Text = adsettings.Attributes["studentsgroupname"].Value;
            #endregion

            #region Home Page Links
            XmlNode hpl = hapConfig.SelectSingleNode("homepagelinks");
            foreach (XmlNode node in hpl.SelectNodes("add"))
                if (node.Attributes["name"].Value == "Update My Details")
                    hpl_updatedetails.Text = node.Attributes["showto"].Value;
                else
                    homepagelinks.Rows.Add(node.Attributes["name"].Value, node.Attributes["description"].Value, node.Attributes["showto"].Value, node.Attributes["linklocation"].Value, node.Attributes["icon"].Value);
            #endregion

            #region UNC Paths
            XmlNode uncp = hapConfig.SelectSingleNode("uncpaths");
            foreach (XmlNode node in uncp.SelectNodes("add"))
                uncpaths.Rows.Add(node.Attributes["drive"].Value, node.Attributes["name"].Value, node.Attributes["unc"].Value, node.Attributes["enablereadto"].Value, node.Attributes["enablewriteto"].Value);
            #endregion

            #region Upload Filters
            XmlNode uf = hapConfig.SelectSingleNode("uploadfilters");
            foreach (XmlNode node in uf.SelectNodes("add"))
                uploadfilters.Rows.Add(node.Attributes["name"].Value, node.Attributes["filter"].Value, node.Attributes["enablefor"].Value);
            #endregion

            #region AnnouncementBox
            XmlNode ab = hapConfig.SelectSingleNode("announcementbox");
            announcementBox_EditTo.Text = ab.Attributes["enableeditto"].Value;
            announcementBox_ShowTo.Text = ab.Attributes["showto"].Value;
            #endregion

            #region Booking System
            XmlNode bs = hapConfig.SelectSingleNode("bookingsystem");
            bs_max.Value = int.Parse(bs.Attributes["maxbookingsperweek"].Value);
            bs_maxdays.Value = int.Parse(bs.Attributes["maxdays"].Value);
            bs_twoweek.Checked = bool.Parse(bs.Attributes["twoweektimetable"].Value);
            foreach (XmlNode node in bs.SelectNodes("resources/add"))
                Resources.Rows.Add(node.Attributes["name"].Value, node.Attributes["type"].Value, node.Attributes["emailadmin"] != null ? bool.Parse(node.Attributes["emailadmin"].Value) : false, node.Attributes["enablecharging"] != null ? bool.Parse(node.Attributes["enablecharging"].Value) : false);
            foreach (XmlNode node in bs.SelectNodes("lessons/add"))
                lessons.Rows.Add(node.Attributes["name"].Value, node.Attributes["type"] == null ? "Lesson" : node.Attributes["type"].Value, node.Attributes["starttime"].Value, node.Attributes["endtime"].Value);
            #endregion

        }

    }
}
