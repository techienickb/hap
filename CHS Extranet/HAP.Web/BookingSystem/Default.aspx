<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.BookingSystem._new" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
	<link href="../style/bookingsystem.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both; position: relative; height: 120px">
		<div class="tiles" style="float: left; margin-top: 45px;">
			<a class="button" href="../">Home Access Plus+ Home</a>
		</div>
		<div class="tiles" style="float: right; text-align: right; margin-top: 45px;">
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
		<label for="overviewsearch"><hap:LocalResource runat="server" StringPath="bookingsystem/quicksearch" /></label><input type="text" id="overviewsearch" />
		<iframe src="OverviewCalendar.aspx" style="border: 0; margin: 0; padding: 0; width: 100%; height: 440px;"></iframe>
		<div id="searchresults" style="height: 440px; overflow: auto;"></div>
		<hap:CompressJS runat="server" Tag="div">
			<script type="text/javascript">
			    $(function () {
			        $('#searchresults').hide();
			        $('#overviewsearch').keyup(function (e) {
			            if ($('#overviewsearch').val().length == 0) {
			                $('#searchresults').hide();
			                $("#overviewcalendar iframe").show();
			            } else {
			                $('#searchresults').html("Searching...").show();
			                $("#overviewcalendar iframe").hide();

			                $.ajax({
			                    type: 'POST',
			                    url: '<%=ResolveUrl("~/api/BookingSystem/Search")%>?' + window.JSON.stringify(new Date()),
			                    dataType: 'json',
			                    data: '{ "Query": "' + $('#overviewsearch').val() + '" }',
			                    contentType: 'application/json',
			                    success: function (data) {
			                        $('#searchresults').html("");
			                        var d = "";
			                        for (var i = 0; i < data.length; i++) {
			                            var item = data[i];
			                            var h = "<div" + (d != (item.Date.match(/[0|1][0-9]\w\w\w/g) ? item.Date.substr(2, item.Date.length - 2) : item.Date) ? ' class="newline"><span class="date">' + (item.Date.match(/[0|1][0-9]\w\w\w/g) ? item.Date.substr(2, item.Date.length - 2) : item.Date) : '><span class="date">') + "</span><span>" + item.Room + "</span><span>" + item.Username + "</span><span>" + item.Name + "</span></div>";
			                            d = item.Date.match(/[0|1][0-9]\w\w\w/g) ? item.Date.substr(2, item.Date.length - 2) : item.Date;
			                            $('#searchresults').append(h);
			                        }
			                    },
			                    error: hap.common.jsonError
			                });
			            }
			        });
			    });
			</script>
		</hap:CompressJS>
	</div>
	<div id="questionbox" title="Question"><span></span></div>
	<div id="bookingform" title="Booking Form">
		<div>
			<p class="ui-state-highlight ui-corner-all" style="margin-bottom: 4px; padding: 4px 6px">
				<span class="ui-icon ui-icon-info" style="float: left; margin-right: 5px;"></span>
				New Booking for <span id="bfdate"></span> <span id="bfres"></span> during <span id="bflesson"></span>
			</p>
			<label for="bfyear">Year: </label><select id="bfyear"></select>
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
					<label for="bflquant">Quantity:&nbsp;</label>
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
			<div class="head tile-color" style="width: <%=(156 * (config.BookingSystem.Lessons.Count + 1)) + 2 %>px">
				<h1><input type="button" id="picker" onclick="return showDatePicker();" /></h1>
				<asp:Repeater runat="server" ID="lessons"><ItemTemplate><h1><%#Eval("Name") %></h1></ItemTemplate></asp:Repeater>
			</div>
			
			<div class="body"<%=BodyCode[0] %> style="width: <%=(156 * (config.BookingSystem.Lessons.Count + 1)) + 2 %>px">
				<div id="resources" class="col tile-color">
					<asp:Repeater runat="server" ID="resources1"><ItemTemplate><div><%#Eval("Name") %></div></ItemTemplate></asp:Repeater>
				</div>
				<asp:Repeater runat="server" ID="resources2">
					<ItemTemplate>
						<div id="<%#Eval("Name").ToString().Replace(" ", "_") %>" class="col">
						</div>
					</ItemTemplate>
				</asp:Repeater>
			</div><%=BodyCode[1] %>
		</div>
	</div>
	<hap:CompressJS runat="server" tag="div">
	<script type="text/javascript">
		var curdate, curres, curles;
		var user = { <%=JSUser %> };
		var resources = <%=JSResources %>;
		var availbookings = [ 0, 0 ];
		function resource(name, type, years, quantities, readonly){
			this.Name = name;
			this.Type = type;
			this.Years = years;
			this.Quantities = quantities;
			this.Data;
			this.ReadOnly = readonly;
			this.Render = function() {
				var h = "";
				for (var x = 0; x < this.Data.length; x++) {
				    h += '<a onclick="return ';
				    if (this.ReadOnly) h += "false";
					else if (this.Data[x].Name == "FREE") h += "doBooking('" +  this.Name + "', '" + this.Data[x].Lesson + "');";
					else {
						if (this.Data[x].Static && $.inArray(this.Name, user.isAdminOf) != -1) h += "doBooking('" + this.Name + "', '" + this.Data[x].Lesson + "');";
						else if (this.Data[x].Static == false && (this.Data[x].Username.toLowerCase() == user.username.toLowerCase() || $.inArray(this.Name, user.isAdminOf) != -1)) h += "doRemove('" + this.Name + "', '" + this.Data[x].Lesson + "', '" + this.Data[x].Name + "');";
						else h += "false;";
					}
					h += '" href="#' + this.Name + '-' + this.Data[x].Lesson.toLowerCase().replace(/ /g, "") + '" class="' + (this.Data[x].Static ? 'static ' : '') + ($.inArray(this.Name, user.isAdminOf) == -1 ? '' : 'admin') + ((this.Data[x].Username.toLowerCase() == user.username.toLowerCase() && $.inArray(this.Name, user.isAdminOf) == -1) ? ' bookie' : '') + ((this.Data[x].Name == "FREE" && !this.ReadOnly) ? ' free' : ' booked') + '">';
					h += (this.Data[x].Static ? '<span class="state static" title="Timetabled Lesson"><i></i><span>Override</span></span>' : (this.Data[x].Name == "FREE" ? '<span class="state book" title="Book"><i></i><span>Book</span></span>' : '<span class="state remove" title="Remove"><i></i><span>Remove</span></span>'));
					h += this.Data[x].Name + '<span>' + this.Data[x].DisplayName;
					if (this.Data[x].Name == "FREE" || this.Data[x].Name == "UNAVAILABLE" || this.Data[x].Name == "CHARGING") { }
					else {
						if (this.Type == "Laptops") h += ' in ' + this.Data[x].LTRoom + ' [' + this.Data[x].LTCount + '|' + (this.Data[x].LTHeadPhones ? 'HP' : 'N-HP') + ']';
						else if (this.Type == "Equipment") h += ' in ' + this.Data[x].EquipRoom;
					}
					h += '</span></a>';
				}
				$("#" + this.Name.replace(/ /g, '_')).html(h);
			};
			this.Refresh = function() {
				$.ajax({
					type: 'GET',
					url: '<%=ResolveUrl("~/api/BookingSystem/LoadRoom/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + this.Name + '?' + window.JSON.stringify(new Date()),
					dataType: 'json',
					context: this,
					success: function (data) {
						this.Data = data;
						this.Render();
					},
					error: hap.common.jsonError
				});
			};
		}
		function subjectchance(box) {
			var chosenoption = box.options[box.selectedIndex];
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
				url: '<%=ResolveUrl("~/api/BookingSystem/Initial/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username.toLowerCase() + '?' + window.JSON.stringify(new Date()),
				dataType: 'json',
				success: function (data) {
					if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
					else {
						$("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
						availbookings = data;
					}
				},
				error: hap.common.jsonError
			});
			for (var i = 0; i < resources.length; i++) {
				$("#" + resources[i].Name).html(" ");
				resources[i].Refresh();
			}
		}
		$(window).hashchange(function () {
			
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curdate = new Date(window.location.href.split('#')[1].split('/')[2], window.location.href.split('#')[1].split('/')[1] - 1, window.location.href.split('#')[1].split('/')[0]);
			else curdate = new Date(<%try { %><%=CurrentDate.Year %>, <%=CurrentDate.Month - 1 %>, <%=CurrentDate.Day %> <% } catch { } %>);
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
			$("#bfsubjects option:selected").removeAttr("selected");
			$("#bfsubjects option:first").attr("selected", "selected");
			$("#bflheadphones").removeAttr("checked");
			$("#bflroom").val("");
			$("#bferoom").val("");
			$("#bfyear option").remove();
			$("#bfyear").append('<option value="">---</option>');
			for (var i = 0; i < curres.Years.length; i++)
			    $("#bfyear").append('<option value="' + curres.Years[i] + '">' + curres.Years[i] + '</option>');
			$("#bflquant input, #bflquant label").remove();
			for (var i = 0; i < curres.Quantities.length; i++)
			    $("#bflquant").append('<input type="radio" name="bflquant" id="bflquant-' + curres.Quantities[i] + '" value="' + curres.Quantities[i] + '" /><label for="bflquant-' + curres.Quantities[i] + '">' + curres.Quantities[i] + '</label>');
			$("#bflquant").buttonset();
			$("#bflesson").html(lesson);
			$("#bookingform").dialog({ 
					modal: true, 
					autoOpen: true,
					minWidth: 500,
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
								url: '<%=ResolveUrl("~/api/BookingSystem/Booking/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '?' + window.JSON.stringify(new Date()),
								dataType: 'json',
								contentType: 'application/json',
								data: d,
								success: function (data) {
									curres.Data = data;
									curres.Render();
									$.ajax({
										type: 'GET',
										url: '<%=ResolveUrl("~/api/BookingSystem/Initial/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username + '?' + window.JSON.stringify(new Date()),
										dataType: 'json',
										success: function (data) {
											if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
											else {
												$("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
												availbookings = data;
											}
										},
										error: hap.common.jsonError
									});
								},
								error: hap.common.jsonError
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
			$("#questionbox span").html("Are you sure you want to remove<br/>" + name + " in/with " + res + " during " + lesson);
			$("#questionbox").dialog({ autoOpen: true, 
				buttons: { "Yes": function() {
					curles = lesson;
					$("#bfdate").html($.datepicker.formatDate('d MM yy', curdate));
					for (var i = 0; i < resources.length; i++)
						if (resources[i].Name == res) curres = resources[i];
					$.ajax({
						type: 'DELETE',
						url: '<%=ResolveUrl("~/api/BookingSystem/Booking/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '?' + window.JSON.stringify(new Date()),
						dataType: 'json',
						contentType: 'application/json',
						data: '{ "booking": { "Room": "' + curres.Name + '", "Lesson": "' + curles + '", "Name": "' + name + '" } }',
						success: function (data) {
							curres.Data = data;
							curres.Render();
							$.ajax({
								type: 'GET',
								url: '<%=ResolveUrl("~/api/BookingSystem/Initial/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username + '?' + window.JSON.stringify(new Date()),
								dataType: 'json',
								success: function (data) {
									if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
									else {
										$("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
										availbookings = data;
									}
								},
								error: hap.common.jsonError
							});
						},
						error: hap.common.jsonError
					});
					$(this).dialog("close");
				}, "No": function() {
					$(this).dialog("close");
				} } 
			});
			return false;
		}
		$(function () {
			try {
				if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curdate = new Date(window.location.href.split('#')[1].split('/')[2], window.location.href.split('#')[1].split('/')[1] - 1, window.location.href.split('#')[1].split('/')[0]);
				else curdate = new Date(<%try { %><%=CurrentDate.Year %>, <%=CurrentDate.Month - 1 %>, <%=CurrentDate.Day %><% } catch { } %>);
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
			$("#questionbox").dialog({ autoOpen: false });
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
	</hap:CompressJS>
</asp:Content>
