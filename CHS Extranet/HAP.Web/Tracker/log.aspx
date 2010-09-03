<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="log.aspx.cs" Inherits="HAP.Web.Tracker.log" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="tracker.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="text-align: center;">Historic <a href="./"><img src="logontracker-small.png" style="vertical-align: middle;" alt="Logon Tracker" /></a></div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <table border="0" class="trackertable">
                <tr valign="top">
                    <th>
                        <div><label>Computer</label>
                            <div>
                                <asp:DropDownList ID="computerfilter" runat="server" Width="140px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                (<asp:DropDownList ID="ipfilter" runat="server" Width="90px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>)
                            </div>
                        </div>
                    </th>
                    <th>
                        <div><label>Username</label>
                            <div>
                                <asp:DropDownList ID="userfilter" runat="server" Width="80px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </th>
                    <th>
                        <div><label>Domain</label>
                            <div>
                                <asp:DropDownList ID="domainfilter" runat="server" Width="100px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </th>
                    <th>
                        <div><label>Logon Server</label>
                            <div>
                                <asp:DropDownList ID="lsfilter" runat="server" Width="90px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </th>
                    <th style="width: 160px">
                        <div><label>Logon Date & Time</label>
                            <div>
                                <asp:DropDownList ID="logondt" runat="server" Width="130px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </th>
                    <th style="width: 160px">
                        <div><label>Logoff Date & Time</label>
                            <div>
                                <asp:DropDownList ID="logoffdt" runat="server" Width="130px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </th>
                </tr>
            <asp:ListView ID="ListView1" runat="server">
                <EmptyDataTemplate>
                    <tr>
                        <td colspan="6">No data was returned.</td>
                    </tr>
                </EmptyDataTemplate>
                <ItemTemplate>
                    <tr style="">
                        <td><%# Eval("ComputerName") %>(<%# Eval("IP") %>)</td>
                        <td><%# Eval("UserName") %></td>
                        <td><%# Eval("DomainName") %></td>
                        <td><%# Eval("LogonServer") %></td>
                        <td style="width: 160px"><%# ((DateTime)Eval("LogOnDateTime")).ToString("f") %></td>
                        <td style="width: 160px"><%# ((DateTime)Eval("LogOffDateTime")).Year == 1 ? "" : ((DateTime)Eval("LogOffDateTime")).ToString("f") %></td>
                    </tr>
                </ItemTemplate>
                <LayoutTemplate>
                    <tr ID="itemPlaceholder" runat="server">
                    </tr>
                </LayoutTemplate>
            </asp:ListView>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
