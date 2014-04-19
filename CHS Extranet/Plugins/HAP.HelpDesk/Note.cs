using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HAP.AD;
using System.Web;

namespace HAP.HelpDesk
{
    public class Note
    {
        public Note() { }
        public string Date { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string NoteText { get; set; }
        public bool Hide { get; set; }

        public Note(XmlNode node)
        {
            NoteText = HttpUtility.UrlEncode(node.InnerXml.Replace("<![CDATA[", "").Replace("]]>", "").Replace("\n", "<br />"), System.Text.Encoding.Default);
            if (node.Attributes["date"] != null && node.Attributes["time"] != null)
                Date = DateTime.Parse(node.Attributes["date"].Value + " " + node.Attributes["time"].Value).ToString("dd/MM/yy HH:mm");
            else Date = DateTime.Parse(node.Attributes["datetime"].Value).ToString("dd/MM/yy HH:mm");
            Username = node.Attributes["username"].Value;
            try
            {
                DisplayName = ADUtils.FindUserInfos(node.Attributes["username"].Value)[0].DisplayName;
            }
            catch { DisplayName = "UNKNOWN"; }
            Hide = (node.Attributes["hide"] != null) ? bool.Parse(node.Attributes["hide"].Value) : false;
        }

        public Note(Data.SQL.Note n)
        {
            NoteText = HttpUtility.UrlEncode(n.Content.Replace("<![CDATA[", "").Replace("]]>", "").Replace("\n", "<br />"), System.Text.Encoding.Default);
            Date = n.DateTime.ToString("dd/MM/yy HH:mm");
            Username = n.Username;
            try
            {
                DisplayName = ADUtils.FindUserInfos(n.Username)[0].DisplayName;
            }
            catch { DisplayName = "UNKNOWN"; }
            Hide = n.Hide;
        }
    }
}
