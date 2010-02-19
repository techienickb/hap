<%@ Page Title="Crickhowell Hgih School - IT - Extranet" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CHS_Extranet.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="width: 240px; margin: 0 auto;">
        <asp:Login ID="Login1" runat="server" DisplayRememberMe="False" 
            DestinationPageUrl="~/Default.aspx" VisibleWhenLoggedIn="False" Width="100%">
            <LayoutTemplate>
                <h1 style="text-align: center;">Login</h1>
                <p>
                    <asp:Label ID="UserNameLabel" style="width: 80px; float: left;" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                    <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                        ControlToValidate="UserName" ErrorMessage="User Name is required." 
                        ToolTip="User Name is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="PasswordLabel" style="width: 80px; float: left;" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                    <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                        ControlToValidate="Password" ErrorMessage="Password is required." 
                        ToolTip="Password is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                </p>
                <p><asp:Literal ID="FailureText" runat="server" EnableViewState="False" /></p>
                <asp:Button ID="LoginButton" runat="server" CommandName="Login" Width="100%" Text="Log In" ValidationGroup="Login1" />
            </LayoutTemplate>
        </asp:Login>
    </div>
</asp:Content>
