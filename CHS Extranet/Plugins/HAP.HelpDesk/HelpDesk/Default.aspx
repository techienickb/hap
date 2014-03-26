<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.HelpDesk.HelpDesk.Default" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="../style/helpdesk.css" rel="stylesheet" type="text/css" />
    <link href="../style/jquery.dataTables.css" rel="stylesheet" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="viewport"><meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" /></asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/HelpDesk/"><hap:LocalResource runat="server" StringPath="helpdesk/helpdesk" /></asp:HyperLink></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
    <div id="toolbar"><a id="opentickets-link" href="#opentickets" onclick="return false;"><hap:LocalResource StringPath="helpdesk/opentickets" runat="server" /></a><a id="closedtickets-link" href="#closedtickets" onclick="return false;"><hap:LocalResource StringPath="helpdesk/closedtickets" runat="server" /></a><%if (isHDAdmin || hasArch) { %><a id="archivedtickets-link" href="#archivedtickets" onclick="return false;"><hap:LocalResource StringPath="helpdesk/archivedtickets" runat="server" /></a><%} %><a href="#faqs" id="faq-link" onclick="return false;"><hap:LocalResource StringPath="helpdesk/faqs" runat="server" /></a><a href="#newticket" id="newticket-link"><hap:LocalResource StringPath="helpdesk/newtickets" runat="server" /></a><%if (isHDAdmin) { %><a href="#stats" id="stats-link"><hap:LocalResource runat="server" StringPath="helpdesk/stats" /></a><%} %><%if (isUpgrade) { %><asp:LinkButton runat="server" ID="migrate" Text="Migrate to SQL" OnClick="migrate_Click" Visible="false" /><%} %></div>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div id="hap-HD">
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
                                        for (var i = 0; i < data.length; i++) x += '<div><a href="#ticket-' + $("#<%=archiveddates.ClientID%>").val() + '/' + data[i].Id + '" id="#ticket-' + $("#<%=archiveddates.ClientID%>").val() + '/' + data[i].Id + '" class="' + data[i].Priority.replace(/ /g, "-") + '">' + data[i].Subject + ' <span>' + data[i].Id + ' - ' + data[i].Username + (data[i].AssignedTo == '' ? '' : (' -> ' + data[i].AssignedTo)) + ' - ' + data[i].Date + '</span></a></div>';
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

		<div id="faqs">
			<div style="text-align: center;"><img src="../images/metroloading.gif" /></div>
            <hap:LocalResource runat="server" StringPath="loading" />
		</div>
	</div>
    <div id="HDmain">
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
        <div id="currentticket" style="display: none;">
            <div style="text-align: center;" id="curtick-loading"><img src="../images/metroloading.gif" /><br /><hap:LocalResource runat="server" StringPath="loading" /></div>
            <div id="HDtop">
                <div style="float: right;" id="HDcontrols">
                    <div style="text-align: right;">
                        <!--<button onclick="return assignTicket();" class="hdadmin">Assign</button>-->
                        <button onclick="return setFAQ();" class="hdadmin">FAQ</button>
                        <button onclick="return updateTicket();">Update</button>
                    </div>
                    <div>
                        <div class="hdadmin"><label>Users Aware: </label><input type="text" id="ticket-AwareI" /></div>
                        <div class="hdadmin"><label>Show To: </label><input type="text" id="ticket-ShowToI" /></div>
                    </div>
                </div>
                <div id="HDsubject">
                    <label>Ticket: </label><span id="ticket-Subject"></span><input type="text" class="hdadmin" id="ticket-SubjectI" />
                </div>
                <div><label>Opened By: </label><span id="ticket-Username"></span></div>
                <div><label>Opened on: </label><span id="ticket-Date"></span></div>
                <div>
                    <label>Priority: </label><span id="ticket-Priority"></span><select id="ticket-PriorityI"><%foreach (string s in config.HelpDesk.Priorities.Split(new char[] { ',' })) { %><option><%=s.Trim() %></option><%} %></select>
                </div>
                <div>
                    <label>Status: </label><span id="ticket-Status"></span><select id="ticket-StatusI">
                        <optgroup id="closedstates" label="Open States"><%foreach (string s in (isHDAdmin ? config.HelpDesk.OpenStates : config.HelpDesk.UserOpenStates).Split(new char[] { ',' })) { %><option><%=s.Trim() %></option><%} %></optgroup>
                        <optgroup id="openstates" label="Closed States"><%foreach (string s in (isHDAdmin ? config.HelpDesk.ClosedStates : config.HelpDesk.UserClosedStates).Split(new char[] { ',' })) { %><option><%=s.Trim() %></option><%}%></optgroup>
                    </select>
                </div>
                <div class="hdadmin"><label>Assigned To: </label><span id="ticket-AssignedTo"></span><asp:DropDownList runat="server" ID="userlist2" /></div>
            </div>
            <div id="notes">
            </div>
            <div id="newnote">
                <label for="ticket-note">New Notes: </label><label class="hdadmin" for="ticket-hidenote">Hide Note </label><input class="hdadmin" type="checkbox" id="ticket-hidenote" /><br />
                <textarea id="ticket-note" style="width: 100%; height: 200px;" rows="8" cols="10"></textarea>
            </div>
            <div style="text-align: right;" id="HDlowercontrols"><button onclick="return updateTicket();">Update</button></div>
        </div>
	    <div id="newticket">
		    <div>
			    <label for="newticket-subject"><hap:LocalResource StringPath="helpdesk/issue" runat="server" />: </label>
			    <input type="text" id="newticket-subject" style="width: 400px;" />
		    </div>
		    <div>
			    <label for="newticket-room">PC/<hap:LocalResource StringPath="helpdesk/room" runat="server" />: </label>
			    <input type="text" id="newticket-room" />
                <asp:HiddenField runat="server" ID="newticket_pc" />
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
    </div>
	<script>
	    userlist2 = $("#<%=userlist2.ClientID%>");
	    userlist = $("#<%=userlist.ClientID %>");
	    newticket_pc = $("#<%=newticket_pc.ClientID%>");
	</script>
    </div>
</asp:Content>