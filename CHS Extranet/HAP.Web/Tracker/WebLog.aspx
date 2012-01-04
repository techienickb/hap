<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="WebLog.aspx.cs" Inherits="HAP.Web.Tracker.WebLog" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="<%=ResolveClientUrl("~/tracker/tracker.css")%>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server"> 
	<div style="overflow: hidden; clear: both; position: relative; height: 120px">
		<div class="tiles" style="position: absolute; left: 0; margin-top: 45px;">
			<a class="button" href="<%=ResolveClientUrl("~/") %>">Home Access Plus+ Home</a>
		</div>
		<div style="text-align: center; margin-left: 60px;">
			<a href="<%=ResolveClientUrl("~/tracker/weblogs.aspx") %>">HAP+ Web <img src="<%=ResolveClientUrl("~/tracker/logontracker-small.png") %>" style="vertical-align: middle;" alt="Logon Tracker" /></a>
		</div>
	</div>
	<div style="text-align: center; margin-bottom: 10px;">
		<asp:Chart ID="pcchart" runat="server" Width="880px" BorderlineColor="Silver" BorderlineDashStyle="Solid" Palette="Excel">
			<Series>
				<asp:Series Name="pcseries">
				</asp:Series>
			</Series>
			<ChartAreas>
				<asp:ChartArea Name="pcchartarea">
					<AxisY IsLabelAutoFit="False" Title="Number of Events" TitleFont="Segoe UI, 9pt">
						<LabelStyle Font="Segoe UI, 9pt" />
					</AxisY>
					<AxisX IsLabelAutoFit="False" Title="Day" TitleFont="Segoe UI, 9pt">
						<MajorGrid Enabled="False" />
						<MinorTickMark Enabled="True" />
						<LabelStyle Font="Segoe UI, 9pt" />
					</AxisX>
					<AxisY2 IsInterlaced="True">
					</AxisY2>
				</asp:ChartArea>
			</ChartAreas>
			<Titles>
				<asp:Title Font="Segoe UI, 18pt" Name="Title1">
				</asp:Title>
			</Titles>
			<BorderSkin PageColor="Transparent" />
		</asp:Chart>
	</div>
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
	<div id="loadingPopup" style="display: none;">
		<div class="modalBackground"></div>
		<div id="ph">
			<div class="popupContent" style="width: 220px">
				<h1>Loading</h1>
				<asp:Image ID="Image1" runat="server" ImageUrl="~/bookingsystem/loading.gif" AlternateText="" />
			</div>
		</div>
	</div>
	<script type="text/javascript">
	    $(function () {
	        $("button").button();
	        $("input[type=submit]").button();
	        $(".button").button();
	    });
		Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
		function endRequestHandler(sender, args) {
			$get('loadingPopup').style.display = "none";
			var error = args.get_error();
			if (error != undefined) {
				alert(error.message);
				args.set_errorHandled(true);
			}
		}
		function beginRequestHandler(sender, args) {
			$get('loadingPopup').style.display = "block";
		}
	</script>
</asp:Content>
