<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="live.aspx.cs" Inherits="HAP.Web.Tracker.live" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/jquery.dataTables.js"></script>
    <link href="../style/jquery.dataTables.css" rel="stylesheet" />
</asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/tracker/live.aspx"><hap:LocalResource runat="server" StringPath="tracker/livelogons" /></asp:HyperLink></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
    <asp:HyperLink runat="server" NavigateUrl="~/tracker/live.aspx"><hap:LocalResource runat="server" StringPath="tracker/livelogons" /></asp:HyperLink>
    <asp:HyperLink runat="server" NavigateUrl="~/tracker/logs.aspx"><hap:LocalResource runat="server" StringPath="tracker/historiclogs" /></asp:HyperLink>
    <asp:HyperLink runat="server" NavigateUrl="~/tracker/weblogs.aspx"><hap:LocalResource runat="server" StringPath="tracker/weblogs" /></asp:HyperLink>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div id="tools" style="position: absolute; right: 200px; z-index: 140;"><input class="noswitch" type="checkbox" id="checkall" /><label for="checkall">Check All</label> | <a href="#" id="logoff">Logoff Selected</a> | <a href="#" id="markasloggedoff">Mark Selected as Logged Off</a>&nbsp;|&nbsp;</div>
    <div id="datagrid"></div>
    <script type="text/javascript">
        var curdata = null;
        $("#tools").hide();
        $("#checkall").change(function () {
            $('#datatable tbody tr input').prop("checked", $(this).is(":checked")).trigger("change");
        });
        $("#markasloggedoff").click(function () {
            for (var i = 0; i < $('#datatable tbody tr input:checked').length; i++)
                logoff($($('#datatable tbody tr input:checked')[i]).next(), true);
            return false;
        });
        $("#logoff").click(function () {
            for (var i = 0; i < $('#datatable tbody tr input:checked').length; i++)
                $($('#datatable tbody tr input:checked')[i]).next().trigger("click");
            return false;
        });
        function logoff(e, clear) {
            var row = $(e);
            if (row.is("a")) row = row.parent().parent();
            $.ajax({
                type: 'POST',
                url: hap.common.formatJSONUrl("~/api/tracker/" + (clear ? "Clear" : "RemoteLogoff")),
                dataType: 'json',
                data: '{ "Computer": "' + $(row.children()[0]).text() + '", "DomainName": "' + $(row.children()[3]).text() + '" }',
                contentType: 'application/json',
                success: function (data) {
                    load();
                },
                error: hap.common.jsonError
            });

            return false;
        }
        function load() {
            $.getJSON(hap.common.formatJSONUrl("~/api/tracker/live"), function (data) {
                for (var i = 0; i < data.length; i++)
                    data[i][7] = '<input type="checkbox" /> <a href="#" onclick="return logoff(this);">Logoff</a>';
                if (curdata != data) {
                    curdata = data;
                    $("#checkall").prop("checked", false);
                    $('#datagrid').html('<table cellpadding="0" cellspacing="0" border="0" class="display" id="datatable"></table>');
                    $('#datatable').dataTable({
                        "aaData": data,
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
                    $('#datatable tbody tr input').change(function () {
                        $(this).parent().parent().toggleClass('row_selected');
                    });
                }
                $("#tools").show();
            });
            return false;
        };
        $(document).ready(function () {
            $("button").button();
            load();
            setInterval(load, 50000);
        });
    </script>
</asp:Content>