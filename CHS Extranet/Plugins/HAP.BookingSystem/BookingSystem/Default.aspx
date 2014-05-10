<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.BookingSystem._new" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
    <style>
        #bookingday #resources, .col a, .col .share, #bookingday .head h1 { width: <%=Math.Round(100.00 / (config.BookingSystem.Lessons.Count + 1), 1)%>%; }
        #bookingday #resources { min-height: <%=(60 * (rez.Count + 1)) - 2 %>px; }
        #bookingday .body .col, #bookingday #resources div { height: <%=Math.Round(100.00 / (rez.Count + 1), 1) %>%; }
    </style>
</asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/BookingSystem/"><hap:LocalResource runat="server" StringPath="bookingsystem/bookingsystem" /></asp:HyperLink></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="viewport"><meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" /></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
	<asp:HyperLink runat="server" NavigateUrl="Admin/" ID="adminlink" Text="Control Panel" style="float: right;" />
	<a href="OverviewCalendar.aspx" id="overview">Overview</a>
    <a href="Cal.aspx" id="wv">Weekview</a>
	<a id="help" href="#" style="float: right;" onclick="hap.help.Load('bookingsystem/index'); return false;"><hap:LocalResource StringPath="help" runat="server" /></a>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
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
			                    url: hap.common.formatJSONUrl('~/api/BookingSystem/Search'),
			                    dataType: 'json',
			                    data: '{ "Query": "' + $('#overviewsearch').val() + '" }',
			                    contentType: 'application/json',
			                    success: function (data) {
			                        $('#searchresults').html("");
			                        var d = "";
			                        for (var i = 0; i < data.length; i++) {
			                            var item = data[i];
			                            var h = "<div" + (d != (item.Date.match(/[0|1][0-9]\w\w\w/g) ? item.Date.substr(2, item.Date.length - 2) : item.Date) ? ' class="newline"><span class="date">' + (item.Date.match(/[0|1][0-9]\w\w\w/g) ? item.Date.substr(2, item.Date.length - 2) : item.Date) : '><span class="date">') + "</span><span>" + item.Lesson + "</span><span>" + item.Room + "</span><span>" + item.Username + "</span><span>" + item.Name + "</span></div>";
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

	<div id="bookingsystemcontent">
	    <p class="ui-state-highlight ui-corner-all" style="padding: 2px 6px">
		    <span class="ui-icon ui-icon-info" style="float: left; margin-right: 5px;"></span>
		    <span id="val">Loading...</span>
	    </p>
        <div id="datepicker" style="position: absolute;"></div>
        <div id="bscontent">
		    <div id="bookingday" class="tile-border-color">		
			    <div class="body"<%=BodyCode[0] %> style="min-width: <%=(200 * (config.BookingSystem.Lessons.Count + 1)) + 2 %>px">
                    <div id="time"></div>
				    <div id="resources" class="tile-color">
                        <div class="head"><input type="button" id="picker" onclick="return showDatePicker();" /></div>
					    <asp:Repeater runat="server" ID="resources1"><ItemTemplate><div><a href="<%#ResolveClientUrl("~/bookingsystem/r-" + Eval("Name").ToString()) %>"><%#Eval("Name") %></a></div></ItemTemplate></asp:Repeater>
				    </div>
                    <div class="head col"><asp:Repeater runat="server" ID="lessons"><ItemTemplate><h1><%#Eval("Name") %></h1></ItemTemplate></asp:Repeater></div>
				    <asp:Repeater runat="server" ID="resources2">
					    <ItemTemplate>
						    <div id="<%#Eval("Name").ToString().Replace(" ", "_") %>" class="col">
						    </div>
					    </ItemTemplate>
				    </asp:Repeater>
			    </div><%=BodyCode[1] %>
		    </div>
            <script type="text/javascript">
                $(document).ready(function () { 
                    if ($("#bookingsystemcontent").width() > $("#bookingday > .head").width() + 2)
                        $("#bookingsystemcontent").css("width", $("#bookingday > .head").width() + 2 + "px");
                    else $("#bookingsystemcontent").removeAttr("style");
                });
                $(window).resize(function () {
                    $("#bookingsystemcontent").removeAttr("style");
                    if ($("#bookingsystemcontent").width() > $("#bookingday > .head").width() + 2)
                        $("#bookingsystemcontent").css("width", $("#bookingday > .head").width() + 2 + "px");
                    else $("#bookingsystemcontent").removeAttr("style");
                    changeTime();
                });
            </script>
        </div>
	</div>
	<div id="sidebaredit">
        <h1>New Booking</h1>
		<p class="ui-state-highlight ui-corner-all" style="margin-top: 0; padding: 4px 6px">
			<span class="ui-icon ui-icon-info" style="float: left; margin-right: 5px;"></span>
			New Booking for <span id="bfdate"></span> <span id="bfres"></span> during <span id="bflesson"></span>
		</p>
		<label for="bfyear">Year: </label><select id="bfyear"></select><br />
		<label for="bfsubject">Subject: </label><select id="bfsubjects" onchange="subjectchance(this)">
            <%if (config.BookingSystem.Subjects.Count > 1) { %>
			<option value="" selected="selected">- Subject -</option>
            <%} %>
			<asp:Repeater runat="server" ID="subjects"><ItemTemplate><option value="<%#Container.DataItem %>"><%#Container.DataItem %></option></ItemTemplate></asp:Repeater>
			<option value="CUSTOM">Custom</option>
		</select>
		<input type="text" id="bfsubject" />
		<span id="subjecterror" style="display: none; color: red;">*</span><br />
		<div id="bfLaptops" class="bfType">
				<div id="bflroom_span"><label for="bflroom">Room Required In: </label><input type="text" id="bflroom" style="width: 60px" /></div>
                <div id="bflsroom_span" style="display: none;"><label for="bflsroom">Room Required In: </label><select id="bflsroom"></select></div>
                <div id="bflroomerror" style="display: none;">*</div>
				<label for="bflheadphones">Headphones?: </label><input type="checkbox" id="bflheadphones" />
		</div>
		<div id="bfEquipment" class="bfType">
			<span id="bferoom_span"><label for="bferoom">Room Required In: </label><input type="text" id="bferoom" style="width: 60px" /></span>
            <span id="bfesroom_span" style="display: none;"><label for="bfesroom">Room Required In: </label><select id="bfesroom"></select></span>
            <span id="bferoomerror" style="display: none;">*</span>
		</div>
		<div id="bfLoan" class="bfType">
			<span id="bfloroom_span"><label for="bfloroom">Room to be used In: </label><input type="text" id="bfloroom" style="width: 60px" /></span>
            <span id="bflosroom_span" style="display: none;"><label for="bflosroom">Room Required In: </label><select id="bflosroom"></select></span>
            <span id="bfloroomerror" style="display: none;">*</span>
		</div>
        <div id="bfquant" style="clear: both;">
            <label>Quantity:&nbsp;</label><div id="bfquants" style="display: inline-block;">
				<input type="radio" name="bfquants" id="bfquants-16" value="16" /><label for="bfquants-16">16</label>
				<input type="radio" name="bfquants" id="bfquants-32" value="32" /><label for="bfquants-32">32</label>
			</div><input type="number" id="bfquantspin" style="width: 40px;" value="1" /><span id="bfquantmax"></span>
            <script>$("#bfquantspin").spinner();</script>
        </div>
        <div id="bfmultilesson">
            <label for="bfmultiroom">Length: </label><select id="bfmultiroom"></select>
        </div>
        <div id="bfadminonly">
            <div>
			    <asp:Label runat="server" AssociatedControlID="userlist" Text="User To Book For: " /><asp:DropDownList runat="server" ID="userlist" />
		    </div>
            <div>
                <label for="bflstatic">Make static?: </label><input type="checkbox" id="bflstatic" />
            </div>
            <div>
                <label for="bfrecur">Recurrence: </label><input type="checkbox" id="bfrecur" />
                <div id="recurbox" style="display: none;">
                    <label for="recurweeks">Number of Weeks: </label><input type="number" id="recurweeks" value="0" />
                    <script>
                        $(function () {
                            setTimeout(function () {
                                $("#bfrecur").prev().click(function () {
                                    if ($("#bfrecur").is(":checked")) $("#recurbox").show();
                                    else $("#recurbox").hide();
                                });
                            }, 100);

                        });
                        $("#recurweeks").val("0").spinner({ min: 1, spin: spins, change: spins });
                        function spins(e, u) {
                            $("#recurcheck").html('<span id="crecurcheck">Checking...</span>');
                            recurs = [];
                            for (var i = 1; i <= (u.value || $("#recurweeks").val()) ; i++) {
                                var ndate = new Date(curdate.getTime() + i * 7 * 24 * 60 * 60 * 1000);
                                recurs.push(ndate);
                                $.ajax({
                                    type: 'GET',
                                    url: hap.common.formatJSONUrl('~/api/BookingSystem/LoadRoom/' + ndate.getDate() + '-' + (ndate.getMonth() + 1) + '-' + ndate.getFullYear() + '/' + curres.Name),
                                    dataType: 'json',
                                    context: ndate,
                                    success: function (data) {
                                        $("#crecurcheck").remove();
                                        var h = "";
                                        for (var cr1 = 0; cr1 < data.length; cr1++)
                                            if (data[cr1][0].Lesson == curles) {
                                                $("#recurcheck").append("<div><span>" + this.getDate() + '-' + (this.getMonth() + 1) + '-' + this.getFullYear() + "</span>" + (data[cr1][0].Name == "FREE" ? "Available" : "Not Available") + "</div>");
                                                recurs[recurs.indexOf(this)] = data[cr1][0].Name;
                                            }
                                    },
                                    error: hap.common.jsonError
                                });
                            }
                        }
                    </script>
                    <div id="recurcheck" style="padding-left: 10px;"></div>
                </div>
            </div>
        </div>
        <div id="bfdisclaimer">
            <label for="bfdisclaim"></label><input type="checkbox" class="noswitch" id="bfdisclaim" />
        </div>
        <div id="bfnote"><label for="bfnotes">Notes:</label><br /><textarea id="bfnotes" style="width: 99%;"></textarea></div>
        <div class="buttons"><button id="bookbutton">Book</button><button onclick="$('#sidebaredit').removeClass('show'); return false;">Cancel</button></div>
	</div>
    <script>
        var curdate, curres, curles, user, resources, date, lessontimes;
        var availbookings = 0;
        var recurs = [];
        var canmulti = false;
    </script>
	<script>
		function resource(name, type, years, quantities, readonly, multiroom, maxlessons, rooms, disclaimer, canshare, notes, allowance) {
			this.Name = name;
			this.Type = type;
			this.Years = years;
			this.Quantities = quantities;
			this.Data;
			this.Disclaimer = disclaimer;
			this.ReadOnly = readonly;
			this.MultiRoom = multiroom;
			this.MaxLessons = maxlessons;
			this.Rooms = rooms;
			this.Notes = notes;
			this.CanShare = canshare;
			this.Allowance = allowance;
			this.Render = function() {
			    var h1 = "";
			    for (var x = 0; x < this.Data.length; x++) {
			        var xy = 0;
			        var h = "";
			        for (var y = 0; y < this.Data[x].length; y++)
			        {
			            h += '<a onclick="return ';
			            var ex1 = (this.Allowance != null && lessontimes[x].FromStart != null && lessontimes[x].FromStart <= this.Allowance && $.inArray(this.Name, user.isAdminOf) == -1);
			            var ex2 = (lessontimes[x].FromEnd != null && lessontimes[x].FromEnd <= 0);
			            var expired = ex1 || ex2;
			            if (this.ReadOnly || expired) h += "false";
			            else if (this.Data[x][y].Name == "FREE") h += "doBooking('" +  this.Name + "', '" + this.Data[x][y].Lesson + "');";
			            else {
			                if (this.Data[x][y].Static && $.inArray(this.Name, user.isAdminOf) != -1 && this.Type != "Loan") h += "doBooking('" + this.Name + "', '" + this.Data[x][y].Lesson + "');";
			                else if (this.Data[x][y].Static == false && this.Type != "Loan" && (this.Data[x][y].Username.toLowerCase() == user.username.toLowerCase() || $.inArray(this.Name, user.isAdminOf) != -1)) h += "doRemove('" + this.Name + "', '" + this.Data[x][y].Lesson + "', '" + this.Data[x][y].Name + "', " + y + ");";
			                else if (this.Data[x][y].Static == false && this.Type == "Loan" && (this.Data[x][y].Username.toLowerCase() == user.username.toLowerCase() || $.inArray(this.Name, user.isAdminOf) != -1)) h += "doReturn('" + this.Name + "', '" + this.Data[x][y].Lesson + "', '" + this.Data[x][y].Name + "');";
			                else h += "false;";
			            }
			            h += '" href="#' + this.Name + '-' + this.Data[x][y].Lesson.toLowerCase().replace(/ /g, "") + '" class="' + (this.Data[x][y].Static ? 'static ' : '') + (this.Type == "Loan" ? 'loan ' : '') + ($.inArray(this.Name, user.isAdminOf) == -1 ? '' : 'admin') + ((this.Data[x][y].Username.toLowerCase() == user.username.toLowerCase() && $.inArray(this.Name, user.isAdminOf) == -1) ? ' bookie' : '') + ((this.Data[x][y].Name == "FREE" && !this.ReadOnly) ? ' free' : ' booked') + (expired ? " expired" : "") + '">';
			            h += (this.Data[x][y].Static ? '<span class="state static" title="Timetabled Lesson"><i></i><span>Override</span></span>' : (this.Data[x][y].Name == "FREE" ? '<span class="state book" title="Book"><i></i><span>Book</span></span>' : (this.Type == "Loan" ? '<span class="state Return" title="Return"><i></i><span>Return</span></span>' : '<span class="state remove">' + (this.Data[x][y].Notes != null ? ('<i class="notes" title="Booking Notes:\n' + unescape(this.Data[x][y].Notes) + '"></i>') : '') + '<i title="Remove"></i><span title="Remove">Remove</span></span>')));
			            h += this.Data[x][y].Name + '<span>' + this.Data[x][y].DisplayName;
			            if (this.Data[x][y].Name == "FREE" || this.Data[x][y].Name == "UNAVAILABLE" || this.Data[x][y].Name == "CHARGING") { }
			            else {
			                if (this.Type == "Laptops") h += ' in ' + this.Data[x][y].LTRoom + ' [' + this.Data[x][y].Count + '|' + (this.Data[x][y].LTHeadPhones ? 'HP' : 'N-HP') + ']';
			                else if (this.Type == "Equipment" || this.Type == "Loan") h += ' in ' + this.Data[x][y].EquipRoom;

			                if (this.Data[x][y].Count != 0 && this.Quantities.length > 0 && this.CanShare && !this.Data[x][0].Static) { xy += this.Data[x][y].Count; h += ' [' + this.Data[x][y].Count + '/' + this.Quantities[this.Quantities.length - 1] +']'; }
			            }
			            h += '</span></a>';
			        }
			        if (this.CanShare && this.Data[x][0].Count > 0 && !this.Data[x][0].Static) {
			            h = '<span class="share' + (xy < this.Quantities[this.Quantities.length - 1] ? '' : ' full') + '">' + h;
			            if (!this.ReadOnly && xy < this.Quantities[this.Quantities.length - 1])
			                h += '<a onclick="return doBooking(\'' +  this.Name + "', '" + this.Data[x][0].Lesson + '\');" href="#' + this.Name + '-' + this.Data[x][0].Lesson.toLowerCase().replace(/ /g, "") + '" class="' + ($.inArray(this.Name, user.isAdminOf) == -1 ? '' : 'admin') + ' free"><span class="state book" title="Book"><i></i><span>Book</span></span>' + (this.Quantities[this.Quantities.length - 1] - xy) + ' FREE</a>';
			            h += '</span>';
			        }
			        h1 += h;
			    }
				$("#" + this.Name.replace(/ /g, '_')).html(h1);
			};
			this.timer = null;
			this.Refresh = function() {
				$.ajax({
					type: 'GET',
					url: hap.common.formatJSONUrl('~/api/BookingSystem/LoadRoom/' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + this.Name),
					dataType: 'json',
					context: this,
					success: function (data) {
						this.Data = data;
						this.Render();
						if (this.timer) clearTimeout(this.timer);
						this.timer = null;
						var _z = 0;
						for (var _i = 0; _i < resources.length; _i++) if (resources[_i] == this) _z = _i;
						this.timer = setTimeout(function () { resources[_z].Refresh(); }, 30000);
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
		function loadDate(norefresh) {
			$("#val").html("Loading...");
			$.ajax({
				type: 'GET',
				url: hap.common.formatJSONUrl('~/api/BookingSystem/Initial/' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username.toLowerCase()),
				dataType: 'json',
				success: function (data) {
					if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
					else {
					    if (data[0] >= 0 ) {
					        $("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
					        availbookings = data[0];
					    } else {
					        $("#val").html("This Week is a Week " + data[1]);
					        availbookings = -1;
					    }
						
					}
				},
				error: hap.common.jsonError
			});
			if (!norefresh) for (var i = 0; i < resources.length; i++) {
				$("#" + resources[i].Name).html(" ");
				resources[i].Refresh();
			}
		}
		$(window).hashchange(function () {
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curdate = new Date(window.location.href.split('#')[1].split('/')[2], window.location.href.split('#')[1].split('/')[1] - 1, window.location.href.split('#')[1].split('/')[0]);
			else curdate = date;
			$('#datepicker').datepicker("setDate", curdate);
			$("#picker").val($.datepicker.formatDate('d MM', curdate));
			$("#wv").attr("href", "cal.aspx#" + $.datepicker.formatDate('dd/mm/yy', curdate));
			breakloc = null;
			for (var i = 0; i < lessontimes.length; i++) lessontimes[i].FromStart = lessontimes[i].FromEnd = null;
			$("#time").removeAttr("style");
			changeTime();
			if (breakloc == null) loadDate();
			else loadDate(true);
		});
		function isAdminOf(res) {
		    for (var i = 0; i < user.isAdminOf.length; i++)
		        if (res == user.isAdminOf[i]) return true;
		    return false;
		}
		$("#bookbutton").click(function () {
		    if (curres.Disclaimer != "" && !$("#bfdisclaimer input").is(":checked")) { alert(hap.common.getLocal('bookingsystem/nodisclaimer')); return false; }
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
							
		    if (curres.Type == "Loan" && $("#bfloroom").val().length == 0) { 
		        $("#bfloroomerror").removeAttr("style").css("color", "red");
		        abort = true;
		    } else $("#bfloroomerror").css("display", "none");
							

		    var n1 = "";
		    if ($("#bfyear option:selected").val() != null || $("#bfyear option:selected").val() != "") n1 = $("#bfyear option:selected").val();
		    if (n1 != "") n1 += " ";
		    n1 += $("#bfsubject").val();
		    if (abort) return false;
		    var d = '{ "booking": { "Room": "' + curres.Name + '", "Lesson": "' + (canmulti && $("#bfmultiroom").val() != null ? $("#bfmultiroom").val() : curles) + '", "Username": "' + (isAdminOf(curres.Name) ? $("#<%=userlist.ClientID %> option:selected").val() : user.username) + '", "Name": "' + n1 + '"';
		    if (isAdminOf(curres.Name) && $("#bflstatic").is(":checked")) d +=  ', "Static": true';
		    if (curres.Type == "Laptops") {
		        d += ', "LTRoom": "' + $("#bflroom").val() + '", "LTHeadPhones": ' + (($('#bflheadphones:checked').val() !== undefined) ? 'true' : 'false');
		    }
		    else if (curres.Type == "Equipment") {
		        d += ', "EquipRoom": "' + $("#bferoom").val() + '"';
		    }
		    else if (curres.Type == "Loan") {
		        d += ', "EquipRoom": "' + $("#bfloroom").val()  + '"';
		    }
		    if (curres.Type != "Loan" && curres.Quantities.length > 0 && ($("#bfquants input").is(":checked") || parseInt($("#bfquantspin").val()) < parseInt(curres.Quantities[curres.Quantities.length - 1]))) {
		        d += ', "Count": ' + (curres.Quantities.length == 1 ? $("#bfquantspin").val() : $("#bfquants input:checked").attr("value"));
		    }
		    if (curres.Notes) {
		        d += ', "Notes": "' + escape($("#bfnotes").val()) + '"';
		    }

		    d += " } }";
		    var recurcount = ((isAdminOf(curres.Name) && $("#bfrecur").is(":checked")) ? parseInt($("#recurweeks").val()) : 0) + 1;
		    for (var recuri = 0; recuri < recurcount; recuri++) {
		        var ndate = new Date(curdate.getTime() + recuri * 7 * 24 * 60 * 60 * 1000);
                if (recuri == 0 || recurs[recuri-1] == "FREE")
		            $.ajax({
		                type: 'POST',
		                url: hap.common.formatJSONUrl('~/api/BookingSystem/Booking/' + ndate.getDate() + '-' + (ndate.getMonth() + 1) + '-' + ndate.getFullYear()),
		                dataType: 'json',
		                context: ndate,
		                contentType: 'application/json',
		                data: d,
		                success: function (data) {
		                    if (this.getDate() == curdate.getDate()) {
		                        curres.Data = data;
		                        curres.Render();
		                        $.ajax({
		                            type: 'GET',
		                            url: hap.common.formatJSONUrl("~/api/BookingSystem/Initial/" + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username),
		                            dataType: 'json',
		                            success: function (data) {
		                                if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
		                                else {
		                                    if (data[0] >= 0) {
		                                        $("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
		                                        availbookings = data[0];
		                                    } else {
		                                        $("#val").html("This Week is a Week " + data[1]);
		                                        availbookings = -1;
		                                    }
		                                }
		                            },
		                            error: hap.common.jsonError
		                        });
		                    }
		                },
		                error: hap.common.jsonError
		            });
		    }
		    $("#sidebaredit").removeClass("show");
		    return false;
		});
		function doBooking(res, lesson) {
			if (availbookings == 0 && !user.isBSAdmin) { 
				alert("You have exceeded your allowed bookings, please contact an Admin if this is wrong");
				return false; 
			}
			recurs = [];
			curles = lesson;
			$("#bflstatic").prop("checked", false);
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
			$("#bflroomerror, #bferoomerror, #bfloroomerror").css("display", "none");
			$("#bfsubject").val("");
			$("#bfsubjects option:selected").removeAttr("selected");
			$("#bfsubjects option:first").attr("selected", "selected");
			$("#bflheadphones").removeAttr("checked");
			$("#bflroom, #bferoom, #bfloroom").val("");
			$("#bflsroom option, #bfesroom option, #bflosroom option").remove();
			if (curres.Notes) {
			    $("#bfnote").show();
			    $("#bfnotes").val("");
			} else $("#bfnote").hide();
			if (curres.Rooms.length > 0) {
			    $("#bflroom_span, #bferoom_span, #bfloroom_span").css("display", "none");
			    $("#bflsroom_span, #bfesroom_span, #bflosroom_span").removeAttr("style");
			    $("#bflsroom, #bfesroom, #bflosroom").append('<option value="">---</option>');
			    for (var i = 0; i < curres.Rooms.length; i++)
			        $("#bflsroom, #bfesroom, #bflosroom").append('<option value="' + curres.Rooms[i] + '">' + curres.Rooms[i] + '</option>');
			}
			else {
			    $("#bflroom_span, #bferoom_span, #bfloroom_span").removeAttr("style");
			    $("#bflsroom_span, #bfesroom_span, #bflosroom_span").css("display", "none");
			}
		    try {
		        $("#bfyear option").remove();
                if (curres.Years.length > 1) $("#bfyear").append('<option value="">---</option>');
		        for (var i = 0; i < curres.Years.length; i++)
		            $("#bfyear").append('<option value="' + curres.Years[i] + '">' + curres.Years[i] + '</option>');
		    } catch (e) { }
		    if (isAdminOf(res)) {
		        $("#bfadminonly").show();
		        if ($("#bfrecur").is(":checked")) $("#bfrecur").prev().trigger("click");
		    } else $("#bfadminonly").hide();
		    try {
		        $("#bfquant, #bfquants, #bfquantspin").hide();
		        $("#bfquantmax").html("");
		        if (curres.CanShare || curres.Type == "Laptops") {
		            $("#bfquant").show();
		            if (curres.Quantities.length == 1) {
		                $("#bfquantspin").show();
		                var cr3 = curres.Quantities[0];
		                for (var cr1 = 0; cr1 < curres.Data.length; cr1++)
		                    if (curres.Data[cr1][0].Lesson == curles)
		                        for (var cr2 = 0; cr2 < curres.Data[cr1].length; cr2++)
		                            cr3 -= curres.Data[cr1][cr2].Count;
		                $("#bfquantmax").html("&nbsp;/&nbsp;" + cr3);
		                $("#bfquantspin").val(cr3).spinner( "option", "min", 1).spinner("option", "max", cr3);
		            }
		            else {
		                $("#bfquants").html("").show();
		                for (var i = 0; i < curres.Quantities.length; i++) 
		                    $("#bfquants").append('<input type="radio" name="bfquants" id="bfquants-' + curres.Quantities[i] + '" value="' + curres.Quantities[i] + '" /><label for="bfquants-' + curres.Quantities[i] + '">' + curres.Quantities[i] + '</label>');
		                $("#bfquants").buttonset();
		            }
		        }
		    } catch (e) { }
		    try {
		        if (curres.MultiRoom && curres.Type != "Loan") {
		            $("#bfmultilesson").show();
		            $("#bfmultiroom option").remove();
		            var l1 = false;
		            var l2 = "";
		            var l3 = 0;
		            for (var i = 0; i < curres.Data.length; i++)
		            {
		                if (!l1) l1 = (curres.Data[i][0].Lesson == lesson);
		                if (l1) {
		                    l3++;
		                    if ((isAdminOf(curres.Name) || (parseInt(curres.MaxLessons) > 0 && l3 <= parseInt(curres.MaxLessons) && (availbookings == -1 || l3 <= availbookings && availbookings > 0))) && curres.Data[i][0].Name == "FREE") {
		                        $("#bfmultiroom").append('<option value="' + l2 + curres.Data[i][0].Lesson + '">' + l3 + ' Lesson' + (l3 == 1 ? '' : 's') + '</option>');
		                        l2 += curres.Data[i][0].Lesson + ',';
		                    } else break;
		                }
		            }
		            if (l2 == "") { canmulti = false; $("#bfmultilesson").hide(); }
		            canmulti = true;
		        }
		        else if (curres.Type == "Loan") {
		            canmulti = true;
		            $("#bfmultiroom option").remove();
		            var l1 = false;
		            var l2 = "";
		            var l3 = 0;
		            for (var i = 0; i < curres.Data.length; i++)
		            {
		                if (!l1) l1 = (curres.Data[i][0].Lesson == lesson);
		                if (l1) {
		                    l3++;
		                    if ((isAdminOf(curres.Name) || (parseInt(curres.MaxLessons) > 0 && l3 <= parseInt(curres.MaxLessons) && (availbookings == -1 || l3 <= availbookings && availbookings > 0))) && curres.Data[i][0].Name == "FREE") {
		                        $("#bfmultiroom").append('<option value="' + l2 + curres.Data[i][0].Lesson + '">' + l3 + ' Lesson' + (l3 == 1 ? '' : 's') + '</option>');
		                        l2 += curres.Data[i][0].Lesson + ',';
		                    } else break;
		                }
		            }
		            $("#bfmultiroom").val($("#bfmultiroom option").last().val());
		        }
		        else {
		            $("#bfmultilesson").hide();
		            canmulti = false;
		        }
		    } catch (e) 
		    {
		        $("#bfmultilesson").hide();
		        canmulti = false; 
		    }
			$("#bflesson").html(lesson);
			if (curres.Disclaimer != "") {
			    $("#bfdisclaimer").show();
			    $("#bfdisclaimer label").html(curres.Disclaimer);
			    $("#bfdisclaimer input").prop("checked", false);
			} else { $("#bfdisclaimer").hide(); }
			$("#sidebaredit").addClass("show");
			return false;
		}
	    function doReturn(res, lesson, name) {
	        $("#questionbox span").html("Are you sure you want to return<br/>" + res + " during " + lesson);
	        $("#questionbox").dialog({ autoOpen: true, 
	            buttons: { "Yes": function() {
	                curles = lesson;
	                $("#bfdate").html($.datepicker.formatDate('d MM yy', curdate));
	                for (var i = 0; i < resources.length; i++)
	                    if (resources[i].Name == res) curres = resources[i];
	                $.ajax({
	                    type: 'POST',
	                    url: hap.common.formatJSONUrl("~/api/BookingSystem/Return/" + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear()),
	                    dataType: 'json',
	                    contentType: 'application/json',
	                    data: '{ "Resource": "' + curres.Name + '", "lesson": "' + curles + '" }',
	                    success: function (data) {
	                        curres.Data = data;
	                        curres.Render();
	                        $.ajax({
	                            type: 'GET',
	                            url: hap.common.formatJSONUrl("~/api/BookingSystem/Initial/" + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username),
	                            dataType: 'json',
	                            success: function (data) {
	                                if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
	                                else {
	                                    if (data[0] >= 0 ) {
	                                        $("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
	                                    } else {
	                                        $("#val").html("This Week is a Week " + data[1]);
	                                    }
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
		function doRemove(res, lesson, name, index) {
			$("#questionbox span").html("Are you sure you want to remove<br/>" + name + " in/with " + res + " during " + lesson);
			$("#questionbox").dialog({ autoOpen: true, 
				buttons: { "Yes": function() {
					curles = lesson;
					$("#bfdate").html($.datepicker.formatDate('d MM yy', curdate));
					for (var i = 0; i < resources.length; i++)
						if (resources[i].Name == res) curres = resources[i];
					$.ajax({
						type: 'DELETE',
						url: hap.common.formatJSONUrl("~/api/BookingSystem/Booking/" + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + "/" + index),
						dataType: 'json',
						contentType: 'application/json',
						data: '{ "booking": { "Room": "' + curres.Name + '", "Lesson": "' + curles + '", "Name": "' + name + '" } }',
						success: function (data) {
							curres.Data = data;
							curres.Render();
							$.ajax({
								type: 'GET',
								url: hap.common.formatJSONUrl("~/api/BookingSystem/Initial/" + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + user.username),
								dataType: 'json',
								success: function (data) {
									if (user.isBSAdmin) $("#val").html("This Week is a Week " + data[1]);
									else {
									    if (data[0] >= 0 ) {
										    $("#val").html("You have " + data[0] + " bookings available to use this week. This Week is a Week " + data[1]);
									    } else {
									        $("#val").html("This Week is a Week " + data[1]);
									    }
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
		    $("button").button();
			try {
				if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curdate = new Date(window.location.href.split('#')[1].split('/')[2], window.location.href.split('#')[1].split('/')[1] - 1, window.location.href.split('#')[1].split('/')[0]);
				else curdate = date;
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
			    $("#overviewcalendar").hapPopup();
				return false;
			});
			$("#bflsroom").change(function () {
			    $("#bflroom").val(this.options[this.selectedIndex].value);
			});
			$("#bfesroom").change(function () {
			    $("#bferoom").val(this.options[this.selectedIndex].value);
			});
			$("#questionbox").dialog({ autoOpen: false });
			$("#overviewcalendar").hide();
			$("#bookingform").dialog({ autoOpen: false });
			$("#picker").val($.datepicker.formatDate('d MM', curdate));
			$("input[type=button]").button();
			$(".button").button();
			$("#datepicker").animate({ height: 'toggle' });
			$("#bflquant").buttonset();
			$("#hapContent").click(function() {
				if (showCal == 2) { $("#datepicker").animate({ height: 'toggle' }); showCal = 0; }
				else if (showCal == 1) showCal = 2;
			});
			$("#datepicker").click(function () { return false; });
			changeTime();
			if (breakloc == null) loadDate();
			else loadDate(true);
			setInterval("changeTime();", 6000);
		});
		var breakloc = null;
		function changeTime() {
		    $("#time").height($("#resources").height() - 30);
		    for (var i = 0; i < lessontimes.length; i++)
		    {
		        var time1 = new Date(curdate.toDateString());
		        time1.setHours(parseInt(lessontimes[i].Start.split(":")[0]));
		        time1.setMinutes(parseInt(lessontimes[i].Start.split(":")[1]));

		        var time2 = new Date(curdate.toDateString());
		        time2.setHours(parseInt(lessontimes[i].End.split(":")[0]));
		        time2.setMinutes(parseInt(lessontimes[i].End.split(":")[1]));
		        if (new Date() < time1) {
		            $("#time").css("left", ($("#bookingday > .body > .head h1:eq(" + i + ")").position().left + 2) + "px");
		            breakloc = null;
		            break;
		        }
		        else if (new Date() >= time1 && new Date() <= time2) {
		            var h = $("#bookingday > .body > .head h1:eq(" + i + ")");
		            var tp = (new Date().getTime() - time1.getTime()) / 1000 / 60 / 60 * 100;
		            $("#time").css("left", Math.round(h.position().left + 2 + (h.outerWidth() / 100 * tp)) + "px");
		            for (var y = 0; y < lessontimes.length; y++) {
		                var time3 = new Date(curdate.toDateString());
		                time3.setHours(parseInt(lessontimes[y].Start.split(":")[0]));
		                time3.setMinutes(parseInt(lessontimes[y].Start.split(":")[1]));
		                lessontimes[y].FromStart = Math.round((time3.getTime() - new Date().getTime()) / 1000 / 60);
		                time3 = new Date(curdate.toDateString());
		                time3.setHours(parseInt(lessontimes[y].End.split(":")[0]));
		                time3.setMinutes(parseInt(lessontimes[y].End.split(":")[1]));
		                lessontimes[y].FromEnd = Math.round((time3.getTime() - new Date().getTime()) / 1000 / 60);
		            }
		            if (breakloc != i) for (var z = 0; z < resources.length; z++) {
		                $("#" + resources[z].Name).html(" ");
		                resources[z].Refresh();
		            }
		            breakloc = i;
		            break;
		        }
		    }
		}
	</script>
    <script>		
        user = { <%=JSUser %> };
        resources = <%=JSResources %>;
        lessontimes = <%=JSLessons%>;
        date = new Date(<%try { %><%=CurrentDate.Year %>, <%=CurrentDate.Month - 1 %>, <%=CurrentDate.Day %> <% } catch { } %>);
    </script>
</asp:Content>
