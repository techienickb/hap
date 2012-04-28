<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.Tracker.Default" %>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div style="text-align: center;"><a href="http://hap.codeplex.com" target="_blank"><img src="logontracker.png" alt="Logon Tracker" style="width: 700px;" /></a></div>
	<div id="HomeButtons" class="tiles">
		<a href="../" title="Go Back to the Home Access Plus+ Homepage">
			<span><i style="background-image: url(../images/icons/white/school.png);"></i></span>
			HAP+ Home
		</a>
		<a href="live.aspx" title="View the live action from the tracker">
			<span><i style="background-image: url(../images/icons/white/tracker-live.png);"></i></span>
			Live Tracker
		</a>
		<a href="logs.aspx" title="Drill down through the Historic Logs">
			<span><i style="background-image: url(../images/icons/white/tracker-historic.png");"></i></span>
			Historic Logs
		</a>
		<asp:HyperLink ID="dbup" runat="server" Visible="false" NavigateUrl="~/Tracker/XML2SQL.aspx" ToolTip="Upgrade from XML to a SQL Database">
			<span><i style="background-image: url(../images/icons/white/dbup.png);"></i></span>
			XML 2 SQL
		</asp:HyperLink>
		<asp:HyperLink ID="weblog" runat="server" Visible="false" NavigateUrl="~/Tracker/WebLogs.aspx" ToolTip="Web Logs">
			<span><i style="background-image: url(../images/icons/white/tracker-historic.png");"></i></span>
			Web Tracker
		</asp:HyperLink>
	</div>
</asp:Content>
