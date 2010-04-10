<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CHS_Extranet.BookingSystem.Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/BookingSystem/DayList.ascx" TagName="DayList" TagPrefix="hap" %>
<%@ Register TagPrefix="hap" TagName="BookingPopup" Src="~/BookingSystem/BookingPopup.ascx" %>
<%@ Register Namespace="CHS_Extranet.BookingSystem" Assembly="CHS Extranet" TagPrefix="hap" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
    <link href="bookingsystem.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="body">
            <div style="position: relative;">
                <asp:ToolkitScriptManager runat="server" />
                <asp:UpdatePanel runat="server"><ContentTemplate>
                    <div onclick="hideCal();">
                        <asp:HyperLink runat="server" style="float: right;" NavigateUrl="admin/" ID="adminlink" Visible="false">Booking System Admin</asp:HyperLink>
                        <h1>IT Booking System</h1>
                        <p>Click on a Free period to book a room, or click on a booking to remove it.  Click on <%=Calendar1.SelectedDate.DayOfWeek.ToString() %> to change the day. Week: <asp:Label runat="server" ID="weeknum" /></p>
                        <hap:DayList runat="server" id="daylist" />
                        <hap:BookingPopup runat="server" ID="bookingpopup" />
                        <asp:Button ID="remove" style="display: none;" OnClick="remove_Click" CausesValidation="false" runat="server" Text="Remove" />
                        <asp:HiddenField ID="removevars" runat="server" />
                    </div>
                    <div id="Cal">
                        <hap:BookingCalendar ID="Calendar1" runat="server" FirstDayOfWeek="Monday" CssClass="Calendar"
                            NextPrevFormat="ShortMonth" BackColor="Transparent" OnVisibleMonthChanged="Calendar1_VisibleMonthChanged"
                            BorderColor="#d9d9d9" BorderWidth="0" CellPadding="4" DayNameFormat="Short" 
                            Font-Size="9pt" Width="100%" onselectionchanged="Calendar1_SelectionChanged">
                            <SelectedDayStyle BackColor="#6d051f" CssClass="Day SelDay" Font-Bold="True" ForeColor="White" />
                            <DayStyle CssClass="Day" />
                            <SelectorStyle CssClass="Selector" />
                            <WeekendDayStyle BackColor="#FFFFCC" />
                            <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <OtherMonthDayStyle ForeColor="Black" />
                            <NextPrevStyle VerticalAlign="Middle" ForeColor="Black" CssClass="PreNextMonth" />
                            <DayHeaderStyle BackColor="White" ForeColor="#646464" CssClass="dayhead" Font-Bold="True" Font-Size="8pt" />
                            <TitleStyle CssClass="calhead" BackColor="Transparent" />
                        </hap:BookingCalendar>
                    </div>
                </ContentTemplate></asp:UpdatePanel>
                <div id="loadingPopup" style="display: none;">
                    <div class="popupContent" style="width: 220px">
                        <h1>Loading</h1>
                        <img src="loading.gif" alt="" />
                    </div>
                </div>
            </div>
        <script type="text/javascript">
            var lessonID = "";
            var roomID = "";
            var bookingvarsID = "";
            var ltbookingID = "";
            var inID = "";
            function book(room, roomtype, lesson) {
                $get('modalBackground').style.display = "block";
                $get('modalPopup').style.display = "block";
                $get(lessonID).innerHTML = lesson;
                $get(bookingvarsID).value = room + "@" + lesson;
                $get(roomID).innerHTML = room;
                if (roomtype.match(/Laptops/gi)) {
                    $get(inID).innerHTML = "&nbsp;with the&nbsp;";
                    $get(ltbookingID).style.display = "";
                } else {
                    $get(inID).innerHTML = "&nbsp;in&nbsp;";
                    $get(ltbookingID).style.display = "none";
                }
            }
            function remove(room, lesson) {
                if (confirm("Are you sure you want to remove this booking?")) {
                    $get('<%=removevars.ClientID %>').value = room + "@" + lesson;
                    $get('<%=remove.ClientID %>').click();
                }
            }
            var showcal = false;
            function changeDate() {
                if ($get('Cal').style.display == "") {
                    $get('Cal').style.display = "block";
                    showcal = true;
                }
            }
            function hideCal() {
                if (!showcal) return;
                showcal = false;
                $get('Cal').style.display = "";
            }
            function resetCal(sender, args) {
                if (showcal) $get('Cal').style.display = "block";
                $get('loadingPopup').style.display = "none";
                setIDs();
            }
            function beginRequestHandler(sender, args) {
                $get('modalBackground').style.display = "block";
                $get('loadingPopup').style.display = "block";
            }
            Sys.Application.add_load(resetCal);
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
        </script>
</asp:Content>