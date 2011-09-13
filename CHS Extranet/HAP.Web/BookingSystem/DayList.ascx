<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DayList.ascx.cs" Inherits="HAP.Web.BookingSystem.DayList" %>
<%@ Register Namespace="HAP.Web.BookingSystem" Assembly="HAP.Web" TagPrefix="hap" %>
            <div>
                <b>Resource Type: </b>
                <asp:DropDownList ID="resourcetype" AutoPostBack="true" runat="server" onselectedindexchanged="resourcetype_SelectedIndexChanged">
                    <asp:ListItem Selected="True">All</asp:ListItem>
                    <asp:ListItem Value="Room">Rooms</asp:ListItem>
                    <asp:ListItem>Laptops</asp:ListItem>
                    <asp:ListItem>Equipment</asp:ListItem>
                    <asp:ListItem>Other</asp:ListItem>
                </asp:DropDownList>
                <b>Lesson: </b>
                <asp:DropDownList ID="lessonsel" AutoPostBack="true" runat="server" 
                    onselectedindexchanged="lessonsel_SelectedIndexChanged">
                    <asp:ListItem Selected="True">All</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div id="daylist">
                <div runat="server" id="daylistrow">
                    <h1>
                        <span><a href="javascript:changeDate()"><asp:Literal runat="server" ID="DayName" /></a></span>
                        <asp:Repeater runat="server" ID="headrepeater"><ItemTemplate><span><%#Eval("Name")%></span></ItemTemplate></asp:Repeater>
                    </h1>
                    <asp:Repeater runat="server" ID="dl">
                        <ItemTemplate>
                            <div class="<%#Eval("Name") %> <%#Eval("Type") %>">
                                <span class="room"><%#Eval("Name") %></span>
                                <hap:DayListRow ID="DayListRow1" runat="server" Show='<%#lessonsel.SelectedValue %>' Date='<%#Date %>' Room='<%#Eval("Name") %>' />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <asp:Literal runat="server" ID="noday" />
            </div>