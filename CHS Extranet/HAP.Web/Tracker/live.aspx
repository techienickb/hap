<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="live.aspx.cs" Inherits="HAP.Web.Tracker.live" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="tracker.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="text-align: center; font-size: 140%">Live <a href="./"><img src="logontracker-small.png" alt="Logon Tracker" style="vertical-align: middle;" /></a></div>
    <asp:ScriptManager runat="server" />
    <asp:Timer ID="refreshtimer" runat="server" Interval="5000" ontick="refreshtimer_Tick" />
    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger controlid="refreshtimer" eventname="Tick" />
        </Triggers>
        <ContentTemplate>
            <asp:ListView ID="ListView1" runat="server" DataSourceID="xmlsource">
                <EmptyDataTemplate>
                    <table runat="server" style="">
                        <tr>
                            <td>No data was returned.</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <ItemTemplate>
                    <tr style="">
                        <td><%# Eval("computername") %>(<%# Eval("ip") %>)
                        </td>
                        <td><%# Eval("username") %></td>
                        <td><%# Eval("domainname") %></td>
                        <td><%# Eval("logonserver") %></td>
                        <td>'<%# DateTime.Parse(Eval("logondatetime").ToString()).ToString("f") %></td>
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
                        </tr>
                        <tr ID="itemPlaceholder" runat="server">
                        </tr>
                    </table>
                </LayoutTemplate>
            </asp:ListView>
            <asp:XmlDataSource ID="xmlsource" runat="server" DataFile="~/App_Data/tracker.xml" XPath="/Tracker/Event[@logoffdatetime='']" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>