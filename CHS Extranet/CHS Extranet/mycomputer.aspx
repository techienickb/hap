<%@ Page Title="Crickhowell High School - IT - Extranet - My Computer" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="mycomputer.aspx.cs" Inherits="CHS_Extranet.mycomputer" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <link href="/Extranet/mycomputer.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Extranet/Scripts/rightclick.js"></script>
    	<script type="text/javascript">
    	    SimpleContextMenu.setup({ 'preventDefault': true, 'preventForms': false });
    	    SimpleContextMenu.attach('container', 'CM1');
	</script>

</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <div id="maincol">
        <h1>My Computer</h1>
        <div style="position: relative;">
        <div id="bar">
            <a href="/Extranet/">Home Access Plus+ Home</a>
            <asp:HyperLink runat="server" ID="newfolderlink" Visible="false" onclick="return popup(this);">New Folder</asp:HyperLink>
            <asp:HyperLink runat="server" ID="fileuploadlink" Visible="false" onclick="return popup(this);">Upload File</asp:HyperLink>
            <a class="right" href="/Extranet/mycomputer.aspx" onclick="return view();"><span>View</span></a>
        </div>
        <div id="viewbox">
            <a href="#" onclick="return changeview('Icons');">Icons</a>
            <a href="#" onclick="return changeview('List');">List</a>
            <a href="#" onclick="return changeview('Tile');">Tiles</a>
        </div>
        </div>
        <asp:Repeater runat="server" ID="breadcrumbrepeater">
            <HeaderTemplate><div id="breadcrumbs"></HeaderTemplate>
            <ItemTemplate><a href="<%#Eval("Path") %>"><%#Eval("Name") %></a></ItemTemplate>
            <SeparatorTemplate><span>&nbsp;</span></SeparatorTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
        <div id="browser">
            <asp:Repeater runat="server" ID="browserrepeater">
                <ItemTemplate>
                    <a href="<%#Eval("Path") %>"<%#((bool)Eval("RightClick")) ? " class=\"container\"" : ""%>>
                        <img src="/Extranet/images/icons/<%#Eval("Image") %>" alt="" />
                        <%#Eval("Name") %>
                        <i><%#Eval("Description") %></i>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <script type="text/javascript" src="/extranet/scripts/viewmode.js">
        </script>
    </div>
	<div id="CM1" class="SimpleContextMenu">
		<a href="#" onclick="return popup(this);">Delete</a>
		<a href="#" onclick="return popup(this);" runat="server" id="rckmove">Move</a>
		<a href="#" onclick="return popup(this);">Rename</a>
        <a href="#" onclick="return popup(this);" style="display: none;">HTML Preview</a>
        <a href="#" onclick="return popup(this);" style="display: none;">Unzip</a>
	</div>
</asp:Content>
