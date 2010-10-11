<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="WeekView.aspx.cs" Inherits="HAP.Web.BookingSystem.WeekView" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Namespace="HAP.Web.BookingSystem" Assembly="HAP.Web" TagPrefix="hap" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="bookingsystem.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="weekview">
        <h1>Week View</h1>
        <a href="./">Back to the Booking System</a>
        <asp:Repeater runat="server" ID="rep">
            <ItemTemplate>
                <div class="dayrow">
                    <h2><%#((DateTime)Container.DataItem).ToLongDateString() %></h2>
                    <hap:WeekViewRow runat="server" Date='<%#(DateTime)Container.DataItem %>' Tag="Div" />
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
