<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="live.aspx.cs" Inherits="HAP.Web.Tracker.live" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="tracker.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="text-align: center; font-size: 140%">Live <a href="./"><img src="logontracker-small.png" alt="Logon Tracker" style="vertical-align: middle;" /></a></div>
    <asp:Timer ID="refreshtimer" runat="server" Interval="5000" ontick="refreshtimer_Tick" />
    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger controlid="refreshtimer" eventname="Tick" />
            <asp:AsyncPostBackTrigger controlid="ListView1" eventname="ItemCommand" />
        </Triggers>
        <ContentTemplate>
            <asp:ListView ID="ListView1" runat="server" onitemcommand="ListView1_ItemCommand">
                <EmptyDataTemplate>
                    <table runat="server" style="">
                        <tr>
                            <td>No data was returned.</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <ItemTemplate>
                    <tr style="">
                        <td><%# Eval("ComputerName") %>(<%# Eval("IP") %>)
                        </td>
                        <td><%# Eval("UserName") %></td>
                        <td><%# Eval("DomainName") %></td>
                        <td><%# Eval("LogonServer") %></td>
                        <td><%# DateTime.Parse(Eval("LogOnDateTime").ToString()).ToString("f")%></td>
                        <td style="width: 60px"><asp:Button OnClientClick="return confirm('Are you sure?');" Font-Size="Smaller" runat="server" Text="Logoff" CommandName="Logoff" CommandArgument='<%# Eval("ComputerName").ToString() + "|" + Eval("DomainName").ToString() %>' /></td>
                    </tr>
                </ItemTemplate>
                <LayoutTemplate>
                    <table ID="itemPlaceholderContainer" runat="server" border="0" style="" class="trackertable">
                        <tr runat="server">
                            <th runat="server"><div><label>Computer</label></div></th>
                            <th runat="server"><div><label>Username</label></div></th>
                            <th runat="server"><div><label>Domain</label></div></th>
                            <th runat="server"><div><label>Logon Server</label></div></th>
                            <th runat="server"><div><label>Logon Date & Time</label></div></th>
                            <th runat="server" style="width: 60px"><div><label>Action</label></div></th>
                        </tr>
                        <tr ID="itemPlaceholder" runat="server">
                        </tr>
                    </table>
                </LayoutTemplate>
            </asp:ListView>
            <div style="overflow: hidden; ">
                <asp:Button runat="server" style="float: right;" id="logalloff" OnClientClick="return confirm('This may take some time...');" onclick="logalloff_Click" Text="Log All Off" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>