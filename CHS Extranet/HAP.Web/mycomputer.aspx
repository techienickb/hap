<%@ Page Title="Crickhowell High School - IT - Extranet - My Computer" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="mycomputer.aspx.cs" Inherits="HAP.Web.mycomputer" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Src="~/Controls/NewFolder.ascx" TagName="NewFolder" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Delete.ascx" TagName="Delete" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Rename.ascx" TagName="Rename" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Unzip.ascx" TagName="Unzip" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Zip.ascx" TagName="Zip" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Upload.ascx" TagName="Upload" TagPrefix="hap" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <link href="/Extranet/mycomputer.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Extranet/Scripts/rightclick.js"></script>
    <script type="text/javascript">
    	SimpleContextMenu.setup({ 'preventDefault': true, 'preventForms': false });
    	SimpleContextMenu.attach('container', 'CM1');
    	function onSilverlightError(sender, args) {
    	    alert(args.ErrorMessage);
    	}
    	var slCtl = null;
    	function onSilverlightLoaded(sender, args) {
    	    slCtl = sender.getHost();
    	}
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <asp:AjaxScriptManager runat="server" />
    <div id="maincol">
        <h1>My Computer</h1>
        <div style="position: relative;">
        <div id="bar">
            <a href="/Extranet/">Home Access Plus+ Home</a>
            <hap:NewFolder runat="server" ID="newfolderlink" Visible="false" />
            <hap:Upload runat="server" id="newfileuploadlink" Visible="false" />
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
        <hap:Delete runat="server" id="DeleteBox" />
        <hap:Rename runat="server" id="RenameBox" />
        <hap:Zip runat="server" id="ZipBox" />
        <hap:Unzip runat="server" id="UnzipBox" />
        <asp:PlaceHolder runat="server" ID="postbackmove" Visible="false">
            <script type="text/javascript">
                function getPosition(obj) {
                    var topValue = 0;
                    while (obj) {
                        topValue += obj.offsetTop;
                        obj = obj.offsetParent;
                    }
                    return topValue;
                }
                self.scrollTo(0, getPosition($get('bar')));
            </script>
        </asp:PlaceHolder>
    </div>
	<div id="CM1" class="SimpleContextMenu">
		<a href="#" onclick="return popup(this);">Delete</a>
		<a href="#" onclick="return popup(this);" runat="server" id="rckmove">Move</a>
		<a href="#" onclick="return popup(this);">Rename</a>
        <a href="#" onclick="return popup(this);" style="display: none;">HTML Preview</a>
        <a href="#" onclick="return popup(this);" style="display: none;">Zip</a>
        <a href="#" onclick="return popup(this);" style="display: none;">Unzip</a>
	</div>
</asp:Content>
