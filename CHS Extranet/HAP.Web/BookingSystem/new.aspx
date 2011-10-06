<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="new.aspx.cs" Inherits="HAP.Web.BookingSystem._new" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
    <link href="../style/bookingsystem.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="datepicker" style="position: absolute;"></div>
    <div id="bookingsystemcontent">
        <div id="bookingday" class="tile-border-color">
            <div class="head tile-color">
                <h1><input type="button" id="picker" onclick="return showDatePicker();" /></h1>
                <h1>Lesson 1</h1>
                <h1>Lesson 2</h1>
                <h1>Lesson 3</h1>
                <h1>Lesson 4</h1>
                <h1>Lesson 5</h1>
            </div>
            <div class="body">
                <div id="resources" class="col tile-color">
                    <div>ICT1</div>
                    <div>ICT2</div>
                    <div>E3</div>
                    <div>A6</div>
                    <div>Laptops</div>
                </div>
                <div id="ICT1" class="col">
                </div>
                <div id="ICT2" class="col">
                </div>
                <div id="E3" class="col">
                </div>
                <div id="A6" class="col">
                </div>
                <div id="Laptops" class="col">
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        var curdate;
        var user = { username: '<%=ADUser.UserName %>', isAdminOf: [ "ICT1", "ICT2", "E3", "A6", "Laptops" ], minDate: new Date(<%=DateTime.Now.Year %>, <%=DateTime.Now.Month - 1 %>, <%=DateTime.Now.Day %>), maxDate: new Date(<%=DateTime.Now.AddDays(14).Year %>, <%=DateTime.Now.AddDays(14).Month - 1 %>, <%=DateTime.Now.AddDays(14).Day %>) };
        var rooms = [ "ICT1", "ICT2", "E3", "A6", "Laptops" ];
        function disableAllTheseDays(date) {
            var noWeekend = $.datepicker.noWeekends(date);
            if (noWeekend[0]) {
                if (date <= user.maxDate) return [true];
                return [false];
            } else {
                return noWeekend;
            }
        }
        var showCal = 0;
        function showDatePicker() {
            if (showCal == 0) { $("#datepicker").animate({ height: 'toggle' }); showCal = 1; }
            return false;
        }
        function loadDate() {
            for (var i = 0; i < rooms.length; i++) {
                $("#" + rooms[i]).html(" ");
                new Resource(rooms[i]).Refresh();
            }
        }
        function Resource(name) {
            this.Name = name;
            this.Refresh = function() {
                $.ajax({
                    type: 'GET',
                    url: '<%=ResolveUrl("~/api/BookingSystem/LoadRoom/")%>' + curdate.getDate() + '-' + (curdate.getMonth() + 1) + '-' + curdate.getFullYear() + '/' + this.Name,
                    dataType: 'json',
                    context: this,
                    success: function (data) {
                        var h = "";
                        for (var x = 0; x < data.length; x++) {
                            h += '<a onclick="return false;" href="#' + this.Name + '-' + data[x].Lesson.toLowerCase().replace(/ /g, "") + '" class="' + ($.inArray(this.Name, user.isAdminOf) == -1 ? '' : 'admin') + ((data[x].Username == user.Username && $.inArray(this.Name, user.isAdminOf) == -1) ? ' bookie' : '') + ((data[x].Name == "FREE") ? ' free' : '') + '">' + (data[x].Static ? '<span class="state static" title="Timetabled Lesson"><i></i><span>Override</span></span>' : (data[x].Name == "FREE" ? '<span class="state book" title="Book"><i></i><span>Book</span></span>' : '<span class="state remove" title="Remove"><i></i><span>Remove</span></span>')) + data[x].Name + '<span>' + data[x].DisplayName + '</span></a>';
                        }
                        $("#" + this.Name).html(h);
                    },
                    error: OnError
                });
            }
        }
        function OnLoadSuccess(response) {
            if (response != null) {
                var x = '';
                var i = 0;
                for (var i = 0; i < response.length; i++)
                    x += '<tr id="' + $.trim(response[i].Lesson.toLowerCase().replace(/ /g, "")) + '" class="' + $.trim(response[i].Lesson.toLowerCase().replace(/ /g, "")) + '"><td class="lesson">' + $.trim(response[i].Lesson.replace(/Lesson /g, "")) + '</td><td><span>' + response[i].Name + '</span>' + response[i].DisplayName + '</td></tr>';
                $("#ict1").html(x);
            }
        }
        function OnError(xhr, ajaxOptions, thrownError) {
            
        }
        $(window).hashchange(function () {
            if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curdate = new Date(window.location.href.split('#')[1].split('/')[2], window.location.href.split('#')[1].split('/')[1] - 1, window.location.href.split('#')[1].split('/')[0]);
            else curdate = new Date();
            loadDate();
        });
        $(function () {
            try {
                if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curdate = new Date(window.location.href.split('#')[1].split('/')[2], window.location.href.split('#')[1].split('/')[1] - 1, window.location.href.split('#')[1].split('/')[0]);
                else curdate = new Date();
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
                    $("#picker").val($.datepicker.formatDate('d MM yy', curdate));
                    location.href = "#" + $.datepicker.formatDate('dd/mm/yy', curdate);
                }
            });
            $("#picker").val($.datepicker.formatDate('d MM yy', curdate));
            $("input[type=button]").button();
            $("#datepicker").css("top", $("#picker").position().top + 29);
            $("#datepicker").animate({ height: 'toggle' });
            $("#bookingsystemcontent").click(function() {
                if (showCal == 2) { $("#datepicker").animate({ height: 'toggle' }); showCal = 0; }
                else if (showCal == 1) showCal = 2;
            });
            loadDate();
        });
    </script>

</asp:Content>
