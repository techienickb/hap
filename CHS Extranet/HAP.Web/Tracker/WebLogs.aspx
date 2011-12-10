<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="WebLogs.aspx.cs" Inherits="HAP.Web.Tracker.WebLogs" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="tracker.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both; position: relative; height: 120px">
		<div class="tiles" style="position: absolute; left: 0; margin-top: 45px;">
			<a class="button" href="../">Home Access Plus+ Home</a>
		</div>
		<div style="text-align: center; margin-left: 60px;">
			<a href="./">HAP+ Web <img src="logontracker-small.png" style="vertical-align: middle;" alt="Logon Tracker" /></a>
		</div>
	</div>
	<asp:Repeater ID="dates" runat="server">
		<HeaderTemplate><ul></HeaderTemplate>
		<ItemTemplate><li><a href="web/<%#Eval("Year")%>/<%#Eval("Month") %>/"><%#((DateTime)Container.DataItem).ToString("MMMM yyyy") %></a></li></ItemTemplate>
		<FooterTemplate></ul></FooterTemplate>
	</asp:Repeater>
	<script type="text/javascript">
	    $(function () {
	        $("button").button();
	        $("input[type=submit]").button();
	        $(".button").button();
	    });
	</script>
</asp:Content>
