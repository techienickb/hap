<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.HelpDesk.New" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
	<link href="../style/helpdesk.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="viewport"><meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" /></asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/HelpDesk/"><hap:LocalResource runat="server" StringPath="helpdesk/helpdesk" /></asp:HyperLink></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
    <div id="toolbar">
        <a id="opentickets-link" href="#opentickets" onclick="return false;"><hap:LocalResource StringPath="helpdesk/opentickets" runat="server" /></a>
        <a id="closedtickets-link" href="#closedtickets" onclick="return false;"><hap:LocalResource StringPath="helpdesk/closedtickets" runat="server" /></a>
        <%if (isHDAdmin || hasArch) { %>
        <a id="archivedtickets-link" href="#archivedtickets" onclick="return false;"><hap:LocalResource StringPath="helpdesk/archivedtickets" runat="server" /></a>
        <%} %>
	    <a href="#newticket" id="newticket-link"><hap:LocalResource StringPath="helpdesk/newtickets" runat="server" /></a>
	    <a href="#faqs" id="faq-link"><hap:LocalResource StringPath="helpdesk/faqs" runat="server" /></a>
        <%if (isHDAdmin) { %>
        <a href="#stats" id="stats-link"><hap:LocalResource runat="server" StringPath="helpdesk/stats" /></a>
        <%} %>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div id="hap-HD">
    <div id="assignticket">
        <div>
			<asp:Label runat="server" AssociatedControlID="userlist2" Text="Assign To: " />
			<asp:DropDownList runat="server" ID="userlist2" />
		</div>
    </div>
	<div id="updateticket" title="Update Ticket">
		<asp:PlaceHolder runat="server" id="adminupdatepanel">
		<div>
			<label for="ticket-subject"><hap:LocalResource StringPath="helpdesk/subject" runat="server" />: </label>
			<input type="text" id="ticket-subject" />
		</div>
		<div>
			<label for="ticket-priority"><hap:LocalResource StringPath="helpdesk/Priority" runat="server" />: </label>
			<div id="ticket-priority">
				<input type="radio" value="Low" id="ticket-priority-low" name="ticket-priority" /><label for="ticket-priority-low"><hap:LocalResource StringPath="helpdesk/low" runat="server" /></label>
				<input type="radio" value="Normal" id="ticket-priority-normal" name="ticket-priority" /><label for="ticket-priority-normal"><hap:LocalResource StringPath="helpdesk/normal" runat="server" /></label>
				<input type="radio" value="High" id="ticket-priority-high" name="ticket-priority" /><label for="ticket-priority-high"><hap:LocalResource StringPath="helpdesk/high" runat="server" /></label>
			</div>
		</div>
		<div>
			<label for="ticket-showto"><hap:LocalResource StringPath="helpdesk/makeaware" runat="server" />: </label>
			<input type="text" id="ticket-showto" /> <hap:LocalResource StringPath="helpdesk/csloun" runat="server" />
		</div>
		<div>
			<label for="ticket-fixed"><hap:LocalResource StringPath="helpdesk/fixed" runat="server" />: </label>
			<input type="checkbox" id="ticket-fixed" />
		</div>
		<div>
			<label for="ticket-userinter"><hap:LocalResource StringPath="helpdesk/userinter" runat="server" />: </label>
			<input type="checkbox" id="ticket-userinter" />
		</div>
		<div>
			<label for="ticket-faq"><hap:LocalResource StringPath="helpdesk/markasfaq" runat="server" />: </label>
			<input type="checkbox" id="ticket-faq" />
		</div>
		</asp:PlaceHolder>
		<div>
			<label for="ticket-note"><hap:LocalResource StringPath="helpdesk/note" runat="server" />: </label>
		</div>
		<textarea id="ticket-note" style="width: 100%; height: 200px;" rows="8" cols="10"></textarea>
	</div>
	<div id="tabs">
		<div id="opentickets">
            <div style="text-align: center;"><img src="../images/metroloading.gif" /></div>
            <hap:LocalResource runat="server" StringPath="loading" />
		</div>
		<div id="closedtickets">
		    <div style="text-align: center;"><img src="../images/metroloading.gif" /></div>
            <hap:LocalResource runat="server" StringPath="loading" />
		</div>
        <div id="archivedtickets">
            <div id="archivedmain">
                <div id="archivedhead">
                    <asp:Label AssociatedControlID="archiveddates" runat="server"><hap:LocalResource StringPath="helpdesk/selectarchive" runat="server" />: </asp:Label>
                    <asp:DropDownList runat="server" ID="archiveddates" />
                    <script>
                        $("#<%=archiveddates.ClientID%>").change(function () {
                            if ($(this).val() == "") {
                                $("#archivedloading").hide();
                                $("#archivedbody").html("No Archived Tickets");
                            } else {
                                $("#archivedloading").show();
                                $("#archivedbody").html("");
                                $.ajax({
                                    type: 'GET',
                                    url: hap.common.formatJSONUrl("~/api/HelpDesk/ATickets/" + $("#<%=archiveddates.ClientID%>").val() + "/Closed<%=isHDAdmin ? "" : "/" + ADUser.UserName%>"),
                                    dataType: 'json',
		                            contentType: 'application/json',
		                            success: function (data) {
		                                var x = "";
		                                for (var i = 0; i < data.length; i++) x += '<div><a href="#ticket-' + $("#<%=archiveddates.ClientID%>").val() + '/' + data[i].Id + '" class="' + data[i].Priority.replace(/ /g, "-") + '">' + data[i].Subject + ' <span>' + data[i].Id + ' - ' + data[i].Username + (data[i].AssignedTo == '' ? '' : (' -> ' + data[i].AssignedTo)) + ' - ' + data[i].Date + '</span></a></div>';
		                                if (data.length == 0) x = "No Tickets";
		                                $("#archivedbody").html(x);
		                                $("#archivedloading").hide();
		                            }, error: hap.common.jsonError
		                        });
                            }
                        });
                    </script>
                </div>
                <asp:Panel runat="server" ID="archiveadmin">
                    <h2><hap:LocalResource StringPath="helpdesk/archivetickets" runat="server" /></h2>
                    <asp:Label AssociatedControlID="archivefrom" runat="server"><hap:LocalResource StringPath="from" runat="server" /></asp:Label>
                    <asp:TextBox ID="archivefrom" type="date" runat="server"></asp:TextBox>
                    <asp:Label AssociatedControlID="archiveto" runat="server"><hap:LocalResource StringPath="to" runat="server" /></asp:Label>
                    <asp:TextBox ID="archiveto" type="date" runat="server"></asp:TextBox>
                    <script>
                        $("#<%=archivefrom.ClientID%>, #<%=archiveto.ClientID%>").datepicker({ dateFormat: 'dd/mm/yy' });
                    </script>
                    <div>
                        <asp:LinkButton runat="server" CssClass="button" ID="archivetickets" OnClick="archivetickets_Click"><hap:LocalResource StringPath="tracker/archive" runat="server" /></asp:LinkButton>
                    </div>
                </asp:Panel>
                <div id="archivedloading" style="display: none;">
		            <div style="text-align: center;"><img src="../images/metroloading.gif" /></div>
                    <hap:LocalResource runat="server" StringPath="loading" />
                </div>
                <div id="archivedbody"></div>
            </div>
        </div>
		<div id="newticket">
			<div>
				<label for="newticket-subject"><hap:LocalResource StringPath="helpdesk/issue" runat="server" />: </label>
				<input type="text" id="newticket-subject" style="width: 400px;" />
			</div>
			<div>
				<label for="newticket-room"><hap:LocalResource StringPath="helpdesk/room" runat="server" />: </label>
				<input type="text" id="newticket-room" />
			</div>
			<div>
				<label for="newticket-note"><hap:LocalResource StringPath="helpdesk/note" runat="server" />: </label>
			</div>
			<textarea id="newticket-note" style="width: 100%; height: 200px;" rows="8" cols="10"></textarea>
			<asp:PlaceHolder runat="server" ID="adminbookingpanel">
			<div>
				<asp:Label runat="server" AssociatedControlID="userlist" Text="Ticket for: " />
				<asp:DropDownList runat="server" ID="userlist" />
			</div>
			<div>
				<label for="newticket-priority"><hap:LocalResource StringPath="helpdesk/priority" runat="server" />: </label>
				<div id="priorityradioes">
					<input type="radio" value="Low" id="newticket-priority-low" name="newticket-priority" /><label for="newticket-priority-low"><hap:LocalResource StringPath="helpdesk/low" runat="server" /></label>
					<input type="radio" value="Normal" id="newticket-priority-normal" name="newticket-priority" /><label for="newticket-priority-normal"><hap:LocalResource StringPath="helpdesk/normal" runat="server" /></label>
					<input type="radio" value="High" id="newticket-priority-high" name="newticket-priority" /><label for="newticket-priority-high"><hap:LocalResource StringPath="helpdesk/high" runat="server" /></label>
				</div>
				<script type="text/javascript">
					$(function () {
						$("#priorityradioes").buttonset();
					});
				</script>
			</div>
			<div>
				<label for="newticket-showto"><hap:LocalResource StringPath="helpdesk/makeaware" runat="server" />: </label>
				<input type="text" id="newticket-showto" />
			</div>
			</asp:PlaceHolder>
			<input type="submit" value="File Ticket" onclick="return fileTicket()" />
		</div>
		<div id="faqs">
			<div style="text-align: center;"><img src="../images/metroloading.gif" /></div>
            <hap:LocalResource runat="server" StringPath="loading" />
		</div>
        <%if (isHDAdmin) { %>
        <div id="stats">
            <div id="stats-loading">
                <img src="../images/metroloading.gif" /><br />
                <hap:LocalResource runat="server" StringPath="loading" />
            </div>
            <div id="stats-content">
                <div id="stats-header">
                    <label for="spinner"><hap:LocalResource runat="server" StringPath="helpdesk/daystoindex" />:</label>
                    <input id="spinner" name="value" value="7" />
                    <button id="refreshstats"><hap:LocalResource runat="server" StringPath="helpdesk/refreshstats" /></button>
                </div>
                <div id="stats-body"></div>
            </div>
        </div>
        <%} %>
	</div>
    <hap:CompressJS runat="server" Tag="div">
	<script>
		var curticket;
		var st = "";
		function assignTicket() {
		    $("#assignticket").dialog({
		        autoOpen: true, buttons: {
		            "Assign": function () {
		                var url = hap.common.resolveUrl("~/api/HelpDesk/AdminTicket/") + curticket + '?' + window.JSON.stringify(new Date());
		                var data = '{ "Note": "", "State": "", "Priority": "", "ShowTo": "", "FAQ": "", "AssignTo": "' + $("#<%=userlist2.ClientID%>").val() + '", "Subject": "" }';
		                $.ajax({
		                    type: 'PUT',
		                    url: url,
		                    dataType: 'json',
		                    data: data,
		                    contentType: 'application/json',
		                    error: hap.common.jsonError,
		                    success: function (data) {
		                        var h = '<button style="float: right;" onclick="return updateTicket();">Update</button>' + (hap.hdadmin ? '<button style="float: right;" onclick="return assignTicket();">Assign</button>' : '') + '<div><label>Ticket ' + curticket + ': </label>' + data.Subject + '</div><div><label>Opened By: </label>' + data.DisplayName + ' (' + data.Username + ')</div><div><label>Opened on: </label>' + data.Date + '</div><div><label>Priority: </label>' + data.Priority + '</div><div><label>Status: </label>' + data.Status + '</div>' + (data.AssignedTo == "" ? "" : '<div><label>Assigned To:</label>' + data.AssignedTo + '</div>') + '<div class="notes tile-border-color">';
		                        for (var i = 0; i < data.Notes.length; i++)
		                            h += data.Notes[i].DisplayName + ' ' + data.Notes[i].Date + '<br /><pre>' + unescape(data.Notes[i].NoteText).replace(/\+/g, ' ') + '</pre>';
		                        h += '</div>';
		                        $("#ticket-" + curticket).html(h);
		                        st = data.ShowTo;
		                        $("button").button();
		                        Update();
		                    }
		                });
		                $(this).dialog("close");
		            }, "Close": function () {
		                $(this).dialog("close");
		            }
		        }
		    });
		    return false;
		}
		function updateTicket() {
			$("#ticket-showto").val(st);
			if ($("#ticket-priority") != null) { 
			    $("#ticket-priority").buttonset();
			    $("#newticket-priority-normal").attr("checked", "checked");
			}
			$("#ticket-subject").val($("#sub").text());
			$("#updateticket").dialog({ autoOpen: true, minWidth: 600, minHeight: 400, buttons: {
				"Update": function () {
					var data = '{ "Note": "' + escape($("#ticket-note").val()) + '", "State": ';
					var url = hap.common.formatJSONUrl("~/api/HelpDesk/Ticket/" + curticket);
					if (hap.hdadmin) {
						data += ($("#ticket-fixed").is(":checked") ? '"Fixed"' : ('"' + ($("#ticket-userinter").is(":checked") ? hap.common.getLocal("helpdesk/userinter") : "With IT") + '"')) + ', "Priority": "' + $("#ticket-priority input:checked").attr("value") + '", "ShowTo": "' + $("#ticket-showto").val() + '", "FAQ": "' + ($("#ticket-faq").is(":checked") ? 'true' : 'false') + '", "AssignTo": "", "Subject": "' + $("#ticket-subject").val() + '"';
						url = hap.common.resolveUrl("~/api/HelpDesk/AdminTicket/") + curticket + '?' + window.JSON.stringify(new Date());
					} else data += '"New"';
					data += ' }';
					$.ajax({
						type: 'PUT',
						url: url,
						dataType: 'json',
						data: data,
						contentType: 'application/json',
						error: hap.common.jsonError,
						success: function (data) {
							$("#ticket-note").val("");
							if (hap.hdadmin) {
								$("#ticket-priority input:checked").removeAttr("checked");
								$("#ticket-showto").val("");
								if ($("#ticket-fixed").is(":checked")) $("#ticket-fixed").prev().trigger("click");
								if ($("#ticket-userinter").is(":checked")) $("#ticket-userinter").prev().trigger("click");
								if ($("#ticket-faq").is(":checked")) $("#ticket-faq").prev().trigger("click");
							}
							var h = '<button style="float: right;" onclick="return updateTicket();">Update</button>' + (hap.hdadmin ? '<button style="float: right;" onclick="return assignTicket();">Assign</button>' : '') + '<div><label>Ticket ' + curticket + ': </label>' + data.Subject + '</div><div><label>Opened By: </label>' + data.DisplayName + ' (' + data.Username + ')</div><div><label>Opened on: </label>' + data.Date + '</div><div><label>Priority: </label>' + data.Priority + '</div><div><label>Status: </label>' + data.Status + '</div>' + (data.AssignedTo == "" ? "" : '<div><label>Assigned To:</label>' + data.AssignedTo + '</div>') + '<div class="notes tile-border-color">';
							for (var i = 0; i < data.Notes.length; i++)
								h += data.Notes[i].DisplayName + ' ' + data.Notes[i].Date + '<br /><pre>' + unescape(data.Notes[i].NoteText).replace(/\+/g, ' ') + '</pre>';
							h += '</div>';
							$("#ticket-" + curticket).html(h);
							st = data.ShowTo;
							$("button").button();
                            Update();
						}
					});
					$(this).dialog("close");
				},
				"Cancel": function () {
					$("#ticket-note").val("");
					if ($("#ticket-priority") != null) {
						$("#ticket-priority input:checked").removeAttr("checked");
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
	        if ($("#newticket-subject").val().length == 0) { alert("Ticket Subject needs to be entered"); return; }
			var data = '{ "Subject": "' + escape($("#newticket-subject").val()) + '", "Room": "' + $("#newticket-room").val() + '", "Note": "' + escape($("#newticket-note").val()) + '"';
			var url = hap.common.formatJSONUrl("~/api/HelpDesk/Ticket");
			if (hap.hdadmin) {
				data += ', "Priority": "' + $("#priorityradioes input:checked").val() + '", "User": "' + $("#<%=userlist.ClientID %> option:selected").attr("value") + '", "ShowTo": "' + $("#newticket-showto").val() + '"';
				url = '<%=ResolveUrl("~/api/HelpDesk/AdminTicket")%>?' + window.JSON.stringify(new Date());
			}
			data += ' }';
			$.ajax({
				type: 'POST',
				url: url,
				dataType: 'json',
				data: data,
				contentType: 'application/json',
				success: function (data) {
					$("#newticket-subject").val("");
					$("#newticket-room").val("");
					$("#newticket-note").val("");
					if (hap.hdadmin) {
						$("#priorityradioes input:checked").removeAttr("checked");
						$("#newticket-showto").val("");
					}
					location.href = "#ticket-" + data.Id;
					Update();
	            },  error: hap.common.jsonError
			});
			return false;
		}

		function loadTicket() {
			$(".ticket").remove();
			if (curticket != null) {
			    $("#tabs > div").hide();
			    $("#toolbar a").removeClass("active");
			    var stringticket = curticket.match(/\//gi) ? curticket.split(/\//g)[1] : curticket;
			    $('<div id="ticket-' + curticket.replace(/\//gi, "_") + '" class="ticket">Loading Ticket ' + stringticket + '...</div>').appendTo("#tabs");
			    $("#toolbar").append('<a href="#ticket-' + curticket + '" class="ticket active">Ticket: ' + stringticket + '</a>');
				$.ajax({
					type: 'GET',
					url: hap.common.formatJSONUrl("~/api/HelpDesk/" + (curticket.match(/\//gi) ? 'A' : '') + "Ticket/" + curticket),
					dataType: 'json',
					contentType: 'application/json',
					success: function (data) {
						$(".ui-tabs-selected a span").html("Ticket: " + data.Subject);
						var h = (curticket.match(/\//gi) ? '' : ('<button style="float: right;" onclick="return updateTicket();">Update</button>' + (hap.hdadmin ? '<button style="float: right;" onclick="return assignTicket();">Assign</button>' : ''))) + '<div><label>Ticket ' + (curticket.match(/\//gi) ? curticket.split(/\//g)[1] : curticket) + ': </label><span id="sub">' + data.Subject + '</span></div><div><label>Opened By: </label>' + data.DisplayName + ' (' + data.Username + ')</div><div><label>Opened on: </label>' + data.Date + '</div><div><label>Priority: </label>' + data.Priority + '</div><div><label>Status: </label>' + data.Status + '</div>' + (data.AssignedTo == "" ? "" : '<div><label>Assigned To:</label>' + data.AssignedTo + '</div>') + '<div class="notes tile-border-color">';
						for (var i = 0; i < data.Notes.length; i++)
							h += data.Notes[i].DisplayName + ' ' + data.Notes[i].Date + '<br /><pre>' + unescape(data.Notes[i].NoteText).replace(/\+/g, ' ') + '</pre>';
						h += '</div>';
						$("#ticket-" + curticket.replace(/\//gi, "_")).html(h);
						$("button").button();
                    },  error: hap.common.jsonError
				});
			}
		}
		$(window).hashchange(function () {
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curticket = window.location.href.split('#')[1].substr(7);
			else curticket = null;
			loadTicket();
		});
		$(function () {
		    $("#updateticket, #assignticket").dialog({ autoOpen: false });
		    $("#tabs > div").hide();
		    $("#toolbar a").click(function () {
		        $("#toolbar a").removeClass("active");
		        $("#tabs > div").hide();
		        $($(this).addClass("active").attr("href")).show();
		        if ($(this).index() < 5) { window.location.href = "#"; $("#updateticket, #assignticket").dialog("close"); }
		        return false;
		    });
			$("button, .button, input[type=submit]").button();
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curticket = window.location.href.split('#')[1].substr(7);
			else curticket = null;
			if (curticket == null) $("#toolbar a").first().click();
			loadTicket();
			Update();
		});
		function Update() {
		    $.ajax({
		        type: 'GET',
		        url: hap.common.formatJSONUrl("~/api/HelpDesk/Tickets/Open<%=isHDAdmin ? "" : "/" + ADUser.UserName%>"),
				dataType: 'json',
				contentType: 'application/json',
				success: function (data) {
				    var x = "";
				    for (var i = 0; i < data.length; i++) x += '<div><a href="#ticket-' + data[i].Id + '" class="' + data[i].Priority.replace(/ /g, "-") + '">' + data[i].Subject + ' <span>' + data[i].Id + ' - ' + data[i].Username + (data[i].AssignedTo == '' ? '' : (' -> ' + data[i].AssignedTo)) + ' - ' + data[i].Date + '</span></a></div>';
				    if (data.length == 0) x = "No Tickets";
				    $("#opentickets").html(x);
				}, error: hap.common.jsonError
			});
            $.ajax({
                type: 'GET',
                url: hap.common.formatJSONUrl("~/api/HelpDesk/Tickets/Closed<%=isHDAdmin ? "" : "/" + ADUser.UserName%>"),
				dataType: 'json',
				contentType: 'application/json',
				success: function (data) {
				    var x = "";
				    for (var i = 0; i < data.length; i++) x += '<div><a href="#ticket-' + data[i].Id + '" class="' + data[i].Priority.replace(/ /g, "-") + '">' + data[i].Subject + ' <span>' + data[i].Id + ' - ' + data[i].Username + (data[i].AssignedTo == '' ? '' : (' -> ' + data[i].AssignedTo)) + ' - ' + data[i].Date + '</span></a></div>';
				    if (data.length == 0) x = "No Tickets";
				    $("#closedtickets").html(x);
				}, error: hap.common.jsonError
			});
            $.ajax({
                type: 'GET',
                url: hap.common.formatJSONUrl("~/api/HelpDesk/FAQs"),
                dataType: 'json',
                contentType: 'application/json',
                success: function (data) {
                    var x = "";
                    for (var i = 0; i < data.length; i++) {
                        x += '<div><a href="#ticket-' + data[i].Id + '" class="' + data[i].Priority.replace(/ /g, "-") + '">' + data[i].Subject + '</a></div>'
                    }
                    if (data.length == 0) x = "No FAQs";
                    $("#faqs").html(x);
                }, error: hap.common.jsonError
            });
            if (hap.hdadmin) {
                $("#spinner").spinner().spinner("value", 7);
                $("#stats-content").fadeOut();
                $("#refreshstats").click(function () {
                    $("#stats-loading").fadeIn();
                    $("#stats-content").fadeOut();
                    $.ajax({
                        type: 'GET',
                        url: hap.common.formatJSONUrl("~/api/HelpDesk/Stats/" + $("#spinner").spinner("value")),
                        dataType: 'json',
                        contentType: 'application/json',
                        success: function (data) {
                            parseStats(data);
                        }, error: hap.common.jsonError
                    });
                    return false;
                });
                setTimeout(function () {
                    $.ajax({
                        type: 'GET',
                        url: hap.common.formatJSONUrl("~/api/HelpDesk/Stats"),
                        dataType: 'json',
                        contentType: 'application/json',
                        success: function (data) {
                            parseStats(data);
                        }, error: hap.common.jsonError
                    });
                }, 500);
            }
		}
	    function parseStats(data) {
	        $("#stats-loading").fadeOut();
	        $("#stats-content").fadeIn();
	        var s = "<div>Closed Tickets: <b>" + data.ClosedTickets + "</b></div><div>New Tickets: <b>" + data.NewTickets + "</b></div><div>Open Tickets: <b>" + data.OpenTickets + "</b></div>";
	        if (data.HighestUser != null) s+= "<div>Highest User: <b>" + data.HighestUser.Username + "</b> with <b>" + data.HighestUser.Tickets + "</b> tickets</div>";
	        $("#stats-body").html(s);
	    }
	</script>
    </hap:CompressJS>
    </div>
</asp:Content>