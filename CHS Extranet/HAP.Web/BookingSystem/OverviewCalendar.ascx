<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OverviewCalendar.ascx.cs" Inherits="HAP.Web.BookingSystem.OverviewCalendar" %>
<%@ Register Assembly="HAP.Web" Namespace="HAP.Web.BookingSystem" TagPrefix="hap" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>
    
    <div ID="OverviewBox" style="display: none;">
        <div class="popupContent" style="width: 600px;">
            <h1>Month Overview</h1>
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
            <div class="modalButtons">
                <input type="button" value="Close" onclick="hideOverview();" />
            </div>
        </div>
    </div>