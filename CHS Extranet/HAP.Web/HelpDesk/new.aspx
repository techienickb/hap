<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="new.aspx.cs" Inherits="HAP.Web.HelpDesk.New" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
	<link href="../style/helpdesk.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both; position: relative;">
		<div class="tiles" style="position: absolute; left: 0;">
			<a class="button" href="../">Home Access Plus+ Home</a>
		</div>
		<div style="text-align: center;">
			<h1>Help Desk</h1>
		</div>
	</div>
	<div id="updateticket" title="Update Ticket">
		<asp:PlaceHolder runat="server" id="adminupdatepanel">
		<div>
			<label for="ticket-priority">Priority: </label>
			<div id="ticket-priority">
				<input type="radio" value="Low" id="ticket-priority-low" name="ticket-priority" /><label for="ticket-priority-low">Low</label>
				<input type="radio" value="Normal" id="ticket-priority-normal" name="ticket-priority" /><label for="ticket-priority-normal">Normal</label>
				<input type="radio" value="High" id="ticket-priority-high" name="ticket-priority" /><label for="ticket-priority-high">High</label>
			</div>
		</div>
		<div>
			<label for="ticket-showto">Make aware: </label>
			<input type="text" id="ticket-showto" /> (comma seperated list of usernames)
		</div>
		<div>
			<label for="ticket-fixed">Fixed: </label>
			<input type="checkbox" id="ticket-fixed" />
		</div>
		</asp:PlaceHolder>
		<div>
			<label for="ticket-note">Note: </label>
		</div>
		<textarea id="ticket-note" style="width: 100%; height: 200px;" rows="8" cols="10"></textarea>
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
			<div>
				<label for="newticket-subject">Issue: </label>
				<input type="text" id="newticket-subject" style="width: 400px;" />
			</div>
			<div>
				<label for="newticket-room">Room: </label>
				<input type="text" id="newticket-room" />
			</div>
			<div>
				<label for="newticket-note">Note: </label>
			</div>
			<textarea id="newticket-note" style="width: 100%; height: 200px;" rows="8" cols="10"></textarea>
			<asp:PlaceHolder runat="server" ID="adminbookingpanel">
			<div>
				<asp:Label runat="server" AssociatedControlID="userlist" Text="Ticker for: " />
				<asp:DropDownList runat="server" ID="userlist" />
			</div>
			<div>
				<label for="newticket-priority">Priority: </label>
				<div id="priorityradioes">
					<input type="radio" value="Low" id="newticket-priority-low" name="newticket-priority" /><label for="newticket-priority-low">Low</label>
					<input type="radio" value="Normal" id="newticket-priority-normal" name="newticket-priority" /><label for="newticket-priority-normal">Normal</label>
					<input type="radio" value="High" id="newticket-priority-high" name="newticket-priority" /><label for="newticket-priority-high">High</label>
				</div>
				<script type="text/javascript">
					$(function () {
						$("#priorityradioes").buttonset();
					});
				</script>
			</div>
			<div>
				<label for="newticket-showto">Make aware: </label>
				<input type="text" id="newticket-showto" /> (comma seperated list of usernames)
			</div>
			</asp:PlaceHolder>
			<input type="submit" value="File Ticket" onclick="return fileTicket()" />
		</div>
	</div>
	<script type="text/javascript">
		var curticket;
		var st = "";
		function updateTicket() {
			$("#ticket-showto").val(st);
			if ($("#ticket-priority") != null) $("#ticket-priority").buttonset();
			$("#updateticket").dialog({ autoOpen: true, minWidth: 600, minHeight: 400, buttons: {
				"Update": function () {
					var data = '{ "Note": "' + escape($("#ticket-note").val()) + '", "State": ';
					var url = '<%=ResolveUrl("~/api/HelpDesk/Ticket/")%>' + curticket;
					if ($("#ticket-priority") != null) {
					    data += ($("#ticket-fixed").is(":checked") ? '"Fixed"' : '"With IT"') + ', "Priority": "' + $("#ticket-priority input:checked").attr("value") + '", "ShowTo": "' + $("#ticket-showto").val() + '"';
						url = '<%=ResolveUrl("~/api/HelpDesk/AdminTicket/")%>' + curticket;
					} else data += '"New"';
					data += ' }';
					$.ajax({
						type: 'PUT',
						url: url,
						dataType: 'json',
						data: data,
						contentType: 'application/json; charset=utf-8',
						success: function (data) {
							$("#ticket-note").val("");
							if ($("#ticket-priority") != null) {
								$("ticket-priority input:checked").removeAttr("checked");
								$("#ticket-showto").val("");
								$("#ticket-fixed").removeAttr("checked");
							}
							var h = '<button style="float: right;" onclick="return updateTicket();">Update</button><div><label>Ticket ' + curticket + ': </label>' + data.Subject + '</div><div><label>Opened By: </label>' + data.DisplayName + ' (' + data.Username + ')</div><div><label>Opened on: </label>' + data.Date + '</div><div><label>Priority: </label>' + data.Priority + '</div><div><label>Status: </label>' + data.Status + '</div><div class="notes tile-border-color">';
							for (var i = 0; i < data.Notes.length; i++)
								h += data.Notes[i].DisplayName + ' ' + data.Notes[i].Date + '<br /><pre>' + unescape(data.Notes[i].NoteText).replace(/\+/g, ' ') + '</pre>';
							h += '</div>';
							$("#ticket-" + curticket).html(h);
							st = data.ShowTo;
							$("button").button();
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
						}
					});
					$(this).dialog("close");
				},
				"Cancel": function () {
					$("#ticket-note").val("");
					if ($("#ticket-priority") != null) {
						$("ticket-priority input:checked").removeAttr("checked");
						$("#ticket-showto").val("");
						$("#ticket-fixed").removeAttr("checked");
					}
					$(this).dialog("close");
				}
			}
			});
			return false;
		}

		function fileTicket() {
			var data = '{ "Subject": "' + escape($("#newticket-subject").val()) + '", "Room": "' + $("#newticket-room").val() + '", "Note": "' + escape($("#newticket-note").val()) + '"';
			var url = '<%=ResolveUrl("~/api/HelpDesk/Ticket")%>';
			if ($("#<%=userlist.ClientID %>") != null) {
			    data += ', "Priority": "' + $("#priorityradioes input:checked").val() + '", "User": "' + $("#<%=userlist.ClientID %> option:selected").attr("value") + '", "ShowTo": "' + $("#newticket-showto").val() + '"';
				url = '<%=ResolveUrl("~/api/HelpDesk/AdminTicket")%>';
			}
			data += ' }';
			$.ajax({
			    type: 'POST',
			    url: url,
			    dataType: 'json',
			    data: data,
			    contentType: 'application/json; charset=utf-8',
			    success: function (data) {
			        $("#newticket-subject").val("");
			        $("#newticket-room").val("");
			        $("#newticket-note").val("");
			        if ($("#<%=userlist.ClientID %>") != null) {
			            $("#priorityradioes input:checked").removeAttr("checked");
			            $("#newticket-showto").val("");
			        }
			        curticket = data.Id;
			        $('<div id="ticket-' + curticket + '" class="ticket">Loading Ticket ' + curticket + '...</div>').appendTo("#tabs");
			        $("#tabs").tabs("add", '#ticket-' + curticket, "Ticket: " + curticket, 3);
			        $("#tabs").tabs("select", 3);
			        $(".ui-tabs-selected a span").html("Ticket: " + data.Subject);
			        var h = '<button style="float: right;" onclick="return updateTicket();">Update</button><div><label>Ticket ' + curticket + ': </label>' + data.Subject + '</div><div><label>Opened By: </label>' + data.DisplayName + ' (' + data.Username + ')</div><div><label>Opened on: </label>' + data.Date + '</div><div><label>Priority: </label>' + data.Priority + '</div><div><label>Status: </label>' + data.Status + '</div><div class="notes tile-border-color">';
			        for (var i = 0; i < data.Notes.length; i++)
			            h += data.Notes[i].DisplayName + ' ' + data.Notes[i].Date + '<br /><pre>' + unescape(data.Notes[i].NoteText).replace(/\+/g, ' ') + '</pre>';
			        h += '</div>';
			        $("#ticket-" + curticket).html(h);
			        $("button").button();
			        window.location.href = "#ticket-" + data.Id;
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
			    }
			});
			return false;
		}

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
						var h = '<button style="float: right;" onclick="return updateTicket();">Update</button><div><label>Ticket ' + curticket + ': </label>' + data.Subject + '</div><div><label>Opened By: </label>' + data.DisplayName + ' (' + data.Username + ')</div><div><label>Opened on: </label>' + data.Date + '</div><div><label>Priority: </label>' + data.Priority + '</div><div><label>Status: </label>' + data.Status + '</div><div class="notes tile-border-color">';
						for (var i = 0; i < data.Notes.length; i++)
							h += data.Notes[i].DisplayName + ' ' + data.Notes[i].Date + '<br /><pre>' + unescape(data.Notes[i].NoteText).replace(/\+/g, ' ') + '</pre>';
						h += '</div>';
						$("#ticket-" + curticket).html(h);
						$("button").button();
					}
				});
			}
		}
		$(window).hashchange(function () {
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curticket = window.location.href.split('#')[1].substr(7);
			else curticket = null;
			loadTicket();
		});
		$(function () {
			$("#updateticket").dialog({ autoOpen: false });
			$("#tabs").tabs({ select: function (event, ui) { if (ui.index < 3) { window.location.href = '#'; $("#updateticket").dialog("close"); } return true; } });
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