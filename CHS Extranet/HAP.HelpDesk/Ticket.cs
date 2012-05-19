using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HAP.AD;
using System.Web;

namespace HAP.HelpDesk
{
    public class Ticket
    {
        public Ticket() { }
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public bool FAQ { get; set; }
        public string ShowTo { get; set; }

        public Ticket(XmlNode node)
        {
            Id = int.Parse(node.Attributes["id"].Value);
            Subject = node.Attributes["subject"].Value;
            Priority = node.Attributes["priority"].Value;
            Status = node.Attributes["status"].Value;
            if (node.SelectNodes("Note")[0].Attributes["date"] != null)
                Date = DateTime.Parse(node.SelectNodes("Note")[0].Attributes["date"].Value + " " + node.SelectNodes("Note")[0].Attributes["time"].Value).ToString("dd/MM/yy HH:mm");
            Date = DateTime.Parse(node.SelectNodes("Note")[0].Attributes["datetime"].Value).ToString("dd/MM/yy HH:mm");
            try
            {
                Username = ADUtils.FindUserInfos(node.SelectNodes("Note")[0].Attributes["username"].Value)[0].UserName;
            }
            catch { Username = "UNKNOWN"; }
            ShowTo = node.Attributes["showto"] == null ? "" : node.Attributes["showto"].Value;
            try 
            {
                DisplayName = ADUtils.FindUserInfos(node.SelectNodes("Note")[0].Attributes["username"].Value)[0].DisplayName;
            }
            catch { DisplayName = "UNKNOWN"; }
            FAQ = false;
            if (node.Attributes["faq"] != null) FAQ = bool.Parse(node.Attributes["faq"].Value);
        }
    }

    public class FullTicket : Ticket
    {
        public FullTicket() : base() { Notes = new List<Note>(); }
        public List<Note> Notes { get; set; }
        public FullTicket(XmlNode node) : base(node)
        {
            Notes = new List<Note>();
            foreach (XmlNode n in node.SelectNodes("Note"))
                Notes.Add(new Note(n));
        }
    }
}
