<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CHS_Extranet.HelpDesk.Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Extranet/HelpDesk/helpdesksheet.css" rel="stylesheet" type="text/css" />
    <%if (!string.IsNullOrEmpty(Request.QueryString["view"])) { %>
    <style type="text/css">
        #ticketlist #tickets a#ticket-<%=Request.QueryString["view"]%> { border: solid 1px #7da2ce; }
    </style>
    <%} %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <asp:AjaxScriptManager runat="server" />
    <div id="maincol">
        <div id="ticketlist">
            <h1>Tickets <asp:DropDownList ID="statusselection" AutoPostBack="true" runat="server">
                <asp:ListItem Selected="True">Open</asp:ListItem>
                <asp:ListItem>Closed</asp:ListItem>
                </asp:DropDownList>
            </h1>
            <div id="tickets">
                <a href="/Extranet/HelpDesk/Default.aspx?view=-1" id="ticket--1">
                    <img src="/Extranet/Images/StatusIcons/newticket.png" alt="" />
                    New Support Ticket
                    <i>Open a New Support Ticket</i>
                </a>
                <a href="/Extranet/HelpDesk/Default.aspx?view=-2" runat="server" id="newadminsupportticket">
                    <img src="/Extranet/Images/StatusIcons/newadmin.png" alt="" />
                    New Admin Support Ticket
                    <i>Open a New Admin Support Ticket</i>
                </a>
            <asp:Repeater runat="server" ID="ticketsrepeater">
                <ItemTemplate>
                    <a href="<%#string.Format("/Extranet/HelpDesk/Default.aspx?view={0}", Eval("Id")) %>" id="ticket-<%#Eval("Id")%>">
                        <img src="<%#string.Format("/Extranet/Images/StatusIcons/{0}.png", Eval("Status")) %>" alt="<%#Eval("Status")%>" />
                        <%# Eval("Subject") %>
                        <i>Opened <%# Eval("Date") %> by <%# getDisplayName(Eval("User")) %></i>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
            </div>
        </div>
        <div id="ticket">
            <asp:PlaceHolder runat="server" ID="NewTicketFiled" Visible="false">
                <h1>Your New ticket has been filed with IT support</h1>
                <div>You can access your support ticket via <%=string.Format("<a href=\"{0}://{1}{2}?view={3}\">{0}://{1}{2}?view={3}</a>", Request.Url.Scheme, Request.Url.Host + (Request.Url.Port != 80 ? ":" + Request.Url.Port.ToString() : ""), Request.Url.AbsolutePath, _id)%></div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="noCurrentTicket">
                <h1><< Select a Ticket or <a href="?view=-1">File for Support</a></h1>
                <div>Open Tickets: <asp:Literal runat="server" ID="openticketcount" /></div>
            </asp:PlaceHolder>
            <asp:Panel runat="server" ID="newadminticket" Visible="false">
                <div class="panel">
                    <h1>New Admin Ticket</h1>
                    <div class="content">
                        <div style="padding: 0 4px;">
                            <asp:Label runat="server" AssociatedControlID="newadminticketsubject">Subject: </asp:Label>
                            <asp:TextBox ID="newadminticketsubject" runat="server" Width="60%" />
                        </div>
                        <asp:Label runat="server" AssociatedControlID="newadminticketeditor">Problem: </asp:Label>
                        <div style="position: relative;">
                        <asp:Editor ID="newadminticketeditor" runat="server" Width="100%" Height="300px" />
                        </div>
                        <div style="padding: 0 4px;">
                            <asp:Label Font-Bold="true" runat="server" Text="Username: " AssociatedControlID="userlist" /><asp:DropDownList runat="server" ID="userlist" />
                            <asp:Label runat="server" AssociatedControlID="newadminticketpriorityList">Priority: </asp:Label>
                            <asp:DropDownList runat="server" ID="newadminticketpriorityList">
                                <asp:ListItem Text="Low" Value="Low" />
                                <asp:ListItem Text="Normal" Value="Normal" Selected="True" />
                                <asp:ListItem Text="High" Value="High" />
                            </asp:DropDownList>
                            <asp:Button ID="FileAdminTicket" runat="server" Text="File Admin Ticket" OnClick="FileAdminTicket_Click" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel runat="server" ID="newticket" Visible="false">
                <div class="panel">
                    <h1>New Ticket</h1>
                    <div class="content">
                        <div style="padding: 0 4px;">
                            <asp:Label runat="server" AssociatedControlID="newticketsubject">Subject: </asp:Label>
                            <asp:TextBox ID="newticketsubject" runat="server" Width="50%" />
                            <asp:Label runat="server" AssociatedControlID="newticketroom">Room: </asp:Label>
                            <asp:TextBox ID="newticketroom" runat="server" />
                        </div>
                        <asp:Label runat="server" AssociatedControlID="newticketeditor">Problem: </asp:Label>
                        <div style="position: relative;">
                        <asp:Editor ID="newticketeditor" runat="server" Width="100%" Height="300px" />
                        </div>
                        <div style="text-align: center;>
                            <asp:Button ID="newticketbutton" runat="server" Text="File Ticket" OnClick="FileTicket_Click" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:PlaceHolder runat="server" ID="canCurrentTicket">
            <div class="panel">
                <asp:Repeater runat="server" ID="currentticket">
                    <ItemTemplate>
                        <h1><span style="float: right;"><%#Eval("Status") %></span><a href="?view=<%#Eval("Id") %>">#<%#Eval("Id") %></a> - <b><%#Eval("Subject") %> - </b></h1>
                        <div style="border-bottom: solid 1px #7da2ce; padding: 4px;">Filed on <%#Eval("Date")%> by <%#((CHS_Extranet.HelpDesk.Ticket)Container.DataItem).User.DisplayName %></div>
                    </ItemTemplate>
                </asp:Repeater>
                <div class="Content">
                    <div style="padding: 4px 6px;">
                    <asp:Repeater runat="server" ID="ticketnotes">
                        <ItemTemplate>
                            <h3><%#Eval("Date")%> by <%#((CHS_Extranet.HelpDesk.Note)Container.DataItem).User.DisplayName %>:</h3>
                            <blockquote><%#Eval("NoteText")%></blockquote>
                        </ItemTemplate>
                        <SeparatorTemplate><hr /></SeparatorTemplate>
                    </asp:Repeater>
                    </div>
                    <asp:PlaceHolder runat="server" ID="AddNote">
                        <h1>Add a Note</h1>
                        <div style="position: relative;">
                        <asp:Editor ID="newnote" runat="server" Width="100%" Height="200px" />
                        </div>
                        <asp:Button runat="server" style="margin-bottom: 4px;" ID="AddNewNote" OnClick="AddNewNote_Click" Text="Add Note &amp; Update" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:PlaceHolder runat="server" ID="AdminNote" Visible="false">
                            <asp:CheckBox ID="CheckFixed" Visible="false" Text="Fixed?" TextAlign="Left" runat="server" />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="Label1" runat="server" AssociatedControlID="PriorityList">Priority: </asp:Label>
                            <asp:DropDownList runat="server" ID="PriorityList">
                                <asp:ListItem Text="Low" Value="Low" />
                                <asp:ListItem Text="Normal" Value="Normal" />
                                <asp:ListItem Text="High" Value="High" />
                            </asp:DropDownList>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>
                </div>
            </div>
            </asp:PlaceHolder>
        </div>
        <script type="text/javascript">
            function setButtonState(editor, buttonName, toolbarNumber) {
                // get collection of buttons in the toolbar
                var buttons = editor.get_editPanel().get_toolbars()[toolbarNumber].get_buttons();
                // looking for the 'buttonName' button
                var x = -1;
                for (var i = 0; i < buttons.length; i++) {
                    var button = buttons[i];
                    if (button.get_buttonName() == buttonName) {
                        x = i + 1;
                        // button's node
                        var element = button.get_element();
                        element.style.display = "none";
                    }
                    else if (i == x) {
                        var element = button.get_element();
                        element.style.display = "none";
                    }
                }
            }

            function setButtonState_HTMLtext(editor) {
                // "HtmlMode" - name of the "HTML text" button 
                // (class: AjaxControlToolkit.HTMLEditor.ToolbarButton.HtmlMode)
                setButtonState(editor, "FixedBackColor", 1); // 0 - bottom toolbar, 1 - top toolbar
                setButtonState(editor, "BackColorSelector", 1); // 0 - bottom toolbar, 1 - top toolbar
                setButtonState(editor, "BackColorClear", 1); // 0 - bottom toolbar, 1 - top toolbar
                setButtonState(editor, "FixedForeColor", 1); // 0 - bottom toolbar, 1 - top toolbar
                setButtonState(editor, "ForeColorSelector", 1); // 0 - bottom toolbar, 1 - top toolbar
                setButtonState(editor, "ForeColorClear", 1); // 0 - bottom toolbar, 1 - top toolbar
                setButtonState(editor, "FontName", 1); // 0 - bottom toolbar, 1 - top toolbar
                setButtonState(editor, "FontSize", 1); // 0 - bottom toolbar, 1 - top toolbar
            }

            function dosomething() {
                try {
                    setButtonState_HTMLtext($find("<%= newadminticketeditor.ClientID %>"));
                } catch (ex) {  }
                try {
                    setButtonState_HTMLtext($find("<%= newticketeditor.ClientID %>"));
                } catch (ex) { }
                try {
                    setButtonState_HTMLtext($find("<%= newnote.ClientID %>"));
                } catch (ex) {  }
            }
            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(dosomething);

        </script>
    </div>
</asp:Content>
