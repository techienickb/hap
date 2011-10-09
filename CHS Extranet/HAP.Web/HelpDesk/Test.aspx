<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="HAP.Web.HelpDesk.Test" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
	<link href="../style/helpdesk.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both;">
		<div class="tiles" style="float: left;">
			<a class="button" href="../">Home Access Plus+ Home</a>
		</div>
		<div style="text-align: center;">
			<h1>Help Desk</h1>
		</div>
	</div>
	<div id="tabs">
		<ul>
			<li><a href="#opentickets">Open Tickets</a></li>
			<li><a href="#closedtickets">Closed Tickets</a></li>
			<li><a href="#newticket">New Ticket</a></li>
		</ul>
		<div id="opentickets">
			
		</div>
		<div id="closedtickets">
		
		</div>
		<div id="newticket">
		
		</div>
	</div>
	<script type="text/javascript">
		var curticket;
		function loadTicket() {
			$("#tabs").tabs("remove", 3);
			$(".ticket").remove();
			if (curticket != null) {
				$('<div id="ticket-' + curticket + '" class="ticket">Loading Ticket ' + curticket + '...</div>').appendTo("#tabs");
				$("#tabs").tabs("add", '#ticket-' + curticket, "Ticket: " + curticket, 3);
				$("#tabs").tabs("select", 3);
				$.ajax({
				    type: 'GET',
				    url: '<%=ResolveUrl("~/api/HelpDesk/Ticket/")%>' + curticket,
				    dataType: 'json',
				    contentType: 'application/json; charset=utf-8',
				    success: function (data) {
				        $(".ui-tabs-selected a span").html("Ticket: " + data.Subject);
				        var h = '<button style="float: right;" onclick="return false;">Update</button>Ticket ' + curticket + ': ' + data.Subject + '<br />Opened By: ' + data.DisplayName + ' (' + data.Username + ')<br />Opened on: ' + data.Date + '<br />Priority: ' + data.Priority + '<br />Status: ' + data.Status + '<br /><div class="notes tile-border-color">';
				        for (var i = 0; i < data.Notes.length; i++)
				            h += data.Notes[i].DisplayName + ' ' + data.Notes[i].Date + '<br /><pre>' + unescape(data.Notes[i].NoteText).replace(/\+/g, ' ') + '</pre>';
				        h += '</div>';
				        $("#ticket-" + curticket).html(h);
				        $("button").button();
				    }
				});
			}
		}

		$(function () {
			$(window).hashchange(function () {
				if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curticket = window.location.href.split('#')[1].substr(7);
				else curticket = null;
				loadTicket();
			});
			$("#tabs").tabs({ select: function (event, ui) { if (ui.index < 3) { window.location.href = '#'; } return true; } });
			$("button").button();
			$("input[type=submit]").button();
			$(".button").button();
			$.ajax({
				type: 'GET',
				url: '<%=ResolveUrl("~/api/HelpDesk/Tickets/Open" + (User.IsInRole("Domain Admins") ? "" : "/" + ADUser.UserName))%>',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				success: function (data) {
					var x = "";
					for (var i = 0; i < data.length; i++) {
						x += '<div><a href="#ticket-' + data[i].Id + '">' + data[i].Subject + '</a></div>'
					}
					$("#opentickets").html(x);
				}
			});
			$.ajax({
				type: 'GET',
				url: '<%=ResolveUrl("~/api/HelpDesk/Tickets/Closed" + (User.IsInRole("Domain Admins") ? "" : "/" + ADUser.UserName))%>',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				success: function (data) {
					var x = "";
					for (var i = 0; i < data.length; i++) {
						x += '<div><a href="#ticket-' + data[i].Id + '">' + data[i].Subject + '</a></div>'
					}
					$("#closedtickets").html(x);
				}
			});
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curticket = window.location.href.split('#')[1].substr(7);
			else curticket = null;
			loadTicket();
		});
	</script>
</asp:Content>