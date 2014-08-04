<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="DirectEdit.aspx.cs" Inherits="HAP.Web.MyFiles.DirectEdit" %>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/MyFiles/"><hap:LocalResource StringPath="myfiles/myfiles" runat="server" /></asp:HyperLink><a href="#"><hap:LocalResource StringPath="myfiles/directedit" runat="server" /></a></asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <h1 style="padding: 0 10px;">Attempting to Launch HAP+ DirectEdit</h1>
    <p style="padding: 10px;">If you don't have DirectEdit you can download it from <a href="http://www.nbdev.uk/projects/hap/directedit.aspx">www.nbdev.uk/projects/hap/directedit.aspx</a></p>
    <p style="padding: 10px;"><a href="#" onclick="history.go(-1); return false;">Go Back</a></p>
    <script>
        location.href = "hap://<%=hap%>";
    </script>
</asp:Content>
