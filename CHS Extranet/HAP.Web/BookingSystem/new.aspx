<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="new.aspx.cs" Inherits="HAP.Web.BookingSystem._new" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
	<link href="../style/bookingsystem.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both; position: relative;">
		<div class="tiles" style="float: left;">
			<a class="button" href="../">Home Access Plus+ Home</a>
		</div>
		<div class="tiles" style="float: right; text-align: right;">
			<asp:HyperLink CssClass="button" runat="server" NavigateUrl="Admin/" ID="adminlink" Text="Control Panel" />
			<a href="OverviewCalendar.aspx" id="overview" class="button">Overview</a>
		</div>
		<div style="text-align: center;">
			<img src="../images/booking-system.png" alt="Booking System" />
		</div>
	</div>
	<p class="ui-state-highlight ui-corner-all" style="padding: 2px 6px">
		<span class="ui-icon ui-icon-info" style="float: left; margin-right: 5px;"></span>
		<span id="val">Loading...</span>
	</p>
	<div id="overviewcalendar" title="Overview Calendar">
		<iframe src="OverviewCalendar.aspx" style="border: 0; margin: 0; padding: 0; width: 100%; height: 400px;"></iframe>
	</div>
	<div id="bookingform" title="Booking Form">
		<div>
			<p class="ui-state-highlight ui-corner-all" style="margin-bottom: 4px; padding: 4px 6px">
				<span class="ui-icon ui-icon-info" style="float: left; margin-right: 5px;"></span>
				New Booking for <span id="bfdate"></span> <span id="bfres"></span> during <span id="bflesson"></span>
			</p>
			<label for="bfyear">Year: </label><select id="bfyear"><option value="">---</option><option value="Year 7">Year 7</option><option value="Year 8">Year 8</option><option value="Year 9">Year 9</option><option value="Year 10">Year 10</option><option value="Year 11">Year 11</option><option value="Year 12">Year 12</option><option value="Year 13">Year 13</option><option value="A-Level">A-Level</option></select>
			<label for="bfsubject">Subject: </label>
			<select id="bfsubjects" onchange="subjectchance(this)">
				<option value="" selected="selected">- Subject -</option>
				<asp:Repeater runat="server" ID="subjects"><ItemTemplate><option value="<%#Container.DataItem %>"><%#Container.DataItem %></option></ItemTemplate></asp:Repeater>
				<option value="CUSTOM">Custom</option>
			</select>
			<input type="text" id="bfsubject" />
			<span id="subjecterror" style="display: none; color: red;">*</span>
			<asp:PlaceHolder runat="server" ID="adminbookingpanel">
			<div>
				<asp:Label runat="server" AssociatedControlID="userlist" Text="User To Book For: " />
				<asp:DropDownList runat="server" ID="userlist" />
			</div>
			</asp:PlaceHolder>
			<div id="bfLaptops" class="bfType">
				<div style="float: left;">
					<label for="bflroom">Room Required In: </label><input type="text" id="bflroom" style="width: 60px" /> <span id="bflroomerror" style="display: none;">*</span>
					<label for="bflheadphones">Headphones?: </label><input type="checkbox" id="bflheadphones" />
					<label for="bflquant">Quantity: </label>
				</div>
				<div id="bflquant" style="float: left;">
					<input type="radio" name="bflquant" id="bflquant-16" value="16" /><label for="bflquant-16">16</label>
					<input type="radio" name="bflquant" id="bflquant-32" value="32" /><label for="bflquant-32">32</label>
				</div>
			</div>
			<div id="bfEquipment" class="bfType">
					<label for="bferoom">Room Required In: </label><input type="text" id="bferoom" style="width: 60px" /> <span id="bferoomerror" style="display: none;">*</span>
			</div>
		</div>
	</div>
	<div id="datepicker" style="position: absolute;"></div>
	<div id="bookingsystemcontent">
		<div id="bookingday" class="tile-border-color">
			<div class="head tile-color">
				<h1><input type="button" id="picker" onclick="return showDatePicker();" /></h1>
				<asp:Repeater runat="server" ID="lessons"><ItemTemplate><h1><%#Eval("Name") %></h1></ItemTemplate></asp:Repeater>
			</div>
			<div class="body"<%=BodyCode[0] %>>
				<div id="resources" class="col tile-color">
					<asp:Repeater runat="server" ID="resources1"><ItemTemplate><div><%#Eval("Name") %></div></ItemTemplate></asp:Repeater>
				</div>
				<asp:Repeater runat="server" ID="resources2">
					<ItemTemplate>
						<div id="<%#Eval("Name") %>" class="col">
						</div>
					</ItemTemplate>
				</asp:Repeater>
			</div><%=BodyCode[1] %>
		</div>
	</div>
	<script type="text/javascript">
		var curdate;
		var user = { <%=JSUser %> };
		var resources = <%=JSResources %>;
		var curres;
		var curles;
		var availbookings = [ 0, 0 ];
		function resource(name, type){
			this.Name = name;
			this.Type = type;
			this.Refresh = function() {
				$.ajax({
					type: 'GET',
					url: '<%=ResolveUrl("~/api/BookingSystem/LoadRoom/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + this.Name,
					dataType: 'json',
					context: this,
					success: function (data) {
						var h = "";
						for (var x = 0; x < data.length; x++) {
							h += '<a onclick="return ';
							if (data[x].Name == "FREE") h += "doBooking('" +  this.Name + "', '" + data[x].Lesson + "');";
							else {
								if (data[x].Static && $.inArray(this.Name, user.isAdminOf) != -1) h += "doBooking('" + this.Name + "', '" + data[x].Lesson + "');";
								else if (data[x].Static == false && (data[x].Username == user.username || $.inArray(this.Name, user.isAdminOf) != -1)) h += "doRemove('" + this.Name + "', '" + data[x].Lesson + "', '" + data[x].Name + "');";
								else h += "false;";
							}
							h += '" href="#' + this.Name + '-' + data[x].Lesson.toLowerCase().replace(/ /g, "") + '" class="' + ($.inArray(this.Name, user.isAdminOf) == -1 ? '' : 'admin') + ((data[x].Username == user.username && $.inArray(this.Name, user.isAdminOf) == -1) ? ' bookie' : '') + ((data[x].Name == "FREE") ? ' free' : '') + '">';
							h += (data[x].Static ? '<span class="state static" title="Timetabled Lesson"><i></i><span>Override</span></span>' : (data[x].Name == "FREE" ? '<span class="state book" title="Book"><i></i><span>Book</span></span>' : '<span class="state remove" title="Remove"><i></i><span>Remove</span></span>'));
							h += data[x].Name + '<span>' + data[x].DisplayName;
							if (data[x].Name == "FREE" || data[x].Name == "UNAVAILABLE" || data[x].Name == "CHARGING") { }
							else {
								if (this.Type == "Laptops") h += ' in ' + data[x].LTRoom + ' [' + data[x].LTCount + '|' + (data[x].LTHeadPhones ? 'HP' : 'N-HP') + ']';
								else if (this.Type == "Equipment") h += ' in ' + data[x].EquipRoom;
							}
							h += '</span></a>';
						}
						$("#" + this.Name).html(h);
					},
					error: OnError
				});
			}
		}
		function subjectchance(box) {
			var chosenoption = box.options[box.selectedIndex] //this refers to "selectmenu"
			if (chosenoption.value == "CUSTOM") {
				$("#bfsubject").val("");
				$("#bfsubject").removeAttr("style");
				$("#bfsubject").select();
			} else {
				$("#bfsubject").val(chosenoption.value);
				$("#bfsubject").css("display", "none");
			}
		}
		function disableAllTheseDays(date) {
			var noWeekend = $.datepicker.noWeekends(date);
			
			if (noWeekend[0]) {
				if (date <= user.maxDate && isDateInTerm(date)) return [true];
				return [false];
			} else {
				return noWeekend;
			}
		}
		function isDateInTerm(date) {
			var terms = [ <%=JSTermDates %> ];
			for (var i = 0; i < terms.length; i++)
			{
				if (terms[i].start <= date && terms[i].end >= date) {
					if (terms[i].halfterm.start <= date && terms[i].halfterm.end >= date) return false;
					else return true;
				}
			}
			return false;
		}
		var showCal = 0;
		function showDatePicker() {
			if (showCal == 0) { $("#datepicker").animate({ height: 'toggle' }); showCal = 1; }
			return false;
		}
		function loadDate() {
			$("#val").html("Loading...");
			$.ajax({
				type: 'GET',
				url: '<%=ResolveUrl("~/api/BookingSystem/Initial/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username,
				dataType: 'json',
				success: function (data) {
					if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
					else {
						$("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
						availbookings = data;
					}
				},
				error: OnError
			});
			for (var i = 0; i < resources.length; i++) {
				$("#" + resources[i].Name).html(" ");
				resources[i].Refresh();
			}
		}
		function OnError(xhr, ajaxOptions, thrownError) {
		}
		$(window).hashchange(function () {
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curdate = new Date(window.location.href.split('#')[1].split('/')[2], window.location.href.split('#')[1].split('/')[1] - 1, window.location.href.split('#')[1].split('/')[0]);
			else curdate = new Date(<%=CurrentDate.Year %>, <%=CurrentDate.Month - 1 %>, <%=CurrentDate.Day %>);
			$('#datepicker').datepicker("setDate", curdate);
			$("#picker").val($.datepicker.formatDate('d MM yy', curdate));
			loadDate();
		});
		function doBooking(res, lesson) {
			if (availbookings[0] <= 0 && !user.isBSAdmin) { 
				alert("You have exceeded your allowed bookings, please contact an Admin if this is wrong");
				return false; 
			}
			curles = lesson;
			$("#bfdate").html($.datepicker.formatDate('d MM yy', curdate));
			for (var i = 0; i < resources.length; i++)
				if (resources[i].Name == res) curres = resources[i];
			$("#bfsubject").css("display", "none");
			$(".bfType").css("display", "none");
			$(".bfType > input").val("");
			if (curres.Type == "Room") $("#bfres").html("in " + curres.Name);
			else {
				$("#bfres").html("with " + curres.Name);
				if ($("#bf" + curres.Type) != null) $("#bf" + curres.Type).css("display", "block");
			}
			$("#bflroomerror").css("display", "none");
			$("#bferoomerror").css("display", "none");
			$("#bfsubject").val("");
			$("#bfsubjects option:selected").removeAttr("selected")
			$("#bfsubjects option:first").attr("selected", "selected")
			$("#bflheadphones").removeAttr("checked");
			$("#bflroom").val("");
			$("#bferoom").val("");
			$("#bflesson").html(lesson);
			$("#bookingform").dialog({ 
					modal: true, 
					autoOpen: true,
					minWidth: 450,
					buttons: {
						"Book": function () {
							var abort = false;
							if ($("#bfsubject").val().length == 0) { 
								$("#subjecterror").removeAttr("style").css("color", "red");
								abort = true;
							} else $("#subjecterror").css("display", "none");

							if (curres.Type == "Laptops" && $("#bflroom").val().length == 0) { 
								$("#bflroomerror").removeAttr("style").css("color", "red");
								abort = true;
							} else $("#bflroomerror").css("display", "none");

							if (curres.Type == "Equipment" && $("#bferoom").val().length == 0) { 
								$("#bferoomerror").removeAttr("style").css("color", "red");
								abort = true;
							} else $("#bferoomerror").css("display", "none");
							
							var n1 = "";
							if ($("#bfyear option:selected").val() != null || $("#bfyear option:selected").val() != "") n1 = $("#bfyear option:selected").val();
							if (n1 != "") n1 += " ";
							n1 += $("#bfsubject").val();
							if (abort) return;
							var d = '{ "booking": { "Room": "' + curres.Name + '", "Lesson": "' + curles + '", "Username": "' + (user.isBSAdmin ? $("#<%=userlist.ClientID %> option:selected").val() : user.username) + '", "Name": "' + n1 + '"';
							if (curres.Type == "Laptops") {
								d += ', "LTCount": ' + $("#bflquant input:checked").attr("value") + ', "LTRoom": "' + $("#bflroom").val() + '", "LTHeadPhones": ' + (($('#bflheadphones:checked').val() !== undefined) ? 'true' : 'false');
							}
							else if (curres.Type == "Equipment") {
								d += ', "EquipRoom": "' + $("#bferoom").val() + '"';
							}
							d += " } }";
							$.ajax({
								type: 'POST',
								url: '<%=ResolveUrl("~/api/BookingSystem/Booking/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear(),
								dataType: 'json',
								contentType: 'application/json; charset=utf-8',
								data: d,
								success: function (data) {
									$.ajax({
										type: 'GET',
										url: '<%=ResolveUrl("~/api/BookingSystem/Initial/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username,
										dataType: 'json',
										success: function (data) {
											if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
											else {
												$("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
												availbookings = data;
											}
										},
										error: OnError
									});
									curres.Refresh();
								},
								error: OnError
							});
							
							$(this).dialog("close");
						},
						"Cancel": function () {
							$(this).dialog("close");
						}
					}
				});
			return false;
		}
		function doRemove(res, lesson, name) {
			if (confirm("Are you sure you want to remove the booking\n" + name + " in " + res + " during " + lesson + "?")) {
				curles = lesson;
				$("#bfdate").html($.datepicker.formatDate('d MM yy', curdate));
				for (var i = 0; i < resources.length; i++)
					if (resources[i].Name == res) curres = resources[i];
				$.ajax({
					type: 'DELETE',
					url: '<%=ResolveUrl("~/api/BookingSystem/Booking/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear(),
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					data: '{ "booking": { "Room": "' + curres.Name + '", "Lesson": "' + curles + '", "Name": "' + name + '" } }',
					success: function (data) {
						$.ajax({
							type: 'GET',
							url: '<%=ResolveUrl("~/api/BookingSystem/Initial/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username,
							dataType: 'json',
							success: function (data) {
								if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
								else {
									$("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
									availbookings = data;
								}
							},
							error: OnError
						});
						curres.Refresh();
					},
					error: OnError
				});
			}
			return false;
		}
		$(function () {
			try {
				if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curdate = new Date(window.location.href.split('#')[1].split('/')[2], window.location.href.split('#')[1].split('/')[1] - 1, window.location.href.split('#')[1].split('/')[0]);
				else curdate = new Date(<%=CurrentDate.Year %>, <%=CurrentDate.Month - 1 %>, <%=CurrentDate.Day %>);
			} catch (ex) { alert(ex); }
			$("#datepicker").datepicker({ 
				minDate: user.minDate,
				maxDate: user.maxDate,
				beforeShowDay: disableAllTheseDays,
				showOtherMonths: true,
				selectOtherMonths: true,
				defaultDate: curdate,
				onSelect: function(dateText, inst) {
					curdate = new Date(dateText);
					location.href = "#" + $.datepicker.formatDate('dd/mm/yy', curdate);
				}
			});
			$("#overview").click(function () {
				$("#overviewcalendar").dialog({ 
					autoOpen: true,
					minWidth: 450
				});
				return false;
			});
			$("#overviewcalendar").dialog({ autoOpen: false });
			$("#bookingform").dialog({ autoOpen: false });
			$("#picker").val($.datepicker.formatDate('d MM yy', curdate));
			$("input[type=button]").button();
			$(".button").button();
			$("#datepicker").css("top", $("#picker").position().top + 29);
			$("#datepicker").animate({ height: 'toggle' });
			$("#bflquant").buttonset();
			$("#bookingsystemcontent").click(function() {
				if (showCal == 2) { $("#datepicker").animate({ height: 'toggle' }); showCal = 0; }
				else if (showCal == 1) showCal = 2;
			});
			loadDate();
		});
	</script>

</asp:Content>
