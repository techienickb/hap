<%@ Page Language="C#" AutoEventWireup="true"  CodeBehind="Display.aspx.cs" Inherits="HAP.Web.BookingSystem.Display" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="padding: 0; margin: 0; height: 100%;">
<head runat="server">
    <title>Display Board System</title>
    <link href="../style/basestyle.css" rel="stylesheet" type="text/css" />
    <script src="<%=ResolveClientUrl("~/Scripts/jquery-1.6.2.min.js")%>" type="text/javascript"></script>
    <script src="../Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>
    <link href="../style/bookingsystem.css" rel="stylesheet" type="text/css" />
</head>
<body class="bookingdisplay">
    <form id="form1" runat="server">
        <asp:Panel runat="server" ID="ICT1_ICT2" Visible="false">
            <h1 style="overflow: hidden; clear: both;">
                <span style="text-align: center; float: left; width: 49%;">ICT1</span>
                <span style="text-align: center; float: right; width: 49%;">ICT2</span>
            </h1>
            <div style="overflow: hidden; clear: both;">
                <table id="ict1" style="float: left; width: 49%;"><asp:Repeater ID="ICT1" runat="server"><ItemTemplate><tr id="<%#Eval("Lesson").ToString().Replace(" ", "").ToLower().Trim() %>" class="<%#Eval("Lesson").ToString().Replace(" ", "").ToLower().Trim() %>"><td class="lesson"><%#Eval("Lesson").ToString().Replace("Lesson ", "").Trim() %></td><td><span><%#Eval("Name")%></span><%#getName(Container.DataItem) %></td></tr></ItemTemplate></asp:Repeater></table>
                <table id="ict2" style="float: right; width: 49%;"><asp:Repeater ID="ICT2" runat="server"><ItemTemplate><tr class="<%#Eval("Lesson").ToString().Replace(" ", "").ToLower().Trim() %>"><td class="lesson"><%#Eval("Lesson").ToString().Replace("Lesson ", "").Trim() %></td><td><span><%#Eval("Name")%></span><%#getName(Container.DataItem) %></td></tr></ItemTemplate></asp:Repeater></table>
            </div>
            <button style="position: absolute; bottom: 0; right: 0; z-index: 1001" onclick="doRefresh(); return false;">Refresh</button>
            <div id="statebar"></div>
            <script type="text/javascript">
            var timings = [<%=getJSTimings() %>];
            var lesson = null;
            var curdate = new Date();
            function findLesson() {
                for (var i = 0; i < timings.length; i++)
                {
                    var d1 = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), timings[i].StartTime.Hour, timings[i].StartTime.Minute, 0, 0);
                    var d2 = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), timings[i].EndTime.Hour, timings[i].EndTime.Minute, 0, 0);
                    if (curdate >= d1 && curdate < d2) return timings[i];
                }
                return null;
            }
            function doMove() {
                var newlesson = findLesson();
                if (newlesson == null && lesson == null) {
                }
                else if (newlesson == null && lesson != null) {
                    $("." + lesson.ID).animate({ "font-size": "16px" }, 100);
                    if ($("#statebar").position().top == 100)
                        $("#statebar").css("height", "0");
                    else {
                        $("#statebar").animate({ top: $("#statebar").position().top + $("#statebar").height(), height: 0 }, 1000);
                    }
                    lesson = newlesson;
                } else if (newlesson != null && lesson != null && lesson != newlesson) {
                    $("." + lesson.ID).animate({ "font-size": "16px" }, 100, 'linear', function() { continueMove(); });
                    lesson = newlesson;
                } else { lesson = newlesson; continueMove(); }
            }
            function continueMove() {
                if (lesson == null) return;
                $("." + lesson.ID).animate({ "font-size": "30px" }, 1000, 'linear', function() { 
                    if (lesson == null) return;
                    $("#statebar").animate({ height: $("." + lesson.ID).height(), top: $("." + lesson.ID).position().top + 50 }, { queue: false, duration: 1000 })
                });
            }
            function reset() {
                if (lesson != null) $("#statebar").height($("#" + lesson.ID).height());
                else $("#statebar").height(0)
                doMove();
            }
            function doRefresh() {
                curdate = new Date();
                $.ajax({
                    type: 'GET',
                    url: '<%=ResolveUrl("~/api/BookingSystem/LoadRoom/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/ICT1',
                    dataType: 'json',
                    success: OnSuccess,
                    error: OnError
                });
                $.ajax({
                    type: 'GET',
                    url: '<%=ResolveUrl("~/api/BookingSystem/LoadRoom/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/ICT2',
                    dataType: 'json',
                    success: OnSuccess2,
                    error: OnError
                });
            }
            function OnSuccess(response) {
                if (response != null) {
                    var x = '';
                    var i = 0;
                    for (var i = 0; i < response.length; i++)
                        x += '<tr id="' + $.trim(response[i].Lesson.toLowerCase().replace(/ /g, "")) + '" class="' + $.trim(response[i].Lesson.toLowerCase().replace(/ /g, "")) + '"><td class="lesson">' + $.trim(response[i].Lesson.replace(/Lesson /g, "")) + '</td><td><span>' + response[i].Name + '</span>' + response[i].DisplayName + '</td></tr>';
                    $("#ict1").html(x);
                }
            }
            function OnSuccess2(response) {
                if (response != null) {
                    var x = '';
                    var i = 0;
                    for (var i = 0; i < response.length; i++)
                        x += '<tr class="' + $.trim(response[i].Lesson.toLowerCase().replace(/ /g, "")) + '"><td class="lesson">' + $.trim(response[i].Lesson.replace(/Lesson /g, "")) + '</td><td><span>' + response[i].Name + '</span>' + response[i].DisplayName + '</td></tr>';
                    $("#ict2").html(x);
                }
            }
            function BuildTR(o) {
            }
            function OnError(xhr, ajaxOptions, thrownError) {
            
            }
            $(window).resize(function() { reset(); });
            reset();
            setInterval("doMove();", 5000);
            setInterval("doRefresh();", 300000);
            </script>        
        </asp:Panel>
        <asp:Panel runat="server" ID="defaultview" Visible="false">
            <h1 style="text-align: center;"><asp:Literal runat="server" ID="roomlabel" /></h1>
            <div>
                <table><asp:Repeater runat="server"><ItemTemplate><tr id="<%#Eval("Lesson").ToString().Replace(" ", "").ToLower().Trim() %>"><td class="lesson"><%#Eval("Lesson").ToString().Replace("Lesson ", "").Trim() %></td><td><span><%#Eval("Name")%></span><%#getName(Container.DataItem) %></td></tr></ItemTemplate></asp:Repeater></table>
            </div>
            <button style="position: absolute; bottom: 0; right: 0; z-index: 1001" onclick="doRefresh(); return false;">Refresh</button>
            <div id="statebar"></div>
            <script type="text/javascript">
            var timings = [<%=getJSTimings() %>];
            var lesson = null;
            var curdate = new Date();
            function findLesson() {
                for (var i = 0; i < timings.length; i++)
                {
                    var d1 = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), timings[i].StartTime.Hour, timings[i].StartTime.Minute, 0, 0);
                    var d2 = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), timings[i].EndTime.Hour, timings[i].EndTime.Minute, 0, 0);
                    if (curdate >= d1 && curdate < d2) return timings[i];
                }
                return null;
            }
            function doMove() {
                var newlesson = findLesson();
                if (newlesson == null && lesson == null) {
                }
                else if (newlesson == null && lesson != null) {
                    $("#" + lesson.ID).animate({ "font-size": "16px" }, 100);
                    if ($("#statebar").position().top == 100)
                        $("#statebar").css("height", "0");
                    else {
                        $("#statebar").animate({ top: $("#statebar").position().top + $("#statebar").height(), height: 0 }, 1000);
                    }
                    lesson = newlesson;
                } else if (newlesson != null && lesson != null && lesson != newlesson) {
                    $("#" + lesson.ID).animate({ "font-size": "16px" }, 100, 'linear', function() { continueMove(); });
                    lesson = newlesson;
                } else { lesson = newlesson; continueMove(); }
            }
            function continueMove() {
                if (lesson == null) return;
                $("#" + lesson.ID).animate({ "font-size": "30px" }, 1000, 'linear', function() { 
                    if (lesson == null) return;
                    $("#statebar").animate({ height: $("#" + lesson.ID).height(), top: $("#" + lesson.ID).position().top + 50 }, { queue: false, duration: 1000 })
                });
            }
            function reset() {
                if (lesson != null) $("#statebar").height($("#" + lesson.ID).height());
                else $("#statebar").height(0)
                doMove();
            }
            function doRefresh() {
                curdate = new Date();
                $.ajax({
                    type: 'GET',
                    url: '<%=ResolveUrl("~/api/BookingSystem/LoadRoom/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/<%=Room%>',
                    dataType: 'json',
                    success: OnSuccess,
                    error: OnError
                });
            }
            function OnSuccess(response) {
                if (response != null) {
                    var x = '';
                    var i = 0;
                    for (var i = 0; i < response.length; i++)
                        x += '<tr id="' + $.trim(response[i].Lesson.toLowerCase().replace(/ /g, "")) + '"><td class="lesson">' + $.trim(response[i].Lesson.replace(/Lesson /g, "")) + '</td><td><span>' + response[i].Name + '</span>' + response[i].DisplayName + '</td></tr>';
                    $("table").html(x);
                }
            }
            function BuildTR(o) {
            }
            function OnError(xhr, ajaxOptions, thrownError) {
            
            }
            $(window).resize(function() { reset(); });
            reset();
            setInterval("doMove();", 5000);
            setInterval("doRefresh();", 300000);
            </script>        
        </asp:Panel>
    </form>
</body>
</html>
