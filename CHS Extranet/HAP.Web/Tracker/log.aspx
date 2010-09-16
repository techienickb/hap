<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="log.aspx.cs" Inherits="HAP.Web.Tracker.log" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="tracker.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="text-align: center;">Historic <a href="./"><img src="logontracker-small.png" style="vertical-align: middle;" alt="Logon Tracker" /></a> <asp:Button runat="server" ID="archive" Text="Archive Logs" /></div>
    <asp:ModalPopupExtender runat="server" TargetControlID="archive" PopupControlID="archivelogs" BackgroundCssClass="modalBackground" OkControlID="ok_btn" />
    <asp:Panel runat="server" ID="archivelogs" style="display: none;" CssClass="modalPopup" Width="300px">
        <h1>Archive Logs</h1>
        <ul style="list-style-type: none;">
            <li><b>Logoff Start Date: </b><asp:TextBox ID="startdate" runat="server" Width="100px" ValidationGroup="archiveg" /><asp:CalendarExtender Format="dd/MM/yyyy" TargetControlID="startdate" runat="server" /><asp:RequiredFieldValidator runat="server" ControlToValidate="startdate" ValidationGroup="archiveg" ErrorMessage="*" /></li>
            <li><b>Logoff End Date: </b><asp:TextBox ID="enddate" runat="server" Width="100px" ValidationGroup="archiveg" /><asp:CalendarExtender Format="dd/MM/yyyy" TargetControlID="enddate" runat="server" /><asp:RequiredFieldValidator runat="server" ControlToValidate="enddate" ValidationGroup="archiveg" ErrorMessage="*" /></li>
        </ul>
        <div class="modalButtons">
            <asp:Button runat="server" Text="Archive Logs" ID="archivelogsb" ValidationGroup="archiveg" onclick="archivelogsb_Click" />
            <asp:Button ID="ok_btn" runat="server" Text="Cancel" />
        </div>
    </asp:Panel>
    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <table border="0" class="trackertable">
                <tr valign="top">
                    <th>
                        <div><label>Computer</label>
                            <div>
                                <asp:DropDownList ID="computerfilter" runat="server" Width="140px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                (<asp:DropDownList ID="ipfilter" runat="server" Width="90px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>)
                                <asp:LinkButton id="computersort" runat="server" oncommand="sort_Command" CommandName="ComputerName">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th>
                        <div><label>Username</label>
                            <div>
                                <asp:DropDownList ID="userfilter" runat="server" Width="80px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="usernamesort" runat="server" oncommand="sort_Command" CommandName="Username">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th>
                        <div><label>Domain</label>
                            <div>
                                <asp:DropDownList ID="domainfilter" runat="server" Width="100px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="domainsort" runat="server" oncommand="sort_Command" CommandName="Domain">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th>
                        <div><label>Logon Server</label>
                            <div>
                                <asp:DropDownList ID="lsfilter" runat="server" Width="90px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="serversort" runat="server" oncommand="sort_Command" CommandName="Server">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th style="width: 160px">
                        <div><label>Logon Date & Time</label>
                            <div>
                                <asp:DropDownList ID="logondt" runat="server" Width="130px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="logonDTsort" runat="server" oncommand="sort_Command" CommandName="LogonDT">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th style="width: 160px">
                        <div><label>Logoff Date & Time</label>
                            <div>
                                <asp:DropDownList ID="logoffdt" runat="server" Width="130px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="logoffDTsort" runat="server" oncommand="sort_Command" CommandName="LogoffDT">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                </tr>
            <asp:Repeater ID="ListView1" runat="server">
                <ItemTemplate>
                    <tr style="">
                        <td><%# Eval("ComputerName") %> (<%# Eval("IP") %>)</td>
                        <td><%# Eval("UserName") %></td>
                        <td><%# Eval("DomainName") %></td>
                        <td><%# Eval("LogonServer") %></td>
                        <td style="width: 160px"><%# ((DateTime)Eval("LogOnDateTime")).ToString("f") %></td>
                        <td style="width: 160px"><%# ((DateTime)Eval("LogOffDateTime")).Year == 1 ? "" : ((DateTime)Eval("LogOffDateTime")).ToString("f") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="loadingPopup" style="display: none;">
        <div class="modalBackground"></div>
        <div id="ph">
            <div class="popupContent" style="width: 220px">
                <h1>Loading</h1>
                <img src="../bookingsystem/loading.gif" alt="" />
            </div>
        </div>
    </div>
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
        function endRequestHandler(sender, args) {
            $get('loadingPopup').style.display = "none";
            var error = args.get_error();
            if (error != undefined) {
                alert(error.message);
                args.set_errorHandled(true);
            }
        }
        function beginRequestHandler(sender, args) {
            $get('loadingPopup').style.display = "block";
        }
    </script>
</asp:Content>
