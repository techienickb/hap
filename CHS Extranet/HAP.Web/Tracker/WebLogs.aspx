<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="WebLogs.aspx.cs" Inherits="HAP.Web.Tracker.WebLogs" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="tracker.css" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]><script language="javascript" type="text/javascript" src="../scripts/excanvas.js"></script><![endif]-->
    <script type="text/javascript" src="../scripts/jquery.jqplot.min.js"></script>
    <script type="text/javascript" src="../Scripts/jqplot.dateAxisRenderer.min.js"></script>
    <script type="text/javascript" src="../Scripts/jqplot.highlighter.min.js"></script>
    <script type="text/javascript" src="../Scripts/jqplot.cursor.min.js"></script>
    <link rel="stylesheet" type="text/css" href="../style/jquery.jqplot.css" />
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
    <div id="chartdiv" style="height:300px;width:99%; "></div>
    <button onclick="plot1.resetZoom(); return false;">Reset Zoom</button>
    <script type="text/javascript">
        var line1 = [<%=Data%>];
    </script>
    <script type="text/javascript">
        var plot1;
        $(document).ready(function(){
            plot1 = $.jqplot('chartdiv', [line1], {
                title:'Overview Tracker Results',
                axes:{yaxis: { min: 0 }, xaxis:{ renderer:$.jqplot.DateAxisRenderer}},
                series:[{lineWidth:2, markerOptions:{ style: 'filledCircle', lineWidth: 1, size: 4 }}],
                highlighter: { show: true }, 
                cursor: { show: true, zoom: true }
            });
        });
    </script>
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
