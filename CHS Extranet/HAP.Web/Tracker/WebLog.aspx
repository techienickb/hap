<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="WebLog.aspx.cs" Inherits="HAP.Web.Tracker.WebLog" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="<%=ResolveClientUrl("~/tracker/tracker.css")%>" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]><script language="javascript" type="text/javascript" src="<%:ResolveClientUrl("~/scripts/excanvas.js") %>"></script><![endif]-->
    <script type="text/javascript" src="<%:ResolveClientUrl("~/scripts/jquery.jqplot.min.js") %>"></script>
    <script type="text/javascript" src="<%:ResolveClientUrl("~/scripts/jqplot.dateAxisRenderer.min.js") %>"></script>
    <script type="text/javascript" src="<%:ResolveClientUrl("~/scripts/jqplot.highlighter.min.js") %>"></script>
    <script type="text/javascript" src="<%:ResolveClientUrl("~/Scripts/jqplot.cursor.min.js") %>"></script>
    <link rel="stylesheet" type="text/css" href="<%:ResolveClientUrl("~/style/jquery.jqplot.css") %>" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server"> 
    <header>
		<div>
			<a href="<%:ResolveClientUrl("~/tracker") %>"><hap:LocalResource StringPath="tracker/weblogs" runat="server" /></a>
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
					<th style="width: 180px">
						<div><label>Date & Time</label>
							<div>
								<asp:LinkButton ID="DTsort" runat="server" oncommand="sort_Command" CommandName="DT">Sort</asp:LinkButton>
							</div>
						</div>
					</th>
					<th style="width: 90px">
						<div><label>Type</label>
							<div>
								<asp:DropDownList ID="eventfilter" runat="server" Width="70px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
								</asp:DropDownList>
								<asp:LinkButton ID="eventsort" runat="server" oncommand="sort_Command" CommandName="EventType">Sort</asp:LinkButton>
							</div>
						</div>
					</th>
					<th>
						<div><label>Computer</label>
							<div>
								<asp:DropDownList ID="computerfilter" runat="server" Width="140px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br />
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
					<th>
						<div><label>Browser</label>
							<div>
								<asp:DropDownList ID="browserfilter" runat="server" Width="70px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
								</asp:DropDownList>
								<asp:LinkButton ID="browsersort" runat="server" oncommand="sort_Command" CommandName="Browser">Sort</asp:LinkButton>
							</div>
						</div>
					</th>
					<th>
						<div><label>OS</label>
							<div>
								<asp:DropDownList ID="osfilter" runat="server" Width="70px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
								</asp:DropDownList>
								<asp:LinkButton ID="ossort" runat="server" oncommand="sort_Command" CommandName="OS">Sort</asp:LinkButton>
							</div>
						</div>
					</th>
					<th>
						<div><label>Details</label>
						</div>
					</th>
				</tr>
			<asp:Repeater ID="ListView1" runat="server">
				<ItemTemplate>
					<tr style="">
						<td style="width: 180px"><a href="<%#ResolveClientUrl("~/tracker/web/" + ((DateTime)Eval("DateTime")).ToString("yyyy/M/") + "/d/" + ((DateTime)Eval("DateTime")).ToString("d/"))%>"><%# ((DateTime)Eval("DateTime")).ToString("dd MMMM yyyy") %></a> <%# ((DateTime)Eval("DateTime")).ToString("HH:mm") %></td>
						<td><%# Eval("EventType") %></td>
						<td><%# Eval("ComputerName") %></td>
						<td><%# Eval("Username") %></td>
						<td><%# Eval("Browser") %></td>
						<td><%# Eval("OS") %></td>
						<td><%# Eval("Details") %></td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
			</table>
	<script type="text/javascript">
	    $(function () {
	        $("button").button();
	        $("input[type=submit]").button();
	        $(".button").button();
	    });
	</script>
</asp:Content>
