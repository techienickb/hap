<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DayList.ascx.cs" Inherits="CHS_Extranet.BookingSystem.DayList" %>
<%@ Register Namespace="CHS_Extranet.BookingSystem" Assembly="CHS Extranet" TagPrefix="hap" %>
            <div id="daylist">
                <h1>
                    <span><a href="javascript:changeDate()"><asp:Literal runat="server" ID="DayName" /></a></span>
                    <asp:Repeater runat="server" ID="headrepeater"><ItemTemplate><span><%#Eval("Name")%></span></ItemTemplate></asp:Repeater>
                </h1>
                <asp:Repeater runat="server" ID="dl">
                    <ItemTemplate>
                        <div class="<%#Eval("Name") %>">
                            <span class="room"><%#Eval("Name") %></span>
                            <hap:DayListRow ID="DayListRow1" runat="server" Date='<%#Date %>' Room='<%#Eval("Name") %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Literal runat="server" ID="noday" />
            </div>