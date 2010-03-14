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
    <div id="maincol" style="position: relative;">
        <h1>My Computer</h1>
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
        <script type="text/javascript">
            function view() {
                document.getElementById('viewbox').className = (document.getElementById('viewbox').className == "show" ? "" : "show");
                return false;
            }
            function changeview(e) {
                document.getElementById('browser').className = e;
                return false;
            }
            function popup(e) {
                if (e.innerHTML == "Rename")
                    window.open(e.href, 'CHSRename', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=400,height=200', true);
                else if (e.innerHTML == "Delete")
                    window.open(e.href, 'CHSDelete', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=400,height=200', true);
                else if (e.innerHTML == "New Folder")
                    window.open(e.href, 'CHSNew', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=400,height=200', true);
                else if (e.innerHTML == "Move")
                    window.open(e.href, 'CHSMove', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=500,height=500', true);
                else if (e.innerHTML == "Unzip")
                    window.open(e.href, 'CHSUnzip', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=400,height=300', true);
                else if (e.innerHTML == "HTML Preview")
                    window.open(e.href, 'CHSPreview', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=800,scrollbars=0,height=600', true);
                else window.open(e.href, 'CHSUpload', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=600,height=400', true);
                return false;
            }
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
