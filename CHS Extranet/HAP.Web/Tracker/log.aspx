<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="log.aspx.cs" Inherits="HAP.Web.Tracker.log" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%=Request.ApplicationPath %>/tracker/tracker.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server"> 
    <div style="text-align: center;" id="logheader">Historic <asp:Hyperlink runat="server" ImageUrl="~/tracker/logontracker-small.png" Text="Logon Tracker" NavigateUrl="~/tracker/" /></div>
    <div style="text-align: center; margin-bottom: 10px;">
        <asp:Chart ID="pcchart" runat="server" Width="880px" BorderlineColor="Silver" BorderlineDashStyle="Solid" Palette="Excel">
            <Series>
                <asp:Series Name="pcseries">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="pcchartarea">
                    <AxisY IsLabelAutoFit="False" Title="Number of Logons" TitleFont="Segoe UI, 9pt">
                        <LabelStyle Font="Segoe UI, 9pt" />
                    </AxisY>
                    <AxisX IsLabelAutoFit="False" Title="Day" TitleFont="Segoe UI, 9pt">
                        <MajorGrid Enabled="False" />
                        <MinorTickMark Enabled="True" />
                        <LabelStyle Font="Segoe UI, 9pt" />
                    </AxisX>
                    <AxisY2 IsInterlaced="True">
                    </AxisY2>
                </asp:ChartArea>
            </ChartAreas>
            <Titles>
                <asp:Title Font="Segoe UI, 18pt" Name="Title1">
                </asp:Title>
            </Titles>
            <BorderSkin PageColor="Transparent" />
        </asp:Chart>
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <asp:Button ID="showdata" runat="server" Text="Show Data" OnClick="showdata_Click" />
            <table border="0" class="trackertable"<%=showtable %>>
                <tr valign="top">
                    <th>
                        <div><label>Computer</label>
                            <div>
                                <asp:DropDownList ID="computerfilter" runat="server" Width="140px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br />
                                (<asp:DropDownList ID="ipfilter" runat="server" Width="90px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>)
                                <asp:LinkButton id="computersort" runat="server" oncommand="sort_Command" CommandName="ComputerName">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th style="width: 90px">
                        <div><label>Username</label>
                            <div>
                                <asp:DropDownList ID="userfilter" runat="server" Width="70px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="usernamesort" runat="server" oncommand="sort_Command" CommandName="Username">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th style="width: 110px">
                        <div><label>Domain</label>
                            <div>
                                <asp:DropDownList ID="domainfilter" runat="server" Width="90px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="domainsort" runat="server" oncommand="sort_Command" CommandName="Domain">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th style="width: 130px">
                        <div><label>Logon Server</label>
                            <div>
                                <asp:DropDownList ID="lsfilter" runat="server" Width="110px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="serversort" runat="server" oncommand="sort_Command" CommandName="Server">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th style="width: 160px">
                        <div><label>Logon Date & Time</label>
                            <div>
                                <asp:DropDownList ID="logondt" runat="server" Width="140px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="logonDTsort" runat="server" oncommand="sort_Command" CommandName="LogonDT">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                    <th style="width: 160px">
                        <div><label>Logoff Date & Time</label>
                            <div>
                                <asp:DropDownList ID="logoffdt" runat="server" Width="140px" onselectedindexchanged="computerfilter_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:LinkButton ID="logoffDTsort" runat="server" oncommand="sort_Command" CommandName="LogoffDT">Sort</asp:LinkButton>
                            </div>
                        </div>
                    </th>
                </tr>
            <asp:Repeater ID="ListView1" runat="server">
                <ItemTemplate>
                    <tr style="">
                        <td><a href="<%=Request.ApplicationPath %>/tracker/<%# ((DateTime)Eval("LogOnDateTime")).ToString("yyyy/M") %>/c/<%# Eval("ComputerName") %>/"><%# Eval("ComputerName") %></a> (<%# Eval("IP") %>)</td>
                        <td><%# Eval("UserName") %></td>
                        <td><%# Eval("DomainName") %></td>
                        <td><%# Eval("LogonServer") %></td>
                        <td style="width: 160px"><a href="<%=Request.ApplicationPath %>/tracker/<%# ((DateTime)Eval("LogOnDateTime")).ToString("yyyy/M") %>/d/<%# ((DateTime)Eval("LogOnDateTime")).Day %>/"><%# ((DateTime)Eval("LogOnDateTime")).ToString("dd MMMM yyyy") %></a> <%# ((DateTime)Eval("LogOnDateTime")).ToString("HH:mm") %></td>
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
                <asp:Image runat="server" ImageUrl="~/bookingsystem/loading.gif" AlternateText="" />
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
