<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="DirectEdit.aspx.cs" Inherits="HAP.Web.MyFiles.DirectEdit" %>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/MyFiles/"><hap:LocalResource StringPath="myfiles/myfiles" runat="server" /></asp:HyperLink><a href="#"><hap:LocalResource StringPath="myfiles/directedit" runat="server" /></a></asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <h1>Attempting to Launch HAP+ DirectEdit</h1>
    <p>If you don't have DirectEdit you can download it from <a href="http://www.nbdev.co.uk/projects/hap/directedit.aspx">www.nbdev.co.uk/projects/hap/directedit.aspx</a></p>
    <script>
        location.href = "hap://<%=hap%>";
    </script>
</asp:Content>
