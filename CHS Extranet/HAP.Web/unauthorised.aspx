<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="unauthorised.aspx.cs" Inherits="HAP.Web.unauthorised" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="maincol">
        <h1>Error: Unauthorised Access</h1>
        <p>You have attempted to access a restricted resource</p>
        <input type="button" value="&lt; Go Back" onclick="window.history.back(-1);" />
    </div>
</asp:Content>
