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
using System.DirectoryServices;
using System.Diagnostics;

namespace HAP.Config
{
    public partial class FirstRun : Form
    {
        public FirstRun()
        {
            InitializeComponent();
        }

        private void wiz_Cancelled(object sender, EventArgs e)
        {
            Close();
        }

        private void wiz_Finished(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode hapConfig = doc.SelectSingleNode("/configuration/hapConfig");
            #region Base Settings
            XmlNode basesettings = hapConfig.SelectSingleNode("basesettings");

            basesettings.Attributes["establishmentname"].Value = base_Name.Text;
            basesettings.Attributes["establishmentcode"].Value = base_Code.Text;
            basesettings.Attributes["studentphotohandler"].Value = base_StudentPhotoEnable.Checked ? base_StudentPhoto.Text : string.Empty;
            basesettings.Attributes["adminemailaddress"].Value = base_AdminEmail.Text;
            basesettings.Attributes["adminemailuser"].Value = base_AdminUsername.Text;
            basesettings.Attributes["smtpserver"].Value = base_SMTPAddress.Text;
            basesettings.Attributes["smtpserverport"].Value = base_SMTPPort.Text;
            basesettings.Attributes["smtpserverssl"].Value = base_SMTPSSL.Checked.ToString();
            basesettings.Attributes["smtpserverusername"].Value = base_SMTPAuth.Checked ? base_SMTPUsername.Text : string.Empty;
            basesettings.Attributes["smtpserverpassword"].Value = base_SMTPAuth.Checked ? base_SMTPPassword.Text : string.Empty;
            doc.SelectSingleNode("/configuration/system.web/authorization/allow").Attributes["roles"].Value = rmcc3.Checked ? string.Format("{0} Easylink", base_Code.Text) : "Domain Users";
            try
            {
                doc.Save(path);
            }
            catch (Exception ex) { MessageBox.Show("I've run into a problem saving the Base Settings\nError:\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            #endregion

            #region AD Settings
            XmlNode adsettings = hapConfig.SelectSingleNode("adsettings");
            XmlNode constring = doc.SelectSingleNode("/configuration/connectionStrings/add[@name='ADConnectionString']");

            constring.Attributes["connectionString"].Value = string.Format("LDAP://{0}/DC={1}", ad_dc.Text, ad_domainname.Text.Replace(".", ",DC="));
            adsettings.Attributes["adusername"].Value = doc.SelectSingleNode("/configuration/system.web/membership/providers/add").Attributes["connectionUsername"].Value = doc.SelectSingleNode("/configuration/system.web/roleManager/providers/add").Attributes["connectionUsername"].Value = ad_Username.Text;
            adsettings.Attributes["adpassword"].Value = doc.SelectSingleNode("/configuration/system.web/membership/providers/add").Attributes["connectionPassword"].Value = doc.SelectSingleNode("/configuration/system.web/roleManager/providers/add").Attributes["connectionPassword"].Value = ad_Password.Text;
            adsettings.Attributes["studentsgroupname"].Value = ad_Student.Text;
            XmlNode ouobs = adsettings.SelectSingleNode("ouobjects");
            ouobs.RemoveAll();
            foreach (DataGridViewRow row in adous.Rows)
            {
                if (!row.IsNewRow)
                {
                    XmlElement el = doc.CreateElement("add");
                    el.SetAttribute("name", row.Cells[0].Value.ToString());
                    el.SetAttribute("path", row.Cells[1].Value.ToString());
                    if (Convert.ToBoolean(row.Cells[2].Value))
                        el.SetAttribute("ignore", row.Cells[2].Value.ToString());
                    ouobs.AppendChild(el);
                }
            }
            try
            {
                doc.Save(path);
            }
            catch (Exception ex) { MessageBox.Show("I've run into a problem saving the AD Settings\nError:\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            #endregion

            #region Tracker
            XmlNode tracker = hapConfig.SelectSingleNode("tracker");
            if (tracker == null)
            {
                XmlElement track = doc.CreateElement("tracker");
                track.SetAttribute("maxstudentlogons", "1");
                track.SetAttribute("maxstafflogons", "4");
                track.SetAttribute("provider", "XML");
                hapConfig.AppendChild(track);
                tracker = hapConfig.SelectSingleNode("tracker");
            }
            tracker.Attributes["maxstudentlogons"].Value = trackermaxstudent.Value.ToString();
            tracker.Attributes["maxstafflogons"].Value = trackermaxstaff.Value.ToString();
            tracker.Attributes["overridecode"].Value = trackeroverride.Text;
            XmlNode sqlconstring = doc.SelectSingleNode("/configuration/connectionStrings/add[@name='SQLConnectionString']");
            if (sqlconstring != null) sqlconstring.Attributes["connectionString"].Value = tracker_sqlconstring.Text;
            tracker.Attributes["provider"].Value = tracker_provider.SelectedIndex == 1 ? "XML" : "SQLConnectionString";
            try
            {
                doc.Save(path);
            }
            catch (Exception ex) { MessageBox.Show("I've run into a problem saving the Tracker Settings\nError:\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            #endregion

            #region MyComputer
            XmlNode mycomp = hapConfig.SelectSingleNode("mycomputer");
            mycomp.Attributes["hideextensions"].Value = mycomputer_exext.Text;
            XmlNode uncp = mycomp.SelectSingleNode("uncpaths");
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
                    if (Convert.ToBoolean(row.Cells[6].Value))
                        el.SetAttribute("enablemove", row.Cells[6].Value.ToString());
                    if (row.Cells[5].Value.ToString() == "Quota Data") el.SetAttribute("usage", "Quota");
                    uncp.AppendChild(el);
                }
            }
            XmlNode filters = mycomp.SelectSingleNode("uploadfilters");
            filters.RemoveAll();
            foreach (DataGridViewRow row in uploadfilters.Rows)
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

            XmlNode quotas = mycomp.SelectSingleNode("quotaservers");
            quotas.RemoveAll();
            foreach (DataGridViewRow row in quotaservers.Rows)
            {
                if (!row.IsNewRow)
                {
                    XmlElement el = doc.CreateElement("add");
                    el.SetAttribute("expression", row.Cells[0].Value.ToString());
                    el.SetAttribute("server", row.Cells[1].Value.ToString());
                    el.SetAttribute("drive", row.Cells[2].Value.ToString());
                    quotas.AppendChild(el);
                }
            }

            try
            {
                doc.Save(path);
            }
            catch (Exception ex) { MessageBox.Show("I've run into a problem saving the My Computer Settings\nError:\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            #endregion

            #region AnnouncementBox
            XmlNode ab = hapConfig.SelectSingleNode("announcementbox");
            ab.Attributes["enableeditto"].Value = announcementBox_EditTo.Text;
            ab.Attributes["showto"].Value = announcementBox_ShowTo.Text;
            ab.Attributes["proxyaddress"].Value = proxyaddress.Text;
            ab.Attributes["proxyport"].Value = proxyport.Value.ToString();

            try
            {
                doc.Save(path);
            }
            catch (Exception ex) { MessageBox.Show("I've run into a problem saving the Announcement Box Settings\nError:\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            #endregion

            #region Booking System
            XmlNode bs = hapConfig.SelectSingleNode("bookingsystem");
            bs.Attributes["maxbookingsperweek"].Value = bs_max.Value.ToString();
            bs.Attributes["maxdays"].Value = bs_maxdays.Value.ToString();
            bs.Attributes["twoweektimetable"].Value = bs_twoweek.Checked.ToString();
            bs.Attributes["keepxmlclean"].Value = bs_keepxmlclean.Checked.ToString();
            if (bs.Attributes["admingroups"] != null) bs.Attributes.RemoveNamedItem("admingroups");
            if (!string.IsNullOrWhiteSpace(bs_admingroups.Text))
            {
                XmlAttribute a = doc.CreateAttribute("admingroups");
                a.Value = bs_admingroups.Text;
                bs.Attributes.Append(a);
            }
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
                    if (!bool.Parse(row.Cells[4].Value.ToString()))
                        el.SetAttribute("enable", row.Cells[4].Value.ToString());
                    res.AppendChild(el);
                }
            }

            XmlNode les = bs.SelectSingleNode("lessons");
            les.RemoveAll();
            foreach (DataGridViewRow row in lessons.Rows)
            {
                if (!row.IsNewRow)
                {
                    XmlElement el = doc.CreateElement("add");
                    el.SetAttribute("name", row.Cells[0].Value.ToString());
                    if (row.Cells[1].Value.ToString() != "Lesson")
                        el.SetAttribute("type", row.Cells[1].Value.ToString());
                    else if (el.HasAttribute("type")) el.RemoveAttribute("type");
                    el.SetAttribute("starttime", row.Cells[2].Value.ToString());
                    el.SetAttribute("endtime", row.Cells[3].Value.ToString());
                    les.AppendChild(el);
                }
            }

            XmlNode subs = bs.SelectSingleNode("subjects");
            subs.RemoveAll();
            foreach (DataGridViewRow row in bssubjects.Rows)
            {
                if (!row.IsNewRow)
                {
                    XmlElement el = doc.CreateElement("add");
                    el.SetAttribute("name", row.Cells[0].Value.ToString());
                    subs.AppendChild(el);
                }
            }
            #endregion

            try
            {
                doc.Save(path);
            }
            catch (Exception ex) { MessageBox.Show("I've run into a problem saving the Booking System Settings\nError:\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            if (MessageBox.Show("Saved\nDo you want to close?", "Saved", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                Close();
        }

        private void base_SMTPAuth_CheckedChanged(object sender, EventArgs e)
        {
            base_SMTPUsername.Enabled = base_SMTPPassword.Enabled = base_SMTPAuth.Checked;
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

        }

        private string path;

        private void FirstRun_Load(object sender, EventArgs e)
        {
            path = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Application.ExecutablePath)).FullName, "web.config");
            if (!File.Exists(path))
            {
                if (folderBrowser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    path = Path.Combine(folderBrowser.SelectedPath, "web.config");
                if (!File.Exists(path))
                {
                    MessageBox.Show(this, "I'm unable to load the Home Access Plus+ Web.Config file, please run me again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode hapConfig = doc.SelectSingleNode("/configuration/hapConfig");
            #region Base Settings
            XmlNode basesettings = hapConfig.SelectSingleNode("basesettings");

            base_Name.Text = basesettings.Attributes["establishmentname"].Value;
            base_Code.Text = basesettings.Attributes["establishmentcode"].Value;
            base_StudentPhoto.Text = basesettings.Attributes["studentphotohandler"].Value;
            base_StudentPhotoEnable.Checked = base_StudentPhoto.Text != string.Empty;
            base_AdminEmail.Text = basesettings.Attributes["adminemailaddress"].Value;
            base_AdminUsername.Text = basesettings.Attributes["adminemailuser"].Value;
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

            ad_dc.Text = constring.Attributes["connectionString"].Value.Substring(7, constring.Attributes["connectionString"].Value.LastIndexOf('/') - 7);
            ad_domainname.Text = constring.Attributes["connectionString"].Value.Remove(0, constring.Attributes["connectionString"].Value.LastIndexOf('/') + 1).Replace("DC=", "").Replace(",", ".");
            ad_Username.Text = adsettings.Attributes["adusername"].Value;
            if (constring.Attributes["connectionString"].Value == "LDAP://chs01.crickhowell.internal/DC=crickhowell,DC=internal")
            {
                try
                {
                    ad_Username.Text = Environment.UserDomainName + "\\Administrator";
                    DirectoryEntry rootDSE = new DirectoryEntry("LDAP://" + Environment.GetEnvironmentVariable("logonserver").Remove(0, 2) + "/rootDSE", @"CRICKHOWELL\NICK", "airbusa320");
                    ad_domainname.Text = rootDSE.Properties["defaultNamingContext"].Value.ToString().Replace("DC=", ".");
                    ad_dc.Text = Environment.GetEnvironmentVariable("logonserver").Remove(0, 2) + "." + ad_domainname.Text;
                }
                catch { }
            }
            ad_Password.Text = adsettings.Attributes["adpassword"].Value;
            ad_Student.Text = adsettings.Attributes["studentsgroupname"].Value;

            XmlNode ouobs = adsettings.SelectSingleNode("ouobjects");
            foreach (XmlNode node in ouobs.SelectNodes("add"))
            {
                DataGridViewRow row = adous.Rows[adous.Rows.Add(node.Attributes["name"].Value, node.Attributes["path"].Value, node.Attributes["ignore"] == null ? false : true)];
                row.ContextMenuStrip = contextMenuStrip1;
            }

            #endregion

            #region Tracker
            XmlNode tracker = hapConfig.SelectSingleNode("tracker");
            XmlNode sqlconstring = doc.SelectSingleNode("/configuration/connectionStrings/add[@name='SQLConnectionString']");
            if (tracker != null)
            {
                trackermaxstaff.Value = int.Parse(tracker.Attributes["maxstafflogons"].Value);
                trackermaxstudent.Value = int.Parse(tracker.Attributes["maxstudentlogons"].Value);
                trackeroverride.Text = tracker.Attributes["overridecode"].Value;
                tracker_sqlconstring.Text = sqlconstring.Attributes["connectionString"].Value;
                if (tracker.Attributes["provider"].Value == "XML") tracker_provider.SelectedIndex = 1;
                else tracker_provider.SelectedIndex = 0;
            }
            #endregion

            #region MyComputer
            XmlNode mycomp = hapConfig.SelectSingleNode("mycomputer");
            mycomputer_exext.Text = mycomp.Attributes["hideextensions"].Value;  
            XmlNode uncp = mycomp.SelectSingleNode("uncpaths");
            foreach (XmlNode node in uncp.SelectNodes("add"))
            {
                DataGridViewRow row = uncpaths.Rows[uncpaths.Rows.Add(node.Attributes["drive"].Value, node.Attributes["name"].Value, node.Attributes["unc"].Value, node.Attributes["enablereadto"].Value, node.Attributes["enablewriteto"].Value, (node.Attributes["usage"] == null ? "Drive Space" : "Quota Data"), (node.Attributes["enablemove"] == null ? false : true))];
                row.ContextMenuStrip = contextMenuStrip1;
            }

            XmlNode uf = mycomp.SelectSingleNode("uploadfilters");
            foreach (XmlNode node in uf.SelectNodes("add"))
            {
                DataGridViewRow row = uploadfilters.Rows[uploadfilters.Rows.Add(node.Attributes["name"].Value, node.Attributes["filter"].Value, node.Attributes["enablefor"].Value)];
                row.ContextMenuStrip = contextMenuStrip1;
            }
                

            XmlNode qs = mycomp.SelectSingleNode("quotaservers");
            foreach (XmlNode node in qs.SelectNodes("add"))
            {
                DataGridViewRow row = quotaservers.Rows[quotaservers.Rows.Add(node.Attributes["expression"].Value, node.Attributes["server"].Value, node.Attributes["drive"].Value.ToUpper())];
                row.ContextMenuStrip = contextMenuStrip1;
            }
                
            #endregion

            #region AnnouncementBox
            XmlNode ab = hapConfig.SelectSingleNode("announcementbox");
            announcementBox_EditTo.Text = ab.Attributes["enableeditto"].Value;
            announcementBox_ShowTo.Text = ab.Attributes["showto"].Value;
            proxyaddress.Text = ab.Attributes["proxyaddress"].Value;
            proxyport.Value = int.Parse(ab.Attributes["proxyport"].Value);
            #endregion

            #region Booking System
            XmlNode bs = hapConfig.SelectSingleNode("bookingsystem");
            bs_max.Value = int.Parse(bs.Attributes["maxbookingsperweek"].Value);
            bs_maxdays.Value = int.Parse(bs.Attributes["maxdays"].Value);
            bs_twoweek.Checked = bool.Parse(bs.Attributes["twoweektimetable"].Value);
            if (bs.Attributes["admingroups"] != null) bs_admingroups.Text = bs.Attributes["admingroups"].Value;
            if (bs.Attributes["keepxmlclean"] != null) bs_keepxmlclean.Checked = bool.Parse(bs.Attributes["keepxmlclean"].Value);
            else bs_keepxmlclean.Checked = true;
            foreach (XmlNode node in bs.SelectNodes("resources/add"))
            {
                DataGridViewRow row = Resources.Rows[Resources.Rows.Add(node.Attributes["name"].Value, node.Attributes["type"].Value, node.Attributes["emailadmin"] != null ? bool.Parse(node.Attributes["emailadmin"].Value) : false, node.Attributes["enablecharging"] != null ? bool.Parse(node.Attributes["enablecharging"].Value) : false, node.Attributes["enable"] != null ? bool.Parse(node.Attributes["enable"].Value) : true)];
                row.ContextMenuStrip = contextMenuStrip1;
            }
                
            foreach (XmlNode node in bs.SelectNodes("lessons/add"))
            {
                DataGridViewRow row = lessons.Rows[lessons.Rows.Add(node.Attributes["name"].Value, node.Attributes["type"] == null ? "Lesson" : node.Attributes["type"].Value, node.Attributes["starttime"].Value, node.Attributes["endtime"].Value)];
                row.ContextMenuStrip = contextMenuStrip1;
            }
                
            foreach (XmlNode node in bs.SelectNodes("subjects/add"))
            {
                DataGridViewRow row = bssubjects.Rows[bssubjects.Rows.Add(node.Attributes["name"].Value)];
                row.ContextMenuStrip = contextMenuStrip1;
            }
                
            #endregion
        }

        private void tracker_provider_SelectedIndexChanged(object sender, EventArgs e)
        {
            tracker_sqlconstringlink.Visible = tracker_sqlconstring.Visible = tracker_provider.SelectedIndex == 0;
        }

        private void tracker_sqlconstringlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try { Process.Start("http://connectionstrings.com/sql-server-2008"); }
            catch { }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wiz.SelectedPage== wizardPage2)
                    foreach (DataGridViewCell cell in adous.SelectedCells)
                        try
                        {
                            adous.Rows.Remove(cell.OwningRow);
                        }
                        catch { }
            else if (wiz.SelectedPage == wizardPage4)
            {
                if (tabControl3.SelectedIndex == 0)
                    foreach (DataGridViewCell cell in uncpaths.SelectedCells)
                        try
                        {
                            uncpaths.Rows.Remove(cell.OwningRow);
                        }
                        catch { }
                else if (tabControl3.SelectedIndex == 1)
                    foreach (DataGridViewCell cell in uploadfilters.SelectedCells)
                        try
                        {
                            uploadfilters.Rows.Remove(cell.OwningRow);
                        }
                        catch { }
                else foreach (DataGridViewCell cell in quotaservers.SelectedCells)
                        try
                        {
                            quotaservers.Rows.Remove(cell.OwningRow);
                        }
                        catch { }
            }
            else if (wiz.SelectedPage == wizardPage6)
            {
                if (bookingsystem_Subjectsd.SelectedIndex == 0)
                    foreach (DataGridViewCell cell in Resources.SelectedCells)
                        try
                        {
                            Resources.Rows.Remove(cell.OwningRow);
                        }
                        catch { }
                else if (bookingsystem_Subjectsd.SelectedIndex == 1)
                    foreach (DataGridViewCell cell in lessons.SelectedCells)
                        try
                        {
                            lessons.Rows.Remove(cell.OwningRow);
                        }
                        catch { }
                else foreach (DataGridViewCell cell in bssubjects.SelectedCells)
                        try
                        {
                            bssubjects.Rows.Remove(cell.OwningRow);
                        }
                        catch { }
            }

        }

    }
}
