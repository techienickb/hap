<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.Tracker.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../mycomputer.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="text-align: center;"><a href="http://hap.codeplex.com" target="_blank"><img src="logontracker.png" alt="Logon Tracker" /></a></div>
    <p id="HomeButtons">
        <a href="live.aspx">
            <img src="../images/icons/tracker-live.png" alt="" />
            Live Tracker
            <i>View the live action from the tracker</i>
        </a>
        <a href="logs.aspx">
            <img src="../images/icons/tracker-historic.png" alt="" />
            Historic Logs
            <i>Drill down through the Historic Logs</i>
        </a>
        <asp:HyperLink ID="dbup" runat="server" Visible="false" NavigateUrl="~/Tracker/XML2SQL.aspx">
            <img src="../images/icons/dbup.png" alt="" />
            XML 2 SQL
            <i>Upgrade from XML to a SQL Database</i>
        </asp:HyperLink>
        <a href="../">
            <img src="../images/icons/school.png" alt="" />
            Home Access Plus+ Home
            <i>Go Back to the Home Access Plus+ Homepage</i>
        </a>
    </p>
</asp:Content>
