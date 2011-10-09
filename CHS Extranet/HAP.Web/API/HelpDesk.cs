using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using System.Xml;
using HAP.AD;
using System.DirectoryServices.AccountManagement;
using HAP.Data.HelpDesk;

namespace HAP.Web.API
{
    [ServiceContract(Namespace = "HAP.Web.API")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class HelpDesk
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/D", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string DoSomething(string data)
        {
            return HttpUtility.UrlEncode(HttpUtility.UrlDecode(data, System.Text.Encoding.Default), System.Text.Encoding.Default);
        }

        [OperationContract]
        [WebGet(UriTemplate = "Tickets/{State}", ResponseFormat = WebMessageFormat.Json)]
        public Ticket[] AllTickets(string State)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            List<Ticket> tickets = new List<Ticket>();
            string xpath = string.Format("/Tickets/Ticket[@status{0}]", State == "Open" ? "!='Fixed'" : "='Fixed'");
            foreach (XmlNode node in doc.SelectNodes(xpath))
                tickets.Add(new Ticket(node));
            return tickets.ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate="Tickets/{State}/{Username}", ResponseFormat=WebMessageFormat.Json)]
        public Ticket[] Tickets(string State, string Username)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            List<Ticket> tickets = new List<Ticket>();
            string xpath = string.Format("/Tickets/Ticket[@status{0}]", State == "Open" ? "!='Fixed'" : "='Fixed'");

            foreach (XmlNode node in doc.SelectNodes(xpath))
                if (node.SelectNodes("Note")[0].Attributes["username"].Value.ToLower() == Username.ToLower())
                    tickets.Add(new Ticket(node));
            return tickets.ToArray();
        }

        [OperationContract]
        [WebGet(UriTemplate = "Ticket/{TicketId}", ResponseFormat = WebMessageFormat.Json)]
        public FullTicket Ticket(string TicketId)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/Tickets.xml"));
            return new FullTicket(doc.SelectSingleNode("/Tickets/Ticket[@id='" + TicketId + "']"));
        }
    }
}