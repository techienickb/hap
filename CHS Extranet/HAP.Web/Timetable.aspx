<%@ Page Title="Crickhowell High School - Home Access Plus+ - Timetable" Language="C#" MasterPageFile="~/Masterpage.master" AutoEventWireup="true" CodeBehind="Timetable.aspx.cs" Inherits="HAP.Web.Timetable" %>
<asp:Content runat="server" ContentPlaceHolderID="head">

</asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/timetable.aspx"><hap:LocalResource runat="server" StringPath="timetable/my" /></asp:HyperLink></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
	<a href="./"><hap:LocalResource StringPath="homeaccessplus" Seperator=" " StringPath2="home" runat="server" /></a>
    <asp:PlaceHolder runat="server" ID="adminconverter">
        <asp:LinkButton runat="server" ID="convert" onclick="convert_Click">Convert SIMS.net Export</asp:LinkButton>
        <a href="#" id="btnoptions">Admin Options</a>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div id="options" class="hapmenu tile-border-color" title="Options">
        Impersonate Student: <input type="text" id="upn" />
        <button>User</button><button>UPN</button>
    </div>
    <asp:Literal runat="server" ID="message" />
    <div id="calendar">

    </div>
    <script>
        var terms = [ <%=JSTermDates %>];
        var lessontimes = [ <%=JSLessons%>];
        var timetable = [];
        var optionalcalendars = [ <%=JSOptCals%> ];
        //example: [ { Roles: '![Students]', Calendar: 'office@crickhowell-hs.powys.sch.uk', Color: '#dddddd' } ]
        // [Students] = HAP Students Group.
        //example2: [ { Roles: '![Students]', Calendar: 'office@crickhowell-hs.powys.sch.uk', Color: '#dddddd' }, { Roles: 'Domain Admins, Teachers', Calendar: 'netmain@crickhowell-hs.powys.sch.uk', Color: '#dddddd' } ]
        $.ajax({
            type: 'GET',
            url: hap.common.formatJSONUrl('~/api/Timetable/LoadUser'),
            dataType: 'json',
            success: function (data) {
                timetable = data;
                $("#calendar").fullCalendar('addEventSource', function (start, end, callback) {
                    var s = start;
                    var dates = [];
                    while (s < end) {
                        if (timetable.length == 0 || isDateInTerm(s)) dates.push(s);
                        var newDate = new Date(s);
                        s = new Date(newDate.setDate(newDate.getDate() + 1));
                    }
                    if (dates.length > 0 && timetable.length > 0)
                        $.ajax({
                            type: 'GET',
                            url: hap.common.formatJSONUrl('~/api/Timetable/Week/' + dates[0].getDate() + '-' + (dates[0].getMonth() + 1) + '-' + dates[0].getFullYear()),
                            dataType: 'json',
                            success: function (data) {
                                if (data < 0) return;
                                var events = [];
                                events.push({
                                    title: "Week " + data,
                                    start: dates[0],
                                    end: dates[dates.length - 1],
                                    allDay: true,
                                });
                                var add = 0;
                                if (data == 2) add = 5;
                                for (var x = dates[0].getDay() - 1; x < dates.length; x++) {
                                    for (var y = 0; y < timetable[x + add].Lessons.length; y++) {
                                        var d = new Date(dates[x].setHours(parseInt(timetable[x + add].Lessons[y].StartTime.split(/\:/g)[0])));
                                        d = new Date(d.setMinutes(parseInt(timetable[x + add].Lessons[y].StartTime.split(/\:/g)[1])));
                                        var d2 = new Date(dates[x].setHours(parseInt(timetable[x + add].Lessons[y].EndTime.split(/\:/g)[0])));
                                        d2 = new Date(d2.setMinutes(parseInt(timetable[x + add].Lessons[y].EndTime.split(/\:/g)[1])));
                                        events.push({
                                            title: timetable[x + add].Lessons[y].Description + " in " + timetable[x + add].Lessons[y].Room,
                                            teacher: timetable[x + add].Lessons[y].Teacher,
                                            start: d,
                                            end: d2,
                                            allDay: false,
                                        });
                                    }
                                }
                                callback(events);
                            },
                            error: hap.common.jsonError
                        });
                    else {
                        $.ajax({
                            type: 'GET',
                            url: hap.common.formatJSONUrl('~/api/Timetable/Load/' + dates[0].getDate() + '-' + (dates[0].getMonth() + 1) + '-' + dates[0].getFullYear() + '/' + dates[dates.length - 1].getDate() + '-' + (dates[dates.length - 1].getMonth() + 1) + '-' + dates[dates.length - 1].getFullYear()),
                            dataType: 'json',
                            success: function (data) {
                                if (data < 0) return;
                                var events = [];
                                for (var x = 0; x < data.length; x++) {
                                    events.push({
                                        title: data[x].Name,
                                        teacher: data[x].Room,
                                        start: data[x].Start,
                                        end: data[x].AllDay ? null : data[x].End,
                                        allDay: data[x].AllDay,
                                    });
                                }
                                callback(events);
                            },
                            error: hap.common.jsonError
                        });
                    }
                });


            },
            error: hap.common.jsonError
        });
        $(function () {
            if ($("#btnoptions").length > 0) {
                $("#options").css("left", $("#btnoptions").position().left).find("button").button().click(function () {
                    $.ajax({
                        type: 'GET',
                        url: hap.common.formatJSONUrl('~/api/Timetable/Load' + ($(this).text() == "User" ? 'User' : '') + ($("#upn").val().length > 0 ? '/' : '') + $("#upn").val()),
                        dataType: 'json',
                        success: function (data) {
                            timetable = data;
                            $("#calendar").fullCalendar('refetchEvents');
                        },
                        error: hap.common.jsonError
                    });
                    return false;
                });
                $("#btnoptions").click(function () {
                    $("#options").animate({ height: 'toggle' });
                    return false;
                }).trigger("click");

            } else $("#options").remove();

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
                    if (event.teacher) element.find('.fc-event-title').css("line-height", "12px").append("<br />" + event.teacher);
                }
            });
            if (optionalcalendars.length > 0) {
                $("#calendar").fullCalendar('addEventSource', function (start, end, callback) {
                    var s = start;
                    var dates = [];
                    while (s < end) {
                        dates.push(s);
                        var newDate = new Date(s);
                        s = new Date(newDate.setDate(newDate.getDate() + 1));
                    }
                    s = [];
                    //[{ Roles: '![Students]', Calendar: 'office@crickhowell-hs.powys.sch.uk' }]
                    for (var i = 0; i < optionalcalendars.length; i++)
                        s.push('{ "Roles": "' + optionalcalendars[i].Roles + '", "Calendar": "' + optionalcalendars[i].Calendar + '", "Color": "' + optionalcalendars[i].Color + '" }');
                    s = s.join();
                    $.ajax({
                        type: 'POST',
                        url: hap.common.formatJSONUrl('~/api/Timetable/Load/' + dates[0].getDate() + '-' + (dates[0].getMonth() + 1) + '-' + dates[0].getFullYear() + '/' + dates[dates.length - 1].getDate() + '-' + (dates[dates.length - 1].getMonth() + 1) + '-' + dates[dates.length - 1].getFullYear()),
                        data: '{ "Calendar": [' + s + '] }',
                        contentType: 'application/json',
                        dataType: 'json',
                        success: function (data) {
                            if (data < 0) return;
                            var events = [];
                            for (var x = 0; x < data.length; x++) {
                                events.push({
                                    title: data[x].Name,
                                    teacher: data[x].Room,
                                    start: data[x].Start,
                                    end: data[x].AllDay ? null : data[x].End,
                                    allDay: data[x].AllDay,
                                    color: data[x].Color
                                });
                            }
                            callback(events);
                        },
                        error: hap.common.jsonError
                    });
                });
            }

        });
        $(window).resize(function () {
            setTimeout('setSize();', 100);
        }).trigger("resize");
        function setSize() {
            $("#calendar").fullCalendar("option", "height", parseInt($("#hapContent").css("min-height").replace(/px/gi, "")));
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
