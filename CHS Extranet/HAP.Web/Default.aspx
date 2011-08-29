<%@ Page Title="Crickhowell High School - IT - Home Access Plus+" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.Default" %>
<%@ Register TagName="announcement" TagPrefix="hap" Src="~/Controls/Announcement.ascx" %>
<%@ Register TagName="version" TagPrefix="hap" Src="~/Controls/UpdateChecker.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <link href="style/mycomputer.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    
    <div id="maincol">
        <hap:announcement runat="server" />
        <hap:version runat="server" />
        <a href="http://hap.codeplex.com" target="_blank" style="text-align: center; display: block;"><img src="<%=ResolveClientUrl("~/images/haplogo.png") %>" alt="Home Access Plus+ Logo" /></a>

        <div id="hometabs">
            <div id="tabheaders">
                <asp:Repeater runat="server" ID="tabheader_repeater"><ItemTemplate><a href="#<%#Eval("Type") %>_tab"><%#Eval("Name") %></a></ItemTemplate></asp:Repeater>
            </div>
            <div id="tabs">
                <asp:PlaceHolder runat="server" ID="tab_Me" Visible="false">
                    <div id="Me_tab" class="tab">
                        <asp:Image runat="server" ID="userimage" ImageUrl="~/images/imageres18.png" style="margin: 10px 40px 30px 0; float: left" />
                        <div><%=string.IsNullOrEmpty(up.GivenName) ? up.DisplayName : up.GivenName + " " + up.Surname %></div>
                        <div><%=Department %></div>
                        <div><%=up.EmailAddress %></div>
                        <asp:Button runat="server" Text="Update My Details" ID="updatemydetails" />
                        <div>
                            <div id="tab_me_progress"><div style="width: <%=Math.Round(space, 0)%>%;"></div><label><%=space %>%</label></div>
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="tab_Password" Visible="false">
                    <div id="Password_tab" class="tab">
                        <asp:Label runat="server" AssociatedControlID="currentpass">Current Password: </asp:Label>
                        <asp:TextBox runat="server" TextMode="Password" ID="currentpass" ValidationGroup="changepass" />
                        <asp:RequiredFieldValidator runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="currentpass" ValidationGroup="changepass" />
                        <br />
                        <asp:Label runat="server" AssociatedControlID="newpass">New Password: </asp:Label>
                        <asp:TextBox runat="server" TextMode="Password" ID="newpass" ValidationGroup="changepass" />
                        <asp:RequiredFieldValidator runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="newpass" ValidationGroup="changepass" />
                        <br />
                        <asp:Label runat="server" AssociatedControlID="confpass">Confirm Password: </asp:Label>
                        <asp:TextBox runat="server" TextMode="Password" ID="confpass" ValidationGroup="changepass" />
                        <asp:CompareValidator runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="confpass" ControlToCompare="newpass" ValidationGroup="changepass" />
                        <br />
                        <asp:Label runat="server" ForeColor="Red" ID="errormess" />
                        <asp:Button runat="server" Text="Change Password" style="margin-top: 5px;" onclick="ChangePass_Click" ID="savepass" ValidationGroup="changepass" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="tab_Bookings" Visible="false">
                    <div id="Bookings_tab" class="tab">
                        <asp:ListView runat="server" ID="bookingslist">
                            <EmptyDataTemplate>No Bookings Available</EmptyDataTemplate>
                            <ItemTemplate>
                                <tr style="">
                                    <td><%# ((DateTime)Eval("Date")).ToLongDateString() %></td>
                                    <td><%# Eval("Lesson") %></td>
                                    <td><%# Eval("Name") %></td>
                                    <td><%# Eval("Room") %></td>
                                </tr>
                            </ItemTemplate>
                            <LayoutTemplate>
                                <table ID="itemPlaceholderContainer" runat="server" border="0" style="" class="bookingtable">
                                    <tr runat="server">
                                        <th runat="server">Date</th>
                                        <th runat="server">Lesson</th>
                                        <th runat="server">Name</th>
                                        <th runat="server">Room</th>
                                    </tr>
                                    <tr ID="itemPlaceholder" runat="server">
                                    </tr>
                                </table>
                            </LayoutTemplate>
                        </asp:ListView>
                        <div style="margin: 10px 0 0 0;">
                            <a href="bookingsystem/">Go to the Booking System</a>
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="tab_Tickets" Visible="false">
                    <div id="Tickets_tab" class="tab">
                        <asp:ListView runat="server" ID="ticketslist">
                            <EmptyDataTemplate>No Open Tickets Available</EmptyDataTemplate>
                            <ItemTemplate>
                                <tr style="">
                                    <td><img width="32px" src="images/statusicons/<%# Eval("Status")%>.png" alt="<%# Eval("Status")%>" /></td>
                                    <td><img width="32px" src="images/statusicons/priority_<%# Eval("Priority")%>.png" alt="<%# Eval("Priority")%>" /></td>
                                    <td><a href="helpdesk/ticket/<%#Eval("Id") %>"><%# Eval("Subject") %></a></td>
                                </tr>
                            </ItemTemplate>
                            <LayoutTemplate>
                                <table ID="itemPlaceholderContainer" runat="server" border="0" style="" class="bookingtable tickets">
                                    <tr id="Tr1" runat="server">
                                        <th runat="server" style="width: 60px">Status</th>
                                        <th runat="server" style="width: 60px">Priority</th>
                                        <th runat="server">Subject</th>
                                    </tr>
                                    <tr ID="itemPlaceholder" runat="server">
                                    </tr>
                                </table>
                            </LayoutTemplate>
                        </asp:ListView>
                        <div style="margin: 10px 0 0 0;">
                            <a href="helpdesk/">Go to the Help Desk</a>
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
        <script src="Scripts/homepagescripts.js" type="text/javascript"></script>
        <div id="HomeButtons">
            <asp:Repeater ID="homepagelinks" runat="server">
                <ItemTemplate>
                    <div>
                        <h1><%#Eval("Name") %></h1>
                        <div>
                            <asp:Repeater runat="server" DataSource='<%#((HAP.Web.Configuration.homepagelinkgroup)Container.DataItem).Links.FilteredLinks %>'>
                                <ItemTemplate> 
                                    <asp:HyperLink runat="server" ID="mycomputer" NavigateUrl='<%#Eval("LinkLocation")%>' Target='<%#Eval("LinkTarget") %>'>
                                        <img runat="server" src='<%#Eval("Icon")%>' alt="" />
                                        <%#Eval("Name") %>
                                        <i><%#Eval("Description") %></i>
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:ModalPopupExtender runat="server" TargetControlID="updatemydetails" PopupControlID="editmode" BackgroundCssClass="modalBackground" OkControlID="ok_btn" />
        <asp:Panel runat="server" ID="editmode" style="display: none;" CssClass="modalPopup" Width="300px">
            <h1>Edit my Details</h1>
            <ul style="list-style-type: none;">
                <li><b>First Name: </b><asp:TextBox ID="txtfname" runat="server" /></li>
                <li><b>Last Name: </b><asp:TextBox ID="txtlname" runat="server" /></li>
                <li><asp:Literal runat="server" ID="formlabel" /><asp:TextBox ID="txtform" runat="server" Columns="4" /></li>
            </ul>
            <div class="modalButtons">
                <asp:Button runat="server" Text="Save these Details" ID="editmydetails" onclick="editmydetails_Click" />
                <asp:Button ID="ok_btn" runat="server" Text="Close" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
