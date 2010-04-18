<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.BookingSystem.admin.Default" %>
<%@ Import Namespace="HAP.Web.BookingSystem" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="body" runat="server">
   <h1>IT Booking System</h1>
   <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />
    <asp:HyperLink runat="server" NavigateUrl="~/">Back to Home Access Plus</asp:HyperLink> | 
    <asp:HyperLink runat="server" NavigateUrl="~/bookingsystem/">Back to the Booking System</asp:HyperLink>
    <asp:TabContainer ID="TabContainer1" runat="server" Width="98%">
        <asp:TabPanel ID="TabPanel1" runat="server">
            <HeaderTemplate>Term Dates</HeaderTemplate>
            <ContentTemplate>
    	        <h2>Term Dates</h2>
                <asp:Repeater ID="termdates" runat="server" DataSourceID="termdatesDataSource">
                    <HeaderTemplate><table width="100%"></HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td colspan="2">
                                <h3 class="CommonFormSubTitle">
                                    Term <%#Container.ItemIndex + 1 %>
                                </h3>
                            </td>
                        </tr>
        				<tr>
        					<td class="CommonFormFieldName" style="width:350px;">
        					    <strong><asp:Label ID="Label1" AssociatedControlID="termname" runat="server">Term Name: </asp:Label>
        					</td>
        					<td class="CommonFormField">
                                <asp:TextBox runat="server" ID="termname" Text='<%#Eval("Name") %>' />
        					</td>
        				</tr>
        				<tr>
        					<td class="CommonFormFieldName" style="width:350px;">
        					    <strong><asp:Label ID="Label2" AssociatedControlID="termstartdate" runat="server">Term Starts: </asp:Label>
        					</td>
        					<td class="CommonFormField">
        						<asp:TextBox runat="server" ID="termstartdate" Text='<%#((DateTime)Eval("StartDate")).ToString("dd/MM/yyyy") %>' />
        						<asp:CalendarExtender ID="CalendarExtender1" Animated="true" TargetControlID="termstartdate" Format="dd/MM/yyyy" runat="server" />
        					</td>
        				</tr>
        				<tr>
        					<td class="CommonFormFieldName" style="width:350px;">
        					    <strong><asp:Label ID="Label3"  AssociatedControlID="termenddate" runat="server">Term Ends: </asp:Label>
        					</td>
        					<td class="CommonFormField">
        						<asp:TextBox runat="server" ID="termenddate" Text='<%#((DateTime)Eval("EndDate")).ToString("dd/MM/yyyy") %>' />
        						<asp:CalendarExtender ID="CalendarExtender2" Animated="true" TargetControlID="termenddate" Format="dd/MM/yyyy" runat="server" />
        					</td>
        				</tr>
        				<tr>
        					<td class="CommonFormFieldName" style="width:350px;">
        					    <strong>Term Starts with Week Number: 
        					</td>
        					<td class="CommonFormField">
        						<asp:RadioButton runat="server" ID="week1" Text="Week 1" Checked='<%#((int)Eval("StartWeekNum")) == 1 %>' GroupName="Week" />
                                <asp:RadioButton runat="server" ID="week2" Text="Week 2" Checked='<%#((int)Eval("StartWeekNum")) == 2 %>' GroupName="Week" />
        					</td>
        				</tr>
        				<tr>
        					<td class="CommonFormFieldName" style="width:350px;">
        					    <strong>Half Term Dates: </asp:Label>
        					</td>
        					<td class="CommonFormField">
        				        From: 
        				        <asp:TextBox runat="server" ID="halftermstart" Text='<%#((DateTime)Eval("HalfTerm.StartDate")).ToString("dd/MM/yyyy") %>' />
        						<asp:CalendarExtender ID="CalendarExtender3" Animated="true" TargetControlID="halftermstart" Format="dd/MM/yyyy" runat="server" />
        						To: 
        						<asp:TextBox runat="server" ID="halftermend" Text='<%#((DateTime)Eval("HalfTerm.EndDate")).ToString("dd/MM/yyyy") %>' />
        						<asp:CalendarExtender ID="CalendarExtender4" Animated="true" TargetControlID="halftermend" Format="dd/MM/yyyy" runat="server" />
        					</td>
        				</tr>
                    </ItemTemplate>
                    <FooterTemplate></table></FooterTemplate>
                </asp:Repeater>
                <asp:ObjectDataSource ID="termdatesDataSource" runat="server" SelectMethod="ToArray" TypeName="HAP.Web.BookingSystem.Terms">
                </asp:ObjectDataSource>
                <p class="PanelSaveButton" style="text-align:right;">
                    <asp:Button id="SaveButton" runat="server" Text="Save" />
                </p>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="TabPanel2" runat="server">
            <HeaderTemplate>Static Bookings</HeaderTemplate>
            <ContentTemplate><h2>Static Bookings</h2>
                <div style="margin: 2px;">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" RenderMode="Block">
                        <ContentTemplate>
                            <asp:GridView ID="staticbookingsgrid" runat="server" AllowPaging="True" AllowSorting="False" AutoGenerateColumns="False" Width="100%" DataSourceID="StaticBookingsDS" EnableModelValidation="True">
                                <Columns>
                                    <asp:BoundField DataField="Day" HeaderText="Day" SortExpression="Day" ReadOnly="True" />
                                    <asp:BoundField DataField="Lesson" HeaderText="Lesson" SortExpression="Lesson" ReadOnly="True" />
                                    <asp:BoundField DataField="Room" HeaderText="Room" SortExpression="Room" ReadOnly="True" />
                                    <asp:BoundField DataField="Name" HeaderText="Lesson Name" SortExpression="Name" />
                                    <asp:BoundField DataField="Username" HeaderText="Teacher (username)" SortExpression="Username" />
                                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" ShowEditButton="True" />
                                </Columns>
                            </asp:GridView>
                            <asp:ObjectDataSource ID="StaticBookingsDS" runat="server" DataObjectTypeName="HAP.Web.BookingSystem.Booking" DeleteMethod="deleteStaticBooking" InsertMethod="addStaticBooking" SelectMethod="getStaticBookingsArray" TypeName="HAP.Web.BookingSystem.BookingSystem" UpdateMethod="updateStaticBooking" />
                            <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="StaticBookingsDS" DefaultMode="Insert" EnableModelValidation="True">
                                <Fields>
                                    <asp:BoundField DataField="Day" HeaderText="Day" SortExpression="Day" ControlStyle-Width="20px" />
                                    <asp:TemplateField HeaderText="Lesson" SortExpression="Lesson">
                                        <InsertItemTemplate>
                                            <asp:DropDownList ID="LessonDDL" runat="server" SelectedValue='<%# Bind("Lesson") %>' DataSourceID="lessonsds" DataTextField="Name" DataValueField="Name" />
                                        </InsertItemTemplate>
                                        <ItemTemplate><asp:Label runat="server" ID="room" Text='<%# Bind("Lesson") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Room" SortExpression="Room">
                                        <InsertItemTemplate>
                                            <asp:DropDownList ID="ResourceDDL" runat="server" SelectedValue='<%# Bind("Room") %>' DataSourceID="resourcesds"  DataTextField="Name" DataValueField="Name">
                                            </asp:DropDownList>
                                        </InsertItemTemplate>
                                        <ItemTemplate><asp:Label runat="server" ID="room" Text='<%# Bind("Room") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Name" HeaderText="Lesson Name" SortExpression="Name" />
                                    <asp:BoundField DataField="Username" HeaderText="Username" SortExpression="Username" />
                                    <asp:CommandField ButtonType="Button" ShowCancelButton="False" ShowInsertButton="True" />
                                </Fields>
                            </asp:DetailsView>
                            <asp:ObjectDataSource ID="resourcesds" runat="server" SelectMethod="getResources" TypeName="HAP.Web.BookingSystem.admin.Default" />
                            <asp:ObjectDataSource ID="lessonsds" runat="server" SelectMethod="getLessons" TypeName="HAP.Web.BookingSystem.admin.Default" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanelAnimationExtender ID="UpdatePanelAnimationExtender1" runat="server" TargetControlID="UpdatePanel1">
                        <Animations>
                            <OnUpdated><FadeIn minimumOpacity=".2" /></OnUpdated>
                            <OnUpdating><FadeOut minimumOpacity=".2" /></OnUpdating>
                        </Animations>
                    </asp:UpdatePanelAnimationExtender>
                </div></ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="TabPanel3" runat="server">
            <HeaderTemplate>Advanced Booking Rights</HeaderTemplate>
            <ContentTemplate>
    	        <h2>Advanced Booking Rights</h2>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="true" RenderMode="Block">
                    <ContentTemplate>
                        <asp:ListView ID="ABR" runat="server" DataSourceID="ABRDS" EnableModelValidation="True" InsertItemPosition="LastItem">
                            <EditItemTemplate>
                                <tr>
                                    <td colspan="2">
                                        <div class="CommonFormSubTitle" style="text-align: right;">
                                            <asp:Button ID="UpdateButton" runat="server" CommandName="Update" Text="Update" />
                                            <asp:Button ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete" />
                                            <asp:Button ID="CancelButton" runat="server" CommandName="Cancel" Text="Cancel" />
                                            <div style="float: left;">EDIT MODE</div>
                                        </div>
                                    </td>
                                </tr>
                				<tr>
                					<td class="CommonFormFieldName" style="width:350px;">
                					    <strong>Username: </strong>
                					</td>
                					<td class="CommonFormField">
                                        <asp:TextBox ID="UsernameTextBox" runat="server" Text='<%# Bind("Username") %>' ReadOnly="true" />
                					</td>
                				</tr>
                				<tr>
                					<td class="CommonFormFieldName" style="width:350px;">
                					    <strong>Weeks in Advanced: </strong>
                					</td>
                					<td class="CommonFormField">
                                        <asp:TextBox ID="WeeksaheadTextBox" runat="server" 
                                            Text='<%# Bind("Weeksahead") %>' Columns="2" />
                					</td>
                				</tr>
                				<tr>
                					<td class="CommonFormFieldName" style="width:350px;">
                					    <strong>Bookings per Week: </strong>
                					</td>
                					<td class="CommonFormField">
                                        <asp:TextBox ID="NumperweekTextBox" runat="server" 
                                            Text='<%# Bind("Numperweek") %>' Columns="2" />
                					</td>
                				</tr>                      
                            </EditItemTemplate>
                            <EmptyDataTemplate>
                                <table id="Table1" runat="server" style="width: 100%;">
                                    <tr>
                                        <td>
                                            No data was returned.</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <InsertItemTemplate>
                                <tr>
                                    <td colspan="2">
                                        <h3 class="CommonFormSubTitle" style="text-align: right;">
                                            <asp:Button ID="InsertButton" runat="server" CommandName="Insert" Text="Insert" />
                                            <asp:Button ID="CancelButton" runat="server" CommandName="Cancel" Text="Clear" />
                                            <div style="float: left;">NEW</div>
                                        </h3>
                                    </td>
                                </tr>
                				<tr>
                					<td class="CommonFormFieldName" style="width:350px;">
                					    <strong>Username: </strong>
                					</td>
                					<td class="CommonFormField">
                                        <asp:TextBox ID="UsernameTextBox" runat="server" Text='<%# Bind("Username") %>' />
                					</td>
                				</tr>
                				<tr>
                					<td class="CommonFormFieldName" style="width:350px;">
                					    <strong>Weeks in Advanced: </strong>
                					</td>
                					<td class="CommonFormField">
                                        <asp:TextBox ID="WeeksaheadTextBox" runat="server" 
                                            Text='<%# Bind("Weeksahead") %>' Columns="2" />
                                            (Default: 2)
                					</td>
                				</tr>
                				<tr>
                					<td class="CommonFormFieldName" style="width:350px;">
                					    <strong>Bookings per Week: </strong>
                					</td>
                					<td class="CommonFormField">
                                        <asp:TextBox ID="NumperweekTextBox" runat="server" 
                                            Text='<%# Bind("Numperweek") %>' Columns="2" />
                                            (Default: 3)
                					</td>
                				</tr>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td colspan="2">
                                        <h3 class="CommonFormSubTitle" style="text-align: right;">
                                            <asp:Button ID="EditButton" runat="server" CommandName="Edit" Text="Edit" />
                                            <div style="float: left;"><asp:Label ID="UsernameLabel" runat="server" Text='<%# Eval("Username") %>' /></div>
                                        </h3>
                                    </td>
                                </tr>
                				<tr>
                					<td class="CommonFormFieldName" style="width:350px;">
                					    <strong>Weeks in Advanced: </strong>
                					</td>
                					<td class="CommonFormField">
                                        <asp:Label ID="WeeksaheadLabel" runat="server" 
                                            Text='<%# Eval("Weeksahead") %>' />
                					</td>
                				</tr>
                				<tr>
                					<td class="CommonFormFieldName" style="width:350px;">
                					    <strong>Bookings per Week: </strong>
                					</td>
                					<td class="CommonFormField">
                                        <asp:Label ID="NumperweekLabel" runat="server" Text='<%# Eval("Numperweek") %>' />
                					</td>
                				</tr>
                            </ItemTemplate>
                            <LayoutTemplate>
                                <table id="Table2" style="width: 100%" runat="server">
                                    <tr ID="itemPlaceholder" runat="server">
                                    </tr>
                                    <tr id="Tr3" runat="server">
                                        <td id="Td2" runat="server" style="">
                                        </td>
                                    </tr>
                                </table>
                            </LayoutTemplate>
                        </asp:ListView>
                        <asp:ObjectDataSource ID="ABRDS" runat="server" 
                            DataObjectTypeName="HAP.Web.BookingSystem.AdvancedBookingRight" InsertMethod="addBookingRights" 
                            SelectMethod="getBookingRights" TypeName="HAP.Web.BookingSystem.BookingSystem" 
                            UpdateMethod="updateBookingRights" DeleteMethod="deleteBookingRights" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanelAnimationExtender ID="UpdatePanelAnimationExtender2" runat="server" TargetControlID="UpdatePanel2">
                    <Animations>
                        <OnUpdated><FadeIn minimumOpacity=".2" /></OnUpdated>
                        <OnUpdating><FadeOut minimumOpacity=".2" /></OnUpdating>
                    </Animations>
                </asp:UpdatePanelAnimationExtender>
            </ContentTemplate>
        </asp:TabPanel>
    </asp:TabContainer>
    <asp:Literal runat="server" ID="message" />
</asp:Content>