﻿<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="unauthorised.aspx.cs" Inherits="CHS_Extranet.unauthorised" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="mainleftcol">
        <h1>Error: Unauthorised Access</h1>
        <p>You have attempted to access a restricted resource</p>
        <input type="button" value="&gt; Go Back" onclick="window.history.back(-1);" />
    </div>
</asp:Content>
