<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="HAP.Web.BookingSystem.admin.Default" %>
<%@ Import Namespace="HAP.Web.BookingSystem" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="../bookingsystem.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="body" runat="server">
    <h1>IT Booking System</h1>
    <asp:HyperLink runat="server" NavigateUrl="~/">Back to Home Access Plus</asp:HyperLink> | <asp:HyperLink runat="server" NavigateUrl="~/bookingsystem/">Back to the Booking System</asp:HyperLink>
    <asp:TabContainer ID="TabContainer1" runat="server" Width="98%">
        <asp:TabPanel ID="TabPanel1" runat="server">
            <HeaderTemplate>
                Term Dates</HeaderTemplate>
            <ContentTemplate>
                <h2>
                    Term Dates</h2>
                <asp:Repeater ID="termdates" runat="server" DataSourceID="termdatesDataSource">
                    <HeaderTemplate>
                        <table style="width: 100%">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td colspan="2">
                                <h3 class="CommonFormSubTitle">
                                    Term
                                    <%#Container.ItemIndex + 1 %>
                                </h3>
                            </td>
                        </tr>
                        <tr>
                            <td class="CommonFormFieldName" style="width: 350px;">
                                <strong><asp:Label AssociatedControlID="termname" runat="server">Term Name: </asp:Label></strong>
                            </td>
                            <td class="CommonFormField">
                                <asp:TextBox runat="server" ID="termname" Text='<%#Eval("Name") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td class="CommonFormFieldName" style="width: 350px;">
                                <strong><asp:Label AssociatedControlID="termstartdate" runat="server">Term Starts: </asp:Label></strong>
                            </td>
                            <td class="CommonFormField">
                                <asp:TextBox runat="server" ID="termstartdate" Text='<%#((DateTime)Eval("StartDate")).ToString("dd/MM/yyyy") %>' Width="100px" />
                                <asp:CalendarExtender Animated="true" TargetControlID="termstartdate" Format="dd/MM/yyyy" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="CommonFormFieldName" style="width: 350px;">
                                <strong><asp:Label AssociatedControlID="termenddate" runat="server">Term Ends: </asp:Label></strong>
                            </td>
                            <td class="CommonFormField">
                                <asp:TextBox runat="server" ID="termenddate" Text='<%#((DateTime)Eval("EndDate")).ToString("dd/MM/yyyy") %>' Width="100px" />
                                <asp:CalendarExtender Animated="true" TargetControlID="termenddate" Format="dd/MM/yyyy" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="CommonFormFieldName" style="width: 350px;">
                                <strong>Term Starts with Week Number:</strong>
                            </td>
                            <td class="CommonFormField">
                                <asp:RadioButton runat="server" ID="week1" Text="Week 1" Checked='<%#((int)Eval("StartWeekNum")) == 1 %>'
                                    GroupName="Week" />
                                <asp:RadioButton runat="server" ID="week2" Text="Week 2" Checked='<%#((int)Eval("StartWeekNum")) == 2 %>'
                                    GroupName="Week" />
                            </td>
                        </tr>
                        <tr>
                            <td class="CommonFormFieldName" style="width: 350px;">
                                <strong><asp:Label AssociatedControlID="halftermstart" runat="server">Talf Term Dates:</asp:Label></strong>
                            </td>
                            <td class="CommonFormField">
                                From:
                                <asp:TextBox runat="server" ID="halftermstart" Text='<%#((DateTime)Eval("HalfTerm.StartDate")).ToString("dd/MM/yyyy") %>' Width="100px" />
                                <asp:CalendarExtender Animated="true" TargetControlID="halftermstart" Format="dd/MM/yyyy" runat="server" />
                                To:
                                <asp:TextBox runat="server" ID="halftermend" Text='<%#((DateTime)Eval("HalfTerm.EndDate")).ToString("dd/MM/yyyy") %>' Width="100px" />
                                <asp:CalendarExtender Animated="true" TargetControlID="halftermend" Format="dd/MM/yyyy" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table></FooterTemplate>
                </asp:Repeater>
                <asp:ObjectDataSource ID="termdatesDataSource" runat="server" SelectMethod="ToArray"
                    TypeName="HAP.Data.BookingSystem.Terms"></asp:ObjectDataSource>
                <p class="PanelSaveButton" style="text-align: right;">
                    <asp:Button ID="SaveButton" runat="server" Text="Save" />
                </p>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="TabPanel2" runat="server">
            <HeaderTemplate>Static Bookings</HeaderTemplate>
            <ContentTemplate>
                <h2>Static Bookings</h2>
                <div style="margin: 2px;">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" RenderMode="Block">
                        <ContentTemplate>
                            <asp:GridView ID="staticbookingsgrid" runat="server" AllowPaging="True" AllowSorting="False"
                                AutoGenerateColumns="False" Width="100%" DataSourceID="StaticBookingsDS" EnableModelValidation="True">
                                <Columns>
                                    <asp:BoundField DataField="Day" HeaderText="Day" SortExpression="Day" ReadOnly="True" />
                                    <asp:BoundField DataField="Lesson" HeaderText="Lesson" SortExpression="Lesson" ReadOnly="True" />
                                    <asp:BoundField DataField="Room" HeaderText="Room" SortExpression="Room" ReadOnly="True" />
                                    <asp:BoundField DataField="Name" HeaderText="Lesson Name" SortExpression="Name" />
                                    <asp:BoundField DataField="Username" HeaderText="Teacher (username)" SortExpression="Username" />
                                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" ShowEditButton="True" />
                                </Columns>
                            </asp:GridView>
                            <asp:ObjectDataSource ID="StaticBookingsDS" runat="server" DataObjectTypeName="HAP.Data.BookingSystem.Booking"
                                DeleteMethod="deleteStaticBooking" InsertMethod="addStaticBooking" SelectMethod="getStaticBookingsArray"
                                TypeName="HAP.Data.BookingSystem.BookingSystem" UpdateMethod="updateStaticBooking" />
                            <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="StaticBookingsDS"
                                DefaultMode="Insert" EnableModelValidation="True">
                                <Fields>
                                    <asp:TemplateField HeaderText="Day" SortExpression="Day">
                                        <InsertItemTemplate>
                                            <asp:DropDownList ID="dayDDL" runat="server" SelectedValue='<%# Bind("Day") %>' DataSourceID="dayds"
                                                DataTextField="Name" DataValueField="Value" />
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="day" Text='<%# Bind("Day") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Lesson" SortExpression="Lesson">
                                        <InsertItemTemplate>
                                            <asp:DropDownList ID="LessonDDL" runat="server" SelectedValue='<%# Bind("Lesson") %>'
                                                DataSourceID="lessonsds" DataTextField="Name" DataValueField="Name" />
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="room" Text='<%# Bind("Lesson") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Room" SortExpression="Room">
                                        <InsertItemTemplate>
                                            <asp:DropDownList ID="ResourceDDL" runat="server" SelectedValue='<%# Bind("Room") %>'
                                                DataSourceID="resourcesds" DataTextField="Name" DataValueField="Name">
                                            </asp:DropDownList>
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="room" Text='<%# Bind("Room") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Name" HeaderText="Lesson Name" SortExpression="Name" />
                                    <asp:TemplateField HeaderText="Username" SortExpression="Username">
                                        <InsertItemTemplate>
                                            <asp:DropDownList ID="UsernameDDL" runat="server" SelectedValue='<%# Bind("Username") %>'
                                                DataSourceID="usersds" DataTextField="Key" DataValueField="Value">
                                            </asp:DropDownList>
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="username" Text='<%# Bind("Username") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CommandField ButtonType="Button" ShowCancelButton="False" ShowInsertButton="True" />
                                </Fields>
                            </asp:DetailsView>
                            <asp:ObjectDataSource ID="resourcesds" runat="server" SelectMethod="getResources"
                                TypeName="HAP.Web.BookingSystem.admin.Default" />
                            <asp:ObjectDataSource ID="lessonsds" runat="server" SelectMethod="getLessons" TypeName="HAP.Web.BookingSystem.admin.Default" />
                            <asp:ObjectDataSource ID="dayds" runat="server" SelectMethod="getDays" TypeName="HAP.Web.BookingSystem.admin.Default" />
                            <asp:ObjectDataSource ID="usersds" runat="server" SelectMethod="getUsers" TypeName="HAP.Web.BookingSystem.admin.Default" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel ID="TabPanel3" runat="server">
            <HeaderTemplate>Advanced Booking Rights</HeaderTemplate>
            <ContentTemplate>
                <h2>Advanced Booking Rights</h2>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="true" RenderMode="Block">
                    <ContentTemplate>
                        <asp:ListView ID="ABR" runat="server" DataSourceID="ABRDS" EnableModelValidation="True"
                            InsertItemPosition="LastItem">
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
                                    <td class="CommonFormFieldName" style="width: 350px;">
                                        <strong>Username: </strong>
                                    </td>
                                    <td class="CommonFormField">
                                        <asp:TextBox ID="UsernameTextBox" runat="server" Text='<%# Bind("Username") %>' ReadOnly="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CommonFormFieldName" style="width: 350px;">
                                        <strong>Weeks in Advanced: </strong>
                                    </td>
                                    <td class="CommonFormField">
                                        <asp:TextBox ID="WeeksaheadTextBox" runat="server" Text='<%# Bind("Weeksahead") %>'
                                            Columns="2" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CommonFormFieldName" style="width: 350px;">
                                        <strong>Bookings per Week: </strong>
                                    </td>
                                    <td class="CommonFormField">
                                        <asp:TextBox ID="NumperweekTextBox" runat="server" Text='<%# Bind("Numperweek") %>'
                                            Columns="2" />
                                    </td>
                                </tr>
                            </EditItemTemplate>
                            <EmptyDataTemplate>
                                <table id="Table1" runat="server" style="width: 100%;">
                                    <tr>
                                        <td>
                                            No data was returned.
                                        </td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <InsertItemTemplate>
                                <tr>
                                    <td colspan="2">
                                        <h3 class="CommonFormSubTitle" style="text-align: right;">
                                            <asp:Button ID="InsertButton" runat="server" CommandName="Insert" Text="Insert" />
                                            <asp:Button ID="CancelButton" runat="server" CommandName="Cancel" Text="Clear" />
                                            <div style="float: left;">
                                                NEW</div>
                                        </h3>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CommonFormFieldName" style="width: 350px;">
                                        <strong>Username: </strong>
                                    </td>
                                    <td class="CommonFormField">
                                        <asp:TextBox ID="UsernameTextBox" runat="server" Text='<%# Bind("Username") %>' />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CommonFormFieldName" style="width: 350px;">
                                        <strong>Weeks in Advanced: </strong>
                                    </td>
                                    <td class="CommonFormField">
                                        <asp:TextBox ID="WeeksaheadTextBox" runat="server" Text='<%# Bind("Weeksahead") %>'
                                            Columns="2" />
                                        (Default: 2)
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CommonFormFieldName" style="width: 350px;">
                                        <strong>Bookings per Week: </strong>
                                    </td>
                                    <td class="CommonFormField">
                                        <asp:TextBox ID="NumperweekTextBox" runat="server" Text='<%# Bind("Numperweek") %>'
                                            Columns="2" />
                                        (Default: 3)
                                    </td>
                                </tr>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td colspan="2">
                                        <h3 class="CommonFormSubTitle" style="text-align: right;">
                                            <asp:Button ID="EditButton" runat="server" CommandName="Edit" Text="Edit" />
                                            <div style="float: left;">
                                                <asp:Label ID="UsernameLabel" runat="server" Text='<%# Eval("Username") %>' /></div>
                                        </h3>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CommonFormFieldName" style="width: 350px;">
                                        <strong>Weeks in Advanced: </strong>
                                    </td>
                                    <td class="CommonFormField">
                                        <asp:Label ID="WeeksaheadLabel" runat="server" Text='<%# Eval("Weeksahead") %>' />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CommonFormFieldName" style="width: 350px;">
                                        <strong>Bookings per Week: </strong>
                                    </td>
                                    <td class="CommonFormField">
                                        <asp:Label ID="NumperweekLabel" runat="server" Text='<%# Eval("Numperweek") %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <LayoutTemplate>
                                <table id="Table2" style="width: 100%" runat="server">
                                    <tr id="itemPlaceholder" runat="server">
                                    </tr>
                                    <tr id="Tr3" runat="server">
                                        <td id="Td2" runat="server" style="">
                                        </td>
                                    </tr>
                                </table>
                            </LayoutTemplate>
                        </asp:ListView>
                        <asp:ObjectDataSource ID="ABRDS" runat="server" DataObjectTypeName="HAP.Data.BookingSystem.AdvancedBookingRight"
                            InsertMethod="addBookingRights" SelectMethod="getBookingRights" TypeName="HAP.Data.BookingSystem.BookingSystem"
                            UpdateMethod="updateBookingRights" DeleteMethod="deleteBookingRights" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </asp:TabPanel>
        <asp:TabPanel runat="server" ID="TabPanel4">
            <HeaderTemplate>Email Templates</HeaderTemplate>
            <ContentTemplate>
                <h2>Email Templates</h2>
                <p>{0} = Username, {1} = Display Name, {2} = Room, {3} = Booking Name, {4} = Date, {5} = Day, {6} = Lesson, {7} = LTRoom or EquipRoom, {8} = LTCount</p>
                <asp:Repeater ID="etemplates" runat="server" onitemcommand="etemplates_ItemCommand" DataSourceID="etemplatesds">
                    <ItemTemplate>
                        <div>
                            Template: <%#Eval("ID") %>
                            <a href="./" onclick="return showEditor(this);">Edit</a>
                            <div style="display: none;">
                                <asp:Label runat="server" AssociatedControlID="esubject" Text="Subject: " /><asp:TextBox runat="server" Text='<%#Eval("Subject") %>' Width="400px" ID="esubject" /><br />
                                <asp:TextBox runat="server" TextMode="MultiLine" Width="99%" Height="100px" ID="eeditor" Text='<%#Eval("Content") %>' />
                                <asp:Button runat="server" Text="Save Template" CommandName="save" CommandArgument='<%#Eval("ID") %>' />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:ObjectDataSource ID="etemplatesds" runat="server" SelectMethod="getTemplates" TypeName="HAP.Web.BookingSystem.admin.Default" />
                <script type="text/javascript">
                    function showEditor(id) {
                        if (id.innerHTML == "Edit") {
                            id.parentNode.getElementsByTagName("div")[0].style.display = "block";
                            id.innerHTML = "Close";
                        }
                        else {

                            id.parentNode.getElementsByTagName("div")[0].style.display = "none";
                            id.innerHTML = "Edit";
                        }
                        return false;
                    }
                </script>
            </ContentTemplate>
        </asp:TabPanel>
    </asp:TabContainer>
    <asp:Literal runat="server" ID="message" />
    <div id="modalBackground" class="modalBackground" style="display: none;">
    </div>
    <div id="loadingPopup" style="display: none;">
        <div class="popupContent" style="width: 220px">
            <h1>
                Loading</h1>
            <img src="../loading.gif" alt="" />
        </div>
    </div>
    <script type="text/javascript">
        function endRequestHandler(sender, args) {
            var error = args.get_error();
            if (error != undefined) {
                alert(error.message);
                args.set_errorHandled(true);
            }
            $get('modalBackground').style.display = $get('loadingPopup').style.display = "none";
        }
        function beginRequestHandler(sender, args) {
            $get('modalBackground').style.display = $get('loadingPopup').style.display = "block";
        }
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
    </script>
</asp:Content>
