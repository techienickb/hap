<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="WebLogs.aspx.cs" Inherits="HAP.Web.Tracker.WebLogs" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
    <link href="tracker.css" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]><script language="javascript" type="text/javascript" src="../scripts/excanvas.js"></script><![endif]-->
    <script type="text/javascript" src="../scripts/jquery.jqplot.min.js"></script>
    <script type="text/javascript" src="../Scripts/jqplot.dateAxisRenderer.min.js"></script>
    <script type="text/javascript" src="../Scripts/jqplot.highlighter.min.js"></script>
    <script type="text/javascript" src="../Scripts/jqplot.cursor.min.js"></script>
    <script src="../Scripts/jquery.dataTables.js"></script>
    <link href="../style/jquery.dataTables.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="../style/jquery.jqplot.css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/tracker/weblogs.aspx"><hap:LocalResource runat="server" StringPath="tracker/weblogs" /></asp:HyperLink></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
    <asp:HyperLink runat="server" NavigateUrl="~/tracker/live.aspx"><hap:LocalResource runat="server" StringPath="tracker/livelogons" /></asp:HyperLink>
    <asp:HyperLink runat="server" NavigateUrl="~/tracker/logs.aspx"><hap:LocalResource runat="server" StringPath="tracker/historiclogs" /></asp:HyperLink>
    <asp:HyperLink runat="server" NavigateUrl="~/tracker/weblogs.aspx"><hap:LocalResource runat="server" StringPath="tracker/weblogs" /></asp:HyperLink>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <header class="commonheader">
		<div>
			<a href="<%:ResolveClientUrl("~/tracker") %>"><hap:LocalResource StringPath="tracker/weblogs" runat="server" /></a>
		</div>
	</header>
    <div id="chartdiv" style="height:300px;width:99%; "></div>
    <div>
        <button onclick="plot1.resetZoom(); return false;">Reset Zoom</button>
        <label for="datepicker">Select Date:</label>
        <input type="date" id="datepicker" />
        <button id="loadmonth">Load Month</button><button id="loadday">Load Day</button>
    </div>
    <img src="../images/metroloading.gif" id="loading" />
    <div id="datagrid"></div>
    <script type="text/javascript">
        var line1 = [<%=Data%>];
    </script>
    <script type="text/javascript">
        var plot1;
        $("#loading").hide();
        $("#loadmonth").click(function () {
            $("#loading").show();
            $.getJSON("../api/tracker/web/" + $('#datepicker').datepicker("getDate").getFullYear() + "/" + ($('#datepicker').datepicker("getDate").getMonth() + 1), function (data) {
                plot1.destroy();
                plot1 = $.jqplot('chartdiv', [data], {
                    title: ($('#datepicker').datepicker("getDate").getMonth() + 1) + '/' + $('#datepicker').datepicker("getDate").getFullYear() + ' Tracker Results',
                    axes: { yaxis: { min: 0 }, xaxis: { renderer: $.jqplot.DateAxisRenderer } },
                    series: [{ lineWidth: 2, markerOptions: { style: 'filledCircle', lineWidth: 1, size: 4 } }],
                    highlighter: { show: true },
                    cursor: { show: true, zoom: true }
                });
                $('#datagrid').html('');
                $("#loading").hide();
            });
            return false;
        });
        $("#loadday").click(function () {
            $("#loading").show();
            $.getJSON("../api/tracker/web/" + $('#datepicker').datepicker("getDate").getFullYear() + "/" + ($('#datepicker').datepicker("getDate").getMonth() + 1) + "/" + $('#datepicker').datepicker("getDate").getDate(), function (data) {
                plot1.destroy();
                plot1 = $.jqplot('chartdiv', [data.LineData], {
                    title: $('#datepicker').datepicker("getDate").getDate() + '/' + ($('#datepicker').datepicker("getDate").getMonth() + 1) + '/' + $('#datepicker').datepicker("getDate").getFullYear() + ' Tracker Results',
                    axes: { yaxis: { min: 0 }, xaxis: { renderer: $.jqplot.DateAxisRenderer } },
                    series: [{ lineWidth: 2, markerOptions: { style: 'filledCircle', lineWidth: 1, size: 4 } }],
                    highlighter: { show: true },
                    cursor: { show: true, zoom: true }
                });
                $('#datagrid').html('<table cellpadding="0" cellspacing="0" border="0" class="display" id="datatable"></table>');
                $('#datatable').dataTable({
                    "aaData": data.Data,
                    "aoColumns": [
                        { "sTitle": "Time" },
                        { "sTitle": "Type" },
                        { "sTitle": "IP" },
                        { "sTitle": "User" },
                        { "sTitle": "Browser" },
                        { "sTitle": "OS" },
                        { "sTitle": "Event Details" },
                    ],
                    "bPaginate": false,
                    "bAutoWidth": false,
                    "aaSorting": [[0, "asc"]]
                });
                $("#loading").hide();
            });
            return false;
        });
        $(document).ready(function () {
            $("button").button();
            $('#datepicker').datepicker({
                changeMonth: true,
                minDate: new Date(line1[0][0].split(/\-/gi)[0], line1[0][0].split(/\-/gi)[1] - 1),
                maxDate: 0,
                defaultDate: new Date(),
                changeYear: true,
                dateFormat: 'd MM y'
            });
            plot1 = $.jqplot('chartdiv', [line1], {
                title: 'Overview Tracker Results',
                axes: { yaxis: { min: 0 }, xaxis: { renderer: $.jqplot.DateAxisRenderer } },
                series: [{ lineWidth: 2, markerOptions: { style: 'filledCircle', lineWidth: 1, size: 4 } }],
                highlighter: { show: true },
                cursor: { show: true, zoom: true }
            });
        });
    </script>
</asp:Content>

