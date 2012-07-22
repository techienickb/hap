<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="log.aspx.cs" Inherits="HAP.Web.Tracker.log" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%=ResolveClientUrl("~/tracker/tracker.css")%>" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]><script language="javascript" type="text/javascript" src="<%:ResolveClientUrl("~/scripts/excanvas.js") %>"></script><![endif]-->
    <script type="text/javascript" src="<%:ResolveClientUrl("~/scripts/jquery.jqplot.min.js") %>"></script>
    <script type="text/javascript" src="<%:ResolveClientUrl("~/scripts/jqplot.dateAxisRenderer.min.js") %>"></script>
    <script type="text/javascript" src="<%:ResolveClientUrl("~/scripts/jqplot.highlighter.min.js") %>"></script>
    <script type="text/javascript" src="<%:ResolveClientUrl("~/Scripts/jqplot.cursor.min.js") %>"></script>
    <link rel="stylesheet" type="text/css" href="<%:ResolveClientUrl("~/style/jquery.jqplot.css") %>" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<header  class="commonheader">
		<div>
			<a href="<%:ResolveClientUrl("~/tracker") %>"><hap:LocalResource StringPath="tracker/historiclogs" runat="server" /></a>
		</div>
	</header>
    <div id="chartdiv" style="height:300px;width:99%; "></div>
    <button onclick="plot1.resetZoom(); return false;">Reset Zoom</button>
    <script type="text/javascript">
        var line1 = [<%=Data%>];
    </script>
    <script type="text/javascript">
        var plot1;
        $(document).ready(function(){
            plot1 = $.jqplot('chartdiv', [line1], {
                title:'Tracker Results',
                axes:{yaxis: { min: 0 }, xaxis:{ renderer:$.jqplot.DateAxisRenderer}},
                series:[{lineWidth:2, markerOptions:{ style: 'filledCircle', lineWidth: 1, size: 4 }}],
                highlighter: { show: true }, 
                cursor: { show: true, zoom: true }
            });
        });
    </script>
    <asp:Button ID="showdata" runat="server" Text="Show Data" OnClick="showdata_Click" />
    <table border="0" class="trackertable"<%=showtable %>>
        <tr valign="top">
            <th>
                <div><label>Computer</label>
                    <div>
                        <asp:DropDownList ID="computerfilter" runat="server" Width="140px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br />
                        (<asp:DropDownList ID="ipfilter" runat="server" Width="90px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>)
                        <asp:LinkButton id="computersort" runat="server" oncommand="sort_Command" CommandName="ComputerName">Sort</asp:LinkButton>
                    </div>
                </div>
            </th>
            <th style="width: 90px">
                <div><label>Username</label>
                    <div>
                        <asp:DropDownList ID="userfilter" runat="server" Width="70px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:LinkButton ID="usernamesort" runat="server" oncommand="sort_Command" CommandName="Username">Sort</asp:LinkButton>
                    </div>
                </div>
            </th>
            <th style="width: 110px">
                <div><label>Domain</label>
                    <div>
                        <asp:DropDownList ID="domainfilter" runat="server" Width="90px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:LinkButton ID="domainsort" runat="server" oncommand="sort_Command" CommandName="Domain">Sort</asp:LinkButton>
                    </div>
                </div>
            </th>
            <th style="width: 130px">
                <div><label>Logon Server</label>
                    <div>
                        <asp:DropDownList ID="lsfilter" runat="server" Width="110px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:LinkButton ID="serversort" runat="server" oncommand="sort_Command" CommandName="Server">Sort</asp:LinkButton>
                    </div>
                </div>
            </th>
            <th style="width: 160px">
                <div><label>Logon Date & Time</label>
                    <div>
                        <asp:DropDownList ID="logondt" runat="server" Width="140px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:LinkButton ID="logonDTsort" runat="server" oncommand="sort_Command" CommandName="LogonDT">Sort</asp:LinkButton>
                    </div>
                </div>
            </th>
            <th style="width: 160px">
                <div><label>Logoff Date & Time</label>
                    <div>
                        <asp:DropDownList ID="logoffdt" runat="server" Width="140px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:LinkButton ID="logoffDTsort" runat="server" oncommand="sort_Command" CommandName="LogoffDT">Sort</asp:LinkButton>
                    </div>
                </div>
            </th>
        </tr>
    <asp:Repeater ID="ListView1" runat="server">
        <ItemTemplate>
            <tr style="">
                <td><a href="<%#ResolveClientUrl("~/tracker/" + ((DateTime)Eval("LogOnDateTime")).ToString("yyyy/M") + "/c/" + Eval("ComputerName").ToString() + "/")%>"><%# Eval("ComputerName") %></a> (<%# Eval("IP") %>)</td>
                <td><%# Eval("UserName") %></td>
                <td><%# Eval("DomainName") %></td>
                <td><%# Eval("LogonServer") %></td>
                <td style="width: 160px"><a href="<%#ResolveClientUrl("~/tracker/" + ((DateTime)Eval("LogOnDateTime")).ToString("yyyy/M") + "/d/" + ((DateTime)Eval("LogOnDateTime")).Day + "/")%>"><%# ((DateTime)Eval("LogOnDateTime")).ToString("dd MMMM yyyy") %></a> <%# ((DateTime)Eval("LogOnDateTime")).ToString("HH:mm") %></td>
                <td style="width: 160px"><%# (Eval("LogOffDateTime") == null) ? "" : ((DateTime)Eval("LogOffDateTime")).ToString("f")%></td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    </table>
</asp:Content>
