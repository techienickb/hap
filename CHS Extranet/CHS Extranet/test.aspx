<%@ Page Title="" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="CHS_Extranet.test" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="mainleftcol">
        <div>
            <b>Base Settings: </b><asp:Literal runat="server" ID="basesettings" />
        </div>
        <div>
            <b>AD Settings: </b><asp:Literal runat="server" ID="adsettings" />
        </div>
        <div>
            <b>UNC Paths: </b><asp:Literal runat="server" ID="unc" />
        </div>
        <div>
            <b>Homepage Links: </b><asp:Literal runat="server" ID="links" />
        </div>
    </div>
</asp:Content>
