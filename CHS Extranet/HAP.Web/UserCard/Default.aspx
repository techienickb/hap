<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.UserCard.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h1>User Card System</h1>
    <h2>Departments</h2>
    <asp:ListView ID="ListView1" runat="server" DataSourceID="Departments" InsertItemPosition="LastItem" DataKeyNames="Name">
        <EmptyDataTemplate>
            <div>No data was returned.</div>
        </EmptyDataTemplate>
        <InsertItemTemplate>
            <div><asp:TextBox ID="NameTextBox" runat="server" Text='<%# Bind("Name") %>' /><asp:Button ID="InsertButton" runat="server" CommandName="Insert" Text="Insert" /></div>
        </InsertItemTemplate>
        <ItemTemplate>
            <div style="overflow: hidden;"><asp:Label ID="NameLabel" runat="server" Text='<%# Eval("Name") %>' style="display: block; float: left; width: 300px;" /><asp:Button ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete" /></div>
        </ItemTemplate>
        <LayoutTemplate>
            <div ID="itemPlaceholderContainer" runat="server" style="">
                <div runat="server" id="itemPlaceholder" />
            </div>
        </LayoutTemplate>
    </asp:ListView>
    <asp:ObjectDataSource ID="Departments" runat="server" 
        DeleteMethod="removeDepartment" InsertMethod="addDepartment" 
        SelectMethod="getDepartments" TypeName="HAP.Web.UserCard.Sys" >
        <DeleteParameters>
            <asp:Parameter Name="name" Type="String" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="name" Type="String" />
        </InsertParameters>
    </asp:ObjectDataSource>
    <h2>Forms</h2>
    <asp:ListView ID="ListView2" runat="server" DataSourceID="Forms" InsertItemPosition="LastItem" DataKeyNames="Name">
        <EmptyDataTemplate>
            <div>No data was returned.</div>
        </EmptyDataTemplate>
        <InsertItemTemplate>
            <div><asp:TextBox ID="NameTextBox" runat="server" Text='<%# Bind("Name") %>' /> OU = <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("OU") %>' /><asp:Button ID="InsertButton" runat="server" CommandName="Insert" Text="Insert" /></div>
        </InsertItemTemplate>
        <ItemTemplate>
            <div style="overflow: hidden;"><asp:Label ID="NameLabel" runat="server" Text='<%# Eval("Name") %>' style="display: block; float: left; width: 80px;" /><span style="float: left; width: 80px;"> OU =</span><asp:Label ID="OULabel" runat="server" Text='<%# Eval("OU") %>' style="display: block; float: left; width: 200px;" /><asp:Button ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete" /></div>
        </ItemTemplate>
        <LayoutTemplate>
            <div ID="itemPlaceholderContainer" runat="server" style="">
                <div runat="server" id="itemPlaceholder" />
            </div>
        </LayoutTemplate>
    </asp:ListView>
    <asp:ObjectDataSource ID="Forms" runat="server" 
        DeleteMethod="removeForms" InsertMethod="addForm" 
        SelectMethod="getForms" TypeName="HAP.Web.UserCard.Sys" >
        <DeleteParameters>
            <asp:Parameter Name="name" Type="String" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="ou" Type="String" />
        </InsertParameters>
    </asp:ObjectDataSource>
</asp:Content>
