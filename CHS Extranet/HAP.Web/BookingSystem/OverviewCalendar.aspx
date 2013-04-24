<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OverviewCalendar.aspx.cs" Inherits="HAP.Web.BookingSystem.OverviewCalendar1" %>
<%@ Register Assembly="HAP.Web" Namespace="HAP.Web.BookingSystem" TagPrefix="hap" %>
<!DOCTYPE html>
<html>
    <head runat="server">
        <title>Overview Calendar</title>
        <link href="<%=ResolveClientUrl("~/style/basestyle.css")%>" rel="stylesheet" type="text/css" />
        <link href="<%=ResolveClientUrl("~/style/bookingpopup.css")%>" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <hap:BigBookingCalendar ID="OverviewCal" CssClass="OverviewCal" runat="server" FirstDayOfWeek="Monday" NextPrevFormat="ShortMonth" BackColor="Transparent" BorderColor="#d9d9d9" BorderWidth="0" CellPadding="4" DayNameFormat="Short" Font-Size="10pt" Width="100%" SelectionMode="None">
                    <SelectedDayStyle BackColor="#6d051f" CssClass="Day SelDay" Font-Bold="True" ForeColor="White" />
                    <DayStyle CssClass="Day" />
                    <SelectorStyle CssClass="Selector" />
                    <WeekendDayStyle BackColor="#FFFFCC" />
                    <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <OtherMonthDayStyle ForeColor="Black" />
                    <NextPrevStyle VerticalAlign="Middle" ForeColor="Black" CssClass="PreNextMonth" />
                    <DayHeaderStyle BackColor="White" ForeColor="#646464" CssClass="dayhead" Font-Bold="True" Font-Size="10pt" />
                    <TitleStyle CssClass="calhead" BackColor="Transparent" />
                </hap:BigBookingCalendar>
            </div>
        </form>
    </body>
</html>
