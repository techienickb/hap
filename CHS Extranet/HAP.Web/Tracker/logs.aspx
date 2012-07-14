<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="logs.aspx.cs" Inherits="HAP.Web.Tracker.logs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="tracker.css" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]><script language="javascript" type="text/javascript" src="../scripts/excanvas.js"></script><![endif]-->
    <script type="text/javascript" src="../scripts/jquery.jqplot.min.js"></script>
    <script type="text/javascript" src="../Scripts/jqplot.dateAxisRenderer.min.js"></script>
    <script type="text/javascript" src="../Scripts/jqplot.highlighter.min.js"></script>
    <script type="text/javascript" src="../Scripts/jqplot.cursor.min.js"></script>
    <link rel="stylesheet" type="text/css" href="../style/jquery.jqplot.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="text-align: center;">Historic <a href="./"><img src="logontracker-small.png" style="vertical-align: middle;" alt="Logon Tracker" /></a> <asp:Button runat="server" ID="archive" Text="Archive Logs" /></div>
    <asp:Panel runat="server" ID="archivelogs" style="display: none;" CssClass="modalPopup" Width="300px">
        <h1>Archive Logs</h1>
        <ul style="list-style-type: none;">
            <li><b>Logoff Start Date: </b><asp:TextBox ID="startdate" runat="server" Width="100px" ValidationGroup="archiveg" /><asp:RequiredFieldValidator runat="server" ControlToValidate="startdate" ValidationGroup="archiveg" ErrorMessage="*" /></li>
            <li><b>Logoff End Date: </b><asp:TextBox ID="enddate" runat="server" Width="100px" ValidationGroup="archiveg" /><asp:RequiredFieldValidator runat="server" ControlToValidate="enddate" ValidationGroup="archiveg" ErrorMessage="*" /></li>
        </ul>
        <div style="font-size: small;">
            <b>Why Archive?</b><br />
            If you are using the XML provider archiving moves logged events into a seperate file from the main tracker file used by the api.  This increases the speed that the tracker's api can respond at.  It doesn't delete any logged events!
        </div>
        <div class="modalButtons">
            <asp:Button runat="server" Text="Archive Logs" ID="archivelogsb" ValidationGroup="archiveg" onclick="archivelogsb_Click" />
            <asp:Button ID="ok_btn" runat="server" Text="Cancel" />
        </div>
    </asp:Panel>
    <asp:Repeater ID="dates" runat="server">
        <HeaderTemplate><ul></HeaderTemplate>
        <ItemTemplate><li><a href="<%#Eval("Year")%>/<%#Eval("Month") %>/"><%#((DateTime)Container.DataItem).ToString("MMMM yyyy") %></a></li></ItemTemplate>
        <FooterTemplate></ul></FooterTemplate>
    </asp:Repeater>
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
</asp:Content>
