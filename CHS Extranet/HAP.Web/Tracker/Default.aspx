<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.Tracker.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../mycomputer.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="text-align: center;"><a href="http://chsextranet.codeplex.com" target="_blank"><img src="logontracker.png" alt="Logon Tracker" /></a></div>
    <p id="HomeButtons">
        <a href="live.aspx">
            <img src="../images/icons/tracker-live.png" alt="" />
            Live Tracker
            <i>View the live action from the tracker</i>
        </a>
        <a href="log.aspx">
            <img src="../images/icons/tracker-historic.png" alt="" />
            Historic Logs
            <i>Drill down through the Historic Logs</i>
        </a>
        <a href="../">
            <img src="../images/icons/school.png" alt="" />
            Home Access Plus+ Home
            <i>Go Back to the Home Access Plus+ Homepage</i>
        </a>
    </p>
</asp:Content>
