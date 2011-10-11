<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.Tracker.Default" %>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div style="text-align: center;"><a href="http://hap.codeplex.com" target="_blank"><img src="logontracker.png" alt="Logon Tracker" style="width: 700px;" /></a></div>
	<div id="HomeButtons" class="tiles">
		<a href="../" title="Go Back to the Home Access Plus+ Homepage">
			<span style="background-image: url(../images/icons/white/school.png);"></span>
			HAP+ Home
		</a>
		<a href="live.aspx" title="View the live action from the tracker">
			<span style="background-image: url(../images/icons/white/tracker-live.png);"></span>
			Live Tracker
		</a>
		<a href="logs.aspx" title="Drill down through the Historic Logs">
			<span style="background-image: url(../images/icons/white/tracker-historic.png");"></span>
			Historic Logs
		</a>
		<asp:HyperLink ID="dbup" runat="server" Visible="false" NavigateUrl="~/Tracker/XML2SQL.aspx" ToolTip="Upgrade from XML to a SQL Database">
			<span style="background-image: url(../images/icons/white/dbup.png);"></span>
			XML 2 SQL
		</asp:HyperLink>
	</div>
</asp:Content>
