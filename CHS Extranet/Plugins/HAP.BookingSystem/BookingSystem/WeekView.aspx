<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="WeekView.aspx.cs" Inherits="HAP.Web.BookingSystem.WeekView" %>
<%@ Register Namespace="HAP.Web.BookingSystem" Assembly="HAP.Web.BookingSystem" TagPrefix="hap" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
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
