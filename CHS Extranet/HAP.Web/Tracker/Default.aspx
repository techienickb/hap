<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.Tracker.Default" %>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/tracker/"><hap:LocalResource runat="server" StringPath="tracker/logontracker" /></asp:HyperLink></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div id="HomeButtons" class="tiles" style="margin-left: 30%">
		<a href="live.aspx" title="View the live action from the tracker">
			<span><i style="background-image: url(../images/icons/white/tracker-live.png);"></i></span>
			<hap:LocalResource StringPath="tracker/livelogons" runat="server" />
		</a>
		<a href="logs.aspx" title="Drill down through the Historic Logs">
			<span><i style="background-image: url(../images/icons/white/tracker-historic.png");"></i></span>
			<hap:LocalResource StringPath="tracker/historiclogs" runat="server" />
		</a>
		<asp:HyperLink ID="dbup" runat="server" Visible="false" NavigateUrl="~/Tracker/XML2SQL.aspx" ToolTip="Upgrade from XML to a SQL Database">
			<span><i style="background-image: url(../images/icons/white/dbup.png);"></i></span>
			XML 2 SQL
		</asp:HyperLink>
		<asp:HyperLink ID="weblog" runat="server" Visible="false" NavigateUrl="~/Tracker/WebLogs.aspx" ToolTip="Web Logs">
			<span><i style="background-image: url(../images/icons/white/tracker-historic.png");"></i></span>
			<hap:LocalResource StringPath="tracker/webtracker" runat="server" />
		</asp:HyperLink>
	</div>
</asp:Content>
