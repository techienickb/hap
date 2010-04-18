<%@ Page Title="Crickhowell High School - IT - Home Access Plus+" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.Default" %>
<%@ Register TagName="announcement" TagPrefix="hap" Src="~/Controls/Announcement.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <link href="mycomputer.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="maincol">
        <asp:AjaxScriptManager runat="server" />
        <hap:announcement runat="server" />
        <asp:Image runat="server" ID="userimage" ImageAlign="Right" />
        <h2>Hello, <asp:Literal runat="server" ID="welcomename" />, welcome to Home Access Plus+</h2>
        <h3>My Details:</h3>
        <ul>
            <li><b>Name: </b><asp:Literal runat="server" ID="name" /></li>
            <li><asp:Literal runat="server" ID="form" /></li>
            <li><b>Email Address: </b><asp:Literal runat="server" ID="email" /></li>
        </ul>
        <asp:Button runat="server" Text="Update My Details" ID="updatemydetails" />
        <p id="HomeButtons">
            <asp:Repeater ID="homepagelinks" runat="server">
                <ItemTemplate>
                    <asp:HyperLink runat="server" ID="mycomputer" NavigateUrl='<%#Eval("LinkLocation")%>'>
                        <img runat="server" src='<%#Eval("Icon")%>' alt="" />
                        <%#Eval("Name") %>
                        <i><%#Eval("Description") %></i>
                    </asp:HyperLink>
                </ItemTemplate>
            </asp:Repeater>
        </p>
        <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="updatemydetails" PopupControlID="editmode" BackgroundCssClass="modalBackground" OkControlID="ok_btn" />
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
