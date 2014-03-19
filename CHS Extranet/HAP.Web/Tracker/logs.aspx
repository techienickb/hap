<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="logs.aspx.cs" Inherits="HAP.Web.Tracker.logs" %>
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
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/tracker/logs.aspx"><hap:LocalResource runat="server" StringPath="tracker/historiclogs" /></asp:HyperLink></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/tracker/live.aspx"><hap:LocalResource runat="server" StringPath="tracker/livelogons" /></asp:HyperLink>
    <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/tracker/logs.aspx"><hap:LocalResource runat="server" StringPath="tracker/historiclogs" /></asp:HyperLink>
    <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/tracker/weblogs.aspx"><hap:LocalResource runat="server" StringPath="tracker/weblogs" /></asp:HyperLink>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <header class="commonheader">
		<div>
			<a href="<%:ResolveClientUrl("~/tracker") %>"><hap:LocalResource StringPath="tracker/historiclogs" runat="server" /></a>
		</div>
	</header>
    <div id="chartdiv" style="height:300px;width:99%; "></div>
    <div>
        <button onclick="plot1.resetZoom(); return false;">Reset Zoom</button>
        <input type="date" id="datepicker" />
        <button id="load">Load Month</button>
        <button id="loadday">Load Day</button>
    </div>
    <div id="datagrid"></div>
    <script type="text/javascript">
        var line1 = [<%=Data%>];
    </script>
    <script type="text/javascript">
        var plot1;
        $("#load").click(function () {
            $.getJSON("../api/tracker/" + $('#datepicker').datepicker("getDate").getFullYear() + "/" + ($('#datepicker').datepicker("getDate").getMonth() + 1), function (data) {
                plot1.destroy();
                plot1 = $.jqplot('chartdiv', [data.LineData], {
                    title: (parseInt($("#monthpicker .ui-datepicker-month :selected").val()) + 1) + '/' + $("#monthpicker .ui-datepicker-year :selected").val() + ' Tracker Results',
                    axes: { yaxis: { min: 0 }, xaxis: { renderer: $.jqplot.DateAxisRenderer } },
                    series: [{ lineWidth: 2, markerOptions: { style: 'filledCircle', lineWidth: 1, size: 4 } }],
                    highlighter: { show: true },
                    cursor: { show: true, zoom: true }
                });
                $('#datagrid').html('<table cellpadding="0" cellspacing="0" border="0" class="display" id="datatable"></table>');
                $('#datatable').dataTable({
                    "aaData": data.Data,
                    "aoColumns": [
                        { "sTitle": "Computer" },
                        { "sTitle": "IP" },
                        { "sTitle": "User" },
                        { "sTitle": "Domain" },
                        { "sTitle": "Logon Server" },
                        { "sTitle": "Operating System" },
                        { "sTitle": "Logon" },
                        { "sTitle": "Logoff" }
                    ],
                    "bPaginate": false,
                    "aaSorting": [[6, "asc"]]
                });
            });
            return false;
        });
        $("#loadday").click(function () {
            $.getJSON("../api/tracker/" + $('#datepicker').datepicker("getDate").getFullYear() + "/" + ($('#datepicker').datepicker("getDate").getMonth() + 1) + "/" + $('#datepicker').datepicker("getDate").getDate(), function (data) {
                plot1.destroy();
                plot1 = $.jqplot('chartdiv', [data.LineData], {
                    title: $('#datepicker').datepicker("getDate").getDate() + "/" + ($('#datepicker').datepicker("getDate").getMonth() + 1) + "/" + $('#datepicker').datepicker("getDate").getFullYear() + ' Tracker Results',
                    axes: { yaxis: { min: 0 }, xaxis: { renderer: $.jqplot.DateAxisRenderer } },
                    series: [{ lineWidth: 2, markerOptions: { style: 'filledCircle', lineWidth: 1, size: 4 } }],
                    highlighter: { show: true },
                    cursor: { show: true, zoom: true }
                });
                $('#datagrid').html('<table cellpadding="0" cellspacing="0" border="0" class="display" id="datatable"></table>');
                $('#datatable').dataTable({
                    "aaData": data.Data,
                    "aoColumns": [
                        { "sTitle": "Computer" },
                        { "sTitle": "IP" },
                        { "sTitle": "User" },
                        { "sTitle": "Domain" },
                        { "sTitle": "Logon Server" },
                        { "sTitle": "Operating System" },
                        { "sTitle": "Logon" },
                        { "sTitle": "Logoff" }
                    ],
                    "bPaginate": false,
                    "aaSorting": [[6, "asc"]]
                });
            });
            return false;
        });
        $(document).ready(function () {
            $("button").button();
            $('#datepicker').val(new Date().getDate() + "/" + (new Date().getMonth() + 1) + "/" + new Date().getFullYear()).datepicker({
                changeMonth: true,
                minDate: new Date(line1[0][0].split(/\-/gi)[0], line1[0][0].split(/\-/gi)[1] - 1),
                maxDate: 0,
                changeYear: true,
                dateFormat: 'd/m/yy'
            });
            plot1 = $.jqplot('chartdiv', [line1], {
                title:'Overview Tracker Results',
                axes:{yaxis: { min: 0 }, xaxis:{ renderer:$.jqplot.DateAxisRenderer}},
                series:[{lineWidth:2, markerOptions:{ style: 'filledCircle', lineWidth: 1, size: 4 }}],
                highlighter: { show: true }, 
                cursor: { show: true, zoom: true }
            });
        });
    </script>
</asp:Content>
