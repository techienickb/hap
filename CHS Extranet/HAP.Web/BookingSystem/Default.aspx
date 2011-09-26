<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.BookingSystem.Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/BookingSystem/DayList.ascx" TagName="DayList" TagPrefix="hap" %>
<%@ Register TagPrefix="hap" TagName="BookingPopup" Src="~/BookingSystem/BookingPopup.ascx" %>
<%@ Register TagPrefix="hap" TagName="Overview" Src="~/BookingSystem/OverviewCalendar.ascx" %>
<%@ Register TagPrefix="hap" TagName="SIMS" Src="~/BookingSystem/SIMS.ascx" %>
<%@ Register Namespace="HAP.Web.BookingSystem" Assembly="HAP.Web" TagPrefix="hap" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
    <link href="bookingsystem.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="body">
            <div>
                <div onclick="hideCal();">
                    <asp:HyperLink runat="server" style="float: right;" NavigateUrl="admin/" ID="adminlink" Visible="false">Booking System Admin</asp:HyperLink>
                    <hap:SIMS runat="server" />
                    <a href="javascript:showOverview()" style="float: right;">Overview</a>
                    <a href="<%=Request.ApplicationPath %>" style="float: right; padding: 0 5px;">Home Access Plus+ Home</a>
                    <h1>IT Booking System</h1>
                    <asp:UpdatePanel ChildrenAsTriggers="true" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="Calendar1" EventName="SelectionChanged" />
                        <asp:AsyncPostBackTrigger ControlID="sub1" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                    <p>Click on a Free period to book a room, or click on a booking to remove it.  Click on <%=Calendar1.SelectedDate.DayOfWeek.ToString() %> to change the day. Week: <asp:Label runat="server" ID="weeknum" /></p>
                    <hap:DayList runat="server" id="daylist" ItemWidth="152" />
                    <hap:Overview runat="server" id="overview" />
                    <hap:BookingPopup runat="server" ID="bookingpopup" />
                    <asp:Button ID="remove" style="display: none;" OnClick="remove_Click" CausesValidation="false" runat="server" Text="Remove" />
                    <asp:HiddenField ID="removevars" runat="server" />
                    </ContentTemplate></asp:UpdatePanel>
                </div>
                <div id="Cal">
                    <asp:UpdatePanel ChildrenAsTriggers="true" runat="server"><ContentTemplate>
                    <hap:BookingCalendar ID="Calendar1" runat="server" FirstDayOfWeek="Monday" CssClass="Calendar"
                        NextPrevFormat="ShortMonth" BackColor="Transparent" OnVisibleMonthChanged="Calendar1_VisibleMonthChanged"
                        BorderColor="#d9d9d9" BorderWidth="0" CellPadding="4" DayNameFormat="Short" OnSelectionChanged="Calendar1_SelectionChanged"
                        Font-Size="9pt" Width="100%">
                        <SelectedDayStyle CssClass="Day SelDay" Font-Bold="True" ForeColor="White" />
                        <DayStyle CssClass="Day" />
                        <SelectorStyle CssClass="Selector" />
                        <WeekendDayStyle BackColor="#FFFFCC" />
                        <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <OtherMonthDayStyle ForeColor="Black" />
                        <NextPrevStyle VerticalAlign="Middle" ForeColor="Black" CssClass="PreNextMonth" />
                        <DayHeaderStyle BackColor="White" ForeColor="#646464" CssClass="dayhead" Font-Bold="True" Font-Size="8pt" />
                        <TitleStyle CssClass="calhead" BackColor="Transparent" />
                    </hap:BookingCalendar>
                    </ContentTemplate></asp:UpdatePanel>
                </div>
                <asp:HiddenField runat="server" id="datestamp" value="" />
                <asp:Button runat="server" id="sub1" OnClick="sub1_Click" style="display: none;" CausesValidation="false" />
                <div id="loadingPopup" style="display: none;">
                    <div class="popupContent" style="width: 220px">
                        <h1>Loading</h1>
                        <img src="loading.gif" alt="" />
                    </div>
                    <div class="modalBackground" style="background: transparent; cursor: wait;"></div>
                </div>
            </div>
            <script type="text/javascript">
                var lessonID = "";
                var roomID = "";
                var bookingvarsID = "";
                var ltbookingID = "";
                var inID = "";
                var equipID = "";
                function book(room, roomtype, lesson) {
                    $get('modalBackground').style.display = "block";
                    $get('modalPopup').style.display = "block";
                    $get(lessonID).innerHTML = lesson;
                    $get(bookingvarsID).value = room + "@" + lesson;
                    $get(roomID).innerHTML = room;
                    if (roomtype.match(/Laptops/gi) || roomtype.match(/Equipment/gi)) {
                        $get(inID).innerHTML = "&nbsp;with the&nbsp;";
                        if (roomtype.match(/Laptops/gi)) $get(ltbookingID).style.display = "";
                        else $get(equipID).style.display = "";
                    } else {
                        $get(inID).innerHTML = "&nbsp;in&nbsp;";
                        $get(ltbookingID).style.display = $get(equipID).style.display = "none";
                    }
                }
                function remove(room, lesson) {
                    if (confirm("Are you sure you want to remove this booking?")) {
                        $get('<%=removevars.ClientID %>').value = room + "@" + lesson;
                        $get('<%=remove.ClientID %>').click();
                    }
                }
                var showcal = false;
                function changeDate(e) {
                    if (e == null) {
                        if ($get('Cal').style.display == "") {
                            $get('Cal').style.display = "block";
                            $get('Cal').style.top = $("#daylist").position().top + "px";
                            $get('Cal').style.left = $("#daylist").position().left + "px";
                            showcal = true;
                        }
                    }
                }
                function hideCal() {
                    if (!showcal) return;
                    showcal = false;
                    $get('Cal').style.display = "";
                }
                function showOverview() {
                    $get('modalBackground').style.display = "block";
                    $get('OverviewBox').style.display = "block";
                }
                function hideOverview() {
                    $get('modalBackground').style.display = "none";
                    $get('OverviewBox').style.display = "none";
                }
                function resetCal(sender, args) {
                    setIDs();
                    if (showcal) changeDate();
                    $get('loadingPopup').style.display = "none";
                    var s = $("#<%=datestamp.ClientID %>").val().replace(/\//g, "-");
                    $("#" + s).parent().addClass("SelDay");
                }
                function endRequestHandler(sender, args) {
                    $get('loadingPopup').style.display = "none";
                    var error = args.get_error();
                    if (error != undefined) {
                        alert(error.message);
                        args.set_errorHandled(true);
                    }
                }
                function beginRequestHandler(sender, args) {
                    $get('modalBackground').style.display = "block";
                    $get('loadingPopup').style.display = "block";
                }
                Sys.Application.add_load(resetCal);
                Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);

                try {
                    if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) {
                        $get('<%=datestamp.ClientID %>').value = window.location.href.split('#')[1];
                        $get('<%=sub1.ClientID%>').click();
                    }
                } catch (ex) { alert(ex); }
                $(window).hashchange(function () {
                    if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) {
                        $get('<%=datestamp.ClientID %>').value = window.location.href.split('#')[1];
                        $get('<%=sub1.ClientID%>').click();
                    }
                });
            </script>
</asp:Content>