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
        <div id="bookingday">
            <div class="head">
                <h1><input type="button" id="picker" onclick="return showDatePicker();" /></h1>
                <h1>Lesson 1</h1>
                <h1>Lesson 2</h1>
                <h1>Lesson 3</h1>
                <h1>Lesson 4</h1>
                <h1>Lesson 5</h1>
            </div>
            <div class="body">
                <div id="resources" class="col">
                    <div>ICT1</div>
                    <div>ICT2</div>
                    <div>E3</div>
                    <div>A6</div>
                    <div>Laptops</div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        var curdate = new Date(<%=DateTime.Now.Year %>, <%=DateTime.Now.Month - 1 %>, <%=DateTime.Now.Day %>);
        function disableAllTheseDays(date) {
            var noWeekend = $.datepicker.noWeekends(date);
            if (noWeekend[0]) {
                if (date <= new Date(<%=DateTime.Now.AddDays(14).Year %>, <%=DateTime.Now.AddDays(14).Month - 1 %>, <%=DateTime.Now.AddDays(14).Day %>)) return [true];
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
        $(function () {
            $("#datepicker").datepicker({ 
                minDate: new Date(<%=DateTime.Now.Year %>, <%=DateTime.Now.Month - 1 %>, <%=DateTime.Now.Day %>),
                maxDate: new Date(<%=DateTime.Now.AddDays(14).Year %>, <%=DateTime.Now.AddDays(14).Month - 1 %>, <%=DateTime.Now.AddDays(14).Day %>),
                beforeShowDay: disableAllTheseDays,
                showOtherMonths: true,
                selectOtherMonths: true,
                defaultDate: new Date(<%=DateTime.Now.Year %>, <%=DateTime.Now.Month - 1 %>, <%=DateTime.Now.Day %>),
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
        });
    </script>

</asp:Content>
