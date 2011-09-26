<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OverviewCalendar.aspx.cs" Inherits="HAP.Web.BookingSystem.OverviewCalendar1" %>
<%@ Register Assembly="HAP.Web" Namespace="HAP.Web.BookingSystem" TagPrefix="hap" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Overview Calendar</title>
        <link href="<%=ResolveClientUrl("~/style/basestyle.css")%>" rel="stylesheet" type="text/css" />
        <link href="bookingsystem.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <asp:ToolKitScriptManager runat="server" />
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
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
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </form>
    </body>
</html>
