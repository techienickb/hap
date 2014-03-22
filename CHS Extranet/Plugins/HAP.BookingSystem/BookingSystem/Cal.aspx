<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Cal.aspx.cs" Inherits="HAP.Web.BookingSystem.Cal" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
    <link href="../style/fullcalendar.css" rel="stylesheet" type="text/css" />
    <link href="../style/fullcalendar.print.css" rel="stylesheet" type="text/css" media="print" />
	<link href="../style/bookingsystem.css" rel="stylesheet" type="text/css" />
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
    <script src="../Scripts/fullcalendar.min.js"></script>
    <style>body { overflow: hidden; }</style>
</asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/BookingSystem/"><hap:LocalResource runat="server" StringPath="bookingsystem/bookingsystem" /></asp:HyperLink></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="viewport"><meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" /></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
    <a href="#" id="btnoptions">Options</a>
	<a id="help" href="#" style="float: right;" onclick="hap.help.Load('bookingsystem/index'); return false;"><hap:LocalResource StringPath="help" runat="server" /></a>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div id="options" class="hapmenu tile-border-color" title="Options">
    </div>
    <div id="calendar">

    </div>
    <script>
        var terms = [ <%=JSTermDates %>];
        var user = { <%=JSUser %> };
        var resources = [ <%=JSRes%>];
        var lessontimes = [ <%=JSLessons%>];
        var res = [];
        $(function () {
            var curdate;
            if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curdate = new Date(window.location.href.split('#')[1].split('/')[2], window.location.href.split('#')[1].split('/')[1] - 1, window.location.href.split('#')[1].split('/')[0]);
            else curdate = new Date();
            for (var r1 = 0; r1 < resources.length; r1++)
                $("#options").append('<div><input type="checkbox" id="res-' + resources[r1].replace(/[ |\-\'\.\,\!\#]/gi + '') + '" value="' + resources[r1] + '" /><label for="res-' + resources[r1].replace(/[ |\-\'\.\,\!\#]/gi + '') + '">' + resources[r1] + '</label></div>');
            $("#options").css("left", $("#btnoptions").position().left).find("input").prop("checked", true)
            $("#options input").change(function () {
                $("#calendar").fullCalendar('refetchEvents');
            })
            $("#btnoptions").click(function () {
                $("#options").animate({ height: 'toggle' });
                return false;
            }).trigger("click");

            var termdates = [];
            for (var i = 0; i < terms.length; i++) {
                termdates.push({
                    title: terms[i].name + " Term",
                    start: terms[i].start,
                    end: terms[i].halfterm.start
                });
                termdates.push({
                    title: terms[i].name + " Term",
                    start: terms[i].halfterm.end,
                    end: terms[i].end
                });
            }

            for (var i = 0; i < resources.length; i++)
                res.push(resources[i]);

            $("#calendar").fullCalendar({
                weekends: false,
                defaultView: 'agendaWeek',
                theme: true,
                events: termdates,
                titleFormat: {
                    month: 'MMMM yyyy',                             // September 2009
                    week: "d [ MMM yyyy] { '&#8212;' d MMMM yyyy}", // Sep 7 - 13 2009
                    day: 'dddd, d MMMM yyyy'                  // Tuesday, Sep 8, 2009
                },
                columnFormat: {
                    month: 'dddd',    // Mon
                    week: 'dddd d/M', // Mon 9/7
                    day: 'dddd d/M'  // Monday 9/7
                },
                header: {
                    left: 'agendaWeek,agendaDay',
                    center: 'title',
                    right: 'today prev,next'
                },
                eventRender: function (event, element) {
                    element.attr("title", event.description);
                    element.tooltip({ items: "[title]", content: function () { return $(this).attr("title").replace(/\n/g, "<br />"); } });
                },
                viewRender: function (view, element) {
                    if (view.end > user.maxDate)
                        $("#calendar").fullCalendar('gotoDate', user.maxDate.getFullYear(), user.maxDate.getMonth(), user.maxDate.getDate());
                    if (view.start < user.minDate)
                        $("#calendar").fullCalendar('gotoDate', user.minDate.getFullYear(), user.minDate.getMonth(), user.minDate.getDate());
                }
            }).fullCalendar('addEventSource', function (start, end, callback) {
                var s = start;
                var dates = [];
                while (s < end) {
                    if (isDateInTerm(s)) dates.push(s);
                    var newDate = new Date(s);
                    s = new Date(newDate.setDate(newDate.getDate() + 1));
                }
                if (dates.length > 0)
                    $.ajax({
                        type: 'GET',
                        url: hap.common.formatJSONUrl('~/api/BookingSystem/Load/' + dates[0].getDate() + '-' + (dates[0].getMonth() + 1) + '-' + dates[0].getFullYear() + '/' + dates[dates.length - 1].getDate() + '-' + (dates[dates.length - 1].getMonth() + 1) + '-' + dates[dates.length - 1].getFullYear()),
                        dataType: 'json',
                        success: function (data) {
                            var events = [];
                            for (var x = 0; x < data.length; x++) {
                                for (var y = 0; y < data[x].length; y++) {
                                    if (check(data[x][y].Room)) {
                                        var d = $.fullCalendar.parseDate(data[x][y].Date);
                                        events.push({
                                            title: data[x][y].Name,
                                            start: data[x][y].Date,
                                            end: data[x][y].Date2,
                                            description: data[x][y].Name + "\n" + data[x][y].DisplayName + " in/with " + data[x][y].Room,
                                            className: (data[x][y].Name == "FREE" ? "free" : "") + (data[x][y].Static ? 'static' : ''),
                                            allDay: false,
                                            url: (data[x][y].Name == "FREE" ? ("./#" + d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getFullYear()) : null)
                                        });
                                    }
                                }
                            }
                            callback(events);
                        },
                        error: hap.common.jsonError
                    });
            }).fullCalendar('gotoDate', curdate.getFullYear(), curdate.getMonth(), curdate.getDate());
            setTimeout('setSize();', 100);
        })
        $(window).resize(function () {
            setTimeout('setSize();', 100);
        })
        function setSize() {
            $("#calendar").fullCalendar("option", "height", parseInt($("#hapContent").css("min-height").replace(/px/gi, "")));
        }
        function check(room) {
            var checkedValues = $("#options input:checked").map(function () {
                return $(this).val();
            }).get();
            return $.inArray(room, checkedValues) != -1;
        }
        function isDateInTerm(date) {
        	for (var i = 0; i < terms.length; i++) {
        		if (terms[i].start <= date && terms[i].end >= date) {
        		    if (terms[i].halfterm.start <= date && terms[i].halfterm.end >= date) return false;
        		    else return true;
        		}
        	}
        	return false;
        }
    </script>
</asp:Content>