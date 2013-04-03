<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true"
	CodeBehind="Default.aspx.cs" Inherits="HAP.Web.BookingSystem.admin.Default" %>
<%@ Import Namespace="HAP.Web.BookingSystem" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="../../style/bookingsystem.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/BookingSystem/"><hap:LocalResource ID="LocalResource1" runat="server" StringPath="bookingsystem/bookingsystem" /></asp:HyperLink></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
	<a href="../"><hap:LocalResource StringPath="bookingsystem/bookingsystem" Seperator=" " StringPath2="home" runat="server" /></a>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">$(function () { $(".selector").datepicker({ dateFormat: "dd/mm/yy" }); });</script>
	<div id="tabs">
		<ul>
			<li><a href="#term-dates">Term Dates</a></li>
			<li><a href="#static-bookings">Static Bookings</a></li>
			<li><a href="#abr">Advanced Booking Rights</a></li>
			<li><a href="#email-templates">Email Templates</a></li>
		</ul>
		<div id="term-dates">
			<asp:ObjectDataSource ID="termdatesDataSource" runat="server" SelectMethod="ToArray" TypeName="HAP.BookingSystem.Terms"></asp:ObjectDataSource>
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
							<asp:TextBox runat="server" CssClass="selector" ID="termstartdate" Text='<%#((DateTime)Eval("StartDate")).ToString("dd/MM/yyyy") %>' Width="100px" />
						</td>
					</tr>
					<tr>
						<td class="CommonFormFieldName" style="width: 350px;">
							<strong><asp:Label AssociatedControlID="termenddate" runat="server">Term Ends: </asp:Label></strong>
						</td>
						<td class="CommonFormField">
							<asp:TextBox runat="server" CssClass="selector" ID="termenddate" Text='<%#((DateTime)Eval("EndDate")).ToString("dd/MM/yyyy") %>' Width="100px" />
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
							<strong><asp:Label AssociatedControlID="halftermstart" runat="server">Half Term Dates:</asp:Label></strong>
						</td>
						<td class="CommonFormField">
							From:
							<asp:TextBox runat="server" ID="halftermstart" CssClass="selector" Text='<%#((DateTime)Eval("HalfTerm.StartDate")).ToString("dd/MM/yyyy") %>' Width="100px" />
							To:
							<asp:TextBox runat="server" ID="halftermend" CssClass="selector" Text='<%#((DateTime)Eval("HalfTerm.EndDate")).ToString("dd/MM/yyyy") %>' Width="100px" />
						</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate>
					</table></FooterTemplate>
			</asp:Repeater>
			<p class="PanelSaveButton" style="text-align: right;">
				<asp:Button ID="SaveButton" runat="server" Text="Save" />
			</p>
		</div>
		<div id="static-bookings">
            <div id="bookingday" class="tile-border-color"><div class="body">
			    <asp:Repeater runat="server" DataSourceID="StaticBookingsDS">
				    <ItemTemplate>
                        <a href="#" class="admin free">
                            <span class="daytitle">Day&nbsp;</span><span class="day"><%# Eval("Day") %></span>
                            <span class="lesson"><%# Eval("Lesson") %></span><span class="daytitle">&nbsp;in</span>
                            <span class="room"><%# Eval("Room") %></span>
						    <span class="name"><%# Eval("Name") %></span>
                            <span class="username"><%# Eval("Username") %></span>
                            <span class="state book"><i></i><span>Edit</span></span>
					    </a>
				    </ItemTemplate>
                </asp:Repeater>
            </div></div>
            <div id="booking-editor" style="position: fixed; top: 20%; left: 40%; background: #fff; z-index: 1058; padding: 20px; border-width: 1px; border-style: solid;" class="tile-border-color">
                <asp:Label runat="server" AssociatedControlID="Edit_dayDDL">Day: </asp:Label><asp:DropDownList ID="Edit_dayDDL" runat="server" DataSourceID="dayds" DataTextField="Name" DataValueField="Value" /><br />
                <asp:Label runat="server" AssociatedControlID="Edit_LessonDDL">Lesson: </asp:Label><asp:DropDownList ID="Edit_LessonDDL" runat="server" DataSourceID="lessonsds" DataTextField="Name" DataValueField="Name" /><br />
                <asp:Label runat="server" AssociatedControlID="Edit_ResourceDDL">Resource: </asp:Label><asp:DropDownList ID="Edit_ResourceDDL" runat="server" DataSourceID="resourcesds" DataTextField="Name" DataValueField="Name" /><br />
                <asp:Label runat="server" AssociatedControlID="Edit_lessonName">Lesson Name: </asp:Label><asp:TextBox runat="server" ID="Edit_lessonName" /><br />
                <asp:Label runat="server" AssociatedControlID="Edit_UsernameDDL">User: </asp:Label><asp:DropDownList ID="Edit_UsernameDDL" runat="server" DataSourceID="usersds" DataTextField="Key" DataValueField="Value" /><br />
                <asp:Button runat="server" ID="Edit_Save" Text="Update" OnClick="Edit_Save_Click" />
                <asp:Button runat="server" ID="Edit_Delete" Text="Delete" OnClick="Edit_Delete_Click" />
                <button onclick="$('#booking-editor').css('display', 'none'); return false;">Cancel</button>
            </div>
            <asp:HiddenField runat="server" ID="Edit_id" />
            <script type="text/javascript">
                $("#bookingday a").click(function () {
                    var a = $(this);
                    $("#<%=Edit_id.ClientID%>").val($(this).children(".day").text() + ":" + $(this).children(".lesson").text() + ":" + $(this).children(".room").text());
                    $("#<%=Edit_dayDDL.ClientID%>").val($(this).children(".day").text());
                    $("#<%=Edit_LessonDDL.ClientID%>").val($(this).children(".lesson").text());
                    $("#<%=Edit_ResourceDDL.ClientID%>").val($(this).children(".room").text());
                    $("#<%=Edit_lessonName.ClientID%>").val($(this).children(".name").text());
                    $("#<%=Edit_UsernameDDL.ClientID%>").val($(this).children(".username").text());
                    $("#booking-editor").css("display", "block");
                    return false;
                });
                $(function () {
                    $("#booking-editor").css("display", "none");
                });
            </script>
			<div style="float: right; padding: 10px; width: 50%">
				<p>Save the SIMS export to <%=Server.MapPath("~/app_data/sims-bookings.xml") %> then click the button:</p>
				<asp:Button runat="server" ID="importSIMS" Text="Import SIMS" CausesValidation="false" style="font-size: 130%" onclick=CAUTION: The Teacher's Name from the SIMS Export (Title Initial Surname) needs to equal the Notes/Display Name field in AD for that user, or the Notes/Display name only has one person with that surname</p>
			</div>
			<asp:ObjectDataSource ID="StaticBookingsDS" runat="server" DataObjectTypeName="HAP.BookingSystem.Booking"
				DeleteMethod="deleteStaticBooking" InsertMethod="addStaticBooking" SelectMethod="getStaticBookingsArray"
				TypeName="HAP.BookingSystem.BookingSystem" UpdateMethod="updateStaticBooking" />
			<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="StaticBookingsDS"
				DefaultMode="Insert" EnableModelValidation="True" GridLines="None" CssClass="tile-border-color" style="border-width: 1px; border-style: solid; border-top: 0;" OnItemInserted="DetailsView1_ItemInserted">
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
		</div>
		<div id="abr">
		    <asp:ListView ID="ABR" runat="server" DataSourceID="ABRDS" EnableModelValidation="True"
			    InsertItemPosition="LastItem" OnItemInserting="ABR_ItemInserting">
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
		    <asp:ObjectDataSource ID="ABRDS" runat="server" DataObjectTypeName="HAP.BookingSystem.AdvancedBookingRight"
			    InsertMethod="addBookingRights" SelectMethod="getBookingRights" TypeName="HAP.BookingSystem.BookingSystem"
			    UpdateMethod="updateBookingRights" DeleteMethod="deleteBookingRights" />>
		</div>
		<div id="email-templates">
			<p>{0} = Username, {1} = Display Name, {2} = Room, {3} = Booking Name, {4} = Date, {5} = Day, {6} = Lesson, {7} = LTRoom or EquipRoom, {8} = LTCountom or EquipRoom, {8} = LTCount</p>
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
		</div>
	</div>
	<script type="text/javascript">
		$(function () { $("#tabs").tabs(); $("input[type=submit]").button(); $(".button").button(); $("input[type=button]").button(); });
	</script>
	<asp:Literal runat="server" ID="message" />
</asp:Content>
